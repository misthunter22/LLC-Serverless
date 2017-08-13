using System;

namespace SAM.Models.Admin
{
    public class SettingModel
    {
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public string Description { get; set; }

        public string ModifiedUser { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
