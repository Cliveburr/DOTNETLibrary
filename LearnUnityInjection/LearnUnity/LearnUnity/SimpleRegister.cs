using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Attributes;

namespace LearnUnity
{
    class SimpleRegister
    {
        public static void Run()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<ITestingOne, TestingOneEasy>();
                container.RegisterType<IFatherAll, FatherAll>();
                container.RegisterType<ITool, Tool>();
                //container.RegisterTypes(
                //  AllClasses.FromLoadedAssemblies(),
                //  WithMappings.FromMatchingInterface,
                //  WithName.Default
                  /*WithLifetime.ContainerControlled for singleton */
                //);


                var testingOne = container.Resolve<ITestingOne>();
                Console.WriteLine($"testingOne.Name = {testingOne.Name}");

                var fatherAll = container.Resolve<IFatherAll>();
                Console.WriteLine($"fatherAll.TestingOneName = {fatherAll.TestingOneName}");
                Console.WriteLine($"fatherAll.Tool.Work() = {fatherAll.Tool.Work()}");
            }
        }
    }
    public interface ITestingOne
    {
        string Name { get; }
    }

    public class TestingOneEasy : ITestingOne
    {
        private static int idIndex = 0;
        private int myid;

        public TestingOneEasy()
        {
            myid = idIndex++;
        }

        public string Name { get => $"Easy{myid}"; }
    }

    public interface IFatherAll
    {
        string TestingOneName { get; }
        ITool Tool { get; set; }
    }

    public class FatherAll : IFatherAll
    {
        private string testingOneName;

        [Dependency]
        public ITool Tool { get; set; }

        public FatherAll(ITestingOne testingOne)
        {
            testingOneName = testingOne.Name;
        }

        public string TestingOneName => testingOneName;
    }

    public interface ITool
    {
        string Work();
    }

    public class Tool : ITool
    {
        public string Work()
        {
            return "Do some work";
        }
    }
}
