using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{

    public class UserFunctionPurivew
    {
        public bool CanCreateReport { get; set; }

        public bool ShowFunctionTree { get; set; }

        public List<FunctionItem> Items { get; set; }

        public UserFunctionPurivew()
        {
            this.Items = new List<FunctionItem>();
        }
    }

    public class FunctionItem
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

    }
}
