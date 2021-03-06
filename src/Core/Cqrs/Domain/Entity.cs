﻿using System;
using System.Runtime.Serialization;
using Spark.Cqrs.Commanding;
using Spark.Cqrs.Eventing;

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

namespace Spark.Cqrs.Domain
{
    /// <summary>
    /// A uniquely identifiable <see cref="Object"/> within a given <see cref="Aggregate"/> root.
    /// </summary>
    public abstract class Entity : StateObject
    {
        internal static class Property
        {
            public const String Id = "id";
        }

        /// <summary>
        /// The unique entity identifier.
        /// </summary>
        [DataMember(Name = Property.Id)]
        public Guid Id { get; internal set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Entity"/>.
        /// </summary>
        protected Entity()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="Entity"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique entity identifier.</param>
        protected Entity(Guid id)
        {
            Verify.NotEqual(Guid.Empty, id, nameof(id));

            Id = id;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="e">The <see cref="Event"/> to be raised.</param>
        protected void Raise(Event e)
        {
            Verify.NotNull(e, nameof(e));

            CommandContext.GetCurrent().Raise(e);
        }
    }
}
