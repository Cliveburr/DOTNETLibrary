namespace CSharpWeb.Kernel.Session;

public class SessionStore
{
    public required IServiceScope Scope { get; set; }
    public int SessionId { get; set; }
}
