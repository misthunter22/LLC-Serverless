using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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
                return -1;
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

            using (var client = S3Helper.S3Client(aKey, sKey))
            {
                S3Helper.CreateBucket(client, bucket, "Packages");

                // Grab AWS Credentials
                var credentials = new BasicAWSCredentials(aKey, sKey);
                using (var dynamo = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2))
                {
                    ConfigureTableMeta(dynamo, env);
                    AddLabelToInvalidReports(dynamo, env);
                }

                return 0;
            }
        }

        private static void ConfigureTableMeta(AmazonDynamoDBClient client, string env)
        {
            var tableName = "LLC-Meta" + env;
            var table     = Table.LoadTable(client, tableName);
            var batch     = table.CreateBatchWrite();
            var keys      = new string[]
            {
                "Buckets",
                "Links",
                "ObjectLinks",
                "Objects",
                "PackageFiles",
                "PackageLinks",
                "Packages",
                "Reports",
                "Settings",
                "Sources",
                "Stats"
            };

            var results = client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            }).Result;

            // Only run this portion if the table is empty
            if (results.Count == 0)
            {
                foreach (var t in keys)
                {
                    var document = new Document();
                    AddToDocument(document, "Id", t);
                    AddToDocument(document, "Key", 0);
                    batch.AddDocumentToPut(document);
                }

                batch.Execute();
            }
        }

        private static void AddLabelToInvalidReports(AmazonDynamoDBClient client, string env)
        {
            var tableName = "LLC-Reports" + env;
            var table     = Table.LoadTable(client, tableName);
            var batch     = table.CreateBatchWrite();
            var key       = GetTableNextKeyIndex(client, env, "Reports");

            var results = client.ScanAsync(new ScanRequest
            {
                TableName = tableName
            }).Result;

            // Process the existing items without a Type
            foreach (var r in results.Items)
            {
                if (!r.ContainsKey("ReportType"))
                {
                    r["ReportType"] = new AttributeValue { S = "Warning" };
                    r.Remove("Type");
                    client.PutItem(new PutItemRequest
                    {
                        Item = r,
                        TableName = tableName
                    });
                }
                else
                {
                    // Temporary, remove me!!
                    if (r["ReportType"].S == "Invalid")
                    {
                        var k = new Dictionary<string, AttributeValue>();
                        k["Id"] = new AttributeValue { N = r["Id"].N };

                        client.DeleteItem(new DeleteItemRequest
                        {
                            TableName = tableName,
                            Key = k
                        });
                    }
                }
            }

            // Only process Invalid items from the other table
            // If there are NO items of Type "Invalid"
            var rows = client.ScanAsync(new ScanRequest
            {
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":val",  new AttributeValue { S = "Invalid" } }
                },
                FilterExpression = $"ReportType = :val",
                TableName = tableName
            }).Result;

            // Continue, since this is the first time
            if (rows.Count == 0)
            {
                Console.WriteLine("zero, proceeding...");
                var last = new Dictionary<string, AttributeValue>();
                last["Id"] = new AttributeValue { N = "0" };

                while (last.ContainsKey("Id"))
                {
                    var invalid = client.ScanAsync(new ScanRequest
                    {
                        TableName = "LLC-Links" + env,
                        ExclusiveStartKey = last["Id"].N == "0" ? null : last
                    }).Result;

                    foreach (var i in invalid.Items)
                    {
                        if (!(i.ContainsKey("Valid") && i["Valid"].BOOL))
                        {
                            var document = new Document();
                            AddToDocument(document, "Id", ++key);
                            AddToDocument(document, "Link", i["Id"].N);
                            AddToDocument(document, "ReportType", "Invalid");
                            AddToDocument(document, "Source", "IDLA S-LOR on AWS");
                            AddToDocument(document, "Url", i["Url"].S);
                            if (i.ContainsKey("DateLastChecked"))
                                AddToDocument(document, "DateLastChecked", i["DateLastChecked"].S);
                            if (i.ContainsKey("DateLastFound"))
                                AddToDocument(document, "DateLastFound", i["DateLastFound"].S);
                            AddToDocument(document, "AttemptCount", i["AttemptCount"].N);
                            batch.AddDocumentToPut(document);
                        }
                    }

                    last = invalid.LastEvaluatedKey;
                }

                batch.Execute();
                SetTableNextKeyIndex(client, env, "Reports", key.ToString());
            }
        }

        private static int GetTableNextKeyIndex(AmazonDynamoDBClient client, string env, string key)
        {
            var tableName = "LLC-Meta" + env;
            var resp = client.QueryAsync(new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "Id = :v_Id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_Id", new AttributeValue { S =  key }}}
            }).Result;

            return int.Parse(resp.Items[0]["Key"].N);
        }

        private static void SetTableNextKeyIndex(AmazonDynamoDBClient client, string env, string key, string value)
        {
            var tableName = "LLC-Meta" + env;
            var item      = new Dictionary<string, AttributeValue>();
            item["Id"]    = new AttributeValue { S = key };
            item["Key"]   = new AttributeValue { N = value };

            client.PutItem(new PutItemRequest
            {
                TableName = tableName,
                Item = item
            });
        }

        /// <summary>
        /// This is the old process for migrating data. It worked,
        /// but was WAYYYY to slow!! This probably won't be used,
        /// but it is worth keeping just in case it's neeeded.
        /// </summary>
        /// <param name="args">The program args</param>
        private static void CopyDbProcess(AmazonDynamoDBClient client, string env)
        {
            using (var db = new LORLinkCheckerEntities())
            {
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
