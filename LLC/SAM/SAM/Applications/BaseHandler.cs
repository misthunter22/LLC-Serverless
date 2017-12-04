using Amazon.DynamoDBv2.DocumentModel;
using DbCore.Models;
using Newtonsoft.Json;
using SAM.DI;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SAM.Applications
{
    public abstract class BaseHandler
    {
        protected ILLCData Service { get; set; }

        protected readonly int MaxUrlLength = 1024;

        protected Regex R = new Regex("<a[^>]* href=\"(http[^\"]*)\">([^<]+)</a>", RegexOptions.IgnoreCase);

        protected BaseHandler()
        {
            Service = new ILLCDataImpl();
        }

        protected void AddToDocument(Document item, string key, object value)
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

        protected void LinkExtractions(Objects obj, string name, string source)
        {
            var date = DateTime.Now;

            // Peform any link extractions
            var content = Service.ObjectGet(name, obj.Key);

            // Find any links 
            if (content == null)
                return;

            Console.WriteLine("Found S3 content");

            var reader = new StreamReader(content.ResponseStream);
            var text = reader.ReadToEnd();

            MatchCollection matchList = R.Matches(text);

            if (matchList.Count > 0)
            {
                Console.WriteLine("Found HTML Regex matches");

                // One or more links were found so we'll include each in the bulk update
                foreach (Match m in matchList)
                {
                    // Allow send through URLs up to 1024 in length to avoid errors
                    var url = m.Groups[1].Value;
                    url = (url.Length >= MaxUrlLength ? url.Substring(0, MaxUrlLength - 1) : url);
                    Console.WriteLine($"Found URL: {url}");

                    var row = new Links
                    {
                        Id = Guid.NewGuid().ToString(),
                        DateLastFound = date,
                        Source = source,
                        Url = url
                    };

                    var existingLink = Service.LinkFromUrl(url);
                    if (existingLink == null)
                    {
                        row.DateFirstFound = date;
                    }
                    else
                    {
                        row = existingLink;
                        row.DateLastFound = date;
                    }

                    var r = Service.SetLink(row);
                    Console.WriteLine(JsonConvert.SerializeObject(r));
                }
            }
        }
    }
}
