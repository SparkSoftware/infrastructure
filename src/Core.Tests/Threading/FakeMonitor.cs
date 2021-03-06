﻿using Spark.Threading;
using System;
using System.Threading;

/* Copyright (c) 2015 Spark Software Ltd.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace Test.Spark.Threading
{
    public sealed class FakeMonitor : ISynchronizeAccess
    {
        public Action BeforeWait { get; set; }
        public Action AfterPulse { get; set; }

        public void Pulse(Object obj)
        {
            Monitor.Pulse(obj);

            AfterPulse?.Invoke();
        }

        public void PulseAll(Object obj)
        {
            Monitor.PulseAll(obj);

            AfterPulse?.Invoke();
        }

        public void Wait(Object obj)
        {
            BeforeWait?.Invoke();

            Monitor.Wait(obj);
        }

        public Boolean Wait(Object obj, TimeSpan timeout)
        {
            BeforeWait?.Invoke();

            return Monitor.Wait(obj, timeout);
        }

        public Boolean Wait(Object obj, TimeSpan timeout, Boolean exitContext)
        {
            BeforeWait?.Invoke();

            return Monitor.Wait(obj, timeout, exitContext);
        }
    }
}
