using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Plugins
{
    public static class Validate
    {
        public static bool IsValid(string file, out JObject parse)
        {
            parse = null;
            return true;
        }
    }
}
