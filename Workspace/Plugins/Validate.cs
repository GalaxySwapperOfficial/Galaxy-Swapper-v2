using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Plugins
{
    public static class Validate
    {
        public static bool IsValid(FileInfo fileInfo, out JObject parse)
        {
            parse = JObject.Parse(File.ReadAllText(fileInfo.FullName));
            return true;
        }
    }
}
