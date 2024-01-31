
namespace Runner.Business.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IndexDefAttribute : Attribute
    {
        public bool Ascending { get; set; }

        public IndexDefAttribute(bool ascending = true)
        {
            Ascending = ascending;
        }
    }
}
