using System.Diagnostics.CodeAnalysis;

namespace BitSystem.Shared.Domain;

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

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }
}
