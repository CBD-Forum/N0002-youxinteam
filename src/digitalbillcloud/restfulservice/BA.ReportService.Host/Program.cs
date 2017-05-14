
using BA.ReportEngine.Designtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Web.Http.SelfHost.Channels;

namespace BA.ReportService.Host
{
    


    class Program
    {
        static void Main(string[] args)
        {

            // Assembly.Load("Lind.DDD.TestApi");  //手工加载某个api程序集的controller


            string port = System.Configuration.ConfigurationManager.AppSettings["Port"];

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(string.Format("http://locahost:{0}",port));


            config.EnableCors();
            config.MaxReceivedMessageSize = int.MaxValue;
            config.MaxBufferSize = int.MaxValue;
            config.MessageHandlers.Add(new MessageHandler2());
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CustomContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new JsonReportBlockConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new JsonModelItemConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new JsonTablixItemConverter());
            config.Routes.MapHttpRoute("api", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
           

            HttpSelfHostServer server = new HttpSelfHostServer(config);
           
            server.OpenAsync().Wait();
            Console.WriteLine(string.Format("数字票据云微服务启动成功,端口:{0}",port));
            Console.ReadLine();
           
        }
    }

   

}
