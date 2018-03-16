using LearnAOP.AOP;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearnAOP.Tests
{
    public class ThreadLifetimeTest
    {
        public static void Run()
        {
            using (var container = new Container())
            {
                var registerResolve = new RegisterResolver();
                container.Resolvers.AddAtEnd(registerResolve);
                registerResolve
                    .RegisterType<ITestingThree, TestingThree>(new ThreadLifetime());

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
}