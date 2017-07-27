const exec = require('child_process').exec;
const crypto = require('crypto');
const fs = require('fs');
const AWS = require('aws-sdk');
const validUrl = require('valid-url');

// overall constants
const screenWidth = 1280;
const screenHeight = 1024;

// screenshot the given url
module.exports.take_screenshot = (event, context, cb) => {
  const targetUrl = event.query.url;
  const linkId    = event.query.linkId;
  const statId    = event.query.statId;
  const timeout   = event.stageVariables.screenshotTimeout;

  // check if the given url is valid
  if (!validUrl.isUri(targetUrl)) {
    cb(`422, please provide a valid url, not: ${targetUrl}`);
    return false;
  }

  const targetBucket = event.stageVariables.bucketName;
  const targetFilename = `${linkId}/${statId}.png`;
  console.log(`Snapshotting ${targetUrl} to s3://${targetBucket}/${targetFilename}`);

  // build the cmd for phantom to render the url
  const cmd = `./phantomjs/phantomjs_linux-x86_64 --debug=yes --ignore-ssl-errors=true ./phantomjs/screenshot.js ${targetUrl} /tmp/${statId}.png ${screenWidth} ${screenHeight} ${timeout}`; // eslint-disable-line max-len
  console.log(cmd);

  // run the phantomjs command
  exec(cmd, (error, stdout, stderr) => {
    if (error) {
      // the command failed (non-zero), fail the entire call
      console.warn(`exec error: ${error}`, stdout, stderr);
      cb(`422, please try again ${error}`);
    } else {
      // snapshotting succeeded, let's upload to S3
      // read the file into buffer (perhaps make this async?)
      const fileBuffer = fs.readFileSync(`/tmp/${statId}.png`);

      // upload the file
      const s3 = new AWS.S3();
      s3.putObject({
        ACL: 'public-read',
        Key: targetFilename,
        Body: fileBuffer,
        Bucket: targetBucket,
        ContentType: 'image/png',
      }, (err) => {
        if (err) {
          console.warn(err);
          cb(err);
        } else {
          // console.info(stderr);
          // console.info(stdout);
          cb(null, {
            key: `${targetFilename}`,
            bucket: targetBucket,
            url: `${event.stageVariables.endpoint}${targetFilename}`,
          });
        }
        return;
      });
    }
  });
};


// gives a list of urls for the given snapshotted url
module.exports.list_screenshots = (event, context, cb) => {
  const targetUrl = event.query.linkId;
  const targetBucket = event.stageVariables.bucketName;
  const targetPath = `${targetUrl}/`;

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
        const size = parts.pop().split('.')[0];
        urls[size] = `${event.stageVariables.endpoint}${content.Key}`;
      });
      cb(null, urls);
    }
    return;
  });
};
