using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class LoginResult
    {
        public string Message { get; set; }
        public bool Result { get; set; }

        public string UserId { get; set; }
        public string UserToken { get; set; }

        public string UserName { get; set; }
        
        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }
    }
}
