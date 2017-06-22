using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Db.Extensions;
using Db.Models;
using System;
using System.Collections.Generic;

namespace Db
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage <bucket name> <access key> <secret key> [<environment>]");
                return 1;
            }

            // Assume the arguments are in the correct order
            // If the access/secret keys are wrong, it won't
            // matter anyway
            string bucket = args[0];
            string aKey   = args[1];
            string sKey   = args[2];
            string env    = string.Empty;

            // See if an environment is set
            if (args.Length > 3)
            {
                env = args[3];
            }

            // First, create all of the folders in the bucket
            // that do not use DynamoDB
            using (var client = S3Helper.S3Client(aKey, sKey))
            {
                S3Helper.CreateBucket(client, bucket, "Packages");
                S3Helper.CreateBucket(client, bucket, "Screenshots");
            }

            // Now, move the data into S3/DynamoDB
            using (var db = new LORLinkCheckerEntities())
            {
                // Grab AWS Credentials
                var credentials = new BasicAWSCredentials(aKey, sKey);
                var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);

                // Process Buckets
                BucketsTable(client, db, env);

                // Process Settings
                SettingsTable(client, db, env);
            }

            return 0;
        }

        private static void BucketsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Buckets" + env);

            // Process each bucket
            var buckets = db.S3Buckets;
            foreach (var b in buckets)
            {
                // Gather up the sources
                var sources = new List<Document>();
                foreach (var s in b.Sources)
                {
                    var document = new Document(new Dictionary<string, DynamoDBEntry>()
                    {
                        { "Id", s.SourceId.ToString() },
                        { "Name", s.Name },
                        { "Description", s.Description },
                        { "AllowLinkChecking", s.AllowLinkChecking.ToString() },
                        { "AllowLinkExtractions", s.AllowLinkExtractions.ToString() },
                        { "BucketId", s.S3BucketId.ToString() },
                        { "DateCreated", s.DateCreated.ToString() }
                    });

                    sources.Add(document);
                }

                // Create the item and add it
                var item = new Document(new Dictionary<string, DynamoDBEntry>()
                {
                    { "Id", b.S3BucketId.ToString() },
                    { "Name", b.Name },
                    { "AccessKey", b.AccessKey },
                    { "SecretKey", b.SecretKey },
                    { "Region", b.Region },
                    { "SearchPrefix", b.SearchPrefix },
                    { "DateCreated", b.DateCreated.ToString() }
                });

                item.Add("Sources", sources);
                table.PutItem(item);
            }
        }

        private static void SettingsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Settings" + env);

            foreach (var s in db.Settings)
            {
                // Create the item and add it
                var item = new Document(new Dictionary<string, DynamoDBEntry>()
                {
                    { "Id", s.SettingId.ToString() },
                    { "DateCreated", s.DateCreated.ToString() },
                    { "DateModified", s.DateModified.ToString() },
                    { "ModifiedUser", s.ModifiedUser }
                });

                if (!string.IsNullOrEmpty(s.Name))
                    item.Add("Name", s.Name);

                if (!string.IsNullOrEmpty(s.Value))
                    item.Add("Value", s.Value);

                if (!string.IsNullOrEmpty(s.Description))
                    item.Add("Description", s.Description);

                table.PutItem(item);
            }
        }
    }
}
