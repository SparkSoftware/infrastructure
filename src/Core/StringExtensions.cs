﻿using System;
using System.Security.Cryptography;
using System.Text;

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
    /// Extension methods of <see cref="String"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns <paramref name="value"/> or <value>String.Empty</value> if <paramref name="value"/> is null.
        /// </summary>
        /// <param name="value">The value to return an empty string for if null.</param>
        public static String EmptyIfNull(this String value)
        {
            return value ?? String.Empty;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The object to format.</param>
        public static String FormatWith(this String format, Object arg0)
        {
            return String.Format(format, arg0);
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        public static String FormatWith(this String format, Object arg0, Object arg1)
        {
            return String.Format(format, arg0, arg1);
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        public static String FormatWith(this String format, Object arg0, Object arg1, Object arg2)
        {
            return String.Format(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static String FormatWith(this String format, params Object[] args)
        {
            return String.Format(format, args);
        }

        /// <summary>
        /// Returns true if a specified string is null; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNull(this String value)
        {
            return value == null;
        }
        
        /// <summary>
        /// Returns true if a specified string is not null; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNotNull(this String value)
        {
            return value != null;
        }
        
        /// <summary>
        /// Returns true if a specified string is empty; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsEmpty(this String value)
        {
            return value == String.Empty;
        }

        /// <summary>
        /// Returns true if a specified string is not empty; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNotEmpty(this String value)
        {
            return value != String.Empty;
        }

        /// <summary>
        /// Returns true if a specified string is null or empty; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNullOrEmpty(this String value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true if a specified string is not null or empty; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNotNullOrEmpty(this String value)
        {
            return !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true if a specified string is null, empty, or consists only of white-space characters; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNullOrWhiteSpace(this String value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Returns true if a specified string is not null, empty, or consists only of white-space characters; otherwise false.
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static Boolean IsNotNullOrWhiteSpace(this String value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Computes the case-sensitive MD5 hash for the string <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The string for which to compute the MD5 hash.</param>
        public static Guid ToGuid(this String value)
        {
            return value.ToGuid(ignoreCase: false);
        }

        /// <summary>
        /// Computes the MD5 hash for the string <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The string for which to compute the MD5 hash.</param>
        /// <param name="ignoreCase">Specify <value>true</value> to first convert the string value to its lower invariant value; otherwise <value>false</value>.</param>
        public static Guid ToGuid(this String value, Boolean ignoreCase)
        {
            value = (value ?? String.Empty).Trim();

            if (ignoreCase)
                value = value.ToLowerInvariant();

            using (var hash = new MD5CryptoServiceProvider())
            {
                var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                return new Guid(bytes);
            }
        }

        /// <summary>
        /// Returns <paramref name="value"/> if <value>null</value> or less than the maximum desired length; otherwise truncates the string from the left returning the first <paramref name="maxLength"/> characters .
        /// </summary>
        /// <param name="value">The string to truncate if required.</param>
        /// <param name="maxLength">The maximum string length allowed.</param>
        public static String Left(this String value, Int32 maxLength)
        {
            Verify.GreaterThan(0, maxLength, nameof(maxLength));

            return value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Returns <paramref name="value"/> if <value>null</value> or less than the maximum desired length; otherwise truncates the string from the right returning the last <paramref name="maxLength"/> characters .
        /// </summary>
        /// <param name="value">The string to truncate if required.</param>
        /// <param name="maxLength">The maximum string length allowed.</param>
        public static String Right(this String value, Int32 maxLength)
        {
            Verify.GreaterThan(0, maxLength, nameof(maxLength));

            return value == null || value.Length <= maxLength ? value : value.Substring(value.Length - maxLength);
        }
    }
}
