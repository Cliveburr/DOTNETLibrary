namespace DependencyResolver.Tests.Subjects
{
    public interface ILevelSubject
    {
        string Name { get; set; }
        int Id { get; set; }
        IRootSubject Root { get; set; }
    }
}