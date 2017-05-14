using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UFIDA.BI.DataAccess;

namespace BA.PortalManagment
{
    public class ConfigurationUtil
    {
        public static string GetMetaConnString()
        {
            /*
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.InitialCatalog = "U8RAMeta";
            sb.UserID = "sa";
            sb.Password = "softdevelop";
            sb.DataSource = "127.0.0.1";
            return sb.ToString();
            */

            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\Configuration\\MetaDatabaseConn.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);

            XmlElement xe = xd.DocumentElement;
            xe = xe.ChildNodes[0] as XmlElement;
            sb.UserID = xe.GetAttribute("UID");
            sb.Password = EncryptHelper.Decrypt(xe.GetAttribute("Password"));
            sb.InitialCatalog = xe.GetAttribute("Database");
            sb.DataSource = xe.GetAttribute("ServerName");

            return sb.ToString();

        }

        public static string GetMetaConnString(string userId)
        {
            /*
            string meta = GetMetaConnString();

            string sql = @"select c.MetaDatabase from [DaaSMeta]..[UserList] u
                            inner join[DaaSMeta]..[CompanyList]
                                    c on u.CompanyId= c.IID
                            and u.IID=@UserId ";
            SqlParameter[] para = {
                    new SqlParameter("@UserId",userId)
            };

            DataTable dt = SqlHelper.ExecuteDataset(meta, System.Data.CommandType.Text, sql, para).Tables[0];

            if (dt.Rows.Count > 0)
            {
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(meta);
                sb.InitialCatalog = dt.Rows[0]["MetaDatabase"].ToString();
                return sb.ToString();
            }

            return meta;
            */
            return GetMetaConnString();

        }
    }
}
