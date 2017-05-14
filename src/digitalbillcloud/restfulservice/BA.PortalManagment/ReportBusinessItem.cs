using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BA.PortalManagment
{
    public class ReportSheetBusinessItem
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ReportSheetBusinessItem( string id,string name)
        {
            this.Id = id;
            this.Name = name;
        }

    }
    public class ReportBusinessItem
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string CreateUser { get; set; }
        public string Description { get; set; }

        public List<ReportSheetBusinessItem> Sheets { get; set; }

        public string HtmlPage { get; set; }

        public ReportBusinessItem()
        {
            this.Sheets = new List<ReportSheetBusinessItem>();
        }
    }
}
