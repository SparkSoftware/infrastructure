﻿using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Principal;
using Spark.Messaging;

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
    /// Base class for all command messages.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// The message header collection associated with this command instance.
        /// </summary>
        [IgnoreDataMember]
        public HeaderCollection Headers { get { return CommandContext.GetCurrent().Headers; } }

        /// <summary>
        /// Returns the origin server name that published the command or an empty string if not set.
        /// </summary>
        public String GetOrigin()
        {
            return Headers.GetOrigin();
        }

        /// <summary>
        /// Returns the timestamp of when the command was published or the current system time if not set.
        /// </summary>
        public DateTime GetTimestamp()
        {
            return Headers.GetTimestamp();
        }

        /// <summary>
        /// Returns the command publisher's remote address or <see cref="IPAddress.None"/> if not set.
        /// </summary>
        public IPAddress GetRemoteAddress()
        {
            return Headers.GetRemoteAddress();
        }

        /// <summary>
        /// Returns the command publisher's client address or <see cref="IPAddress.None"/> if not set.
        /// </summary>
        /// <remarks>Value will be the same as <see cref="GetRemoteAddress"/> unless the request went through an intermediary such as a load-balancer or proxy.</remarks>
        public IPAddress GetUserAddress()
        {
            return Headers.GetUserAddress();
        }

        /// <summary>
        /// Returns the <see cref="IIdentity.Name"/> of the user principal that published this command, or <see cref="String.Empty"/> if not set.
        /// </summary>
        public String GetUserName()
        {
            return Headers.GetUserName();
        }

        /// <summary>
        /// Creates a new <see cref="CommandContext"/> based on the current command using the specified <paramref name="aggregateId"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier associated with the new command context.</param>
        public CommandContext CreateContext(Guid aggregateId)
        {
            var headers = CommandContext.Current == null ? HeaderCollection.Empty : CommandContext.Current.Headers;
            return new CommandContext(GuidStrategy.NewGuid(), headers, new CommandEnvelope(aggregateId, this));
        }
    }
}
