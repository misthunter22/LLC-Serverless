using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Db.Extensions;
using Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

                // Process Links
                LinksTable(client, db, env);

                // Process LinkStats
                LinkStatsTable(client, db, env);

                // Process Reports
                ReportsTable(client, db, env);

                // Process Packages
                PackagesTable(client, db, env);

                // Process Objects
                ObjectsTable(client, db, env);

                // Process ObjectLinks
                ObjectLinksTable(client, db, env);
            }

            return 0;
        }

        private static void BucketsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Buckets...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Buckets" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var buckets = db.S3Buckets;
            foreach (var b in buckets)
            {
                // Gather up the sources
                var sources = new List<Document>();
                foreach (var s in b.Sources)
                {
                    var document = new Document();

                    AddToDocument(document, "Id", s.SourceId.ToString());
                    AddToDocument(document, "Name", s.Name);
                    AddToDocument(document, "Description", s.Description);
                    AddToDocument(document, "AllowLinkChecking", s.AllowLinkChecking);
                    AddToDocument(document, "AllowLinkExtractions", s.AllowLinkExtractions);
                    AddToDocument(document, "Bucket", s.S3BucketId);
                    AddToDocument(document, "DateCreated", s.DateCreated);

                    sources.Add(document);
                }

                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", b.S3BucketId.ToString());
                AddToDocument(item, "Name", b.Name);
                AddToDocument(item, "AccessKey", b.AccessKey);
                AddToDocument(item, "SecretKey", b.SecretKey);
                AddToDocument(item, "Region", b.Region);
                AddToDocument(item, "SearchPrefix", b.SearchPrefix);
                AddToDocument(item, "DateCreated", b.DateCreated);
                item.Add("Sources", sources);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void SettingsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Settings...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Settings" + env);
            var batch = table.CreateBatchWrite();

            foreach (var s in db.Settings)
            {
                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", s.SettingId.ToString());
                AddToDocument(item, "DateCreated", s.DateCreated);
                AddToDocument(item, "DateModified", s.DateModified);
                AddToDocument(item, "ModifiedUser", s.ModifiedUser);
                AddToDocument(item, "Name", s.Name);
                AddToDocument(item, "Value", s.Value);
                AddToDocument(item, "Description", s.Description);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void LinksTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Links...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Links" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var links = db.Links;
            foreach (var l in links)
            {
                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", l.LinkId.ToString());
                AddToDocument(item, "Url", l.LinkUrl);
                AddToDocument(item, "DateFirstFound", l.DateFirstFound);
                AddToDocument(item, "DateLastFound", l.DateLastFound);
                AddToDocument(item, "DisabledUser", l.LinkCheckDisabledUser);
                AddToDocument(item, "AttemptCount", l.AttemptCount);
                AddToDocument(item, "Source", l.SourceId);
                AddToDocument(item, "DisabledDate", l.LinkCheckDisabledDate);
                AddToDocument(item, "Valid", l.IsValid);
                AddToDocument(item, "DateLastChecked", l.DateLastChecked);
                AddToDocument(item, "AllTimeMinDownloadTime", l.AllTimeMinDownloadTime);
                AddToDocument(item, "AllTimeMaxDownloadTime", l.AllTimeMaxDownloadTime);
                AddToDocument(item, "AllTimeStdDevDownloadTime", l.AllTimeStdDevDownloadTime);
                AddToDocument(item, "PastWeekMinDownloadTime", l.PastWeekMinDownloadTime);
                AddToDocument(item, "PastWeekMaxDownloadTime", l.PastWeekMaxDownloadTime);
                AddToDocument(item, "PastWeekStdDevDownloadTime", l.PastWeekStdDevDownloadTime);
                AddToDocument(item, "DateUpdated", l.DateStatsUpdated);
                AddToDocument(item, "ReportNotBeforeDate", l.ReportNotBeforeDate);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void LinkStatsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Stats...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Stats" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var stats = db.LinkStats;
            foreach (var s in stats)
            {
                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", s.LinkStatId.ToString());
                AddToDocument(item, "Link", s.LinkId);
                AddToDocument(item, "DateChecked", s.DateChecked);
                AddToDocument(item, "Error", s.ErrorMessage);
                AddToDocument(item, "StatusCode", s.StatusCode);
                AddToDocument(item, "StatusDescription", s.StatusDesc);
                AddToDocument(item, "ContentType", s.ContentType);
                AddToDocument(item, "ContentSize", s.ContentSize);
                AddToDocument(item, "DownloadTime", s.DownloadTime);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void ReportsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Reports...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Reports" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var reports = db.LinkReports;
            foreach (var r in reports)
            {
                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", r.Id.ToString());
                AddToDocument(item, "Mean", r.Mean);
                AddToDocument(item, "StandardDeviation", r.StandardDeviation);
                AddToDocument(item, "SdMaximum", r.SdMaximum);
                AddToDocument(item, "Link", r.Link_LinkId);
                AddToDocument(item, "ContentSize", r.ContentSize);
                AddToDocument(item, "Stat", r.LinkStatId);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void PackagesTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Packages...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Packages" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var packages = db.PackageUploads;
            foreach (var p in packages)
            {
                // Gather up the sources
                var files = new List<Document>();
                foreach (var f in p.PackageUploadFiles)
                {
                    var document = new Document();

                    AddToDocument(document, "Id", f.Id.ToString());
                    AddToDocument(document, "CourseLocation", f.CourseLocation);
                    AddToDocument(document, "Link", f.Link_LinkId);
                    AddToDocument(document, "Protocol", f.Protocol);
                    AddToDocument(document, "LinkName", f.LinkName);
                    AddToDocument(document, "ParentFolder", f.ParentFolder);

                    files.Add(document);
                }

                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", p.Id.ToString());
                AddToDocument(item, "Name", p.Name);
                AddToDocument(item, "Description", p.Description);
                AddToDocument(item, "UploadedBy", p.UploadedBy);
                AddToDocument(item, "DateUploaded", p.DateUploaded);
                AddToDocument(item, "Key", p.Key);
                AddToDocument(item, "FileName", p.FileName);
                AddToDocument(item, "ImsSchema", p.ImsSchema);
                AddToDocument(item, "ImsSchemaVersion", p.ImsSchemaVersion);
                AddToDocument(item, "ImsTitle", p.ImsTitle);
                AddToDocument(item, "ImsDescription", p.ImsDescription);
                AddToDocument(item, "Processed", p.PackageProcessed);
                item.Add("Files", files);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void ObjectsTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Objects...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-Objects" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var objects = db.S3Objects;
            foreach (var o in objects)
            {
                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", o.S3ObjectId.ToString());
                AddToDocument(item, "Bucket", o.S3BucketId);
                AddToDocument(item, "Key", o.Key);
                AddToDocument(item, "ItemName", o.ItemName);
                AddToDocument(item, "ETag", o.ETag);
                AddToDocument(item, "IsFolder", o.IsFolder);
                AddToDocument(item, "DateFirstFound", o.DateFirstFound);
                AddToDocument(item, "DateLastFound", o.DateLastFound);
                AddToDocument(item, "DisabledUser", o.LinkCheckDisabledUser);
                AddToDocument(item, "ContentLastModified", o.ContentLastModified);
                AddToDocument(item, "DateLinksLastExtracted", o.DateLinksLastExtracted);
                AddToDocument(item, "DisabledDate", o.LinkCheckDisabledDate);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void ObjectLinksTable(AmazonDynamoDBClient client, LORLinkCheckerEntities db, string env)
        {
            Console.WriteLine("Processing Object Links...");

            // Grab LLC-Buckets table
            var table = Table.LoadTable(client, "LLC-ObjectLinks" + env);
            var batch = table.CreateBatchWrite();

            // Process each bucket
            var objects = db.S3Objects_Links;
            foreach (var o in objects)
            {
                // Create the item and add it
                var item = new Document();

                AddToDocument(item, "Id", o.S3ObjectLinkId.ToString());
                AddToDocument(item, "Object", o.S3ObjectId);
                AddToDocument(item, "Link", o.LinkId);
                AddToDocument(item, "DateFirstFound", o.DateFirstFound);
                AddToDocument(item, "DateLastFound", o.DateLastFound);
                AddToDocument(item, "DateRemoved", o.DateRemoved);

                batch.AddDocumentToPut(item);
            }

            batch.Execute();
        }

        private static void AddToDocument(Document item, string key, object value)
        {
            if (value != null)
            {
                if (value is int || value is int?)
                    item.Add(key, (int)value);

                else if (value is decimal || value is decimal?)
                    item.Add(key, (decimal)value);

                else if (value is long || value is long?)
                    item.Add(key, (long)value);

                else if (value is double || value is double?)
                    item.Add(key, (double)value);

                else
                    item.Add(key, value.ToString());
            }
        }
    }
}
