const exec      = require('child_process').exec;
const crypto    = require('crypto');
const fs        = require('fs');
const AWS       = require('aws-sdk');
const validUrl  = require('valid-url');
const phantomjs = require('phantomjs-prebuilt');
const uuidv4    = require('uuid/v4');

// overall constants
const screenWidth  = 1280;
const screenHeight = 1024;

// define all the thumbnails that we want
const widths = {
  '320x240': `-crop ${screenWidth}x${screenHeight}+0x0 -thumbnail 320x240`,
  '640x480': `-crop ${screenWidth}x${screenHeight}+0x0 -thumbnail 640x480`,
  '800x600': `-crop ${screenWidth}x${screenHeight}+0x0 -thumbnail 800x600`,
  '1024x768': `-crop ${screenWidth}x${screenHeight}+0x0 -thumbnail 1024x768`,
  100: '-thumbnail 100x',
  200: '-thumbnail 200x',
  320: '-thumbnail 320x',
  400: '-thumbnail 400x',
  640: '-thumbnail 640x',
  800: '-thumbnail 800x',
  1024: '-thumbnail 1024x',
};

// screenshot the given url
module.exports.take_screenshot = (event, context, cb) => {
  console.info('Received event', event);
  const targetUrl = event.queryStringParameters.url.toLowerCase();
  const timeout   = event.stageVariables.screenshotTimeout;

  // check if the given url is valid
  if (!validUrl.isUri(targetUrl)) {
    cb(`422, please provide a valid url, not: ${targetUrl}`);
    return false;
  }

  const uid            = uuidv4();
  const targetBucket   = event.stageVariables.bucketName;
  const targetHash     = crypto.createHash('md5').update(targetUrl).digest('hex');
  var targetFilename   = `${targetHash}/1/original.png`;
  console.log(`Snapshotting ${targetUrl} to s3://${targetBucket}/${targetFilename}`);
  
  var program = phantomjs.exec('screenshot.js', targetUrl, `/tmp/${targetHash}.png`, screenWidth, screenHeight, timeout);
  program.stdout.pipe(process.stdout);
  program.stderr.pipe(process.stderr);
  program.on('exit', code => {
		
	if (code !== 0) {
      cb(`phantomjs return code was ${code}`);
      return;
	}
	  
    // snapshotting succeeded, let's upload to S3
    // read the file into buffer (perhaps make this async?)
    const fileBuffer = fs.readFileSync(`/tmp/${targetHash}.png`);

    // upload the file
    const s3 = new AWS.S3({apiVersion: '2006-03-01'});
    var params = {
	  Bucket: targetBucket, 
	  Key: targetFilename
	};
	
    s3.headObject(params, function (err, metadata) { 
      var obj = {};
      if (err && err.code !== 'NotFound') {
		console.log(err, err.stack); // an error occurred   	
		cb(err);
	  }
	  else if (err && err.code === 'NotFound') {
	    obj = {
	      Body: fileBuffer, 
	      Bucket: targetBucket, 
	      Key: targetFilename
	    };
	  } 
	  else {  
	    targetFilename = `${targetHash}/${uid}/original.png`;
        obj = {
	      Body: fileBuffer, 
	      Bucket: targetBucket, 
	      Key: targetFilename
	    }; 
      }
	  
	  s3.putObject(obj, function(err, data) {
	    if (err) {
	      console.log(err, err.stack); // an error occurred  
		  cb(err);
	    } 
	    else {
		  var resp = {
		    "hash": `${targetHash}`,
		    "key": `${targetFilename}`,
            "bucket": targetBucket,
            "url": `${event.stageVariables.endpoint}${targetFilename}`
		  };
		
		  cb(null, 
		  {
		    "statusCode": 200, 
		    "body": JSON.stringify(resp)
		  });
	    }     
	  });
    });
  })
};

