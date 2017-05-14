using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using BA.ReportModel;
using BA.ReportEngine.Designtime;
using BA.ReportEngine.Runtime;
using BA.ReportEngine.ProxyReportRuntime;
using BA.ReportEngine.ProxyReportRuntime.JSON;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Data;
using BA.PortalManagment;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BA.ReportService.Host
{

    [EnableCors("http://localhost:8020", "*", "*")]
    public class RestfulApiController: ApiController
    {
        public string Options()
        {
            return null; // HTTP 200 response with empty body 
        }

        private string md5(string str)
        {
            string res;
            MD5 m = new MD5CryptoServiceProvider();
            byte[] s = m.ComputeHash(UnicodeEncoding.UTF8.GetBytes(str));
            res = BitConverter.ToString(s);
            res = res.ToLower();
            res = res.Replace("-", "");
            return res;
        }

        private void CreateChildTreeItem(KendoTreeListItem parent, JObject jo)
        {
            foreach (JProperty p in jo.Properties())
            {
                KendoTreeListItem ki = new KendoTreeListItem();
                ki.text = p.Name;

                ki.fieldType = p.Value.Type.ToString();
                parent.items.Add(ki);

                if (p.Value is JValue)
                {
                    ki.sampleValue = p.Value.ToString();
                }

                if (p.Value is JObject)
                {
                    CreateChildTreeItem(ki, p.Value as JObject);
                }

                else if (p.Value is JArray)
                {
                    if ((p.Value as JArray).Count > 0)
                    {
                        JObject m = (p.Value as JArray)[0] as JObject;

                        CreateChildTreeItem(ki, m);
                    }
                }
            }
        }


        public List<KendoTreeListItem> GetUOrder()
        {
            string appkey = "005ae74a725129dd8e17c56692d60534d86d3b16";
            string secret = "fda7d2fd6a892a826152e0f6ae17ad4cb82d5961";
            string token = "!*NsT7ZBzNau8n6RXFskTP7ZF~VhNMEwYNtGEp8B3itng*-";


            int pageSize = 1;
            int pageIndex = 1;


            StringBuilder sb = new StringBuilder();
            sb.Append(secret);
            sb.Append(string.Format("appkey{0}", appkey));
            sb.Append(string.Format("format{0}", "json"));
            sb.Append(string.Format("pageindex{0}", pageIndex));
            sb.Append(string.Format("pagesize{0}", pageSize));
            sb.Append(string.Format("token{0}", token));
            sb.Append(secret);
            string sign = sb.ToString();

            sign = md5(sign);
            sign = sign.ToUpper();

            string u = string.Format("https://udhapi.yonyouup.com/rs/Orders/getSummaryOrders?appkey={0}&token={1}&pagesize={2}&pageindex={3}&format=json&sign={4}", appkey, token, pageSize, pageIndex, sign);


            Uri url = new Uri(u);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string jsonString = wc.DownloadString(url);
            JObject obj = JObject.Parse(jsonString);


            string ordernos = "U-O-541-189-20151018-000002";

            sb = new StringBuilder();
            sb.Append(secret);
            sb.Append(string.Format("appkey{0}", appkey));
            sb.Append(string.Format("format{0}", "json"));
            sb.Append(string.Format("ordernos{0}", ordernos));
            sb.Append(string.Format("token{0}", token));
            sb.Append(secret);
            sign = sb.ToString();
            sign = md5(sign);
            sign = sign.ToUpper();
            u = string.Format("https://udhapi.yonyouup.com/rs/Orders/getOrders?appkey={0}&token={1}&ordernos={2}&format=json&sign={3}", appkey, token, ordernos, sign);

            url = new Uri(u);
            wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            jsonString = wc.DownloadString(url);
            obj = JObject.Parse(jsonString);


            KendoTreeListItem root = new KendoTreeListItem();
            root.id = null;

            foreach (JProperty p in obj.Properties())
            {
                KendoTreeListItem pitem = new KendoTreeListItem();
                pitem.fieldType = p.Value.Type.ToString();
                pitem.text = p.Name;
                root.items.Add(pitem);

                if (p.Value is JValue)
                {
                    pitem.sampleValue = p.Value.ToString();
                }

                if (p.Value is JObject)
                {
                    CreateChildTreeItem(pitem, p.Value as JObject);
                }
                else if (p.Value is JArray)
                {
                    if ((p.Value as JArray).Count > 0)
                    {
                        JObject m = (p.Value as JArray)[0] as JObject;

                        CreateChildTreeItem(root, m);
                    }
                }
            }

            return root.GetOneLevelList();

        }
    }
}
