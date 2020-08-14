using System;
using System.Collections.Generic;
using System.Text;

namespace TyneTCP.Message
{
    public enum MessageType : byte
    {
        NotInternal = 0,
        Ping = 1,
        Pong = 2
    }
}