// gives a list of urls for the given snapshotted url
module.exports.list_screenshot = (event, context, cb) => {
  const targetUrl = event.queryStringParameters.url.toLowerCase();

  // check if the given url is valid
  if (!validUrl.isUri(targetUrl)) {
    cb(`422, please provide a valid url, not: ${targetUrl}`);
    return false;
  }

  const targetHash   = crypto.createHash('md5').update(targetUrl).digest('hex');
  const targetBucket = event.stageVariables.bucketName;
  const targetPath   = `${targetHash}/`;

  const s3 = new AWS.S3({apiVersion: '2006-03-01'});
  s3.listObjects({
    Bucket: targetBucket,
    Prefix: targetPath,
    EncodingType: 'url',
  }, (err, data) => {
    if (err) {
      cb(err);
    } else {
      const urls   = {};
	  const map    = {};
	  urls["urls"] = [];
	  urls["last"] = null;
	  
	  var lastDate = new Date(-8640000000000000);
	  
      // for each key, get the image width and add it to the output object
      data.Contents.forEach((content) => {
		const parts = content.Key.split('/');
        const size  = parts.pop().split('.')[0];
        const pre   = parts.join('/');
        const item  = parts.pop();
		const key   = `${event.stageVariables.endpoint}${content.Key}`;
		
		var url = {};
		if (map[item]) {
          url = map[item];
		}
		else {
		  map[item] = url;
		  urls["urls"].push(url);
		}
		
        url["s_" + size] = key;
        url["key"]       = pre;
		url["date"]      = content.LastModified;
		
		if (new Date(content.LastModified) > lastDate) {
		  lastDate     = new Date(content.LastModified);
		  urls["last"] = pre;
		}
      });
	  
      cb(null, {
		  "statusCode": 200, 
		  "body": JSON.stringify(urls)
	  });
    }
    return;
  });
};

module.exports.create_thumbnails = (event, context, cb) => {
  const record = event.Records[0];
  console.info(JSON.stringify(record));

  // we only want to deal with originals
  if (record.s3.object.key.indexOf('original.png') === -1) {
    console.warn('Not an original, skipping');
    cb('Not an original, skipping');
    return false;
  }

  // get the prefix, and get the hash
  const folder = record.s3.object.key.split('/')[0];
  const prefix = record.s3.object.key.split('/')[1];
  const hash   = prefix;

  // download the original to disk
  const s3         = new AWS.S3({apiVersion: '2006-03-01'});
  const sourcePath = '/tmp/original.png';
  s3.getObject({
    Bucket: record.s3.bucket.name,
    Key: record.s3.object.key
  }, function(err, data) {
	   if (err) {
	     console.warn(JSON.stringify(err));
	   }
	   else {
		 fs.writeFile(sourcePath, data.Body, "binary", function(err) {
           if(err) {
             console.log(err);
           } 
		   else {
             // resize to every configured size
			 Object.keys(widths).forEach((size) => {
			   const cmd = `convert ${widths[size]} ${sourcePath} /tmp/${hash}-${size}.png`;
			   console.log('Running ', cmd);

			   exec(cmd, (error, stdout, stderr) => {
			     if (error) {
				   // the command failed (non-zero), fail
				   console.warn(`exec error: ${error}, stdout, stderr`);
				   // continue
				 } else {
				   // resize was succesfull, upload the file
				   console.info(`Resize to ${size} OK`);
				   var fileBuffer = fs.readFileSync(`/tmp/${hash}-${size}.png`);
				   s3.putObject({
					  ACL: 'public-read',
					  Key: `${folder}/${prefix}/${size}.png`,
					  Body: fileBuffer,
					  Bucket: record.s3.bucket.name,
					  ContentType: 'image/png'
					}, function(err, data){
					  if(err) {
						console.warn(err);
					  } else {
						console.info(`${size} uploaded`)
					  }
					});
				  }
				})
			  });
           }
         });
	   }
  });
};
