using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class KendoTreeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }

        public string image { get; set; }
        public bool report { get; set; }

        public string docId { get; set; }
        // public string spriteCssClass { get; set; }

        public List<KendoTreeItem> items { get; set; }
        public KendoTreeItem(string i, string t, bool r = false)
        {
            this.id = i;
            this.docId = i;
            this.text = t;
            this.report = r;
            
            this.expanded = true;

            this.items = new List<KendoTreeItem>();
        }

    }
}
