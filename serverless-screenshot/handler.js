const exec      = require('child_process').exec;
const crypto    = require('crypto');
const fs        = require('fs');
const AWS       = require('aws-sdk');
const validUrl  = require('valid-url');
const phantomjs = require('phantomjs-prebuilt');

// overall constants
const screenWidth  = 1280;
const screenHeight = 1024;

// screenshot the given url
module.exports.take_screenshot = (event, context, cb) => {
  console.info('Received event', event);
  const targetUrl = event.queryStringParameters.url;
  const linkId    = event.queryStringParameters.linkId;
  const statId    = event.queryStringParameters.statId;
  const timeout   = event.stageVariables.screenshotTimeout;

  // check if the given url is valid
  if (!validUrl.isUri(targetUrl)) {
    cb(`422, please provide a valid url, not: ${targetUrl}`);
    return false;
  }

  const targetBucket   = event.stageVariables.bucketName;
  const targetFilename = `${linkId}/${statId}.png`;
  console.log(`Snapshotting ${targetUrl} to s3://${targetBucket}/${targetFilename}`);
  
  var program = phantomjs.exec('screenshot.js', targetUrl, `/tmp/${statId}.png`, screenWidth, screenHeight, timeout);
  program.stdout.pipe(process.stdout);
  program.stderr.pipe(process.stderr);
  program.on('exit', code => {
		
	if (code !== 0) {
      cb(`phantomjs return code was ${code}`);
      return;
	}
	  
    // snapshotting succeeded, let's upload to S3
    // read the file into buffer (perhaps make this async?)
    const fileBuffer = fs.readFileSync(`/tmp/${statId}.png`);

    // upload the file
    const s3 = new AWS.S3({apiVersion: '2006-03-01'});
    var params = {
	  Body: fileBuffer, 
	  Bucket: targetBucket, 
	  Key: targetFilename
	};
		 
	s3.putObject(params, function(err, data) {
	  if (err) {
	    console.log(err, err.stack); // an error occurred  
	  } 
	  else {
		var resp = {
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
  })
};


// gives a list of urls for the given snapshotted url
module.exports.list_screenshots = (event, context, cb) => {
  console.info('Received event', event);
  const targetUrl    = event.query.linkId;
  const targetBucket = event.stageVariables.bucketName;
  const targetPath   = `${targetUrl}/`;

  const s3 = new AWS.S3();
  s3.listObjects({
    Bucket: targetBucket,
    Prefix: targetPath,
    EncodingType: 'url',
  }, (err, data) => {
    if (err) {
      cb(err);
    } else {
      const urls = {};
      // for each key, get the image width and add it to the output object
      data.Contents.forEach((content) => {
        const parts = content.Key.split('/');
        const size  = parts.pop().split('.')[0];
        urls[size]  = `${event.stageVariables.endpoint}${content.Key}`;
      });
      cb(null, urls);
    }
    return;
  });
};
