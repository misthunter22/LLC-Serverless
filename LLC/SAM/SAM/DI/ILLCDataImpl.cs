using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DbCore.Models;
using Newtonsoft.Json;
using SAM.Models.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SAM.Models.Reports;
using SAM.Models.EF;
using SAM.Models.Auth;
using System.Security.Claims;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net.Http;
using SAM.Models;
using System.Data.SqlClient;
using System.Data;

namespace SAM.DI
{
    public class ILLCDataImpl : ILLCData
    {
        protected RegionEndpoint _region = RegionEndpoint.USWest2;

        private int _maxQueue = 10;

        private int _maxDequeue = 1;

        protected string ApiKey = Environment.GetEnvironmentVariable("ApiKey");

        protected string ScreenshotUrl = Environment.GetEnvironmentVariable("Screenshot") + "screenshots";

        protected string DbConnection = Environment.GetEnvironmentVariable("DbConnection");

        public RegionEndpoint Region()
        {
            return _region;
        }

        public User User(IEnumerable<Claim> claims)
        {
            return new User(claims);
        }

        public List<StatsExt> Stats()
        {
            var stats = new List<StatsExt>();

            using (var conn = new SqlConnection(DbConnection))
            {
                using (var command = new SqlCommand("p_Reports_SourceStats", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    using (SqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var ext = new StatsExt
                            {
                                HtmlFiles     = rdr["HTMLCount"]          is DBNull ? 0    : (int)rdr["HTMLCount"],
                                InvalidLinks  = rdr["InvalidLinkCount"]   is DBNull ? 0    : (int)rdr["InvalidLinkCount"],
                                LastChecked   = rdr["LinksLastChecked"]   is DBNull ? null : (DateTime?)rdr["LinksLastChecked"],
                                LastExtracted = rdr["LinksLastExtracted"] is DBNull ? null : (DateTime?)rdr["LinksLastExtracted"],
                                Objects       = rdr["ObjectCount"]        is DBNull ? 0    : (int)rdr["ObjectCount"],
                                Source        = rdr["Source"]             is DBNull ? ""   : (string)rdr["Source"],
                                TotalLinks    = rdr["LinkCount"]          is DBNull ? 0    : (int)rdr["LinkCount"],
                            };

                            stats.Add(ext);
                        }
                    }
                }

                return stats;
            }
        }

