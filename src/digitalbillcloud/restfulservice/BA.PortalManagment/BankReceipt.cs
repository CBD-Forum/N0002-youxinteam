using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class BankReceipt
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }

        public string ReceiptNo { get; set; }

        public DateTime BillDate { get; set; }

        public string CreateUserId { get; set; }
        public string FromCompanyId { get; set; }
        public string FromCompanyName { get; set; }

        public string SignCompanyName { get; set; }
        public string FromBankAccount { get; set; }

        public string ToCompanyId { get; set; }
        public string ToCompanyName { get; set; }
   
        public string ToBankAccount { get; set; }
        public string ToBankName { get; set; }

        public string AcceptBankNo { get; set; }
        public string AcceptBankName { get; set; }

        public string AcceptBankAddress { get; set; }

        public decimal Amount { get; set; }

        public DateTime AcceptDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string Protocols { get; set; }

        public string ContractAddress { get; set; }

        public string Status { get; set; }


    }


    public class CompanyBankAccout
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string BankAccount { get; set; }
        public string BankNO { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }

    }

    public class BCUser
    {
       
        public string UserName { get; set; }
        public string PublicKey { get; set; }
    }
}
