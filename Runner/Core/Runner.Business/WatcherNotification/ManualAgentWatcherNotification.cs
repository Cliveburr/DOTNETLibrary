﻿using Runner.Business.Entities;
using Runner.Business.Entities.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.WatcherNotification
{
    public class ManualAgentWatcherNotification : IAgentWatcherNotification
    {
        public event OnJobCreatedDelegate? OnJobCreated;

        public void InvokeJobCreated(Job job)
        {
            OnJobCreated?.Invoke(job);
        }
    }
}