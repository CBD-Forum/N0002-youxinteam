using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UFIDA.BI.DataAccess;

namespace BA.PortalManagment
{
    public class PortalFacade
    {
        private string UserId;
        public PortalFacade(string token)
        {
            string[] tt = token.Split(new[] { '_' });
            this.UserId = tt[0];
        }

        private string GetCompanyId()
        {
            string sql = "select CompanyId from UserList where IID=@UserId";
            SqlParameter[] para = {
                    new SqlParameter("@UserId",this.UserId),
            };

            return SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, para).ToString();
        }

        private BCUser GetBCUser(string companyId)
        {

            BCUser u = new BCUser();
          
            string sql = "select BCUser,BCPublicKey from CompanyList where IID=@CompanyId";
            SqlParameter[] para = {
                    new SqlParameter("@CompanyId",companyId),
            };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, para).Tables[0];
            if( dt.Rows.Count>0)
            {
                u.UserName = dt.Rows[0]["BCUser"].ToString();
                u.PublicKey = dt.Rows[0]["BCPublicKey"].ToString();
            }

            return u;

        }


        public PortalFacade()
        {

        }
        private  string MD5Encrypt(string text)
        {
     
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(text));
         
            md5.Clear();
            string str = string.Empty;
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;

            
        }

        public LoginResult Login(string userCode, string passwordMd5)
        {
            LoginResult rm = new LoginResult();

            string sql = "select count(1) from [UserList] where UserCode=@UserCode and PasswordMD5=@PasswordMD5";
            SqlParameter[] para = {
                    new SqlParameter("@UserCode",userCode),
                    new SqlParameter("@PasswordMD5",passwordMd5)
            };

            int count = Convert.ToInt32( SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text,sql, para) );

            if (count > 0)
            {
                sql = "select a.IID as UID,b.Code,a.UserName,b.Name as CompanyName from [UserList] a inner join CompanyList b on a.CompanyId = b.IID where a.UserCode=@UserCode and a.PasswordMD5=@PasswordMD5";
                DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, para).Tables[0];
                if( dt.Rows.Count>0)
                {
                    string userId = dt.Rows[0]["UID"].ToString();
                    rm.UserId = userId;
                    rm.CompanyCode = dt.Rows[0]["Code"].ToString();
                    rm.UserName = dt.Rows[0]["UserName"].ToString();
                    rm.CompanyName = dt.Rows[0]["CompanyName"].ToString();

                    rm.Result = true;
                    string x = string.Format("{0}:{1}:{2}:{3}", userCode, passwordMd5, rm.UserName,rm.CompanyCode);

                    rm.UserToken =string.Format("{0}_{1}", userId, MD5Encrypt(x));
                }
                else
                {
                    rm.Message = "用户未对应企业";
                    rm.Result = false;
                }
                return rm;
            }

            rm.Result = false;
            sql = "select count(1) from [UserList] where UserCode=@UserCode";

            count = Convert.ToInt32(SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, para));

            if (count == 1)
                rm.Message = "密码错误";
            else
                rm.Message = "用户名错误";


            return rm;
        }


        public void WriteReportHistory(string reportId)
        {
            string sql = "delete from HistoryReport where UserId=@UserId and ReportId=@ReportId";

            SqlParameter[] para = {
                                       new SqlParameter("@UserId",this.UserId),
                                       new SqlParameter("@ReportId",reportId)
                                  };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, para);

            sql = "insert into  HistoryReport(UserId,ReportId) values(@UserId,@ReportId)";
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, para);

        }
        public SaveResult SaveDashboard(string blockId,string reportId,bool linked,bool cloud)
        {
            SaveResult sr = new SaveResult();
            string sql = "delete from MyDashboard where UserId=@UserId and BlockId=@BlockId ";
            SqlParameter[] paras = {
                                new SqlParameter("@UserId",this.UserId),
                                new SqlParameter("@BlockId",blockId),
                                new SqlParameter("@ReportId",reportId),
                                new SqlParameter("@Cloud",cloud)
            };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

            if (linked)
            {
                sql = "insert into MyDashboard(UserId,BlockId,ReportId,Cloud) values(@UserId,@BlockId,@ReportId,@Cloud)";
                SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);
            }
            return sr;
        }

        private string GetWebCloudAddress()
        {
            string connString = ConfigurationUtil.GetMetaConnString(this.UserId);
            string sql = "select KeyValue from SCMInformation where Description='C_CloudURL'";
            object obj = SqlHelper.ExecuteScalar(connString, CommandType.Text, sql);
            if (obj == null)
                return "http://ba.yonyouup.com";

            return obj.ToString();
        }

        public MyDashboard GetDashboard()
        {
            MyDashboard md = new MyDashboard();

            string sql = "select * from MyDashboard where UserId=@UserId";
            SqlParameter[] paras = {
                                new SqlParameter("@UserId",this.UserId)
            };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras).Tables[0];
            foreach(DataRow r in dt.Rows)
            {
                bool cloud = Convert.ToBoolean(r["Cloud"]);

                DashboardBlock db = new DashboardBlock();
                db.BlockId = r["BlockId"].ToString();
                db.ReportId = r["ReportId"].ToString();
                if (cloud)
                    db.URL = GetWebCloudAddress();
                else
                    db.URL = string.Empty;

                md.Blocks.Add(db);
            }

            return md;

        }

        public UserFunctionPurivew GetFunctionList()
        {
            UserFunctionPurivew ufp = new UserFunctionPurivew();
            SqlParameter[] para = {
                new SqlParameter("@userId",this.UserId)
            };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.StoredProcedure, "PF_BA_GetHolderFunction", para).Tables[0];

            foreach( DataRow r in dt.Rows)
            {
                string code = r["Code"].ToString();
                bool showOnTree = Convert.ToBoolean(r["ShowFunctionTree"].ToString());
                if (showOnTree)
                {
                    FunctionItem fi = new FunctionItem();
                    fi.Code = code;
                    fi.Name = r["Name"].ToString();
                    fi.URL = r["URL"].ToString();
                    ufp.Items.Add(fi);
                }
                else
                {
                    if (code == "099")
                        ufp.CanCreateReport = true;
                }
            }

            if (ufp.Items.Count >0)
                ufp.ShowFunctionTree = true;
         

            return ufp;
        }

        public User GetUser(string id)
        {
            User u = new User();
            string sql = "select * from UserList where IID=@UserId";
            SqlParameter[] para = {
                    new SqlParameter("@UserId",id)
            };
            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, para).Tables[0];

            if (dt.Rows.Count == 0)
                return u;
            DataRow r = dt.Rows[0];
          
            u.Id = r["IID"].ToString();
            u.Code = r["UserCode"].ToString();
            u.Name = r["UserName"].ToString();
            u.EMail = r["UserMail"].ToString();
            u.Telephone = r["UserTel"].ToString();
            u.Weixin = r["Weixin"].ToString();
            u.Forbidden = Convert.ToBoolean( r["Forbidden"]);

  
            return u;
        }
        public Role GetRole(string id)
        {
            Role role = new Role();
            string sql = "select * from RoleList where IID=@RoleId";
            SqlParameter[] para = {
                    new SqlParameter("@RoleId",id)
            };
            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, para).Tables[0];

            if (dt.Rows.Count == 0)
                return role;
            DataRow r = dt.Rows[0];

            role.Id = r["IID"].ToString();
            role.Code = r["RoleCode"].ToString();
            role.Name = r["RoleName"].ToString();
           
            return role;
        }

        public List<IndicatorItem> GetFavoriteIndicatorList()
        {
            List<IndicatorItem> list = new List<IndicatorItem>();
            string sql = @"select top 6 * from FavoriteIndicator where UserId=@UserId";

            SqlParameter[] para = {
                                        new SqlParameter("@UserId",this.UserId)
                                  };
            DataSet ds = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, para);
            DataTable dt1 = ds.Tables[0];


            foreach (DataRow r in dt1.Rows)
            {
                IndicatorItem rd = new IndicatorItem();
                
                rd.Name = r["Name"].ToString();
                rd.HtmlPage = r["HtmlPage"].ToString();
                list.Add(rd);
            }

            return list;
        }

        public IndicatorValue GetIndicatorValue(string cid)
        {
            string sql = @"select Name from FavoriteIndicator where IID=@IID";

            SqlParameter[] para = {
                                        new SqlParameter("@IID",cid)
                                  };
            DataSet ds = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, para);
            DataTable dt = ds.Tables[0];

            IndicatorValue iv = new IndicatorValue();
            iv.Name = dt.Rows[0]["Name"].ToString();

            if (cid == "1")
                iv.Value = GetMonthNAPAmount();
            else if (cid == "2")
                iv.Value = GetMonthAPAmount();
            else if (cid == "3")
                iv.Value = GetAPAmount();
            else if (cid == "4")
                iv.Value = GetMonthARAmount();
            else if (cid == "5")
                iv.Value = GetARAmount();

            else if (cid == "6")
                iv.Value = GetFinancingAmount();
            

            return iv;
        }

        public List<ReceiptTreeItem> GetReceiptChain(string receiptId)
        {
            List<ReceiptTreeItem> items = new List<ReceiptTreeItem>();

            ReceiptTreeItem item = new ReceiptTreeItem();
            item.createDate = "2009-09-09";
            item.amount = "111";
            item.companyName = "aaaa";

            ReceiptTreeItem item2 = new ReceiptTreeItem();
            item2.createDate = "2009-09-09";
            item2.amount = "111";
            item2.companyName = "bbbb";
            item.items.Add(item2);

            items.Add(item);

            return items;
        }

        public ReceiptChain GetReceiptChainDetail(string receiptId)
        {
            ReceiptChain rc = new ReceiptChain();

            string sql = "select top 1 receiptNo from BankReceiptChain where IID= @ReceiptId";
            SqlParameter[] paras1 = {
                                new SqlParameter("@ReceiptId",receiptId )
            };
            rc.receiptNo = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras1).ToString();
            


            sql = @"select m.IID+'_'+m.FromCompanyId Id, fc.Name CompanyName,m.Amount,fc.CompanyType from [dbo].[BankReceiptChain] m
                            inner join[CompanyList] fc on m.FromCompanyId = fc.IID
                            where m.ReceiptNO = @ReceiptNo and UpstreamReceiptId  is null
                            union all
                            select m.IID + '_' + m.ToCompanyId Id,tc.Name CompanyName, m.Amount,tc.CompanyType from[dbo].[BankReceiptChain] m 
                            inner join[CompanyList] tc on m.ToCompanyId= tc.IID
                            where m.ReceiptNO= @ReceiptNo ";
            SqlParameter[] paras = {
                                new SqlParameter("@ReceiptNO",rc.receiptNo )
            };


            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras).Tables[0];
            foreach( DataRow r in dt.Rows)
            {
                string color = "#00C0EF";
                string ct = r["CompanyType"].ToString();
                if (ct == "企业")
                    color = "#DD4B39";
                else if (ct == "银行")
                    color = "#F39C12";
                else if (ct == "保理公司")
                    color = "#00a65a";
                rc.nodes.Add(new ReceiptChainNode() { id = r["Id"].ToString(), companyName = r["CompanyName"].ToString(), amount = r["Amount"].ToString(),  color=color});
            }


            sql = @"select isnull(UpstreamReceiptId,IID)+'_'+FromCompanyId LinkFrom,IID+'_'+ToCompanyId LinkTo,CreateDate from [BankReceiptChain] where ReceiptNO= @ReceiptNo";

            dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras).Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                rc.links.Add(new ReceiptChainLink() { from = r["LinkFrom"].ToString(), to = r["LinkTo"].ToString(), label = Convert.ToDateTime(r["CreateDate"]).ToString("yyyy-MM-dd")});
            }


            sql = @"select fc.Name CompanyName, sum(m.BalanceAmount) BalanceAmount from[BankReceiptChain] m
                inner join[CompanyList] fc on m.ToCompanyId = fc.IID and m.BalanceAmount > 0
                where ReceiptNO = @ReceiptNO
                group by fc.Name ";
            dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras).Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                rc.seriesdata.Add(new seriesdataItem() { category = r["CompanyName"].ToString(), value = Convert.ToDecimal(r["BalanceAmount"]) });
            }


            return rc;
        }

        private  string GetMonthNAPAmount()
        {
            string companyId = this.GetCompanyId();
            string sql = "select  isnull(sum(amount),0) amount  from [dbo].[BankReceipt] where Year(BillDate)=Year(getdate()) and Month(BillDate)=Month(getdate()) and FromCompanyId =@CompanyId ";
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);
                
            decimal x = Convert.ToDecimal(obj);
            return x.ToString("N2");
        }

        private string GetMonthAPAmount()
        {

            string companyId = this.GetCompanyId();
            string sql = "select  isnull(sum(amount),0) amount  from [dbo].[BankReceipt] where Year(AcceptDate)=Year(getdate()) and Month(AcceptDate)=Month(getdate()) and FromCompanyId =@CompanyId ";
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            decimal x = Convert.ToDecimal(obj);
            return x.ToString("N2");
        }

        private string GetAPAmount()
        {

            string companyId = this.GetCompanyId();
            string sql = "select  Isnull(sum(amount),0) amount  from [dbo].[BankReceipt] where  FromCompanyId =@CompanyId ";
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            decimal x = Convert.ToDecimal(obj);
            return x.ToString("N2");
            
        }


        private string GetARAmount()
        {
            string companyId = this.GetCompanyId();
            string sql = "select  isnull(sum(BalanceAmount),0) amount  from [dbo].[BankReceiptChain] where ToCompanyId =@CompanyId ";
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            decimal x = Convert.ToDecimal(obj);
            return x.ToString("N2");
        }

        private string GetMonthARAmount()
        {

            string companyId = this.GetCompanyId();
            string sql = @"select  isnull(sum(BalanceAmount), 0) amount from[dbo].[BankReceiptChain] a
                            inner join BankReceipt b on a.ReceiptNO = b.ReceiptNO and
                             Year(b.AcceptDate)=Year(getdate()) and Month(b.AcceptDate)=Month(getdate()) and a.ToCompanyId =@CompanyId";
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            decimal x = Convert.ToDecimal(obj);
            return x.ToString("N2");
        }

        private string GetFinancingAmount()
        {
            string companyId = this.GetCompanyId();
            string sql = @"select  isnull(sum(FinancingValue), 0) amount from[dbo].[FinancingReceipt] where BankId =@CompanyId";
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            decimal x = Convert.ToDecimal(obj);
            return x.ToString("N2");
        }
        public BankReceipt GetBankReceipt(string receiptId,bool raw)
        {
            BankReceipt br = new BankReceipt();

            /*
            string sql = @"select a.IID,a.FromCompanyId,a.ToCompanyId,
                            a.ReceiptNO,a.BillDate,fc.Name FromCompanyName, a.FromBankAccount,a.ToBankAccount,a.Amount,a.AcceptBankNo,a.AcceptDate,a.Protocol,
                            fb.BankNo,fb.BankName,fb.Address,tb.BankName ToBankName, tc.Name ToCompanyName
                             from[dbo].[BankReceipt] a
                            inner join vw_CompanyBankAccount fb on a.FromBankAccount = fb.BankAccount
                            inner join vw_CompanyBankAccount tb on a.FromBankAccount = tb.BankAccount

                            inner join CompanyList fc on a.FromCompanyId = fc.IID
                            inner join CompanyList tc on a.ToCompanyId = tc.IID and a.IID=@ReceiptId";
            SqlParameter[] paras = {
                                new SqlParameter("@ReceiptId",receiptId)
            };
            */

            string sql = @"select m.IID,a.FromCompanyId,m.ToCompanyId ,a.ContractAddress,
                            a.ReceiptNO,a.BillDate,fc.Name FromCompanyName, a.FromBankAccount,m.ToBankAccount,m.BalanceAmount,a.AcceptBankNo,a.AcceptDate,a.Protocol,m.Amount,
                            fb.BankNo,fb.BankName,fb.Address,tb.BankName ToBankName, tc.Name ToCompanyName
                             from[dbo].[BankReceipt] a

						    inner join BankReceiptChain m on a.ReceiptNO = m.ReceiptNO and m.IID=@ReceiptId

                            inner join vw_CompanyBankAccount fb on a.FromBankAccount = fb.BankAccount
                            inner join vw_CompanyBankAccount tb on m.ToBankAccount = tb.BankAccount
                          
                            inner join CompanyList fc on a.FromCompanyId = fc.IID
                            inner join CompanyList tc on m.ToCompanyId = tc.IID  ";

            SqlParameter[] paras = {
                                new SqlParameter("@ReceiptId",receiptId)
            };


            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras).Tables[0];
            br.Id = dt.Rows[0]["IID"].ToString();
            br.ReceiptNo = dt.Rows[0]["ReceiptNO"].ToString();
            br.BillDate = Convert.ToDateTime(dt.Rows[0]["BillDate"].ToString());
            br.FromCompanyId = dt.Rows[0]["FromCompanyId"].ToString();
            br.FromCompanyName = dt.Rows[0]["FromCompanyName"].ToString();
          

            br.FromBankAccount = dt.Rows[0]["FromBankAccount"].ToString();
            br.AcceptBankNo = dt.Rows[0]["AcceptBankNo"].ToString();
            br.AcceptBankName = dt.Rows[0]["BankName"].ToString();
            br.AcceptBankAddress = dt.Rows[0]["Address"].ToString();

            br.ToCompanyId = dt.Rows[0]["ToCompanyId"].ToString();
            br.ToCompanyName = dt.Rows[0]["ToCompanyName"].ToString();
            br.ToBankAccount = dt.Rows[0]["ToBankAccount"].ToString();
            br.ToBankName = dt.Rows[0]["ToBankName"].ToString();

            br.Amount = Convert.ToDecimal(dt.Rows[0]["BalanceAmount"]);
            if( br.Amount==0)
                br.Amount = Convert.ToDecimal(dt.Rows[0]["Amount"]);

            if( raw )
                br.Amount = Convert.ToDecimal(dt.Rows[0]["Amount"]);

            br.AcceptDate = Convert.ToDateTime(dt.Rows[0]["AcceptDate"]);
            br.Protocols = dt.Rows[0]["Protocol"].ToString();
            br.ContractAddress = dt.Rows[0]["ContractAddress"].ToString();

            return br;

        }


       



        public List<BankReceipt> GetBankReceiptList()
        {
            List<BankReceipt> list = new List<BankReceipt>();
            string companyId = this.GetCompanyId();
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            string sql = @"select b.IID,b.ReceiptNo,c1.Name FromCompanyName ,c2.Name ToCompanyName,
b.FromBankAccount,b.ToBankAccount,a1.BankName FromBankName,
a2.BankName ToBankName, b.Amount,b.AcceptDate,b.BillDate,b.ContractAddress
  from BankReceipt b
inner join CompanyList c1 on b.FromCompanyId = c1.IID
inner join CompanyList c2 on b.ToCompanyId = c2.IID
inner join vw_CompanyBankAccount a1 on b.FromBankAccount = a1.BankAccount
inner join vw_CompanyBankAccount a2 on b.ToBankAccount = a2.BankAccount and b.FromCompanyId=@CompanyId order by b.ReceiptNo desc ";

            DataSet ds = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            DataTable dt = ds.Tables[0];
            foreach( DataRow  r in dt.Rows)
            {
                BankReceipt br = new BankReceipt();
                br.Id = r["IID"].ToString();
                br.ReceiptNo = r["ReceiptNo"].ToString();
                br.BillDate = Convert.ToDateTime(r["BillDate"]);
              
                br.FromBankAccount = r["FromBankAccount"].ToString();
                br.AcceptBankName = r["FromBankName"].ToString();
                br.FromCompanyName = r["FromCompanyName"].ToString();
               
                br.ToBankAccount = r["ToBankAccount"].ToString();
                br.ToBankName = r["ToBankName"].ToString();
                br.ToCompanyName = r["ToCompanyName"].ToString();
                br.Amount = Convert.ToDecimal(r["Amount"]);
                br.AcceptDate = Convert.ToDateTime(r["AcceptDate"].ToString());
                br.ContractAddress =  r["ContractAddress"].ToString();
                list.Add(br);
            }



            return list;
        }

        public List<FinancingReceipt> GetFinancingReceiptList()
        {
            List<FinancingReceipt> list = new List<FinancingReceipt>();
            string companyId = this.GetCompanyId();
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
         

            string sql = @"select m.IID,ffc.Name SignCompanyName,b.ReceiptNo,c1.Name FromCompanyName, c2.Name ToCompanyName ,
            b.FromBankAccount,m.ToBankAccount,
           m.BalanceAmount,b.AcceptDate,b.BillDate,b.ContractAddress
		   ,n.Discount,n.FinancingValue,n.CreateDate
            from BankReceipt b

            inner join BankReceiptChain m on b.ReceiptNO = m.ReceiptNO
            inner join FinancingReceipt n on n.ReceiptChainId = m.IID  and n.BankId=@CompanyId
			inner join CompanyList ffc on b.FromCompanyId = ffc.IID
            inner join CompanyList c1 on m.FromCompanyId = c1.IID
            inner join CompanyList c2 on m.ToCompanyId = c2.IID
    
            order by n.CreateDate desc";

            DataSet ds = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            DataTable dt = ds.Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                FinancingReceipt br = new FinancingReceipt();
                br.Id = r["IID"].ToString();
                br.ReceiptNo = r["ReceiptNo"].ToString();
                br.BillDate = Convert.ToDateTime(r["BillDate"]);

              
                br.ApplyCompanyName = r["ToCompanyName"].ToString();
                br.SignCompanyName = dt.Rows[0]["SignCompanyName"].ToString();

                
                br.Discount = Convert.ToDecimal(r["Discount"]);
                br.FinancingValue = Convert.ToDecimal(r["FinancingValue"]);
                br.Amount = Convert.ToDecimal(r["BalanceAmount"]);
                br.AcceptDate = Convert.ToDateTime(r["AcceptDate"].ToString());
                br.CreateDate = Convert.ToDateTime(dt.Rows[0]["CreateDate"]);
                br.ContractAddress = r["ContractAddress"].ToString();
                list.Add(br);
            }


            return list;
        }

        public List<BankReceipt> GetMyBankReceiptList()
        {
            List<BankReceipt> list = new List<BankReceipt>();
            string companyId = this.GetCompanyId();
            SqlParameter[] paras = {
                                new SqlParameter("@CompanyId",companyId)
            };
            /*
            string sql = @"select b.IID,b.ReceiptNo,c1.Name FromCompanyName ,c2.Name ToCompanyName,
b.FromBankAccount,b.ToBankAccount,a1.BankName FromBankName,
a2.BankName ToBankName, b.Amount,b.AcceptDate,b.BillDate
  from BankReceipt b
inner join CompanyList c1 on b.FromCompanyId = c1.IID
inner join CompanyList c2 on b.ToCompanyId = c2.IID
inner join vw_CompanyBankAccount a1 on b.FromBankAccount = a1.BankAccount
inner join vw_CompanyBankAccount a2 on b.ToBankAccount = a2.BankAccount and b.ToCompanyId=@CompanyId";
*/

            string sql = @"select m.IID,ffc.Name SignCompanyName,b.ReceiptNo,c1.Name FromCompanyName, c2.Name ToCompanyName,m.CreateDate ,
            b.FromBankAccount,m.ToBankAccount,a1.BankName FromBankName,
            a2.BankName ToBankName, m.BalanceAmount,b.AcceptDate,b.BillDate,b.ContractAddress
              from BankReceipt b

            inner join BankReceiptChain m on b.ReceiptNO = m.ReceiptNO and m.BalanceAmount > 0 and m.ToCompanyId=@CompanyId
			inner join CompanyList ffc on b.FromCompanyId = ffc.IID
            inner join CompanyList c1 on m.FromCompanyId = c1.IID
            inner join CompanyList c2 on m.ToCompanyId = c2.IID
            inner join vw_CompanyBankAccount a1 on b.FromBankAccount = a1.BankAccount
            inner join vw_CompanyBankAccount a2 on m.ToBankAccount = a2.BankAccount
            order by m.CreateDate desc";

            DataSet ds = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);

            DataTable dt = ds.Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                BankReceipt br = new BankReceipt();
                br.Id = r["IID"].ToString();
                br.ReceiptNo = r["ReceiptNo"].ToString();
                br.BillDate = Convert.ToDateTime(r["BillDate"]);

                br.FromBankAccount = r["FromBankAccount"].ToString();
                br.AcceptBankName = r["FromBankName"].ToString();
                br.FromCompanyName = r["FromCompanyName"].ToString();
                br.SignCompanyName = dt.Rows[0]["SignCompanyName"].ToString();

                br.ToBankAccount = r["ToBankAccount"].ToString();
                br.ToBankName = r["ToBankName"].ToString();
                br.ToCompanyName = r["ToCompanyName"].ToString();
                br.Amount = Convert.ToDecimal(r["BalanceAmount"]);
                br.AcceptDate = Convert.ToDateTime(r["AcceptDate"].ToString());
                br.CreateDate = Convert.ToDateTime(dt.Rows[0]["CreateDate"]);
                br.ContractAddress = r["ContractAddress"].ToString();
                list.Add(br);
            }



            return list;
        }

     

        private  string GetBankReceiptString(BankReceipt receipt)
        {
            // string pulicKey = "ALP5az8PPaq8C6MBYHRFaqMnPfQWyyfVi9hVKTU32UB8EDNy7rCZi";
            // string toUserPublicKey = "ALP6LqtKLqTsdVYQkb1BD48JkZ7V1WviEvQ5maYmaNKeFPLSAzJ9R";
            //  string bankPublicKey = "ALP7VmLuRHux2AvHz9tM4PdjvPNkkPqxzcoBjuH84CATr4DyMJzzv";
            //  string thirdPublicKey = "ALP8gwSLWkv3CGRDpLHPAZP9r7cjKuiHAQdezmJMHXyG9iYJmkAcG";

            string pulicKey = this.GetBCUser(receipt.FromCompanyId).PublicKey;
             string toUserPublicKey = this.GetBCUser(receipt.ToCompanyId).PublicKey;
            string bankPublicKey = this.GetBCUser("11e9ae96-e3b3-405c-abfc-ea829f57ba69").PublicKey;
            string thirdPublicKey = this.GetBCUser("d169018c-696d-4b29-9d6d-90a7697c760f").PublicKey;


            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            string x = (receipt.AcceptDate - startTime).TotalSeconds.ToString();


            StringBuilder sb = new StringBuilder();

            sb.Append(receipt.ReceiptNo);
            sb.Append("|");
            sb.Append(pulicKey);
            sb.Append("|");
            sb.Append(x);
            sb.Append("|");
            sb.Append(toUserPublicKey);
            sb.Append("|");
            sb.Append(receipt.Amount);
            sb.Append("|");
            sb.Append(bankPublicKey);
            sb.Append("|");
            sb.Append(thirdPublicKey);


            return sb.ToString();
        }

       
        public List<CompanyBankAccout> GetCompanyBankAccoutList()
        {
            List<CompanyBankAccout> cc = new List<CompanyBankAccout>();
            string companyId = this.GetCompanyId();
            string sql = @"select c.IID CompanyId,c.Name,a.BankAccount,b.* from [dbo].[CompanyBankAccount] a
                            inner join[dbo].[BankList]
                                    b on a.BankNO=b.BankNo
                            inner join[dbo].[CompanyList]
                                    c on a.CompanyId=c.IID and a.CompanyId<>@CompanyId";
            SqlParameter[] para = {
                    new SqlParameter("@CompanyId",companyId),
            };
            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, para).Tables[0];
            foreach(DataRow r in dt.Rows)
            {
                CompanyBankAccout cb = new CompanyBankAccout();
                cb.CompanyId = r["CompanyId"].ToString();
                cb.CompanyName = r["Name"].ToString();
                cb.BankAccount = r["BankAccount"].ToString();
                cb.BankNO = r["BankNO"].ToString();
                cb.BankName = r["BankName"].ToString();
                cb.BankAddress = r["Address"].ToString();

                cc.Add(cb);
            }
            return cc;
        }

      

        public BankReceipt GetNextBankReceipt()
        {
            string sql = "select Max(ReceiptNO)  from [dbo].[BankReceipt]";
            object x = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql);

            if(x==null||string.IsNullOrEmpty(x.ToString()))
               x = "X-0001-000"; 


            string m = x.ToString().Substring(7, 3);
            string n = x.ToString().Substring(0,7);

            if (m.Substring(0, 2) == "00")
                m = m.Substring(2, 1);
            else if (m.Substring(0,1)=="0")
                m = m.Substring(1, 2);

            m = (Convert.ToInt32(m) + 1).ToString().PadLeft(3, '0');


            BankReceipt br = new BankReceipt();
            br.ReceiptNo = n+m;

            sql = @"select top 1 d.Name,c.CompanyId, c.BankAccount,b.BankNo,b.BankName,b.Address,c.Protocols from [dbo].[UserList] u inner join 
                    [dbo].[CompanyBankAccount] c on u.CompanyId = c.CompanyId
                     inner join [dbo].[BankList] b on c.BankNo = b.BankNo 
					 inner join  CompanyList d on d.IID = c.CompanyId and  u.IID=@UserId";

            SqlParameter[] paras = {
                new SqlParameter("@UserId",this.UserId)
            };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras).Tables[0];

            if( dt.Rows.Count>0)
            {
                br.FromCompanyId = dt.Rows[0]["CompanyId"].ToString();
                br.FromCompanyName = dt.Rows[0]["Name"].ToString();

                br.FromBankAccount = dt.Rows[0]["BankAccount"].ToString();
                br.AcceptBankNo = dt.Rows[0]["BankNo"].ToString();
                br.AcceptBankName = dt.Rows[0]["BankName"].ToString();
                br.AcceptBankAddress = dt.Rows[0]["Address"].ToString();
                br.Protocols = dt.Rows[0]["Protocols"].ToString();
            }
            

            try
            {
              
                
                string user = this.GetBCUser(br.FromCompanyId).UserName;
                List<string> ps = new List<string>();
                ps.Add("wallet");
                PostToBlockChain("wallet_open", ps);

                ps.Clear();
                ps.Add("99999999");
                ps.Add("12345678");
                PostToBlockChain("wallet_unlock", ps);


                sql = "select top 1 ContractAddress from NextContract ";
                object obj = SqlHelper.ExecuteScalar(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql);
                if (obj != null && string.IsNullOrEmpty(obj.ToString()))
                {
                    br.ContractAddress = obj.ToString();
                    return br;
                }
               
                ps.Clear();
                string contract = System.Configuration.ConfigurationManager.AppSettings["ContractTemplate"].ToString();
                ps.Add(user);
                ps.Add(contract);
                ps.Add("ALP");
                ps.Add("0.1");
                string r = PostToBlockChain("register_contract", ps);

                RegisterContractResult rcr = JsonConvert.DeserializeObject<RegisterContractResult>(r);
                br.ContractAddress = rcr.result;

                sql = "insert into NextContract(ContractAddress) values(@ContractAddress)";
                SqlParameter[] paras1 = {
                                             new SqlParameter("@ContractAddress",br.ContractAddress)
                                        };

                SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql,paras1);

            }
            catch
            {

            }



            return br;
        }

        public SaveResult SaveBankReceipt(BankReceipt receipt)
        {
            SaveResult sr = new SaveResult();
            string sql = string.Empty;
            string contractId = receipt.ContractAddress;

            try
            {
                string user = this.GetBCUser(receipt.FromCompanyId).UserName;
                List<string> ps = new List<string>();
                ps.Clear();
                ps.Add(contractId);
                ps.Add(user);
                ps.Add("bill_create");
                ps.Add(GetBankReceiptString(receipt));
                ps.Add("ALP");
                ps.Add("1");
               
                PostToBlockChain("call_contract", ps);


                ConfirmReceipt(contractId,this.GetBCUser("d169018c-696d-4b29-9d6d-90a7697c760f").UserName);

            }
            catch(Exception ex)
            {
                sr.Result = false;
                sr.Message = ex.Message.ToString();
                return sr;
            }

            sql = "delete from NextContract where ContractAddress=@ContractAddress";
            SqlParameter[] paras2 = {
                                             new SqlParameter("@ContractAddress",contractId)
                                        };

            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras2);


            receipt.Id = Guid.NewGuid().ToString();
            sql = "insert into BankReceipt (IID,ReceiptNo,BillDate,CreateUserId,FromCompanyId,FromBankAccount,ToCompanyId,ToBankAccount,Amount,AcceptDate,AcceptBankNO,Protocol,ContractAddress,Status) values(@IID,@ReceiptNo,@BillDate,@CreateUserId,@FromCompanyId,@FromBankAccount,@ToCompanyId,@ToBankAccount,@Amount,@AcceptDate,@AcceptBankNO,@Protocol,@ContractAddress,@Status)";

            /*
            if (string.IsNullOrEmpty(receipt.Id))
            {
                receipt.Id = Guid.NewGuid().ToString();
                sql = "insert into BankReceipt (IID,ReceiptNo,BillDate,CreateUserId,FromCompanyId,FromBankAccount,ToCompanyId,ToBankAccount,Amount,AcceptDate,AcceptBankNO,Protocol,Status) values(@IID,@ReceiptNo,@BillDate,@CreateUserId,@FromCompanyId,@FromBankAccount,@ToCompanyId,@ToBankAccount,@Amount,@AcceptDate,@AcceptBankNO,@Protocol,@Status)";

            }
            else
                sql = "update BankReceipt set ReceiptNo =@ReceiptNo,BillDate=@BillDate,FromCompanyId=@FromCompanyId,FromBankAccount=@FromBankAccount,ToCompanyId=@ToCompanyId,ToBankAccount=@ToBankAccount,Amount=@Amount,AcceptDate=@AcceptDate,AcceptBankNO=@AcceptBankNO,Protocol=@Protocol,Status=@Status where IID=@IID";
            */
            SqlParameter[] paras = {
                                new SqlParameter("@IID",receipt.Id),
                                new SqlParameter("@ReceiptNo",receipt.ReceiptNo),
                                new SqlParameter("@BillDate",receipt.BillDate),
                                new SqlParameter("@CreateUserId",this.UserId),
                                new SqlParameter("@FromCompanyId",receipt.FromCompanyId),
                                new SqlParameter("@FromBankAccount",receipt.FromBankAccount),
                                new SqlParameter("@ToCompanyId",receipt.ToCompanyId),
                                new SqlParameter("@ToBankAccount",receipt.ToBankAccount),
                                new SqlParameter("@Amount",receipt.Amount),
                                new SqlParameter("@AcceptDate",receipt.AcceptDate),
                                new SqlParameter("@AcceptBankNo",receipt.AcceptBankNo),
                                new SqlParameter("@Protocol",receipt.Protocols),
                                new SqlParameter("@ContractAddress",contractId),
                                new SqlParameter("@Status",receipt.Status)
                                };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);


            sql = @"insert into BankReceiptChain (IID,ReceiptNo,FromCompanyId,ToCompanyId,ToBankAccount,Amount,CreateUserId,CreateDate,BalanceAmount,Status) 
                                                values(@IID,@ReceiptNo,@FromCompanyId,@ToCompanyId,@ToBankAccount,@Amount,@CreateUserId,@CreateDate,@BalanceAmount,@Status)";

            SqlParameter[] paras1 = {
                                new SqlParameter("@IID",receipt.Id),
                                new SqlParameter("@ReceiptNo",receipt.ReceiptNo),
                                new SqlParameter("@FromCompanyId",receipt.FromCompanyId),
                                new SqlParameter("@ToCompanyId",receipt.ToCompanyId),
                                new SqlParameter("@ToBankAccount",receipt.ToBankAccount),
                                new SqlParameter("@Amount",receipt.Amount),
                                new SqlParameter("@CreateUserId",this.UserId),
                                new SqlParameter("@CreateDate",DateTime.Now),
                                new SqlParameter("@BalanceAmount",receipt.Amount),
                                new SqlParameter("@Status",receipt.Status)

                                };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras1);
            sr.Result = true;

            return sr;
        }

        private void ConfirmReceipt(string contractAddress,string user)
        {
            List<string> ps = new List<string>();
            ps.Add(contractAddress);
            ps.Add(user);
            ps.Add("bill_create_confirm");
            ps.Add("");
            ps.Add("ALP");
            ps.Add("0.1");

            PostToBlockChain("call_contract", ps);
        }

        
        public SaveResult FinancingBankReceipt(string receiptId,string bankId,decimal discount,decimal money)
        {
            SaveResult sr = new SaveResult();
            string sql = @"insert into FinancingReceipt (IID,ReceiptChainId,BankId,Discount,FinancingValue,CreateDate) 
                                                values(@IID,@ReceiptChainId,@BankId,@Discount,@FinancingValue,@CreateDate)";

            SqlParameter[] paras1 = {
                                new SqlParameter("@IID",Guid.NewGuid().ToString()),
                                new SqlParameter("@ReceiptChainId",receiptId),
                                new SqlParameter("@BankId",bankId),
                                new SqlParameter("@Discount",discount),
                                new SqlParameter("@FinancingValue",money),
                                new SqlParameter("@CreateDate",DateTime.Now)

                                };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras1);

           

            BankReceipt br = this.GetBankReceipt(receiptId,false);
            br.ToCompanyId = bankId;


            ExchangeBankReceipt(br);

            return sr;
        }


        public SaveResult ExchangeBankReceipt(BankReceipt receipt)
        {
            SaveResult sr = new SaveResult();

            string upStreamReceiptId = receipt.Id;

            string companyId = this.GetCompanyId();

            string sql = string.Format("update BankReceiptChain set BalanceAmount =BalanceAmount-{0} where IID=@IID", receipt.Amount);
            SqlParameter[] paras = {
                                new SqlParameter("@IID",receipt.Id)
            };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras);


            receipt.Id = Guid.NewGuid().ToString();

            sql = @"insert into BankReceiptChain (IID,ReceiptNo,FromCompanyId,ToCompanyId,ToBankAccount,Amount,CreateUserId,CreateDate,BalanceAmount,Status,UpStreamReceiptId) 
                                                values(@IID,@ReceiptNo,@FromCompanyId,@ToCompanyId,@ToBankAccount,@Amount,@CreateUserId,@CreateDate,@BalanceAmount,@Status,@UpStreamReceiptId)";

            SqlParameter[] paras1 = {
                                new SqlParameter("@IID",receipt.Id),
                                new SqlParameter("@ReceiptNo",receipt.ReceiptNo),
                                new SqlParameter("@FromCompanyId",companyId),
                                new SqlParameter("@ToCompanyId",receipt.ToCompanyId),
                                new SqlParameter("@ToBankAccount",receipt.ToBankAccount),
                                new SqlParameter("@Amount",receipt.Amount),
                                new SqlParameter("@CreateUserId",this.UserId),
                                new SqlParameter("@CreateDate",DateTime.Now),
                                new SqlParameter("@BalanceAmount",receipt.Amount),
                                new SqlParameter("@Status",receipt.Status),
                                new SqlParameter("@UpStreamReceiptId",upStreamReceiptId),
                                };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, paras1);




            try
            {
                if (string.IsNullOrEmpty(receipt.ContractAddress))
                    return sr;


             
                BCUser toBCUser = this.GetBCUser(receipt.ToCompanyId);

                string user2 = this.GetBCUser(this.GetCompanyId()).UserName;
                string toUser = toBCUser.UserName;
                List<string> ps = new List<string>();
                ps.Add("wallet");
                PostToBlockChain("wallet_open", ps);

                ps.Clear();
                ps.Add("99999999");
                ps.Add("12345678");
                PostToBlockChain("wallet_unlock", ps);


                ps.Clear();
                ps.Add(receipt.ContractAddress);
                ps.Add(user2);
                ps.Add("bill_circulation_transfer");
                ps.Add(string.Format("{0}|{1}|{2}", receipt.ReceiptNo,receipt.Amount,toBCUser.PublicKey));
                ps.Add("ALP");
                ps.Add("0.1");

                PostToBlockChain("call_contract", ps);

                System.Threading.Thread.Sleep(1000);

                ps.Clear();
                ps.Add(receipt.ContractAddress);
                ps.Add(toUser);
                ps.Add("bill_circulation_confirm");
                ps.Add(receipt.ReceiptNo);
                ps.Add("ALP");
                ps.Add("0.1");

                PostToBlockChain("call_contract", ps);


            }
            catch(Exception ex)
            {

            }





            return sr;

        }

        private string CreateAuthorizationString()
        {
            string userName = "admin";
            string password = "123456";
            byte[] buffer = System.Text.Encoding.Default.GetBytes(string.Format("{0}:{1}", userName, password));
            string s = Convert.ToBase64String(buffer);
            return "000000" + s;
        }

        private string PostToBlockChain(string method,List<string> parameters)
        {
         
            JsonRequest jr = new JsonRequest();
            jr.id = "1";
            jr.method = method;

              
            foreach (string x in parameters)
                jr.parameters.Add(x);

            string s = JsonConvert.SerializeObject(jr);

            Uri url = new Uri("http://127.0.0.1:8080/rpc");
            WebClient wc = new WebClient();

            wc.Headers["Content-type"] = "application/json";
            wc.Headers["Authorization"] = CreateAuthorizationString();
           

            wc.Encoding = Encoding.UTF8;

            System.Diagnostics.Trace.WriteLine(s);
            string msg = wc.UploadString(url, "POST", s);
            System.Diagnostics.Trace.WriteLine(msg);
            return msg;

        }

        public List<KendoTreeItem> GetUserTree()
        {
            List<KendoTreeItem> tree = new List<KendoTreeItem>();
            KendoTreeItem root = new KendoTreeItem(string.Empty, "所有用户");
            tree.Add(root);

            List<User> us = new List<User>();

            string sql = "select * from UserList order by UserCode";
          
            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql).Tables[0];

            foreach( DataRow r in dt.Rows)
            {
                KendoTreeItem item = new KendoTreeItem(r["IID"].ToString(), string.Format("({0}){1}", r["UserCode"].ToString(), r["UserName"].ToString()));
                root.items.Add(item);
            }
            return tree;
        }

        public List<KendoTreeItem> GetRoleTree()
        {
            List<KendoTreeItem> tree = new List<KendoTreeItem>();
            KendoTreeItem root = new KendoTreeItem(string.Empty, "所有角色");
            tree.Add(root);

            List<User> us = new List<User>();

            string sql = "select * from RoleList order by RoleCode";

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                KendoTreeItem item = new KendoTreeItem(r["IID"].ToString(), string.Format("({0}){1}", r["RoleCode"].ToString(), r["RoleName"].ToString()));
                root.items.Add(item);
            }
            return tree;
        }

        public List<KendoTreeItem> GetUserRoleTree()
        {
            List<KendoTreeItem> tree = new List<KendoTreeItem>();
            KendoTreeItem root = new KendoTreeItem(string.Empty, "角色");
            tree.Add(root);

            string sql = "select * from RoleList order by RoleCode";
            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql).Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                KendoTreeItem item = new KendoTreeItem(r["IID"].ToString(), string.Format("({0}){1}", r["RoleCode"].ToString(), r["RoleName"].ToString()));
                root.items.Add(item);
            }

         
            root = new KendoTreeItem(string.Empty, "用户");
            tree.Add(root);
            sql = "select * from UserList order by UserCode";
            dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                KendoTreeItem item = new KendoTreeItem(r["IID"].ToString(), string.Format("({0}){1}", r["UserCode"].ToString(), r["UserName"].ToString()));
                root.items.Add(item);
            }
            return tree;
        }

        public SaveResult DeleteRole(string roleId)
        {
            SaveResult sr = new SaveResult();
            string sql = "delete from RoleList where IID=@RoleId";
            SqlParameter[] paras = {
                                new SqlParameter("@RoleId",roleId)
            };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);
            return sr;
        }

        public SaveResult AssignUserRole( string userId,string roleId,bool used)
        {
            SaveResult sr = new SaveResult();
            string sql = "delete from UserRoles where UserId=@UserId and RoleId=@RoleId";
            SqlParameter[] paras = {
                                new SqlParameter("@UserId",userId),
                                new SqlParameter("@RoleId",roleId)
            };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

            if (used)
            {
                sql = "insert into UserRoles(UserId,RoleId) values(@UserId,@RoleId)";
                SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);
            }
            return sr;
        }

        public SaveResult DeleteUser(string userId)
        {
            SaveResult sr = new SaveResult();
            string sql = "delete from UserList where IID=@UserId";
            SqlParameter[] paras = {
                                new SqlParameter("@UserId",userId)
            };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);
            return sr;
        }

        public List<Role> GetRoleList(string token)
        {
            List<Role> us = new List<Role>();

            string sql = "select * from RoleList order by RoleCode";

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                Role u = new Role();
                u.Code = r["RoleCode"].ToString();
                u.Name = r["RoleName"].ToString();
                us.Add(u);
            }
            return us;
        }

        public SaveResult ModifyPassword(string passwordMd5)
        {
            SaveResult sr = new SaveResult();
            string sql = @"update UserList set Password =@Password where IID=@UserId;
                        update UserList set PasswordMD5 =@Password from UserList where IID=@UserId";


            SqlParameter[] paras = {
                                new SqlParameter("@UserId",this.UserId),
                                new SqlParameter("@Password",passwordMd5)
                                };

            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

            return sr;
        }

        public SaveResult SaveUser(User user)
        {
            SaveResult sr = new SaveResult();
            string sql = string.Empty;
            if (string.IsNullOrEmpty(user.Id))
            {
              
                user.Id = Guid.NewGuid().ToString();
                sql = @"insert into UserList (IID,UserCode,UserName,Password,UserMail,UserTel,Weixin,Forbidden) values(@UserId,@UserCode,@UserName,@Password,@EMail,@Telephone,@Weixin,@Forbidden);
                        insert into UserList(IID,UserCode,UserName,PasswordMD5,CompanyId) values(@UserId,@UserCode,@UserName,@Password,@CompanyId)";

            }
            else
                sql = @"update UserList set UserCode =@UserCode,UserName=@UserName,UserMail=@EMail,UserTel=@Telephone,Weixin=@Weixin,Forbidden=@Forbidden where IID=@UserId;
                        update UserList set UserCode =@UserCode,UserName=@UserName from UserList where IID=@UserId";

            string companyId = GetCompanyId();
            SqlParameter[] paras = {
                                new SqlParameter("@UserId",user.Id),
                                new SqlParameter("@UserCode",user.Code),
                                new SqlParameter("@Password",user.PasswordMd5),
                                new SqlParameter("@UserName",user.Name),
                                new SqlParameter("@EMail",user.EMail),
                                new SqlParameter("@Telephone",user.Telephone),
                                new SqlParameter("@Weixin",user.Weixin),
                                new SqlParameter("@Forbidden",user.Forbidden),
                                new SqlParameter("@CompanyId",companyId)
                                };

            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

            return sr;
        }

        public SaveResult SaveRole(Role role)
        {
            SaveResult sr = new SaveResult();
            string sql = string.Empty;

            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = Guid.NewGuid().ToString();
                sql = "insert into RoleList (IID,RoleCode,RoleName) values(@RoleId,@RoleCode,@RoleName)";

            }
            else
                sql = "update RoleList set RoleCode =@RoleCode,RoleName=@RoleName where IID=@RoleId";

            SqlParameter[] paras = {
                                new SqlParameter("@RoleId",role.Id),
                                new SqlParameter("@RoleCode",role.Code),
                                new SqlParameter("@RoleName",role.Name)
                                };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql,paras);

            return sr;
        }

        public List<UserRole> GetUserRoles(string userId)
        {
            List<UserRole> us = new List<UserRole>();

            string sql = @"select *,case when b.userId is null then 2 else 1 end x from RoleList a left join UserRoles b on a.IID = b.RoleId
                            and b.UserId = @UserId
                            order by case when b.userId is null then 2 else 1 end  ,RoleCode";


            SqlParameter[] paras = {
                                new SqlParameter("@UserId",userId)
                                };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                string x = r["x"].ToString();
                UserRole u = new UserRole();
                u.id = r["IID"].ToString();
                u.code = r["RoleCode"].ToString();
                u.name = r["RoleName"].ToString();
                if (x == "1")
                    u.used = true;
                else
                    u.used = false;

                us.Add(u);
            }
            return us;
        }



        public bool CheckUserToken(string token)
        {

            string[]  tt = token.Split(new[] { '_' });
            string userCode = tt[0];
      
            string sql = "select b.Code,a.UserName,a.UserCode,a.PasswordMd5,b.Name as CompanyName from [UserList] a inner join CompanyList b on a.CompanyId = b.IID where a.UserCode=@UserCode";
            SqlParameter[] para = {
                    new SqlParameter("@UserCode",userCode)
            };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(), CommandType.Text, sql, para).Tables[0];
            foreach( DataRow r in dt.Rows)
            {
              
                string userName =r["UserName"].ToString();
                string passwordMd5 = r["PasswordMd5"].ToString();
                string companyCode = r["Code"].ToString();
     
                string x = string.Format("{0}:{1}:{2}:{3}", userCode, passwordMd5, userName, companyCode);

                string xtoken = string.Format("{0}_{1}", userCode, MD5Encrypt(x));

                if (xtoken == token)
                    return true;
            }

            return false;
           
        }

        public HoderPurview GetHolderPurview(string holderId)
        {
            HoderPurview hp = new HoderPurview();
            hp.HolderId = holderId;
            string sql  =@"select b.Name,b.IID,ISNULL(holderId,'') HolderId from FunctionPurview a right join FunctionList b on a.FunctionId = b.IID
                            and a.HolderId = @HolderId
                            order by b.Code";
            SqlParameter[] paras = {
                                new SqlParameter("@HolderId",holderId)
                                };

            DataTable dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras).Tables[0];

            foreach( DataRow r in dt.Rows)
            {
                FunctionPurview fp = new FunctionPurview();
                fp.id = r["IID"].ToString();
                fp.Name = r["Name"].ToString();
                fp.Allowed = string.IsNullOrEmpty(r["HolderId"].ToString())?false:true;
                hp.FunctionPurviews.Add(fp);
            }


            sql = @"select b.Name,b.IID,ISNULL(holderId,'') HolderId  from BusinessModelPurview a right join BusinessModelList b on a.ModelId = b.IID
                        and a.HolderId = @HolderId
                        order by b.Name";
            dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                ModelPurview fp = new ModelPurview();
                fp.id = r["IID"].ToString();
                fp.Name = r["Name"].ToString();
                fp.Forbidden = string.IsNullOrEmpty(r["HolderId"].ToString()) ? false : true;
                hp.ModelPurviews.Add(fp);
            }


            sql = "select PKID,ParentID,Name from NewReportFolder where IsFavorite = 0 order by Name";
            dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                ReportPurview fp = new ReportPurview();
                fp.id = r["PKID"].ToString();
                fp.name = r["Name"].ToString();
                if (string.IsNullOrEmpty(r["ParentID"].ToString()))
                    fp.parentId = null;
                else
                    fp.parentId = r["ParentID"].ToString();
                fp.type = "Folder";
                fp.canView = fp.canModify = fp.canNotExport = fp.canNotPrint = false;
               
                hp.ReportPurviews.Add(fp);
            }


            sql = @"select a.IID,a.Name,isnull(HolderId,'') HolderId ,FolderId, isnull(b.CanView,0) canview,isnull(b.CanModify,0) CanModify, isnull(b.CanNotExport,0) CanNotExport, isnull(b.CanNotPrint,0) CanNotPrint  from NewReportList a left join ReportPurview b on a.IID = b.ReportId
                     and b.HolderId = @HolderId
                    order by a.Name";
            dt = SqlHelper.ExecuteDataset(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                ReportPurview fp = new ReportPurview();
                fp.id = r["IID"].ToString();
                fp.name = r["Name"].ToString();
                fp.parentId = r["FolderId"].ToString();
                fp.type = "Report";
                fp.canView = Convert.ToBoolean(r["canview"]);
                fp.canModify = Convert.ToBoolean(r["CanModify"]);
                fp.canNotExport = Convert.ToBoolean(r["CanNotExport"]);
                fp.canNotPrint = Convert.ToBoolean(r["CanNotPrint"]);

                hp.ReportPurviews.Add(fp);
            }

            return hp;
        }

        public void SaveHolderPurview(string holderId, string resId, int op, bool result)
        {

            if (op <= 4)
                UpdateReportHolder(holderId, resId, op, result);
            else if (op == 5)
                UpdateFunctionPurview(holderId, resId, result);
            else if (op == 6)
                UpdateModelPurview(holderId, resId, result);
        }
        private void UpdateFunctionPurview(string holderId, string resId, bool result)
        {
         
            string sql = @"delete from  [FunctionPurview] where HolderId = @HolderId and FunctionId = @ResId";
            
            SqlParameter[] paras = {
                                new SqlParameter("@HolderId",holderId),
                                new SqlParameter("@ResId",resId)
            };


            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

            if( result)
            {
                sql = @"insert into [FunctionPurview](HolderId,FunctionId) values(@HolderId,@ResId)";

                SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);
            }
        }

        private void UpdateModelPurview(string holderId, string resId, bool result)
        {
            string sql = @"delete from  [BusinessModelPurview] where HolderId = @HolderId and ModelId = @ResId";

            SqlParameter[] paras = {
                                new SqlParameter("@HolderId",holderId),
                                new SqlParameter("@ResId",resId)
            };


            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

            if (result)
            {
                sql = @"insert into [BusinessModelPurview](HolderId,ModelId) values(@HolderId,@ResId)";

                SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);
            }
        }

        private void UpdateReportHolder( string holderId,string resId, int op, bool result)
        {
            string sql = @"if not exists(select 1 from ReportPurview where HolderId = @HolderId and ReportId = @ReportId)
                                insert into ReportPurview(HolderId,ReportId,CanView,CanModify,CanNotExport,CanNotPrint) values(@HolderId,@ReportId,0,0,0,0)";
            SqlParameter[] paras = {
                                new SqlParameter("@HolderId",holderId),
                                new SqlParameter("@ReportId",resId),
                                new SqlParameter("@Result",result)
            };
            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);


            if (op == 1)
                sql = "update ReportPurview set CanView=@Result from ReportPurview where HolderId=@HolderId and ReportId=@ReportId";
            else if (op == 2)
                sql = "update ReportPurview set CanModify=@Result from ReportPurview where HolderId=@HolderId and ReportId=@ReportId";
            else if (op == 3)
                sql = "update ReportPurview set CanNotExport=@Result from ReportPurview where HolderId=@HolderId and ReportId=@ReportId";
            else if (op == 4)
                sql = "update ReportPurview set CanNotPrint=@Result from ReportPurview where HolderId=@HolderId and ReportId=@ReportId";

            SqlHelper.ExecuteNonQuery(ConfigurationUtil.GetMetaConnString(this.UserId), CommandType.Text, sql, paras);

        }
    }
}
