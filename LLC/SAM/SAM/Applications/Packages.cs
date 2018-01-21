using Amazon.Lambda.Core;
using Amazon.S3.Model;
using DbCore.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml;

namespace SAM.Applications
{
    public class Pckgs : BaseHandler
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            try
            {
                Console.WriteLine(input.GetType());
                JObject obj = (JObject)input;

                // Determine the type of event (+/-)
                var record = obj["Records"][0];
                var k = record["s3"]["object"]["key"].ToString();
                var evt = record["eventName"].ToString();
                var isPut = evt.StartsWith("ObjectCreated:", StringComparison.CurrentCultureIgnoreCase);
                var isCopy = evt.EndsWith("Copy", StringComparison.CurrentCultureIgnoreCase);
                var isDel = evt.StartsWith("ObjectRemoved:", StringComparison.CurrentCultureIgnoreCase);

                Console.WriteLine(evt);
                Console.WriteLine(k);

                var t = record["s3"]["object"]["eTag"];
                var e = t != null ? t.ToString() : string.Empty;
                var b = record["s3"]["bucket"]["name"].ToString();

                // Only process if it meets the criteria
                if (isPut || isCopy)
                {
                    var o = Service.ObjectGet(b, k);
                    var d = string.Empty;
                    var n = string.Empty;

                    if (o.Metadata != null)
                    {
                        d = o.Metadata["x-amz-meta-description"];
                        n = o.Metadata["x-amz-meta-name"];
                    }

                    if (isPut && !isCopy)
                    {
                        // Set the object in the table on 
                        // the create action
                        var newId = Guid.NewGuid().ToString();
                        var date = DateTime.Now;
                        var keySplit = k.Split('/');
                        var itemName = keySplit[keySplit.Length - 1];
                        var existingRow = Service.ObjectFromKey(k);
                        Console.WriteLine($"New ID is : {newId}");
                        var newRow = new Packages
                        {
                            DateUploaded = date,
                            Description = d,
                            FileName = k.Split('/')[k.Split('/').Length - 1],
                            Id = newId,
                            Key = k,
                            Name = n,
                            PackageProcessed = true
                        };

                        // Process File Contents
                        ProcessPackage(o, newRow);
                        Service.AddPackage(newRow);

                    }
                    else if (isCopy)
                    {
                        var package = Service.PackageFromKey(k);
                        package.Name = n;
                        package.Description = d;
                        Service.SavePackage(package);
                    }
                }
                else if (isDel)
                {
                    Service.DeletePackage(k);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure, not going to allow retry");
                Console.WriteLine(ex);
            }
        }

