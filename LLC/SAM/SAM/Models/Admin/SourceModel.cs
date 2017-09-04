using System;

namespace SAM.Models.Admin
{
    public class SourceModel
    {
        public int Source { get; set; }

        public bool? AllowLinkChecking { get; set; }

        public bool? AllowLinkExtractions { get; set; }

        public string DateCreated { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string DateLastChecked { get; set; }

        public string DateLastExtracted { get; set; }

        public string S3ObjectName { get; set; }

        public int S3BucketId { get; set; }

        public int S3ObjectCount { get; set; }

        public int LinkCount { get; set; }

        public int InvalidLinkCount { get; set; }

        public int HtmlFileCount { get; set; }
    }
}
