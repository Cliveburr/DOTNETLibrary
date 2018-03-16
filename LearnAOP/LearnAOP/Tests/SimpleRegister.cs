using LearnAOP.AOP;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Resolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.Tests
{
    public class SimpleRegister
    {
        public static void Run()
        {
            Console.WriteLine("SimpleRegister test");

            using (var container = new Container())
            {
                container.SetRegisterResolver()
                    .RegisterType<ITestOne, TestOne>()
                    .RegisterType<IOneDep, OneDep>(new SingletonLifetime());



                for (var i = 0; i < 10; i++)
                {
                    var testOne = container.Resolve<ITestOne>();

                    testOne.WriteText("primeiro teste");
                }
            }
        }
    }

    public interface ITestOne
    {
        void WriteText(string text);
    }

    public class TestOne : ITestOne
    {
        private IOneDep _oneDep;

        public TestOne(IOneDep oneDep)
        {
            _oneDep = oneDep;
        }

        public void WriteText(string text)
        {
            Console.WriteLine($"{text} - {_oneDep.Name} - hit {_oneDep.Hit}");
        }
    }

    public interface IOneDep
    {
        string Name { get; }
        uint Hit { get; }
    }

    public class OneDep : IOneDep
    {
        public string Name => "one dependencie";

        private uint _hit;

        public OneDep()
        {
            _hit = 0;
        }

        public uint Hit { get { return _hit++; } }
    }
}