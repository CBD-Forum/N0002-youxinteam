using BA.ReportEngine.ProxyReportRuntime;
using BA.ReportEngine.ProxyReportRuntime.JSON;
using BA.ReportModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BA.ReportService.Host
{
    public class CustomContractResolver : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty p = base.CreateProperty(member, memberSerialization);
            if (p.PropertyName == "columns")
            {
                //依性别决定是否要序列化
                p.ShouldSerialize = instance =>
                {
                    if (instance is Column)
                    {
                        Column c = (Column)instance;
                        return c.columns.Count > 0;
                    }


                    return true;
                };
            }
            else if (p.PropertyName == "width")
            {
                //依性别决定是否要序列化
                p.ShouldSerialize = instance =>
                {
                    if (instance is Column)
                    {
                        Column c = (Column)instance;
                        return c.width > 0;
                    }
                    return true;
                };
            }
            else if (p.PropertyName == "field")
            {
                //依性别决定是否要序列化
                p.ShouldSerialize = instance =>
                {
                    if (instance is Column)
                    {
                        Column c = (Column)instance;
                        return !string.IsNullOrEmpty(c.field);
                    }
                    return true;
                };
            }

            else if (p.PropertyName == "items")
            {
                //依性别决定是否要序列化
                p.ShouldSerialize = instance =>
                {
                    if (instance is H5MenuItem)
                    {
                        H5MenuItem c = (H5MenuItem)instance;
                        return c.items.Count > 0;
                    }
                    return true;
                };
            }


            return p;
        }
    }
}
