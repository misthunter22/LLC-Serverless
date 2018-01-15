using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM.Models
{
    public class NewScreenshot
    {
        public string hash { get; set; }

        public string key { get; set; }

        public string bucket { get; set; }

        public string url { get; set; }
    }

    public class ExistingScreenshotList
    {
        public List<ExistingScreenshot> urls { get; set; }

        public string last { get; set; }
    }

    public class ExistingScreenshot
    {
        public string s_100 { get; set; }

        public string s_200 { get; set; }

        public string s_320 { get; set; }

        public string s_400 { get; set; }

        public string s_640 { get; set; }

        public string s_800 { get; set; }

        public string s_1024 { get; set; }

        public string s_1024x768 { get; set; }

        public string s_320x240 { get; set; }

        public string s_640x480 { get; set; }

        public string s_800x600 { get; set; }

        public string s_original { get; set; }

        public string key { get; set; }
    }
}