        public HttpClient ScreenshotClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1)
            };

            client.DefaultRequestHeaders.Add("x-api-key", ApiKey);
            return client;
        }

        public int ObjectsCount(string bucket)
        {
            using (var client = new LLCContext())
            {
                var count = client.Objects.Where(
                    x => x.Bucket == bucket &&
                    x.DisabledDate == null &&
                    x.Key.Contains(".htm") &&
                    (x.DateLinksLastExtracted == null || x.DateLinksLastExtracted < x.ContentLastModified) &&
                    x.IsFolder == false).Count();

                return count;
            }
        }

        public Objects Object(string id)
        {
            using (var client = new LLCContext())
            {
                var obj = client.Objects.FirstOrDefault(x => x.Id == id);
                return obj;
            }
        }

        public Objects ObjectFromKey(string key)
        {
            using (var client = new LLCContext())
            {
                var obj = client.Objects.FirstOrDefault(x => x.Key == key);
                return obj;
            }
        }

        public Objects SetObject(Objects obj)
        {
            using (var client = new LLCContext())
            {
                var update = client.Objects.Update(obj);
                client.SaveChanges();
                return obj;
            }
        }

        public List<ObjectsExt> LinkExtractor(string bucket, int offset, int maximum)
        {
            using (var client = new LLCContext())
            {
                var result = client.Objects.Where(
                    x => x.Bucket == bucket &&
                    x.DisabledDate == null &&
                    x.Key.Contains(".htm") &&
                    (x.DateLinksLastExtracted == null || x.DateLinksLastExtracted < x.ContentLastModified) &&
                    x.IsFolder == false)
                  .Skip(offset)
                  .Take(maximum)
                  .Select(x => new ObjectsExt
                  {
                      Bucket = x.Bucket,
                      ContentLastModified = x.ContentLastModified,
                      DateFirstFound = x.DateFirstFound,
                      DateLastFound = x.DateLastFound,
                      DateLinksLastExtracted = x.DateLinksLastExtracted,
                      DisabledDate = x.DisabledDate,
                      DisabledUser = x.DisabledUser,
                      Etag = x.Etag,
                      Id = x.Id,
                      IsFolder = x.IsFolder,
                      ItemName = x.ItemName,
                      Key = x.Key
                  })
                  .ToList();

                return result;
            }
        }

        public int LinksCount(string source)
        {
            using (var client = new LLCContext())
            {
                var count = client.Links.Where(x => x.Source == source).Count();
                return count;
            }
        }

        public Links Link(string id)
        {
            using (var client = new LLCContext())
            {
                var obj = client.Links.FirstOrDefault(x => x.Id == id);
                return obj;
            }
        }

        public Links LinkFromUrl(string url)
        {
            using (var client = new LLCContext())
            {
                var obj = client.Links.FirstOrDefault(x => x.Url == url);
                return obj;
            }
        }

        public Links SetLink(Links link)
        {
            using (var client = new LLCContext())
            {
                var update = client.Links.Update(link);
                client.SaveChanges();
                return link;
            }
        }

        public Links AddLink(Links link)
        {
            using (var client = new LLCContext())
            {
                var update = client.Links.Add(link);
                client.SaveChanges();
                return link;
            }
        }

        public Stats AddStat(Stats stat)
        {
            using (var client = new LLCContext())
            {
                var update = client.Stats.Add(stat);
                client.SaveChanges();
                return stat;
            }
        }

        public List<Stats> LinkStats(LinksExt link)
        {
            using (var client = new LLCContext())
            {
                var stats = 
                    (from stat in client.Stats
                     join l in client.Links on stat.Link equals l.Id
                     where l.Id == link.Id && (l.ReportNotBeforeDate == null || stat.DateChecked >= l.ReportNotBeforeDate)
                     select stat)
                    .ToList();

                return stats;
            }
        }

        public List<LinksExt> LinkChecker(string source, int offset, int maximum)
        {
            using (var client = new LLCContext())
            {
                var result = client.Links.Where(x => x.Source == source)
                    .Skip(offset)
                    .Take(maximum)
                    .Select(x => new LinksExt
                    {
                        AllTimeMaxDownloadTime = x.AllTimeMaxDownloadTime,
                        AllTimeMinDownloadTime = x.AllTimeMinDownloadTime,
                        AllTimeStdDevDownloadTime = x.AllTimeStdDevDownloadTime,
                        AttemptCount = x.AttemptCount,
                        DateFirstFound = x.DateFirstFound,
                        DateLastChecked = x.DateLastChecked,
                        DateLastFound = x.DateLastFound,
                        DateUpdated = x.DateUpdated,
                        DisabledDate = x.DisabledDate,
                        DisabledUser = x.DisabledUser,
                        Id = x.Id,
                        PastWeekMaxDownloadTime = x.PastWeekMaxDownloadTime,
                        PastWeekMinDownloadTime = x.PastWeekMinDownloadTime,
                        PastWeekStdDevDownloadTime = x.PastWeekStdDevDownloadTime,
                        ReportNotBeforeDate = x.ReportNotBeforeDate,
                        Source = x.Source,
                        Url = x.Url,
                        Valid = x.Valid
                    })
                    .ToList();

                return result;
            }
        }

        public List<ReportsExt> InvalidLinks()
        {
            using (var client = new LLCContext())
            {
                var results =
                    (from reports in client.Reports
                     join link in client.Links on reports.Link equals link.Id
                     where reports.ReportType == "Invalid"
                     select new ReportsExt
                     {
                        AttemptCount = link.AttemptCount,
                        ContentSize = reports.ContentSize,
                        DateLastChecked = link.DateLastChecked,
                        DateLastFound = link.DateLastFound,
                        Id = reports.Id,
                        Link = link.Id,
                        Mean = reports.Mean,
                        ReportType = reports.ReportType,
                        SdMaximum = reports.SdMaximum,
                        Source = link.Source,
                        StandardDeviation = reports.StandardDeviation,
                        Stat = reports.Stat,
                        Url = link.Url
                     })
                     .ToList();

                return results;
            }
        }

        public List<ReportsExt> WarningLinks()
        {
            using (var client = new LLCContext())
            {
                var results =
                    (from reports in client.Reports
                     join link in client.Links on reports.Link equals link.Id
                     where reports.ReportType == "Warning"
                     select new ReportsExt
                     {
                         AttemptCount = link.AttemptCount,
                         ContentSize = reports.ContentSize,
                         DateLastChecked = link.DateLastChecked,
                         DateLastFound = link.DateLastFound,
                         Id = reports.Id,
                         Link = link.Id,
                         Mean = reports.Mean,
                         ReportType = reports.ReportType,
                         SdMaximum = reports.SdMaximum,
                         Source = link.Source,
                         StandardDeviation = reports.StandardDeviation,
                         Stat = reports.Stat,
                         Url = link.Url
                     })
                     .ToList();

                return results;
            }
        }

        public Reports Report(string id)
        {
            using (var client = new LLCContext())
            {
                var obj = client.Reports.FirstOrDefault(x => x.Link == id);
                return obj;
            }
        }

        public void AddReport(Reports report)
        {
            using (var client = new LLCContext())
            {
                client.Reports.Add(report);
                client.SaveChanges();
            }
        }

        public void RemoveReport(Reports report)
        {
            using (var client = new LLCContext())
            {
                client.Reports.Remove(report);
                client.SaveChanges();
            }
        }

        public void RemoveReport(string link)
        {
            using (var client = new LLCContext())
            {
                var report = client.Reports.FirstOrDefault(x => x.Link == link);
                if (report != null)
                {
                    client.Reports.Remove(report);
                    client.SaveChanges();
                }
            }
        }

        public Reports SetReport(Reports report)
        {
            using (var client = new LLCContext())
            {
                var update = client.Reports.Update(report);
                client.SaveChanges();
                return report;
            }
        }

        public ExistingScreenshotList Screenshots(BucketLocationsRequest m)
        {
            var link              = Link(m.id);
            var url               = ScreenshotUrl + "?url=" + link.Url;
            var client            = ScreenshotClient();
            var screenshotRequest = client.GetAsync(new Uri(url)).Result;
            var screenshotData    = screenshotRequest.Content.ReadAsStringAsync().Result;

            Console.WriteLine(url);
            Console.WriteLine(screenshotData);

            var screenshotExists  = JsonConvert.DeserializeObject<ExistingScreenshotList>(screenshotData);
            var screenshotRet     = new ExistingScreenshotList
            {
                urls = new List<ExistingScreenshot>(),
                last = screenshotExists.last
            };

            var first = screenshotExists.urls.FirstOrDefault(x => x.key.EndsWith("/1"));
            if (first != null)
                screenshotRet.urls.Add(first);

            var last = screenshotExists.urls.FirstOrDefault(x => x.key == screenshotExists.last && !x.key.EndsWith("/1"));
            if (last != null)
                screenshotRet.urls.Add(last);

            return screenshotRet;
        }

        public List<SourcesExt> Sources()
        {
            using (var client = new LLCContext())
            {
                var sources = client.Sources.Select(x => new SourcesExt {
                        AllowLinkChecking = x.AllowLinkChecking,
                        AllowLinkExtractions = x.AllowLinkExtractions,
                        DateCreated = x.DateCreated,
                        Description = x.Description,
                        Id = x.Id,
                        Name = x.Name,
                        S3bucketId = x.S3bucketId
                    })
                    .ToList();

                foreach (var m in sources)
                {
                    // Skip the internal sources
                    if (m.S3bucketId != null)
                    {
                        var bucket = client.Buckets.FirstOrDefault(x => x.Id == m.S3bucketId);
                        m.S3bucketName = bucket.Name;
                        m.S3bucketSearchPrefix = bucket.SearchPrefix;
                        Console.WriteLine($"S3 object name is {m.S3bucketName}");
                    }
                }

                sources = sources.OrderBy(x => x.Id).ToList();
                return sources;
            }
        }

        public SourcesExt Source(string id, SearchType type)
        {
            var results = Sources();
            switch (type)
            {
                case SearchType.Id:
                    return results.FirstOrDefault(x => x.Id.Equals(id));
                case SearchType.Name:
                    return results.FirstOrDefault(x => id.Equals(x.S3bucketName, StringComparison.CurrentCultureIgnoreCase));
                default:
                    return null;
            }
        }

        public Save SaveSource(SourcesExt source)
        {
            Console.WriteLine($"ID is {source.Id} and Bucket ID is {source.S3bucketId}");
            Console.WriteLine($"ID is null ? {string.IsNullOrEmpty(source.Id)} and Bucket ID is null? {string.IsNullOrEmpty(source.S3bucketId)}");
            Console.WriteLine($"Check is {source.AllowLinkChecking} and Extractions is {source.AllowLinkExtractions}");

            // If Id or S3BucketId are both not null or occupied, error!
            if ((string.IsNullOrEmpty(source.Id) && !string.IsNullOrEmpty(source.S3bucketId)) ||
                (!string.IsNullOrEmpty(source.Id) && string.IsNullOrEmpty(source.S3bucketId))) {
                return new Save { Status = false };
            }

            using (var client = new LLCContext())
            {
                if (string.IsNullOrEmpty(source.Id) && string.IsNullOrEmpty(source.S3bucketId))
                {
                    Console.WriteLine("Adding source");

                    var bucket = Guid.NewGuid().ToString();
                    var sid    = Guid.NewGuid().ToString();
                    var now    = DateTime.Now;
                    client.Buckets.Add(new Buckets
                    {
                        AccessKey = source.AccessKey,
                        DateCreated = now,
                        Id = bucket,
                        Name = source.S3bucketName,
                        Region = source.Region,
                        SearchPrefix = source.S3bucketSearchPrefix,
                        SecretKey = source.SecretKey
                    });

                    client.Sources.Add(new Sources
                    {
                        AllowLinkChecking = source.AllowLinkChecking == null ? false : source.AllowLinkChecking,
                        AllowLinkExtractions = source.AllowLinkExtractions == null ? false : source.AllowLinkExtractions,
                        Id = sid,
                        DateCreated = now,
                        Description = source.Description,
                        Name = source.Name,
                        S3bucketId = bucket
                    });
                }
                else
                {
                    Console.WriteLine("Update source");

                    client.Buckets.Update(new Buckets
                    {
                        AccessKey = source.AccessKey,
                        Id = source.S3bucketId,
                        Name = source.S3bucketName,
                        Region = source.Region,
                        SearchPrefix = source.S3bucketSearchPrefix,
                        SecretKey = source.SecretKey
                    });

                    client.Sources.Update(new Sources
                    {
                        AllowLinkChecking = source.AllowLinkChecking,
                        AllowLinkExtractions = source.AllowLinkExtractions,
                        Id = source.Id,
                        Description = source.Description,
                        Name = source.Name,
                        S3bucketId = source.S3bucketId
                    });
                }

                try
                {
                    client.SaveChanges();
                    return new Save { Status = true };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new Save { Status = false };
                }
            }
        }

        public Save DeleteSource(SourcesExt source)
        {
            using (var client = new LLCContext())
            {
                Console.WriteLine(JsonConvert.SerializeObject(source));

                var obj = Source(source.Id, SearchType.Id);
                client.Buckets.Remove(new Buckets
                {
                    Id = obj.S3bucketId
                });

                client.Sources.Remove(new Sources
                {
                    Id = source.Id,
                    S3bucketId = obj.S3bucketId
                });

                try
                {
                    client.SaveChanges();
                    return new Save { Status = true };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new Save { Status = false };
                }
            }
        }

        public List<SettingsExt> Settings()
        {
            using (var client = new LLCContext())
            {
                var settings = client.Settings
                    .Select(x => new SettingsExt
                    {
                        DateCreated = x.DateCreated,
                        DateModified = x.DateModified,
                        Description = x.Description,
                        Id = x.Id,
                        ModifiedUser = x.ModifiedUser,
                        Name = x.Name,
                        Value = x.Value
                    })
                    .ToList();

                settings = settings.OrderBy(x => x.Name).ToList();
                return settings;
            }
        }

        public SettingsExt Setting(string id, SearchType type)
        {
            var results = Settings();
            switch (type)
            {
                case SearchType.Id:
                    return results.FirstOrDefault(x => x.Id.Equals(id));
                case SearchType.Name:
                    return results.FirstOrDefault(x => id.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                default:
                    return null;
            }
        }

        public Save SaveSetting(SettingsExt setting, User user)
        {
            Console.WriteLine($"ID is {setting.Id}");
            Console.WriteLine($"ID is null ? {string.IsNullOrEmpty(setting.Id)}");

            var now = DateTime.Now;
            using (var client = new LLCContext())
            {
                if (string.IsNullOrEmpty(setting.Id))
                {
                    Console.WriteLine("Adding setting");

                    var id = Guid.NewGuid().ToString();
                    client.Settings.Add(new Settings
                    {
                        DateCreated = now,
                        DateModified = now,
                        Description = setting.Description,
                        Id = id,
                        ModifiedUser = user.Email,
                        Name = setting.Name,
                        Value = setting.Value
                    });
                }
                else
                {
                    Console.WriteLine("Update setting");

                    client.Settings.Update(new Settings
                    {
                        DateModified = now,
                        Description = setting.Description,
                        Id = setting.Id,
                        ModifiedUser = user.Email,
                        Name = setting.Name,
                        Value = setting.Value
                    });
                }

                try
                {
                    client.SaveChanges();
                    return new Save { Status = true };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new Save { Status = false };
                }
            }
        }

        public Save DeleteSetting(SettingsExt setting)
        {
            using (var client = new LLCContext())
            {
                Console.WriteLine(JsonConvert.SerializeObject(setting));

                client.Settings.Remove(new Settings
                {
                    Id = setting.Id
                });

                try
                {
                    client.SaveChanges();
                    return new Save { Status = true };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new Save { Status = false };
                }
            }
        }

        public List<PackagesExt> Packages()
        {
            using (var client = new LLCContext())
            {
                var packages = client.Packages
                    .Select(x => new PackagesExt
                    {
                        DateUploaded = x.DateUploaded,
                        Delete = false,
                        Description = x.Description,
                        FileName = x.FileName,
                        Id = x.Id,
                        ImsDescription = x.ImsDescription,
                        ImsSchema = x.ImsSchema,
                        ImsSchemaVersion = x.ImsSchemaVersion,
                        ImsTitle = x.ImsTitle,
                        Key = x.Key,
                        Name = x.Name,
                        PackageProcessed = x.PackageProcessed,
                        UploadedBy = x.UploadedBy
                    })
                    .ToList();


                return packages;
            }
        }

        public PackagesExt Package(string id)
        {
            using (var client = new LLCContext())
            {
                var package = client.Packages.FirstOrDefault(x => x.Id == id);
                return new PackagesExt
                {
                    DateUploaded = package.DateUploaded,
                    Delete = false,
                    Description = package.Description,
                    FileName = package.FileName,
                    Id = package.Id,
                    ImsDescription = package.ImsDescription,
                    ImsSchema = package.ImsSchema,
                    ImsSchemaVersion = package.ImsSchemaVersion,
                    ImsTitle = package.ImsTitle,
                    Key = package.Key,
                    Name = package.Name,
                    PackageProcessed = package.PackageProcessed,
                    UploadedBy = package.UploadedBy
                };
            }
        }

        public Save SavePackage(PackagesExt package, User user)
        {
            Console.WriteLine($"ID is {package.Id}");
            Console.WriteLine($"ID is null ? {string.IsNullOrEmpty(package.Id)}");

            var now = DateTime.Now;
            using (var client = new LLCContext())
            {
                if (!string.IsNullOrEmpty(package.Id))
                {
                    Console.WriteLine("Adding package");
                    client.Packages.Add(new Packages
                    {
                        DateUploaded = now,
                        Description = package.Description,
                        FileName = package.FileName,
                        Id = package.Id,
                        Key = package.Key,
                        Name = package.Name,
                        PackageProcessed = false,
                        UploadedBy = user.Name
                    });
                }

                try
                {
                    client.SaveChanges();
                    return new Save { Status = true };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new Save { Status = false };
                }
            }
        }

        public Save DeletePackage(PackagesExt setting)
        {
            return null;
        }

        public List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m)
        {
            using (var client = new LLCContext())
            {
                var buckets  = Buckets();
                var response = new List<BucketLocationsModel>();
                var links    = client.ObjectLinks.Where(x => x.Link == m.id);
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        var obj = client.Objects.FirstOrDefault(x => x.Id == link.Object);
                        response.Add(new BucketLocationsModel
                        {
                            data = string.Format("https://{0}.s3.amazonaws.com/{1}", buckets.FirstOrDefault(x => x.Id == obj.Bucket).Name, obj.Key)
                        });
                    }
                }

                return response;
            }
        }

        public void EnqueueObjects<T>(List<T> objects) where T : Models.Dynamo.ReceiptBase
        {
            if (objects.Count == 0)
                return;

            using (var sqsClient = new AmazonSQSClient())
            {
                var success = 0;
                var failed  = 0;
                var count   = objects.Count < _maxQueue ? 1 : (int)Math.Ceiling(((double)objects.Count) / ((double)_maxQueue));
                for (var i = 0; i < count; i++)
                {
                    var result = sqsClient.SendMessageBatchAsync(new SendMessageBatchRequest
                    {
                        Entries = objects.Skip(i * _maxQueue).Take(_maxQueue).Select(x => new SendMessageBatchRequestEntry
                        {
                            Id = Guid.NewGuid().ToString(),
                            MessageBody = JsonConvert.SerializeObject(x)
                        })
                        .ToList(),
                        QueueUrl = Environment.GetEnvironmentVariable("Queue")
                    }).Result;

                    success += result.Successful.Count;
                    failed += result.Failed.Count;
                }

                Console.WriteLine($"Successful message count is {success}");
                Console.WriteLine($"Failed message count is {failed}");
            }
        }

        public List<T> DequeueObjects<T>() where T : Models.Dynamo.ReceiptBase
        {
            using (var sqsClient = new AmazonSQSClient())
            {
                var list = new List<T>();

                var messages = sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    MaxNumberOfMessages = _maxDequeue,
                    QueueUrl = Environment.GetEnvironmentVariable("Queue")
                });

                foreach (var message in messages.Result.Messages)
                {
                    var obj = JsonConvert.DeserializeObject<T>(message.Body);
                    obj.ReceiptHandle = message.ReceiptHandle;
                    list.Add(obj);
                }

                return list;
            }
        }

        public void RemoveObjectsFromQueue<T>(List<T> objects) where T : Models.Dynamo.ReceiptBase
        {
            if (objects != null && objects.Count > 0)
            {
                Console.WriteLine($"Removing list {JsonConvert.SerializeObject(objects)}");
                using (var sqsClient = new AmazonSQSClient())
                {
                    var result = sqsClient.DeleteMessageBatchAsync(new DeleteMessageBatchRequest
                    {
                        Entries = objects.Select(x => new DeleteMessageBatchRequestEntry
                        {
                            Id = Guid.NewGuid().ToString(),
                            ReceiptHandle = x.ReceiptHandle
                        }).ToList(),
                        QueueUrl = Environment.GetEnvironmentVariable("Queue")
                    }).Result.Successful;
                }
            }
        }

        public bool QueueEmpty()
        {
            using (var sqsClient = new AmazonSQSClient())
            {
                var attr = sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
                {
                    AttributeNames = new List<string> { "ApproximateNumberOfMessages" },
                    QueueUrl = Environment.GetEnvironmentVariable("Queue")
                });

                Console.WriteLine($"Queue count: {0}", attr.Result.ApproximateNumberOfMessages);
                return attr.Result.ApproximateNumberOfMessages == 0;
            }
        }

        public List<Buckets> Buckets()
        {
            using (var client = new LLCContext())
            {
                var buckets = client.Buckets.ToList();
                return buckets;
            }
        }

        public GetObjectResponse ObjectGet(string bucket, string key)
        {
            using (var client = new AmazonS3Client(_region))
            {
                return client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                }).Result;
            }
        }

        public PutObjectResponse ObjectPut<T>(string bucket, string key, T obj)
        {
            using (var client = new AmazonS3Client(_region))
            {
                return client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = key,
                    ContentType = "application/json",
                    InputStream = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj)))
                }).Result;
            }
        }

        public PutObjectResponse FilePut(string bucket, string key, Stream stream)
        {
            using (var client = new AmazonS3Client(_region))
            {
                return client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = key,
                    InputStream = stream
                }).Result;
            }
        }
    }
}
