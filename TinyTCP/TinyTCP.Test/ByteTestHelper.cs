using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyTCP.Test
{
    public static class ByteTestHelper
    {
        public static void Compare(byte[] one, byte[] two)
        {
            if (one.Length != two.Length)
            {
                Assert.Fail("Arrays not same size!");
            }

            for (var i = 0; i < one.Length; i++)
            {
                Assert.AreEqual(one[i], two[i]);
            }
        }
    }
}