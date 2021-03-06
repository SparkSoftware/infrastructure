﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

namespace Spark.Cqrs.Eventing
{
    /// <summary>
    /// A read-only collection of events.
    /// </summary>
    [Serializable]
    public sealed class EventCollection : ReadOnlyCollection<Event>
    {
        /// <summary>
        /// Represents an empty <see cref="EventCollection"/>. This field is read-only.
        /// </summary>
        public static readonly EventCollection Empty = new EventCollection(Enumerable.Empty<Event>());

        /// <summary>
        /// Initializes a new instance of <see cref="EventCollection"/>.
        /// </summary>
        /// <param name="events">The set of events used to populate this <see cref="EventCollection"/>.</param>
        public EventCollection(IEnumerable<Event> events)
            : base(events.AsList())
        { }
    }
}
