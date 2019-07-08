using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DistrictsLib.Json
{
    public class InterfaceContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            if (!type.IsInterface)
            {
                var intTypes = type.GetTypeInfo().ImplementedInterfaces.ToList();

                if (intTypes.Any())
                    return intTypes.SelectMany(x => base.CreateProperties(x, memberSerialization)).ToList();
            }

            return base.CreateProperties(type, memberSerialization);
        }
    }
}
