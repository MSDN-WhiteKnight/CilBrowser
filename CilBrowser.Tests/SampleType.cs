/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2023,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.Text;

namespace CilBrowser.Tests
{
    public class SampleType
    {
        public float X;
        public float Y;

        public float CalcSum() { return this.X + this.Y; }
    }
}
