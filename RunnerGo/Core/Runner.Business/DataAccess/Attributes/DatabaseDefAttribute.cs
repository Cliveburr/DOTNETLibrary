
namespace Runner.Business.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseDefAttribute : Attribute
    {
        public string? CollectionName { get; set; }

        public DatabaseDefAttribute()
        {
        }

        public DatabaseDefAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
