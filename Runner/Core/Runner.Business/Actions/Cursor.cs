﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public class Cursor
    {
        public int ActionId { get; set; }
        public required List<int> ActionsPasseds { get; set; }
    }
}
