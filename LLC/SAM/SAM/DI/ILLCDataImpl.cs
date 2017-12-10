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
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SAM.Models.EF;
using SAM.Models.Auth;
using System.Security.Claims;

namespace SAM.DI
{
    public class ILLCDataImpl : ILLCData
    {
        protected RegionEndpoint _region = RegionEndpoint.USWest2;

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
            using (var client = new LLCContext())
            {
                var stats = new List<StatsExt>();
                var sources = Sources();
                foreach (var s in sources)
                {
                    if (s.S3bucketId == null)
                        continue;

                    var html = (from m in client.Objects
                                where m.Bucket == s.S3bucketId && m.ItemName.Contains(".htm")
                                select m).Count();

                    var objects = (from m in client.Objects
                                   where m.Bucket == s.S3bucketId
                                   select m).Count();

                    var extracted = (from m in client.Objects
                                     where m.Bucket == s.S3bucketId
                                     select m).Max(x => x.DateLinksLastExtracted);

                    var invalid = (from m in client.Links
                                   where m.Source == s.Id && m.Valid == false
                                   select m).Count();

                    var total = (from m in client.Links
                                 where m.Source == s.Id
                                 select m).Count();

                    var chked = (from m in client.Links
                                 where m.Source == s.Id
                                 select m).Max(x => x.DateLastChecked);

                    var ext = new StatsExt
                    {
                        HtmlFiles = html,
                        InvalidLinks = invalid,
                        LastChecked = chked,
                        LastExtracted = extracted,
                        Objects = objects,
                        Source = s.Name,
                        TotalLinks = total
                    };

                    stats.Add(ext);
                }

                return stats;
            }
        }

        public int ObjectsCount()
        {
            using (var client = new LLCContext())
            {
                var count = client.Objects.Count();
                return count;
            }
        }

        public Objects Object(string id)
        {
            using (var client = new LLCContext())
            {
                return client.Objects.FirstOrDefault(x => x.Id == id);
            }
        }

        public Objects ObjectFromKey(string key)
        {
            using (var client = new LLCContext())
            {
                return client.Objects.FirstOrDefault(x => x.Key == key);
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

        public List<Objects> LinkExtractor(string bucket, int offset, int maximum)
        {
            using (var client = new LLCContext())
            {
                var result = client.Objects.Where(
                    x => x.Bucket == bucket &&
                    x.DisabledDate == null &&
                    x.Key.Contains(".htm") &&
                    x.IsFolder == false &&
                    (x.DateLinksLastExtracted == null || x.DateLinksLastExtracted < x.ContentLastModified))
                  .Skip(offset)
                  .Take(maximum)
                  .ToList();

                return result;
            }
        }

        public int LinksCount()
        {
            using (var client = new LLCContext())
            {
                var count = client.Links.Count();
                return count;
            }
        }

        public Links Link(string id)
        {
            using (var client = new LLCContext())
            {
                return client.Links.FirstOrDefault(x => x.Id == id);
            }
        }

        public List<ReportsExt> InvalidLinks()
        {
            using (var client = new LLCContext())
            {
                var results = client.Reports.Where(x => x.ReportType == "Invalid")
                    .Select(x => new ReportsExt
                    {
                        ContentSize = x.ContentSize,
                        Id = x.Id,
                        Link = x.Link,
                        Mean = x.Mean,
                        ReportType = x.ReportType,
                        SdMaximum = x.SdMaximum,
                        StandardDeviation = x.StandardDeviation,
                        Stat = x.Stat
                    })
                    .ToList();

                return results;
            }
        }

        public List<ReportsExt> WarningLinks()
        {
            using (var client = new LLCContext())
            {
                var results = client.Reports.Where(x => x.ReportType == "Warning")
                    .Select(x => new ReportsExt {
                        ContentSize = x.ContentSize,
                        Id = x.Id,
                        Link = x.Link,
                        Mean = x.Mean,
                        ReportType = x.ReportType,
                        SdMaximum = x.SdMaximum,
                        StandardDeviation = x.StandardDeviation,
                        Stat = x.Stat
                    })
                    .ToList();

                foreach (var r in results)
                {
                    var link          = Link(r.Link);
                    r.AttemptCount    = link.AttemptCount;
                    r.DateLastChecked = link.DateLastChecked;
                    r.DateLastFound   = link.DateLastFound;
                    r.Source          = link.Source;
                    r.Url             = link.Url;
                }

                return results;
            }
        }

        public Links LinkFromUrl(string url)
        {
            using (var client = new LLCContext())
            {
                return client.Links.FirstOrDefault(x => x.Url == url);
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

        public List<Links> LinkChecker(string source, int offset, int maximum)
        {
            using (var client = new LLCContext())
            {
                var result = client.Links.Where(x => x.Source == source)
                    .Skip(offset)
                    .Take(maximum)
                    .ToList();

                return result;
            }
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

                return sources.OrderBy(x => x.Id).ToList();
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

        public List<Buckets> Buckets()
        {
            using (var client = new LLCContext())
            {
                var buckets = client.Buckets.ToList();
                return buckets;
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

                return settings.OrderBy(x => x.Name).ToList();
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

        public List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m)
        {
            using (var client = new AmazonDynamoDBClient(_region))
            {
                var buckets = Buckets();

                var id = m.id;
                if ("stat".Equals(m.type))
                {
                    //id = QueryDataAttribute("LLC-Stats", id, "Link").Result.S;
                }

                var ret = new List<string>();
                var last = new Dictionary<string, AttributeValue>();
                last["Link"] = new AttributeValue { S = "0" };

                while (last.ContainsKey("Link"))
                {
                    var rows = client.ScanAsync(new ScanRequest
                    {
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            { ":val",  new AttributeValue { S = id } }
                        },
                        FilterExpression = "Link = :val",
                        TableName = "LLC-ObjectLinks",
                        ExclusiveStartKey = last["Link"].S == "0" ? null : last
                    }).Result;

                    last = rows.LastEvaluatedKey;
                    ret.AddRange(rows.Items.Select(x => x.ContainsKey("Object") ? x["Object"].S : "").ToList());
                }

                var resp = new List<BucketLocationsModel>();
                foreach (var o in ret)
                {
                    var obj = client.QueryAsync(new QueryRequest
                    {
                        TableName = "LLC-Objects",
                        KeyConditionExpression = "Id = :v_Id",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            { ":v_Id", new AttributeValue { S =  o }}
                        }
                    });

                    var item = obj.Result.Items.FirstOrDefault();
                    resp.Add(new BucketLocationsModel
                    {
                        data = string.Format("https://{0}.s3.amazonaws.com/{1}", buckets.FirstOrDefault(x => x.Id == item["Bucket"].S).Name, item["Key"].S)
                    });
                }

                return resp;
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
    }
}
