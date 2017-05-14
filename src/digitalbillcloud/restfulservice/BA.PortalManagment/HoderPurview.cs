using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class ReportPurview
    {
        public string id { get; set; }

        public string parentId { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public bool canView { get; set; }

        public bool canModify { get; set; }

        public bool canNotExport { get; set; }

        public bool canNotPrint { get; set; }
    }

    public class FunctionPurview
    {
        public string id { get; set; }
        public string Name { get; set; }

        public string parentId { get; set; }

        public bool Allowed { get; set; }
    }

    public class ModelPurview
    {
        public string id { get; set; }
        public string Name { get; set; }

        public bool Forbidden { get; set; }

        public string parentId { get; set; }
    }
    public class HoderPurview
    {
        public string HolderId { get; set; }

        public List<ReportPurview> ReportPurviews { get; set; }
        public List<FunctionPurview> FunctionPurviews { get; set; }
        public List<ModelPurview> ModelPurviews { get; set; }

        public HoderPurview()
        {
            this.ReportPurviews = new List<ReportPurview>();
            this.FunctionPurviews = new List<FunctionPurview>();
            this.ModelPurviews = new List<ModelPurview>();
        }
    }
}
