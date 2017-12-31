using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using DbCore.Models;
using SAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SAM.Applications.LinkChecker
{
    public class LinkChecker : BaseHandler
    {
        private string ScreenshotUrl = Environment.GetEnvironmentVariable("Screenshot") + "screenshots";

        private string ApiKey = Environment.GetEnvironmentVariable("ApiKey");

        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handler(object input, ILambdaContext context)
        {
            var objs   = Service.DequeueObjects<LinksExt>();
            var report = new List<Reports>();

            if (objs == null)
                return;

            foreach (var obj in objs)
            {
                AnalyzeLink(obj);
                ComputeScreenshot(obj, report);
            }

            Service.RemoveObjectsFromQueue(objs);
        }

        // Iterate through the list of links and analyze/store their stats
        public void AnalyzeLink(LinksExt link)
        {
            Stats newStat = null;
            var now       = DateTime.Now;
            var linkValid = true;

            try
            {
                // Setup the web request and execute the get
                var url     = link.Url;
                var request = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(1)
                };

                // Start timing the response
                var downloadStart = DateTime.Now;
                var response      = request.GetAsync(new Uri(url)).Result;

                // Extract the status information
                var statusCode  = response.StatusCode.ToString();
                var statusDesc  = response.ReasonPhrase;
                var contentType = response.Content.Headers.ContentType.MediaType;

                // Retrieve the actual contents
                var pageContents = response.Content.ReadAsStringAsync().Result;

                // Stop timing the response
                var downloadDuration = DateTime.Now.Subtract(downloadStart);

                // Create new stat
                newStat = new Stats
                {
                    ContentSize = pageContents == null ? (long?)null : pageContents.Length,
                    ContentType = contentType,
                    DateChecked = now,
                    DownloadTime = (int)downloadDuration.TotalMilliseconds,
                    Error = null,
                    Id = Guid.NewGuid().ToString(),
                    Link = link.Id,
                    StatusCode = statusCode,
                    StatusDescription = statusDesc
                };

                if (!"OK".Equals(statusCode, StringComparison.CurrentCultureIgnoreCase))
                    linkValid = false;
            }
            catch (HttpRequestException ex)
            {
                // A web exception occurred so we can get a little more detail about the failure
                // Add a row to indicate the link is bad
                newStat = new Stats
                {
                    DateChecked = now,
                    Error = ex.Message,
                    Id = Guid.NewGuid().ToString(),
                    Link = link.Id,
                    StatusCode = "500",
                    StatusDescription = ex.InnerException != null ? ex.InnerException.Message : string.Empty
                };

                linkValid = false;
            }
            catch (Exception ex)
            {
                // Catch any errors caused by bad URLs
                // Add a row to indicate the link is bad
                newStat = new Stats
                {
                    DateChecked = now,
                    Error = ex.Message,
                    Id = Guid.NewGuid().ToString(),
                    Link = link.Id,
                    StatusCode = "500",
                    StatusDescription = ex.InnerException != null ? ex.InnerException.Message : string.Empty
                };

                linkValid = false;
            }

            // Save if we have an object
            if (newStat != null)
            {
                Service.AddStat(newStat);
            }

            var report = Service.Report(link.Id);
            var l      = Service.Link(link.Id);

            // Update the link if it's not valid
            if (!linkValid)
            {
                Console.WriteLine($"Found invalid link {link.Id}");
                l.Valid = false;
                l.AttemptCount += 1;

                if (report == null)
                {
                    Console.WriteLine("Adding invalid report");
                    report = new Reports
                    {
                        Id = Guid.NewGuid().ToString(),
                        Link = link.Id,
                        ReportType = "Invalid",
                        Stat = newStat.Id
                    };

                    Service.AddReport(report);
                }
                else
                {
                    Console.WriteLine("Updating invalid report");
                    report.Stat = newStat.Id;
                    Service.SetReport(report);
                }
            }
            else
            {
                Console.WriteLine("Removing invalid report");
                Service.RemoveReport(link.Id);
            }

            // Compute stats
            var stats = Service.LinkStats(link);
            var sum   = stats.Sum(x => x.DownloadTime);
            var mean  = sum / stats.Count;
            var sd    = sum / mean;

            var week  = stats.Where(x => x.DateChecked > DateTime.Now.AddDays(-7)).ToList();
            var wsum  = week.Sum(x => x.DownloadTime);
            var wmean = wsum / week.Count;
            var wsd   = wsum / wmean;

            l.DateLastChecked = now;
            l.DateUpdated     = now;

            l.AllTimeMaxDownloadTime    = stats.Max(x => x.DownloadTime);
            l.AllTimeMinDownloadTime    = stats.Min(x => x.DownloadTime);
            l.AllTimeStdDevDownloadTime = sd;

            l.PastWeekMaxDownloadTime    = week.Max(x => x.DownloadTime);
            l.PastWeekMinDownloadTime    = week.Min(x => x.DownloadTime);
            l.PastWeekStdDevDownloadTime = wsd;

            Service.SetLink(l);
        }

        private void ComputeScreenshot(LinksExt link, List<Reports> reports)
        {
            // Grab all of the link stats for the link
            var newCheck = Service.LinkStats(link);

            // Compute the number of days back to check links for
            var days  = int.Parse(Service.Setting("LinkCheckDays", Models.Admin.SearchType.Name).Value) * -1;
            var t     = DateTime.Today.AddDays(days);
            var stats = newCheck
                .Where(x => x.DateChecked > t)
                .OrderByDescending(x => x.DateChecked)
                .ToList();

            Console.WriteLine($"Processing URL {link.Url}");
            Console.WriteLine($"Found {stats.Count} stats");

            // Grab the first (bottom one) to work from
            var first  = stats.FirstOrDefault();
            var client = ScreenshotClient();

            // Compute the standard deviation and mean for the links
            var mean              = ComputeMean(stats);
            var standardDeviation = ComputeStandardDeviation(stats, mean);
            var sdRange           = int.Parse(Service.Setting("LinkCheckStandardDeviations", Models.Admin.SearchType.Name).Value);
            var existingLink      = Service.Report(link.Id);
            var takeScreenshot    = false;

            // See if the standard deviation requires a notification to be sent out
            // If the first does not exist, not much we can do! Go ahead and bail out
            if (first != null && first.ContentType != null && first.ContentType.Contains("text/html"))
            {
                var screenshotRequest = client.GetAsync(new Uri(ScreenshotUrl + "?url=" + link.Url)).Result;
                var screenshotData    = screenshotRequest.Content.ReadAsStringAsync().Result;
                var screenshotExists  = Newtonsoft.Json.JsonConvert.DeserializeObject<ExistingScreenshot>(screenshotData);
                Console.WriteLine($"Existing screenshot? {screenshotData}");

                // Check to see if no screenshots exist
                if (screenshotExists.s_original == null)
                {
                    takeScreenshot = true;
                }

                // Significant change?
                if (first.ContentSize > (mean + (sdRange * standardDeviation)) ||
                    first.ContentSize < (Math.Abs(mean - (sdRange * standardDeviation))))
                {
                    Console.WriteLine("Found significant change");
                    takeScreenshot = true;

                    // Create new since one doesn't exist
                    if (existingLink == null)
                    {
                        existingLink = new Reports();
                    }

                    existingLink.ContentSize       = first.ContentSize;
                    existingLink.Link              = first.Link;
                    existingLink.Mean              = mean;
                    existingLink.SdMaximum         = sdRange;
                    existingLink.StandardDeviation = standardDeviation;
                    existingLink.Stat              = first.Id;
                    existingLink.ReportType        = "Warning";

                    Service.AddReport(existingLink);
                }
                else if (existingLink != null)
                {
                    // Remove link
                    Console.WriteLine("Link no longer significantly impacted. Removing from report list");
                    Service.RemoveReport(existingLink);
                }
            }

            // Send screenshot request
            if (takeScreenshot)
            {
                var ss = client.PostAsync(ScreenshotUrl + "?url=" + link.Url, new StringContent(string.Empty)).Result;
                Console.WriteLine($"Performed screenshot: {ss.Content.ReadAsStringAsync().Result}");
            }
        }

        private HttpClient ScreenshotClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1)
            };

            client.DefaultRequestHeaders.Add("x-api-key", ApiKey);
            return client;
        }

        private long ComputeMean(List<Stats> stats)
        {
            // Compute mean
            long mean = 0;
            foreach (var size in stats)
            {
                if (size.ContentSize != null)
                    mean += (long)size.ContentSize;
            }

            if (stats.Count > 0)
                mean = mean / stats.Count;

            return mean;
        }

        private double ComputeStandardDeviation(List<Stats> stats, long mean)
        {
            // Compute the variance off the mean
            double variance = 0;
            var varianceCount = 0;

            if (mean > 0)
            {
                foreach (var size in stats)
                {
                    // Count how many valid values exist. What if they are not
                    // all valid? At least one must be since the mean is > 0
                    if (size.ContentSize != null)
                    {
                        variance += Math.Pow((long)size.ContentSize - mean, 2);
                        varianceCount++;
                    }
                }

                variance = variance / varianceCount;
            }

            var standardDeviation = Math.Sqrt(variance);
            return standardDeviation;
        }
    }
}
