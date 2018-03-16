using ExampleAOPInterface;
using LearnAOP.AOP;
using LearnAOP.AOP.Resolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.Tests
{
    public class HotLoadResolverTest
    {
        public static void Run()
        {
            Console.WriteLine("HotLoadResolver test");

            using (var container = new Container())
            {
                var hotResolve = new HotLoadResolver(type => @"bin\Debug\netcoreapp2.0\ExampleAOPBusiness.dll");
                container.Resolvers.AddAtEnd(hotResolve);


                var oneLoaded = container.Resolve<IOneLoadedInterface>();

                Console.WriteLine(oneLoaded.GetTextFrom());
            }
        }
    }
}