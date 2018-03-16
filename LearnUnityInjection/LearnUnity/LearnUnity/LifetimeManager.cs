using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace LearnUnity
{
    public class LifetimeManager
    {
        public static void Run()
        {
            using (var container = new CustomResolve())
            {
                //container.RegisterType<ITestingThree, TestingThree>(
                //    new PerThreadLifetimeManager());

                var threadIds = new string[] { "Thread 0", "Thread 1", "Thread 2", "Thread 3", "Thread 4", "Thread 5" };

                var threads = threadIds
                    .Select(t => new System.Threading.Thread(() =>
                    {
                        var testingThree = container.Resolve<ITestingThree>();

                        Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId} - {t} have TestingThreeName = {testingThree.name}");

                        testingThree.DoWork();
                    }))
                    .ToArray();

                foreach (var thread in threads)
                {
                    thread.Start();
                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }

            //IDependencyResolver
            //    IFactProvider
        }

        public static void RunCustomResolve()
        {
            using (var custom = new CustomResolve())
            {

            }
        }
    }

    public interface ITestingThree
    {
        int id { get; }
        string name { get; }
        void DoWork();
    }

    public class TestingThree : ITestingThree
    {
        private static int indexId = 0;

        public int id { get; private set; }

        public string name { get => $"testing.three.name.{id}"; }

        public TestingThree()
        {
            id = indexId++;
        }

        public void DoWork()
        {
            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId} - Doing work of class id {id}");
        }
    }

    public class CustomResolve : IDisposable
    {
        public UnityContainer Container { get; set; }
        private Object resolveLock = new object();

        public CustomResolve()
        {
            Container = new UnityContainer();
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            lock (resolveLock)
            {
                var has = Container.Registrations
                    .Any(r => r.RegisteredType?.AssemblyQualifiedName.Equals(type.AssemblyQualifiedName) ?? false);

                if (!has)
                {
                    Container.RegisterType<ITestingThree, TestingThree>(
                        new PerThreadLifetimeManager());

                }
            }

            return Container.Resolve<T>();
        }
    }
}