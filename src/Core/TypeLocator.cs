﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Spark
{
    /// <summary>
    /// Assembly type locator.
    /// </summary>
    public interface ILocateTypes
    {
        /// <summary>
        /// Scans a pre-defined set of assemblies for types matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate on which to filter the set of known types.</param>
        Type[] GetTypes(Func<Type, Boolean> predicate);
    }

    /// <summary>
    /// Assembly type locator.
    /// </summary>
    public sealed class TypeLocator : ILocateTypes
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly Assembly[] assemblies;

        /// <summary>
        /// Initializes a default instance of <see cref="TypeLocator"/> with all currently loaded assemblies in this <see cref="AppDomain"/>.
        /// </summary>
        public TypeLocator()
            : this(AppDomain.CurrentDomain.GetAssemblies())
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="TypeLocator"/> with the specified set of zero or more assemblies.
        /// </summary>
        /// <param name="assemblies">The set of assemblies from which types are located.</param>
        public TypeLocator(params Assembly[] assemblies)
        {
            this.assemblies = assemblies == null || assemblies.Length == 0 ? AppDomain.CurrentDomain.GetAssemblies() : assemblies.Concat(CoreAssembly.Reference).ToArray();
        }

        /// <summary>
        /// Scans a pre-defined set of assemblies for types matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate on which to filter the set of known types.</param>
        public Type[] GetTypes(Func<Type, Boolean> predicate)
        {
            return assemblies.SelectMany(GetTypesSafe).Where(predicate).ToArray();
        }

        /// <summary>
        /// Gets the set of loadable types that exist within the specified assembly. 
        /// </summary>
        /// <param name="assembly">The assembly from wich to retrieve type information.</param>
        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Log.Warn(ex.Message);

                return ex.Types.Where(type => type != null);
            }
        }
    }
}
