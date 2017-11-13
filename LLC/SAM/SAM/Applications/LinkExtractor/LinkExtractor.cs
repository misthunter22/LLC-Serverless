using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SAM.Applications.LinkExtractor
{
    public class LinkExtractor : BaseHandler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            var objs = Service.DequeueObjects();
            if (objs == null)
                return;

            var buckets = Service.Buckets();
            var sources = Service.Sources();

            foreach (var obj in objs)
            {
                // Get the object's file contents
                var bucket  = buckets.FirstOrDefault(x => x.Id == obj.Id);
                var source  = sources.FirstOrDefault(x => x.S3BucketId == bucket.Id);
                var content = Service.ObjectGet(bucket.Name, obj.Key);

                // Find any links 
                if (content != null)
                {
                    var reader = new StreamReader(content.ResponseStream);
                    string text = reader.ReadToEnd();

                    MatchCollection matchList = R.Matches(text);
                    if (matchList.Count > 0)
                    {
                        // One or more links were found so we'll include each in the bulk update
                        foreach (Match m in matchList)
                        {
                            // Add the link to the bulk data table
                            //row = dtLinks.NewRow();
                            //row["SourceId"] = sourceId;
                            //row["Key"] = objectRow["Key"].GetNullableString();

                            // Allow send through URLs up to 1024 in length to avoid errors
                            string url = m.Groups[1].Value;
                            //row["Url"] = (url.Length >= _maxUrlLength ? url.Substring(0, _maxUrlLength - 1) : url);

                            //dtLinks.Rows.Add(row);
                            //linkCount++;
                        }
                    }
                    else // No links found but we'll include the object in the update without a url
                    {
                        // Add the link to the bulk data table
                        //row = dtLinks.NewRow();
                        //row["SourceId"] = sourceId;
                        //row["Key"] = objectRow["Key"].GetNullableString();
                        //row["Url"] = string.Empty;
                        //dtLinks.Rows.Add(row);
                    }
                }
            }

            Service.RemoveObjectsFromQueue(objs);
        }
    }
}
