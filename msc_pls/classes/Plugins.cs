using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.Remoting;

namespace msc_pls.classes
{
    public class Plugins
    {
        private String PluginPath;
        private List<Plugin> PluginStore = new List<Plugin>();
        private List<String> Log = new List<String>();

        private class Plugin
        {
            public Type Type;
            public Object PluginObject;

            public Plugin(Type type, Object plugin)
            {
                this.Type = type;
                this.PluginObject = plugin;
            }
        }

        public Plugins(String PluginPath)
        {
            this.PluginPath = PluginPath;
            this.Load();
        }

        private void Load()
        {
            if (Directory.Exists(this.PluginPath))
            {
                foreach (String plugin in Directory.GetFiles(this.PluginPath))
                {
                    Assembly assembly = Assembly.LoadFrom(plugin);
                    this.Register(assembly);
                }
            }
        }

        private void Register(Assembly Assembly)
        {
            foreach (Type type in Assembly.GetTypes())
            {
                if (type.Name == "Plugin")
                {
                    Object plugin = Activator.CreateInstance(type);
                    this.PluginStore.Add(new Plugin(type, plugin));
                    break;
                }
            }
        }

        public void Run(String EventName, object[] Arguments = null)
        {
            // @TODO improve performance with method caching
            foreach (Plugin plugin in this.PluginStore)
            {
                try
                {
                    MethodInfo method = plugin.Type.GetMethod(EventName);
                    method.Invoke(plugin.PluginObject, Arguments);
                }
                catch (Exception e)
                {
                    this.Log.Add(e.ToString());
                    if (EventName != "Log")
                        this.Run("Log", new object[] { this.Log });
                }
            }
        }
    }
}
