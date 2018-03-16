using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnUnityInjection
{
    public class InterceptionTest
    {
        public static void Run()
        {
            using (var container = new UnityContainer())
            {
                container.AddNewExtension<Interception>();
                //container.RegisterType<ITestingTwo, TestingTwo>(
                //    new Interceptor<InterfaceInterceptor>(),
                //    new InterceptionBehavior<LoggingInterceptionBehavior>());

                container.RegisterType<ITestingTwo, TestingTwo>(
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());

                container.Configure<Interception>()
                    .AddPolicy("logging")
                    .AddCallHandler<LoggingCallHandler>(
                        new ContainerControlledLifetimeManager(),
                        new InjectionConstructor());

                var testingTwo = container.Resolve<ITestingTwo>();

                testingTwo.MethodOne();
                Console.WriteLine($"testingTwo.GetMethod = {testingTwo.GetMethod()}");
            }
        }
    }

    public interface ITestingTwo
    {
        void MethodOne();
        string GetMethod();
    }

    public class TestingTwo : ITestingTwo
    {
        public string GetMethod()
        {
            //throw new Exception("error test");
            return "content of get method";
        }

        [LoggingCallHandler(1)]
        public void MethodOne()
        {
            Console.WriteLine("Execution of MethodOne");
        }
    }

    public class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            // Before invoking the method on the original target.
            WriteLog(String.Format(
              "Invoking method {0} at {1}",
              input.MethodBase, DateTime.Now.ToLongTimeString()));

            // Invoke the next behavior in the chain.
            var result = getNext()(input, getNext);

            // After invoking the method on the original target.
            if (result.Exception != null)
            {
                WriteLog(String.Format(
                  "Method {0} threw exception {1} at {2}",
                  input.MethodBase, result.Exception.Message,
                  DateTime.Now.ToLongTimeString()));
            }
            else
            {
                WriteLog(String.Format(
                  "Method {0} returned {1} at {2}",
                  input.MethodBase, result.ReturnValue,
                  DateTime.Now.ToLongTimeString()));
            }

            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return new Type[] { typeof(ITestingTwo) };
            //return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        private void WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class LoggingCallHandler : ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            // Before invoking the method on the original target
            WriteLog(String.Format("Invoking method {0} at {1}",
              input.MethodBase, DateTime.Now.ToLongTimeString()));

            // Invoke the next handler in the chain
            var result = getNext().Invoke(input, getNext);

            // After invoking the method on the original target
            if (result.Exception != null)
            {
                WriteLog(String.Format("Method {0} threw exception {1} at {2}",
                  input.MethodBase, result.Exception.Message,
                  DateTime.Now.ToLongTimeString()));
            }
            else
            {
                WriteLog(String.Format("Method {0} returned {1} at {2}",
                  input.MethodBase, result.ReturnValue,
                  DateTime.Now.ToLongTimeString()));
            }

            return result;
        }

        private void WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LoggingCallHandlerAttribute : HandlerAttribute
    {
        private readonly int order;

        public LoggingCallHandlerAttribute(int order)
        {
            this.order = order;
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LoggingCallHandler() { Order = order };
        }
    }
}
