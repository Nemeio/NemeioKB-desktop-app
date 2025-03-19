using System;
using System.Linq;
using System.Reflection;

namespace Nemeio.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static Assembly GetAssemblyByName(this Assembly currentAssembly, string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
        }
    }
}
