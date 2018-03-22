using LearnAOP.AOP;
using LearnAOP.AOP.Builder;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Resolver;
using System;
using System.Collections.Generic;
using System.Reflection;
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

                container.Interception.Add(new TestInterception());

                for (var i = 0; i < 10; i++)
                {
                    var testOne = container.Resolve<ITestOne>();

                    testOne.WriteText("primeiro teste");
                }
            }
        }
    }

    public class TestInterception : InterceptionQuery
    {
        public override bool HasPreExecute => true;

        public override bool IsApply(MethodInfo method)
        {
            return true;
        }

        public override void PreExecute(InterceptionRunContext context)
        {
            Console.WriteLine("hit");
        }
    }

    //[SingletonLifetime]
    public interface ITestOne
    {
        void WriteText(string text);
    }

    //[Lifetime(typeof(ThreadLifetime))]
    public class TestOne : ITestOne
    {
        //private IOneDep _oneDep;

        //public TestOne(IOneDep oneDep)
        //{
        //    _oneDep = oneDep;
        //}

        public void WriteText(string text)
        {
            //Console.WriteLine($"{text} - {_oneDep.Name} - hit {_oneDep.Hit}");
            Console.WriteLine("TestOne simple text method");
        }
    }

    public interface IOneDep
    {
        string Name { get; }
        uint Hit { get; }
        void SomeMethod();
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

        public void SomeMethod()
        {
        }
    }

    public class SingletonLifetimeAttribute : LifetimeAttribute
    {
        public override Type LifetimeType => typeof(SingletonLifetime);
    }
}