using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module
{
    public abstract class TriggerModule
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public TriggerModule()
        {
            
        }

        public TriggerModule(string name, string description)
        {
            Name = name;
            Description = description;
        }
        

        public abstract void Start();
        public abstract void Trigger();
        public abstract void Stop();

        public void Log(string s)
        {
            Console.WriteLine($"[{Name}] {s}");
        }
    }
}
