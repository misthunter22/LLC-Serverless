using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;

/*
 *  LOR Link Checker Common - S3 Helper
 *  
 *  Description: The S3 Helper class contains basic
 *  configuration code for connecting to Amazon S3.
 *  This is used all over the place, so it makes
 *  sense to keep it all in one spot.
 *  
 *  Created By				: Brian Merrick
 *  Created On				: March 12, 2017 
 *  
 */
namespace Db.Extensions
{
    public static class S3Helper
    {
        public static AmazonS3Client S3Client(string accessKey, string secretKey)
        {
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
                return null;

            // Setup the S3 connection
            var creds = new BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.USWest2
            };

            var s3Client = new AmazonS3Client(creds, config);
            return s3Client;
        }

        public static void CreateBucket(AmazonS3Client client, string bucket, string folder)
        {
            try
            {
                client.PutBucket(bucket);
                if (folder != null)
                {
                    client.PutObject(new PutObjectRequest
                    {
                        BucketName = bucket,
                        Key = folder + "/",
                        ContentBody = folder
                    });
                }
            }
            catch (Exception) { }
        }

        public static PutObjectResponse AddObject(AmazonS3Client client, string bucket, string key, Stream ms)
        {
            var resp = client.PutObject(new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = ms
            });

            return resp;
        }

        // Get object contents for a specific key
        public static string S3ObjectContents(AmazonS3Client s3Client, string bucketName, string keyName)
        {
            // Track the contents to be returned
            string content = string.Empty;

            try
            {
                // Prepare the request to get the object's contents
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                // Get the object data
                using (GetObjectResponse response = s3Client.GetObject(request))

                // Read in the contents of the specific file into a string
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") ||
                                             ex.ErrorCode.Equals("InvalidSecurity")))
                {
                    Log("Please check the provided AWS Credentials.");
                }

                Log("Error in GetS3ObjectContents() method",
                    $"Caught Exception:{ex.Message}; Response Status Code:{ex.StatusCode}; Error Code:{ex.ErrorCode}; Request ID:{ex.RequestId}",
                    true, ex.ToString());
            }
            catch (Exception ex) // Catch other unexpected errors
            {
                content = null; // Return null for the content so we can skip processing the results since there was an error
                Log("  Error attempting to get S3 object: " + ex.Message);
            }

            // Return the contents of the retrieved object data
            return content;
        }

        // Downloads an object contents for a specific key on the filesystem
        public static void DownloadS3ObjectContents(AmazonS3Client client, string bucket, string keyName, string path)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = keyName
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    response.WriteResponseStreamToFile(path);
                }
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") || ex.ErrorCode.Equals("InvalidSecurity")))
                {
                    Log("Please check the provided AWS Credentials.");
                }

                Log("Error in GetS3ObjectContents() method",
                    $"Caught Exception:{ex.Message}; Response Status Code:{ex.StatusCode}; Error Code:{ex.ErrorCode}; Request ID:{ex.RequestId}",
                    true, ex.ToString());
            }
            catch (Exception ex) // Catch other unexpected errors
            {
                Log("  Error attempting to get S3 object: " + ex.Message);
            }
        }

        public static ListObjectsResponse ListFolder(AmazonS3Client client, string bucket, string f, string prefix)
        {
            var folder = string.IsNullOrEmpty(f) ? prefix : f + "/" + prefix;
            var listFolder = client.ListObjects(bucket, folder);
            return listFolder;
        }

        // Log output to the database log
        private static void Log(string title, string notes = null,
            bool isError = false, string exceptionDetails = null)
        {
            Console.WriteLine(title);
        }
    }
}
