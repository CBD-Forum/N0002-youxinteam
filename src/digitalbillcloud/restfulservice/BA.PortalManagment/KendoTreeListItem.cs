using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class KendoTreeListItem
    {
        public string id { get; set; }
        public string parentId { get; set; }

        public string cname { get; set; }
        public string text { get; set; }

        public string fieldType { get; set; }

        public string sampleValue { get; set; }

        public int order { get; set; }
        public string usingAnalysis { get; set; }
        public List<KendoTreeListItem> items { get; set; }

        public string htmlPage { get; set; }

        public KendoTreeListItem()
        {
            this.items = new List<KendoTreeListItem>();
            this.id = Guid.NewGuid().ToString();

        }


        public List<KendoTreeListItem> GetOneLevelList()
        {
            List<KendoTreeListItem> list = new List<KendoTreeListItem>();
            GetOneLevelList(list, this);

            foreach (KendoTreeListItem item in list)
                item.order = list.IndexOf(item) + 1;
            return list;
        }

        private void GetOneLevelList(List<KendoTreeListItem> list, KendoTreeListItem item)
        {
            foreach (KendoTreeListItem x in item.items)
            {
                x.parentId = item.id;
                list.Add(x);
                GetOneLevelList(list, x);
            }
        }
    }
}
