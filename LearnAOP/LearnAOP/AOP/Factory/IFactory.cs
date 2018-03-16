namespace LearnAOP.AOP.Factory
{
    public interface IFactory<T>
    {
        T GetInstance();
    }
}