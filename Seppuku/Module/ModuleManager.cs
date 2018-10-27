using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Seppuku.Switch;

namespace Seppuku.Module
{
    public enum EmitType
    {
        Start,
        Stop,
        Trigger,
        Reset
    }

    public class ModuleManager
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public const string AssemblyDirectory = "Modules";

        public static readonly string AssemblyPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            AssemblyDirectory);

        public static ModuleManager Instance = new ModuleManager();


        [ImportMany]
        public Lazy<SeppukuModule>[] SeppukuModules { get; set; }
        public static Lazy<SeppukuModule>[] Modules => Instance.SeppukuModules;
        private CompositionContainer _container;

        public bool Initialize()
        {
            try
            {
                Directory.CreateDirectory(AssemblyPath);
                var catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                catalog.Catalogs.Add(new DirectoryCatalog(AssemblyPath, "Seppuku.Module.*.dll"));
                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Send a message to all modules
        /// </summary>
        public void Emit(EmitType type)
        {
            foreach (var tm in SeppukuModules)
                try
                {
                    switch (type)
                    {
                        case EmitType.Start:
                            tm.Value.OnStart(SwitchControl.IsTriggered);
                            break;
                        case EmitType.Stop:
                            tm.Value.OnStop();
                            break;
                        case EmitType.Reset:
                            tm.Value.OnReset();
                            break;
                        case EmitType.Trigger:
                            tm.Value.OnTrigger();
                            break;
                    }
                }
                catch (Exception e)
                {
                    L.Error(e, "[{0}] {1} is misbehaving!", type, tm.Value.Name);
                }
        }

    }
}