        private void ProcessPackage(GetObjectResponse get, Packages package)
        {
            using (var stream = get.ResponseStream)
            {
                var path    = Path.GetTempPath();
                var file    = path + package.FileName;
                var extract = path + Guid.NewGuid().ToString() + Path.DirectorySeparatorChar;
                using (var fileStream = File.Create(file))
                {
                    stream.CopyTo(fileStream);
                }

                ZipFile.ExtractToDirectory(file, extract);

                // Check if it is a ZIP or a IMSCC extension
                var extension = Path.GetExtension(file);
                if (".zip".Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                {
                    ProcessZip(extract, package);
                }
                else if (".imscc".Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                {
                    ProcessImscc(extract, package);
                }
                else
                {
                    // Don't know what to do with the file!
                    Console.WriteLine($"Don't know how to process file with {extension} extension!");
                }
            }
        }

        private void ProcessZip(string path, Packages package)
        {
            Console.WriteLine($"Extracting links from zip file under path {path}...");

            var files = new List<string>();
            DirSearch(path, files);

            foreach (var f in files)
            {
                var href = f.ToLower().Replace(path.ToLower(), string.Empty);
                ProcessLink(R, path, href, package);
            }
        }

        private static void DirSearch(string d, List<string> files)
        {
            foreach (string f in Directory.GetFiles(d))
            {
                if (f.EndsWith(".html") || f.EndsWith(".htm"))
                    files.Add(f);
            }

            foreach (string dir in Directory.GetDirectories(d))
            {
                DirSearch(dir, files);
            }
        }

        private void ProcessLink(Regex r, string path, string href, Packages package)
        {
            Console.WriteLine(path + href);
            var text      = File.ReadAllText(path + href);
            var matchList = r.Matches(text);
            var source    = Service.Source("Package Uploads", Models.Admin.SearchType.SourceName);
            if (matchList.Count > 0)
            {
                // One or more links were found so we'll include each in the bulk update
                foreach (Match m in matchList)
                {
                    // Allow send through URLs up to 1024 in length to avoid errors
                    string url = m.Groups[1].Value;
                    url = (url.Length >= MaxUrlLength ? url.Substring(0, MaxUrlLength - 1) : url);

                    // Check the link for existance
                    var link = Service.LinkFromUrl(url);
                    if (link == null)
                    {
                        Console.WriteLine($"Adding link {url}");
                        var now = DateTime.Now;
                        link = new Links
                        {
                            AllTimeMaxDownloadTime = 0,
                            AllTimeMinDownloadTime = 0,
                            AllTimeStdDevDownloadTime = 0,
                            AttemptCount = 0,
                            DateFirstFound = now,
                            DateLastChecked = null,
                            DateLastFound = null,
                            DateUpdated = null,
                            DisabledDate = null,
                            DisabledUser = null,
                            Id = Guid.NewGuid().ToString(),
                            PastWeekMaxDownloadTime = 0,
                            PastWeekMinDownloadTime = 0,
                            PastWeekStdDevDownloadTime = 0,
                            ReportNotBeforeDate = null,
                            Source = source.Id,
                            Url = url,
                            Valid = true
                        };

                        Service.AddLink(link);
                    }

                    var uri       = new Uri(url);
                    var split     = href.Split(Path.DirectorySeparatorChar);
                    string parent = null;
                    if (split != null && split.Length > 1)
                    {
                        parent = split[split.Length - 2];
                    }

                    // Add the link package course location
                    var newFile = new PackageFiles
                    {
                        CourseLocation = href,
                        Id = Guid.NewGuid().ToString(),
                        Link = link.Id,
                        LinkName = m.Groups[2].Value,
                        PackageId = package.Id,
                        ParentFolder = parent,
                        Protocol = uri.Scheme
                    };

                    Service.AddPackageFile(newFile);
                }
            }
            else
            {
                // No links found but we'll include the object in the update without a url (maybe?)
            }
        }

        private void ProcessImscc(string path, Packages package)
        {
            var imsPath = string.Empty;
            var dirPath = string.Empty;
            var dirs    = Directory.GetDirectories(path);
            if (dirs != null && dirs.Length == 1)
            {
                dirPath = dirs[0];
            }

            // Make sure the imsmanifest.xml file exists
            if (File.Exists(path + "imsmanifest.xml"))
            {
                imsPath = path + "imsmanifest.xml";
            }
            else if (File.Exists(dirPath + Path.DirectorySeparatorChar + "imsmanifest.xml"))
            {
                path    = dirPath + Path.DirectorySeparatorChar;
                imsPath = path + "imsmanifest.xml";
            }
            else
            {
                Console.WriteLine("imsmanifest.xml file does not exist! Cannot continue.");
                return;
            }

            Console.WriteLine($"Extracting links from imscc file under path {path}...");

            // Load in the document
            var reader = XmlReader.Create(imsPath);
            var doc = new XmlDocument();
            doc.Load(reader);

            // Grab the root note
            var manifest = doc.GetElementsByTagName("manifest")[0];

            // Create the namespace manager
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("ns", manifest.NamespaceURI);

            var metadata = doc.SelectSingleNode("/ns:manifest/ns:metadata", ns);

            // Update the metadata for the package
            package.ImsSchema = metadata.SelectSingleNode("ns:schema", ns).InnerText;
            package.ImsSchemaVersion = metadata.SelectSingleNode("ns:schemaversion", ns).InnerText;

            if (doc.GetElementsByTagName("lomimscc:title") != null &&
                doc.GetElementsByTagName("lomimscc:title").Count > 0 &&
                doc.GetElementsByTagName("lomimscc:title").Item(0) != null)
            {
                package.ImsTitle = doc.GetElementsByTagName("lomimscc:title").Item(0).InnerText;
            }

            if (doc.GetElementsByTagName("lomimscc:description") != null &&
                doc.GetElementsByTagName("lomimscc:description").Count > 0 &&
                doc.GetElementsByTagName("lomimscc:description").Item(0) != null)
            {
                package.ImsTitle = doc.GetElementsByTagName("lomimscc:description").Item(0).InnerText;
            }

            // Process the resource nodes
            var resources = doc.SelectNodes("/ns:manifest/ns:resources/ns:resource", ns);
            for (var i = 0; i < resources.Count; i++)
            {
                var resource = resources.Item(i);
                var href = resource.Attributes != null &&
                           resource.Attributes.GetNamedItem("href") != null
                           ? resource.Attributes.GetNamedItem("href").Value : "";

                if (href.EndsWith(".htm") || href.EndsWith(".html"))
                {
                    ProcessLink(R, path, href, package);
                }
            }
        }
    }
}
