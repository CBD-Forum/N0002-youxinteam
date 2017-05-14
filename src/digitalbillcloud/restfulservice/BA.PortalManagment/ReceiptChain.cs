using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class ReceiptChainNode
    {
        public string id { get; set; }

        public string createDate { get; set; }
        public string amount { get; set; }

        public string companyName { get; set; }

        public string color { get; set; }

        public ReceiptChainNode()
        {
            this.color = "#00C0EF";

        }
    }

    public class seriesdataItem
    {
        public string category { get; set; }
        public decimal value { get; set; }
    }

    public class ReceiptChainLink
    {
        public string from { get; set; }
        public string to { get; set; }
        public string label { get; set; }
    }

    public class ReceiptChain
    {
        public string receiptNo { get; set; }
        public List<ReceiptChainNode> nodes { get; set; }
        public List<ReceiptChainLink> links { get; set; }

        public List<seriesdataItem> seriesdata { get; set; }

        public ReceiptChain()
        {
            this.nodes = new List<ReceiptChainNode>();
            this.links = new List<ReceiptChainLink>();
            this.seriesdata = new List<seriesdataItem>();
        }
    }
}
