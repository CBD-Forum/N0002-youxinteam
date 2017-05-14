using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class User
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string EMail { get; set; }

        public string Telephone { get; set; }

        public string Weixin { get; set; }
        public string PasswordMd5 { get; set; }

        public bool Forbidden { get; set; }

        public List<Role> Roles { get; set; }

        public User()
        {
            this.Roles = new List<Role>();
        }
    }
}
