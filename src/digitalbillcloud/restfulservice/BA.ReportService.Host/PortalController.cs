using BA.PortalManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BA.ReportService.Host
{
    [EnableCors("http://localhost:8020", "*", "*")]
    public class PortalController : ApiController
    {
        [HttpGet]
        public LoginResult Login(string userCode, string passwordMd5)
        {
            PortalFacade pf = new PortalFacade();
            return pf.Login(userCode, passwordMd5);
        }

        [HttpGet]
        public SaveResult ModifyPassword(string token, string passwordMd5)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.ModifyPassword(passwordMd5);
        }
        [HttpGet]
        public List<KendoTreeItem> GetUserTree(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetUserTree();
        }

        [HttpGet]
        public SaveResult WriteReportHistory(string token, string reportId)
        {
            PortalFacade pf = new PortalFacade(token);
            pf.WriteReportHistory(reportId);
            return new SaveResult();
        }

        [HttpGet]
        public List<KendoTreeItem> GetUserRoleTree(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetUserRoleTree();
        }

        [HttpGet]
        public UserFunctionPurivew GetFunctionList(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetFunctionList();
        }
        [HttpGet]
        public User GetUser(string token, string userId)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetUser(userId);
        }
        [HttpGet]
        public List<KendoTreeItem> GetRoleTree(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetRoleTree();
        }


        [HttpGet]
        public Role GetRole(string token, string roleId)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetRole(roleId);
        }

        [HttpGet]
        public SaveResult DeleteRole(string token, string roleId)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.DeleteRole(roleId);
        }

        [HttpGet]
        public SaveResult DeleteUser(string token, string userId)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.DeleteUser(userId);
        }


        [HttpGet]
        public SaveResult AssignUserRole(string token, string userId, string roleId, bool used)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.AssignUserRole(userId, roleId, used);
        }


        [HttpGet]
        public List<UserRole> GetUserRoles(string token, string userId)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetUserRoles(userId);
        }

        [HttpGet]
        public HoderPurview GetHolderPurview(string token, string holderId)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetHolderPurview(holderId);
        }

        [HttpGet]
        public void SaveHolderPurview(string token, string holderId, string resId, int op, bool result)
        {
            PortalFacade pf = new PortalFacade(token);
            pf.SaveHolderPurview(holderId, resId, op, result);
        }

        [HttpPost]
        public SaveResult SaveRole(string token, Role r)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.SaveRole(r);
        }

        [HttpPost]
        public SaveResult SaveUser(string token, User u)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.SaveUser(u);
        }



        [HttpGet]
        public SaveResult SaveDashboard(string token, string blockId, string reportId, bool linked,bool cloud)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.SaveDashboard(blockId, reportId, linked, cloud);
        }


        [HttpGet]
        public MyDashboard  GetDashboard(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetDashboard();
        }


        [HttpGet]
        public MyDashboard Hello(string token )
        {
            MyDashboard md = new MyDashboard();
            md.Blocks.Add(new DashboardBlock() { BlockId = "ABC" });
            return md;
        }


        [HttpPost]
        public void SaveHello(string token,MyDashboard ccc)
        {
            System.Diagnostics.Trace.WriteLine("aaa");
        }


        public SaveResult SaveBankReceipt(string token, BankReceipt receipt)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.SaveBankReceipt(receipt);
        }

        public SaveResult ExchangeBankReceipt(string token, BankReceipt receipt)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.ExchangeBankReceipt(receipt);
        }

        [HttpGet]
        public SaveResult FinancingBankReceipt(string token, string receiptId,string bankId,decimal discount,decimal money)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.FinancingBankReceipt(receiptId, bankId, discount,money);
        }

        public List<BankReceipt> GetBankReceiptList(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetBankReceiptList();
        }

        public List<BankReceipt> GetMyBankReceiptList(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetMyBankReceiptList();
        }


        public List<FinancingReceipt> GetFinancingReceiptList(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetFinancingReceiptList();
        }



        [HttpGet]
        public BankReceipt GetNextBankReceipt(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetNextBankReceipt();
        }
        [HttpGet]
        public List<CompanyBankAccout> GetCompanyBankAccoutList(string token)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetCompanyBankAccoutList();
        }
        [HttpGet]
        public BankReceipt GetBankReceipt(string token,string receiptId,bool raw)
        {
            PortalFacade pf = new PortalFacade(token);
            return pf.GetBankReceipt(receiptId, raw);
        }

        public List<IndicatorItem> GetFavoriteIndicatorList(string token)
        {
            PortalFacade rdf = new PortalFacade(token);
            return rdf.GetFavoriteIndicatorList();
        }

        public IndicatorValue GetIndicatorValue(string token,string cid)
        {
            PortalFacade rdf = new PortalFacade(token);
            return rdf.GetIndicatorValue(cid);
        }

        public List<ReceiptTreeItem> GetReceiptChain(string token,string receiptId)
        {
            PortalFacade rdf = new PortalFacade(token);
            return rdf.GetReceiptChain(receiptId);
        }

        public ReceiptChain GetReceiptChainDetail(string token, string receiptId)
        {
            PortalFacade rdf = new PortalFacade(token);
            return rdf.GetReceiptChainDetail(receiptId);
        }

    }
}
