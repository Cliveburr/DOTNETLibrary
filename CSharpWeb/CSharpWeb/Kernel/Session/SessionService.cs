namespace CSharpWeb.Kernel.Session;

public class SessionService
{
    private readonly List<SessionStore> _sessions;
    private readonly IServiceProvider _serviceProvider;
    private int _nextSessionId;

    public SessionService(IServiceProvider serviceProvider)
    {
        _sessions = new List<SessionStore>();
        _serviceProvider = serviceProvider;
        _nextSessionId = 1;
    }

    public SessionStore Create()
    {
        //TODO: put a timer to wait ws connection, if fail release the session

        var session = new SessionStore
        {
            Scope = _serviceProvider.CreateScope(),
            SessionId = _nextSessionId++,
        };
        _sessions.Add(session);

        return session;
    }

    public SessionStore Get(int sessionId)
    {
        var session = _sessions
            .FirstOrDefault(s => s.SessionId == sessionId);
        if (session is null)
        {
            throw new Exception($"Invalid SessionId: {sessionId}");
        }

        return session;
    }
}
