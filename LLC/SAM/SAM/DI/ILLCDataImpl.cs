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

namespace SAM.DI
{
    public class ILLCDataImpl : ILLCData
    {
        protected RegionEndpoint _region = RegionEndpoint.USWest2;

        public RegionEndpoint Region()
        {
            return _region;
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

        public List<Reports> InvalidLinks()
        {
            using (var client = new LLCContext())
            {
                var results = client.Reports.Where(x => x.ReportType == "Invalid").ToList();
                return results;
            }
        }

        public List<Reports> WarningLinks()
        {
            using (var client = new LLCContext())
            {
                var results = client.Reports.Where(x => x.ReportType == "Warning").ToList();
                foreach (var r in results)
                {
                    r.Obj = Link(r.Link);
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

        public List<Sources> Sources()
        {
            using (var client = new LLCContext())
            {
                var sources = client.Sources.ToList();
                foreach (var m in sources)
                {
                    // Skip the internal sources
                    if (m.S3bucketId != null)
                    {
                        var bucket = client.Buckets.FirstOrDefault(x => x.Id == m.S3bucketId);
                        m.S3ObjectName = bucket.Name;
                        m.S3BucketSearchPrefix = bucket.SearchPrefix;
                        Console.WriteLine($"S3 object name is {m.S3ObjectName}");
                    }
                }

                return sources.OrderBy(x => x.Id).ToList();
            }
        }

        public Sources Source(string id, SourceSearchType type)
        {
            using (var client = new LLCContext())
            {
                var results = Sources();
                switch (type)
                {
                    case SourceSearchType.Id:
                        return results.FirstOrDefault(x => x.Id.Equals(id));
                    case SourceSearchType.Name:
                        return results.FirstOrDefault(x => id.Equals(x.S3ObjectName, StringComparison.CurrentCultureIgnoreCase));
                    default:
                        return null;
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

        public List<Settings> Settings()
        {
            using (var client = new LLCContext())
            {
                var settings = client.Settings.ToList();
                return settings.OrderBy(x => x.Name).ToList();
            }
        }

        public Settings Settings(string name)
        {
            var settings = Settings();
            return settings.FirstOrDefault(x => x.Name == name);
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
