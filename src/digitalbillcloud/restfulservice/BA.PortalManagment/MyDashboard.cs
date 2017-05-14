using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class DashboardBlock
    {
        public string ReportId { get; set; }
        public string BlockId { get; set; }

        public string URL { get; set; }
    }

    public class MyDashboard
    {
        public List<DashboardBlock> Blocks { get; set; }

        public MyDashboard()
        {
            this.Blocks = new List<DashboardBlock>();
        }
    }
}
