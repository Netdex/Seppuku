using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Seppuku.Module.Utility;

namespace Seppuku.Module
{
    public enum EmitType
    {
        Start,
        Stop,
        Trigger,
        Reset
    }

    /// <summary>
    ///     Manages all MEF extensions
    ///     Q: Why are you wrapping a singleton instance with static methods that just call the object methods?
    ///     A: MEF requires the array of contracts be inside an object, but I want the module manager to be
    ///     static in nature, so naturally this disaster of a design pattern arose.
    /// </summary>
    public class ModuleManager
    {
        public const string AssemblyDirectory = "Modules";

        public static readonly string AssemblyPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            AssemblyDirectory);

        private static ModuleManager _instance;

        private CompositionContainer _container;

        /// <summary>
        ///     Singleton instance
        /// </summary>
        private static ModuleManager I => _instance ?? (_instance = new ModuleManager());

        [ImportMany]
        public Lazy<SeppukuModule>[] SeppukuModules { get; set; }

        public static Lazy<SeppukuModule>[] Modules => I.SeppukuModules;

        #region Static Singleton Wrappers

        public static bool Init() => I.Initialize();
        public static void Emit(EmitType type) => I.EmitMessage(type);

        #endregion

        #region Singleton Instance Methods

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
        public void EmitMessage(EmitType type)
        {
            foreach (var tm in SeppukuModules)
                try
                {
                    switch (type)
                    {
                        case EmitType.Start:
                            tm.Value.OnStart();
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
                    C.WriteLine("`e &f[{0}]&r {1} is misbehaving!", type, tm.Value.Name);
                    Console.WriteLine(e.ToString());
                }
        }

        #endregion
    }
}