using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visual.Domain
{
    public class Site
    {
        public int? SiteId;
        public string SiteName;
        public string SiteKey;

        public string ProductKey;

        public bool AllowSignup;

        public int? LicenseId;

        public string Domain;
    }
}
