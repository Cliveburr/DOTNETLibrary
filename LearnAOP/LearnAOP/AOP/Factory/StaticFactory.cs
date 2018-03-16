namespace LearnAOP.AOP.Factory
{
    public class StaticFactory<T> : IFactory<T>
    {
        private T _instance;

        public StaticFactory(T instance)
        {
            _instance = instance;
        }

        public T GetInstance()
        {
            return _instance;
        }
    }
}