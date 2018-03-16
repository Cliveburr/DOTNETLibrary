using System;

namespace LearnAOP.AOP.Factory
{
    public class SingletonFactory<Tinterface, Tclass> : IFactory<Tinterface> where Tclass : Tinterface
    {
        private Object _lockGet;
        private Tinterface _instance;

        public SingletonFactory()
        {
            _lockGet = new Object();
        }

        public Tinterface GetInstance()
        {
            lock (_lockGet)
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance<Tclass>();
                }
            }
            return _instance;
        }
    }
}