using LearnAOP.AOP;
using LearnAOP.AOP.Resolver;
using System;

namespace LearnAOP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Aspect Oriented Programming!");

            Tests.SimpleRegister.Run();

            //Tests.ThreadLifetimeTest.Run();

            //Tests.HotLoadResolverTest.Run();


            Console.ReadKey();
        }
    }

    
}
