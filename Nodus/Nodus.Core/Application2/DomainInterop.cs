using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Application2
{
    [Serializable()]
    public class DomainInterop : MarshalByRefObject
    {
        private Assembly _rootAssembly;

        public void SetConsoleOut(TextWriter consoleOut)
        {
            Console.SetOut(consoleOut);
        }

        public void LoadAssembly(string assemblyFile)
        {
            var fullAssemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyFile);
            if (!File.Exists(fullAssemblyFile))
                throw new FileNotFoundException($@"Script file ""${fullAssemblyFile}"" not found!");
            _rootAssembly = Assembly.LoadFile(fullAssemblyFile);
        }

        public void Run(string function, params object[] arguments)
        {
            var split = function.Split('.').ToList();
            var funcName = split.Last();
            split.RemoveAt(split.Count - 1);
            var className = string.Join(".", split);

            var classType = _rootAssembly.GetType(className);
            if (classType == null)
                throw new Exception($@"Class ""${className}"" not found!");

            var methodType = classType.GetMethod(funcName);
            if (methodType == null)
                throw new Exception($@"Method ""${funcName}"" not found on class ""${className}""!");

            var classInst = Activator.CreateInstance(classType);
            methodType.Invoke(classInst, arguments);
        }
    }
}