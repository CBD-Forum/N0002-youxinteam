using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class ReceiptTreeItem
    {
        public string createDate { get; set; }
        public string amount { get; set; }

        public string companyName { get; set; }

        public string color { get; set; }

        public List<ReceiptTreeItem> items { get; set; }

        public ReceiptTreeItem()
        {
            this.color = "#00C0EF";
            this.items = new List<ReceiptTreeItem>();
        }
    }
}
