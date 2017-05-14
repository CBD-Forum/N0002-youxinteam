using BA.ReportEngine.Designtime;
using BA.ReportModel;
using BA.ReportModel.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BA.ReportService.Host
{
    public class BusinessModelController : ApiController
    {
        [HttpGet]
        public List<OlapConnection> GetOlapConnectionList(string token)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetOlapConnectionList();
        }

        public List<RelationDiagram> GetRelationDiagramList(string token,string modelId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GeRelationDiagramList(modelId);
        }

        public RelationDiagram GetRelationDiagram(string token,string diagramId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetRelationDiagram(diagramId);

        }
        [HttpGet]
        public void DeleteRelationDiagram(string token,string diagramId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            fmf.DeleteRelationDiagram(diagramId);

        }


        [HttpGet]
        public bool DeleteBusinessModelDefinition(string token,string modelId)
        {
            BusinessModelFacade bmf = new BusinessModelFacade(token);
            return bmf.DeleteBusinessModelDefinition(modelId);
        }
        [HttpGet]
        public List<BusinessModelDefinition> GetBusinessModelDefinitionList(string token)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetBusinessModelDefinitionList();
        }

        public void SaveRelationDiagram(string token,RelationDiagram diagram)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            fmf.SaveRelationDiagram(diagram);
        }

        public OlapConnection GetOlapConnection(string token,string connId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetOlapConnection(connId);
        }

        public string GetTestOlapConnection(string token,string connId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.TestOlapConnection(connId);
        }

        public List<string> GetDatabases(string token,int connType, string serverName, string userName, string password)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetDatabases(connType, serverName, userName, password);
        }
        public List<BusinessModelTableField> GetBusinessModelTableFields(string token,string connId, string tableName)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetBusinessModelTableFields(connId, tableName);
        }


        public bool DeleteOlapConnection(string token,string connId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.DeleteOlapConnection(connId);
        }

        public bool SaveOlapConnection(string token,OlapConnection connection)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.SaveOlapConnection(connection);
        }

      

        public BusinessModelDefinition GetCloundBusinessModelDefinition(string modelId, string token)
        {

            CloudProxy cp = new CloudProxy(token);
            return cp.GetBusinessModelDefinition(modelId);

        }

        public BusinessModelDefinition GetBusinessModelDefinitionOnClound(string modelId, string userName, string passwordMd5)
        {

            BusinessModelFacade fmf = new BusinessModelFacade(userName, passwordMd5);
            return fmf.GetBusinessModel(modelId);

        }

        public List<BusinessModelDefinition> GetCloudBusinessModelDefinitionList(string userName,string passwordMd5)
        {
            return BusinessModelFacade.GetBusinessModelDefinitionList(userName, passwordMd5);
        }

        public BusinessModelDefinition GetBusinessModelDefinition(string token,string modelId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetBusinessModel(modelId);
        }

        public string SaveBusinessModelDefinition(string token,BusinessModelDefinition model)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.SaveBusinessModelDefinition(model);
        }

        public List<BusinessModelTable> GetBusinessModelTables(string token,string connId)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.GetBusinessModelTables(connId);
        }

        [HttpGet]
        public VerificationResult VerifyExpression(string token,string connId, string expression)
        {
            BusinessModelFacade fmf = new BusinessModelFacade(token);

            return fmf.VerifyExpression(connId, expression);
        }
    }

}