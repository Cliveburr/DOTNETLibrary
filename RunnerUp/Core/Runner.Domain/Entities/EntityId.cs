
namespace Runner.Domain.Entities
{
    public struct EntityId
    {
        public string? Content;

        public static EntityId Empty
        {
            get
            {
                return new EntityId { Content = null };
            }
        }

        public override string ToString()
        {
            return $"ObjectId(\'{Content}\')";
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Content == right.Content;
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return left.Content != right.Content;
        }
    }
}
