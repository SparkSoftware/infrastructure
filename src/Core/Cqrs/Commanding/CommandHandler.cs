﻿using System;
using Spark.Cqrs.Domain;
using Spark.Logging;

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

namespace Spark.Cqrs.Commanding
{
    /// <summary>
    /// Represents an <see cref="Aggregate"/> command handler method executor.
    /// </summary>
    public sealed class CommandHandler
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly Action<Aggregate, Command> executor;
        private readonly IStoreAggregates aggregateStore;
        private readonly Type aggregateType;
        private readonly Type commandType;

        /// <summary>
        /// The aggregate <see cref="Type"/> associated with this command handler executor.
        /// </summary>
        public Type AggregateType { get { return aggregateType; } }

        /// <summary>
        /// The aggregate <see cref="Type"/> associated with this command handler executor.
        /// </summary>
        public Type CommandType { get { return commandType; } }

        /// <summary>
        /// Initializes a new instance of <see cref="CommandHandler"/>.
        /// </summary>
        /// <param name="aggregateType">The aggregate type.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="aggregateStore">The aggregate store.</param>
        /// <param name="executor">The command handler executor.</param>
        public CommandHandler(Type aggregateType, Type commandType, IStoreAggregates aggregateStore, Action<Aggregate, Command> executor)
        {
            Verify.NotNull(executor, nameof(executor));
            Verify.NotNull(commandType, nameof(commandType));
            Verify.NotNull(aggregateType, nameof(aggregateType));
            Verify.NotNull(aggregateStore, nameof(aggregateStore));
            Verify.TypeDerivesFrom(typeof(Command), commandType, nameof(commandType));
            Verify.TypeDerivesFrom(typeof(Aggregate), aggregateType, nameof(aggregateType));

            this.aggregateStore = aggregateStore;
            this.aggregateType = aggregateType;
            this.commandType = commandType;
            this.executor = executor;
        }

        /// <summary>
        /// Invokes the underlying <see cref="Aggregate"/> command handler method for <see cref="Command"/>.
        /// </summary>
        /// <param name="context">The current command context.</param>
        public void Handle(CommandContext context)
        {
            var command = context.Command;
            var aggregate = aggregateStore.Get(AggregateType, context.AggregateId);

            Log.Trace("Executing {0} command on aggregate {1}", command, aggregate);

            aggregate.VerifyCanHandleCommand(command);
            executor(aggregate, command);

            if (context.HasRaisedEvents)
            {
                aggregateStore.Save(aggregate, context);
            }
            else
            {
                Log.Warn("Executing {0} command on aggregate {1} raised no events", command, aggregate);
            }
        }

        /// <summary>
        /// Returns the <see cref="CommandHandler"/> description for this instance.
        /// </summary>
        public override String ToString()
        {
            return $"{CommandType} Command Handler ({AggregateType})";
        }
    }
}
