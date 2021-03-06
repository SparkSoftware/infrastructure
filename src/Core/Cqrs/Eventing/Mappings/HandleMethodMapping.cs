﻿using System;
using System.Collections.Generic;

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

namespace Spark.Cqrs.Eventing.Mappings
{
    /// <summary>
    /// Represents an explicit <see cref="EventHandlerAttribute"/> handle method mapping.
    /// </summary>
    public abstract class HandleMethodMapping
    {
        /// <summary>
        /// Apply method mapping builder.
        /// </summary>
        protected sealed class HandleMethodMappingBuilder
        {
            private readonly IDictionary<Type, Action<Object, Event>> handleMethods = new Dictionary<Type, Action<Object, Event>>();
            private readonly IServiceProvider serviceProvider;

            internal IDictionary<Type, Action<Object, Event>> Mappings { get { return handleMethods; } }

            /// <summary>
            /// Initializes a new instance of <see cref="HandleMethodMapping"/> with the specified <paramref name="serviceProvider"/>.
            /// </summary>
            /// <param name="serviceProvider">The underlying service provider (IoC Container).</param>
            internal HandleMethodMappingBuilder(IServiceProvider serviceProvider)
            {
                Verify.NotNull(serviceProvider, nameof(serviceProvider));

                this.serviceProvider = serviceProvider;
            }

            /// <summary>
            /// Register the handle method for the specified <paramref name="eventType"/>.
            /// </summary>
            /// <param name="eventType">The event type associated with the specified <paramref name="handleMethod"/>.</param>
            /// <param name="handleMethod">The handle method to be invoked for events of <paramref name="eventType"/>.</param>
            public void Register(Type eventType, Action<Object, Event> handleMethod)
            {
                Verify.NotNull(eventType, nameof(eventType));
                Verify.NotNull(handleMethod, nameof(handleMethod));
                Verify.TypeDerivesFrom(typeof(Event), eventType, nameof(eventType));

                handleMethods.Add(eventType, handleMethod);
            }

            /// <summary>
            /// Get the specified service instance.
            /// </summary>
            public T GetService<T>()
            {
                return (T)GetService(typeof(T));
            }

            /// <summary>
            /// Get the specified service instance.
            /// </summary>
            /// <param name="type">The service type to retrieve.</param>
            public Object GetService(Type type)
            {
                Verify.NotNull(type, nameof(type));

                return serviceProvider.GetService(type);
            }
        }

        /// <summary>
        /// Gets the explicitly registered handle methods.
        /// </summary>
        internal IDictionary<Type, Action<Object, Event>> GetMappings(IServiceProvider serviceProvider)
        {
            var builder = new HandleMethodMappingBuilder(serviceProvider);

            RegisterMappings(builder);

            return builder.Mappings;
        }

        /// <summary>
        /// Register the event type handle methods for a given event handler type.
        /// </summary>
        /// <param name="builder">The <see cref="HandleMethodMappingBuilder"/> for the underlying event handler type.</param>
        protected abstract void RegisterMappings(HandleMethodMappingBuilder builder);
    }
}
