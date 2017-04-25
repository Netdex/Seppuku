using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module
{
    class ModuleManager
    {
        public const string AssemblyDirectory = "Modules";
        public static readonly string AssemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            AssemblyDirectory);
        public static ModuleManager Instance => _instance ?? (_instance = new ModuleManager());
        private static ModuleManager _instance;

        private CompositionContainer _container;
        [ImportMany] public Lazy<TriggerModule>[] TriggerModules { get; set; }

        public bool Initialize()
        {
            try
            {
                Directory.CreateDirectory(AssemblyPath);
                var catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                catalog.Catalogs.Add(new DirectoryCatalog(AssemblyPath, "*.dll"));
                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void EmitStart()
        {
            foreach (var tm in TriggerModules)
                tm.Value.Start();
        }

        public void EmitTrigger()
        {
            foreach (var tm in TriggerModules)
                tm.Value.Trigger();
        }

        public void EmitStop()
        {
            foreach (var tm in TriggerModules)
                tm.Value.Stop();
        }
    }
}
