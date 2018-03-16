using System;

namespace LearnAOP.AOP.Factory
{
    public class TransientFactory<Tinterface, Tclass> : IFactory<Tinterface> where Tclass : Tinterface
    {
        public Tinterface GetInstance()
        {
            return Activator.CreateInstance<Tclass>();
        }
    }
}