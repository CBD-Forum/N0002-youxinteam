using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class UserRole
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }

        public string parentId { get; set; }

        public bool used { get; set; }
    }
}
