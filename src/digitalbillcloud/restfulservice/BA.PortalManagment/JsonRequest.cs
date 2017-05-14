using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class JsonRequest
    {
        public string id { get; set; }
        public string jsonrpc { get; set; }
        public string method { get; set; }

        [JsonProperty("params")]
        public List<string> parameters { get; set; }

        public JsonRequest()
        {
            this.jsonrpc = "2.0";
            this.parameters = new List<string>();
        }
    }
}
