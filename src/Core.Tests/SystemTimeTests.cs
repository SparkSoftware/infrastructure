﻿using System;
using Spark;
using Spark.Resources;
using Xunit;

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

namespace Test.Spark
{
    // ReSharper disable NotResolvedInText
    namespace UsingSystemTime
    {
        public class WhenGettingCurrentTime
        {
            [Fact]
            public void AlwaysReturnUtcTime()
            {
                Assert.Equal(DateTimeKind.Utc, SystemTime.Now.Kind);
            }

            [Fact]
            public void MustOverrideWithUtcTime()
            {
                var expectedEx = new ArgumentOutOfRangeException("timeRetriever", DateTimeKind.Local, Exceptions.ArgumentNotEqualToValue.FormatWith(DateTimeKind.Utc));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => SystemTime.OverrideWith(() => DateTime.Now));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void CanClearOveride()
            {
                SystemTime.OverrideWith(() => DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)));
                SystemTime.ClearOverride();

                Assert.InRange(SystemTime.Now, DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1)), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));
            }
        }
    }
}
