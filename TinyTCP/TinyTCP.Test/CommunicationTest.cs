using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyTCP.Test
{
    public class CommunicationTest
    {
        public async void JustPing()
        {
            using (var server = new SimpleServer(new ServerParameters
            {
                Channels = new int[] { 11000 }
            }))
            {
                server.Open();

                using (var client = new SimpleClient(new ClientParameters
                {
                    Host = new URI()
                }))
                {

                    await client.PingAsync();

                }
            }
        }
    }
}