using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class FinancingReceipt
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }

        public string ReceiptNo { get; set; }

        public string SignCompanyName { get; set; }

        public string ApplyCompanyName { get; set; }

        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinancingValue { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime BillDate { get; set; }
        public DateTime AcceptDate { get; set; }

        public string ContractAddress { get; set; }
    }
}
