using BA.ReportEngine.Runtime;
using BA.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BA.ReportService.Host
{
    public class ReportDataSourceResult
    {
        public string Message { get; set; }
        public bool Successful { get; set; }
    }

    /*

    public class FilterItm
    {
        public string Name { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }

    }


    public class ReportFilter
    {
        public string Name { get; set; }

        public List<FilterItm> Items { get; set; }
        public ReportFilter()
        {
            this.Items = new List<FilterItm>();
        }
    }

    */

    public class ReportDataSourceController: ApiController
    {
        public ReportDataSourceResult Calculate(string companyId, string reportId, string tableName,string tempTableName, [FromBody] RuntimeReportFilter filter)
        {
            ReportDataSourceResult result = new ReportDataSourceResult();

            string userName = "root";
            string password = string.Empty;
            string serverName = "localhost";
            string databaseName = "test2";

            string connString = string.Format("Database='{3}';Data Source='{2}';User Id='{0}';Password='{1}';charset='utf8';pooling=true", userName, password, serverName, databaseName);

            string sql = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (tableName.ToLower().Equals("dim_product"))
            {

                for (int i = 0; i < 100; i++)
                {
                    sql = string.Format("insert into {0} (Code,Name) values('{1}','笔记本-{1}');", tempTableName, i + 1);
                    sb.AppendLine(sql);
                }
            }
            else if (tableName.ToLower().Equals("fact_sale"))
            {
                for (int i = 0; i < 100; i++)
                {
                    sql = string.Format("insert into {0} (IID,ProductCode,Quantity) values({2},'{2}',{1});", tempTableName, (new Random()).Next(), i + 1);
                    sb.AppendLine(sql);
                }
            }

            sql = sb.ToString();
            MySqlHelper.ExecuteNonQuery(connString, CommandType.Text, sql);


            return result;
        }
    }
}
