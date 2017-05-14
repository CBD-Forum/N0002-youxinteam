using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using BA.ReportEngine.Designtime;
using BA.ReportEngine.Runtime;
using BA.ReportEngine.ProxyReportRuntime;
using BA.ReportEngine.ProxyReportRuntime.JSON;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Data;
using BA.ReportEngine.Designtime.Models;

namespace BA.ReportService.Host
{
   
   [EnableCors("http://localhost:8020","*","*")]
   public class ReportController : ApiController
    {
        public string Options()
        {
            return null; // HTTP 200 response with empty body 
        }

     
        /*
        public List<BusinessModelDefinition> GetBusinessModelDefinitionListOnCloud(string userName,string passwordMd5)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(userName,passwordMd5);
            return fmf.GetBusinessModelDefinitionList();
        }
        */


  

      

        [HttpGet]
        public void DeleteFolder( string folderId,string token)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);
            rdf.DeleteFolder(folderId);
        }

        

      

        [HttpGet]
        public void SaveFavoriteReport(string token, string reportId,int docType)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            rdf.SaveFavoriteReport(reportId, docType);
        }


        /*
        [HttpGet]
        public void SaveFavoriteMetro(string userId, string reportId)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade();

            rdf.SaveFavoriteMetro(userId,reportId);
        }

        [HttpGet]
        public void SaveFavoriteReport(string userId, string reportId)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade();

            rdf.SaveFavoriteReport(userId,reportId);
        }

        [HttpGet]
        public void SaveFavoriteAdhocQuery(string userId, string reportId)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade();

            rdf.SaveFavoriteAdhocQuery(userId,reportId);
        }*/

       

        public List<ReportEngine.Designtime.ReportBusinessItem> GetReportList(string token,string folderId)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetReportList(folderId);
        }

       
        /*
        public List<FolderItem> GetFolderList(string token,int type)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetFolderList(type);
        }
        */

        public List<KendoTreeItem> GetFileTree(string token,int type)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetFileTree(type);
        }

        public List<KendoTreeItem> GetCloudFileTree(string token, int type)
        {
            CloudProxy cp = new CloudProxy(token);

            return cp.GetFileTree(type);
        }

        public H5ReportMenu GetH5MenuItemList(string reportId,string token)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetH5MenuItemList();
        }

        [HttpGet]
        public List<ReportBusinessItem> GetFavoriteMetroList(string token)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);
            return rdf.GetFavoriteReportList(2);
        }

        [HttpGet]
        public List<ReportBusinessItem> GetFavoriteReportList(string token)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetFavoriteReportList(0);
        }
        [HttpGet]
        public List<ReportBusinessItem> GetFavoriteQueryList(string token)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetFavoriteReportList(1);
        }
        [HttpGet]
        public List<ReportBusinessItem> GetHistoryReportList(string token)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetHistoryReportList();
        }

        [HttpGet]
        /* 此接口针对UPCloud Report*/
        public ReportFilterContext GetReportFilterContext(string reportId)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(string.Empty);

            return rdf.GetReportFilterContext(reportId);
        }

        [HttpGet]
        public void DeleteUserReport(string reportId,string token,int documentType)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            rdf.DeleteUserReport(reportId, documentType);
        }



      

        [HttpGet]
        public List<ReportSession> GetReportSessionList()
        {
            return ReportEngineApplication.GetReportSessionList();
        }

        [HttpGet]
        public  ReportSession GetReportSession(string userId,string reportId)
        {
            return ReportEngineApplication.GetReportSession(userId, reportId);
        }

        [HttpGet]
        public void CloseReportSession(string userId, string reportId)
        {
             ReportEngineApplication.CloseReportSession(userId, reportId);
        }


     

        /*
        public VerticalTable GetVerticalTableJson(string reportId, string blockId, string userName, int pageIndex, int pageSize, string filterItem, string filterValue,[FromBody]ReportFilter filter)
        {
            NewProxyReportEngine proxyReportEngine = new NewProxyReportEngine(reportId, userName, "U8RAMeta", "yonyou");

            ReportBlockDefinition rbd = proxyReportEngine.GetReportBlockDefinition(reportId, blockId);
            BlockDataTable bdt = proxyReportEngine.CalculatorBlock(reportId, blockId, filterItem, filterValue, pageIndex, pageSize);


            TableJsonStringStrategy strategy = new TableJsonStringStrategy();

            return strategy.CreateJSONObject(rbd, bdt);
        }
        */



        /*
       [HttpPost]
       public VerticalTable GetVerticalTableJson(string reportId, string blockId, string userName, int pageIndex, int pageSize, string filterItem, string filterValue, [FromBody]ReportFilterContext context)
       {
           RuntimeReportFilter filter = new RuntimeReportFilter();

           NewProxyReportEngine proxyReportEngine = ReportEngineApplication.CreateRuntimeReportEngine(userName, reportId, filter);

           ReportBlockDefinition rbd = proxyReportEngine.GetReportBlockDefinition(reportId, blockId);
           BlockDataTable bdt = proxyReportEngine.CalculatorBlock(reportId, blockId, filterItem, filterValue, pageIndex, pageSize);


           TableJsonStringStrategy strategy = new TableJsonStringStrategy();

           return strategy.CreateJSONObject(rbd, bdt);
       }
       */

       
        [HttpGet]
        public  string GetReportName(string token,string reportId)
        {
            ReportDesignerFacade rdf = new ReportDesignerFacade(token);

            return rdf.GetReportName(reportId);
        }

        [HttpGet]
        public AdhocQueryResult AdhocQuery(string token, string queryId,string keyword)
        {

            return NewProxyReportEngine.AdhocQuery(token, queryId, keyword);

           
        }

        
    }
   

}
