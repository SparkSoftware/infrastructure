﻿using System;

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
    /// Retrieves an aggregate from the underlying event store.
    /// </summary>
    public interface IRetrieveAggregates
    {
        /// <summary>
        /// Retrieve the aggregate of the specified <paramref name="aggregateType"/> and aggregate <paramref name="id"/>.
        /// </summary>
        /// <param name="aggregateType">The type of aggregate to retrieve.</param>
        /// <param name="id">The unique aggregate id.</param>
        Aggregate Get(Type aggregateType, Guid id);
    }

    /// <summary>
    /// Extension methods of <see cref="IRetrieveAggregates"/> and <see cref="IStoreAggregates"/>.
    /// </summary>
    public static class RetrieveAggregateExtensions
    {
        /// <summary>
        /// Retrieve the aggregate of the specified <typeparamref name="TAggregate"/> type and aggregate <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="TAggregate">The type of aggregate to retrieve.</typeparam>
        /// <param name="aggregateRepository">The aggregate repository from which the aggregate is to be retrieved.</param>
        /// <param name="id">The unique aggregate id.</param>
        public static TAggregate Get<TAggregate>(this IRetrieveAggregates aggregateRepository, Guid id)
            where TAggregate : Aggregate
        {
            Verify.NotNull(aggregateRepository, nameof(aggregateRepository));

            return (TAggregate)aggregateRepository.Get(typeof(TAggregate), id);
        }
    }
}
