using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using Utility;
using RazorEngine.Configuration;
using System.IO;


namespace Utility.Razor
{
    public class RazorSvr
    {
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IRazorEngineService service;
        InvalidatingCachingProvider cache = new InvalidatingCachingProvider(t => { });
        DelegateTemplateManager tempMgr = new DelegateTemplateManager();
        FileSystemWatcher m_Watcher = new FileSystemWatcher();

        public RazorSvr(Type baseTemplateType = null)
        {
            try
            {
                var config = new TemplateServiceConfiguration();
                config.Debug = false; //System.Diagnostics.Debugger.Break();
                if (baseTemplateType == null)
                    config.BaseTemplateType = typeof(RazorTemplateBase<>);
                else
                    config.BaseTemplateType = baseTemplateType;

                config.ReferenceResolver = new HuberReferenceResolver();
                config.DisableTempFileLocking = true; //E:\temp\S-1-5-21-1703685765-3720740729-2978859178-500\temp\RazorEngine_***
                //config.CachingProvider = new DefaultCachingProvider(t => { });//不往控制台输出清除日志的警告

                config.CachingProvider = cache;
                cache.InvalidateAll();

                config.TemplateManager = tempMgr;

                service = RazorEngineService.Create(config);

                //添加文件修改监控，以便在cshtml文件修改时重新编译该文件
                m_Watcher.Path = Global.TemplateDir;
                m_Watcher.IncludeSubdirectories = true;
                m_Watcher.Filter = "*.*";
                m_Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                m_Watcher.Created += new FileSystemEventHandler(OnChanged);
                m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
                m_Watcher.Deleted += new FileSystemEventHandler(OnChanged);
                m_Watcher.EnableRaisingEvents = true;
            }
            catch (TemplateCompilationException ex)
            {
                ex.Errors.ToList().ForEach(p => log.Error(p.ErrorText, ex));
            }
        }

        //当视图被修改后清除缓存
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".cshtml"))
            {
                string s = e.FullPath.Replace(Global.TemplateDir, "");
                InvalidateCache(s);
                log.Info("模板更改：" + e.FullPath);
            }
        }

        //使缓存无效
        public void InvalidateCache(string key)
        {
            cache.InvalidateCache(service.GetKey(key));
        }

        public void Compile(string key, string templateSource = null)
        {
            ITemplateKey iKey = service.GetKey(key);
            ICompiledTemplate cacheTemplate;
            cache.TryRetrieveTemplate(iKey, null, out cacheTemplate);

            if (cacheTemplate == null)
            {
                if(templateSource == null)
                    templateSource = FilesUtil.ReadTxtFile(Global.TemplateDir + key);

                tempMgr.RemoveDynamic(iKey);
                service.Compile(templateSource, key);
            }
        }

        public string Run(string key, string templateSource = null, object model = null)
        {
            Compile(key, templateSource);

            return service.Run(key, null, model);
        }


    }
}
