using RazorEngine.Compilation;
using RazorEngine.Compilation.ReferenceResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using System.Text.RegularExpressions;

namespace Utility.Razor
{
    public class HuberReferenceResolver : IReferenceResolver
    {

        static List<CompilerReference> compilerReference;
        static HuberReferenceResolver()
        {
            //加载本地所有类库,@using 使用
            compilerReference = new List<CompilerReference>();
            IEnumerable<string> loadedAssemblies = (new UseCurrentAssembliesReferenceResolver())
                .GetReferences(null, null)
                .Select(r => r.GetFile())
                .ToArray();
            foreach (var l in loadedAssemblies)
            {
                compilerReference.Add(CompilerReference.From(l));
                Global.log.Debug(l);
            }

            XmlConfigUtil xmlUtil = new XmlConfigUtil(Global.XmlDir + @"RazorEngine.xml");
            var assemblyList = xmlUtil.Read("", "CompilerReference");
            try
            {
                assemblyList = assemblyList.Replace("[Global.AppDir]", Global.AppDir);
                Regex regex = new Regex(@"#(.+)\r");

                var matches = regex.Matches(assemblyList);

                foreach (Match match in matches)
                {
                    compilerReference.Add(CompilerReference.From(match.Groups[1].Value));
                }

            }
            catch (Exception e)
            {
                Global.log.Error("HuberReferenceResolver", e);
            }

        }

        public string FindLoaded(IEnumerable<string> refs, string find)
        {
            return refs.First(r => r.EndsWith(System.IO.Path.DirectorySeparatorChar + find));
        }
        public IEnumerable<CompilerReference> GetReferences(TypeContext context, IEnumerable<CompilerReference> includeAssemblies)
        {

            #region 加载依赖程序集  此处是加载所有程序集，效率需要改进

            return compilerReference;
            #endregion
        }
    }
}
