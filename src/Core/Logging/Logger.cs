﻿using System;
using System.Diagnostics;

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

namespace Spark.Logging
{
    /// <summary>
    /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
    /// </summary>
    /// <param name="format">A composite format <see cref="String"/>.</param>
    /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
    public delegate String FormatMessageHandler(String format, params Object[] args);

    /// <summary>
    /// A <see cref="System.Diagnostics.Trace"/> based implementation of <see cref="ILog"/>.
    /// </summary>
    internal sealed class Logger : ILog
    {
        private const Int32 NoIdentifier = 0;
        private readonly TraceSource traceSource;
        private readonly Boolean fatalEnabled;
        private readonly Boolean errorEnabled;
        private readonly Boolean infoEnabled;
        private readonly Boolean warnEnabled;
        private readonly Boolean debugEnabled;
        private readonly Boolean traceEnabled;
        private readonly Boolean activityEnabled;

        /// <summary>
        /// Returns the name of this <see cref="Logger"/> instance.
        /// </summary>
        public String Name { get { return traceSource.Name; } }

        /// <summary>
        /// Returns <value>true</value> if logging is enabled for <value>FATAL</value> level messages; otherwise <value>false</value>.
        /// </summary>
        public Boolean IsFatalEnabled { get { return fatalEnabled; } }

        /// <summary>
        /// Returns <value>true</value> if logging is enabled for <value>ERROR</value> level messages; otherwise <value>false</value>.
        /// </summary>
        public Boolean IsErrorEnabled { get { return errorEnabled; } }

        /// <summary>
        /// Returns <value>true</value> if logging is enabled for <value>INFO</value> level messages; otherwise <value>false</value>.
        /// </summary>
        public Boolean IsInfoEnabled { get { return infoEnabled; } }

        /// <summary>
        /// Returns <value>true</value> if logging is enabled for <value>WARN</value> level messages; otherwise <value>false</value>.
        /// </summary>
        public Boolean IsWarnEnabled { get { return warnEnabled; } }

        /// <summary>
        /// Returns <value>true</value> if logging is enabled for <value>DEBUG</value> level messages; otherwise <value>false</value>.
        /// </summary>
        public Boolean IsDebugEnabled { get { return debugEnabled; } }

        /// <summary>
        /// Returns <value>true</value> if logging is enabled for <value>TRACE</value> level messages; otherwise <value>false</value>.
        /// </summary>
        public Boolean IsTraceEnabled { get { return traceEnabled; } }

        /// <summary>
        /// Gets the underlying trace source.
        /// </summary>
        internal TraceSource TraceSource { get { return traceSource; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with the specified <paramref name="name"/> and logging <paramref name="level"/>.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The log level of the logger.</param>
        public Logger(String name, SourceLevels level)
            : this(name, level, System.Diagnostics.Trace.Listeners)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with the specified <paramref name="name"/> and logging <paramref name="level"/>.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The log level of the logger.</param>
        /// <param name="listeners">The underlying set of trace listeners.</param>
        public Logger(String name, SourceLevels level, TraceListenerCollection listeners)
        {
            Verify.NotNullOrWhiteSpace(name, nameof(name));

            // Create the trace source.
            traceSource = new TraceSource(name, level);

            // Configure trace source listeners.
            traceSource.Listeners.Clear();
            traceSource.Listeners.AddRange(listeners);

            // Determine supported log operations.
            fatalEnabled = (level & SourceLevels.Critical) == SourceLevels.Critical;
            errorEnabled = (level & SourceLevels.Error) == SourceLevels.Error;
            warnEnabled = (level & SourceLevels.Warning) == SourceLevels.Warning;
            infoEnabled = (level & SourceLevels.Information) == SourceLevels.Information;
            debugEnabled = (level & SourceLevels.Verbose) == SourceLevels.Verbose;
            activityEnabled = (level & SourceLevels.ActivityTracing) == SourceLevels.ActivityTracing;
            traceEnabled = (level & SourceLevels.All) == SourceLevels.All;
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public void Fatal(Exception ex)
        {
            if (fatalEnabled)
                traceSource.TraceData(TraceEventType.Critical, NoIdentifier, ex);
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Fatal(String message)
        {
            if (fatalEnabled)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, message);
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg">The <see cref="Object"/> to format.</param>
        public void Fatal(String format, Object arg)
        {
            if (fatalEnabled)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, String.Format(format, arg));
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        public void Fatal(String format, Object arg0, Object arg1)
        {
            if (fatalEnabled)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, String.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        /// <param name="arg2">The third <see cref="Object"/> to format.</param>
        public void Fatal(String format, Object arg0, Object arg1, Object arg2)
        {
            if (fatalEnabled)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, String.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
        public void Fatal(String format, params Object[] args)
        {
            if (fatalEnabled)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, String.Format(format, args));
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Fatal(Func<String> messageBuilder)
        {
            if (fatalEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, messageBuilder());
        }

        /// <summary>
        /// Writes a <value>FATAL</value> diagnostic message if <value>IsFatalEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Fatal(Func<FormatMessageHandler, String> messageBuilder)
        {
            if (fatalEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Critical, NoIdentifier, messageBuilder(String.Format));
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public void Error(Exception ex)
        {
            if (errorEnabled)
                traceSource.TraceData(TraceEventType.Error, NoIdentifier, ex);
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(String message)
        {
            if (errorEnabled)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, message);
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg">The <see cref="Object"/> to format.</param>
        public void Error(String format, Object arg)
        {
            if (errorEnabled)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, String.Format(format, arg));
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        public void Error(String format, Object arg0, Object arg1)
        {
            if (errorEnabled)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, String.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        /// <param name="arg2">The third <see cref="Object"/> to format.</param>
        public void Error(String format, Object arg0, Object arg1, Object arg2)
        {
            if (errorEnabled)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, String.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
        public void Error(String format, params Object[] args)
        {
            if (errorEnabled)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, String.Format(format, args));
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Error(Func<String> messageBuilder)
        {
            if (errorEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, messageBuilder());
        }

        /// <summary>
        /// Writes an <value>ERROR</value> diagnostic message if <value>IsErrorEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Error(Func<FormatMessageHandler, String> messageBuilder)
        {
            if (errorEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Error, NoIdentifier, messageBuilder(String.Format));
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public void Warn(Exception ex)
        {
            if (warnEnabled)
                traceSource.TraceData(TraceEventType.Warning, NoIdentifier, ex);
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warn(String message)
        {
            if (warnEnabled)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, message);
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg">The <see cref="Object"/> to format.</param>
        public void Warn(String format, Object arg)
        {
            if (warnEnabled)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, String.Format(format, arg));
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        public void Warn(String format, Object arg0, Object arg1)
        {
            if (warnEnabled)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, String.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        /// <param name="arg2">The third <see cref="Object"/> to format.</param>
        public void Warn(String format, Object arg0, Object arg1, Object arg2)
        {
            if (warnEnabled)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, String.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
        public void Warn(String format, params Object[] args)
        {
            if (warnEnabled)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, String.Format(format, args));
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Warn(Func<String> messageBuilder)
        {
            if (warnEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, messageBuilder());
        }

        /// <summary>
        /// Writes a <value>WARN</value> diagnostic message if <value>IsWarnEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Warn(Func<FormatMessageHandler, String> messageBuilder)
        {
            if (warnEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Warning, NoIdentifier, messageBuilder(String.Format));
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public void Info(Exception ex)
        {
            if (infoEnabled)
                traceSource.TraceData(TraceEventType.Information, NoIdentifier, ex);
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(String message)
        {
            if (infoEnabled)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, message);
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg">The <see cref="Object"/> to format.</param>
        public void Info(String format, Object arg)
        {
            if (infoEnabled)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, String.Format(format, arg));
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        public void Info(String format, Object arg0, Object arg1)
        {
            if (infoEnabled)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, String.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        /// <param name="arg2">The third <see cref="Object"/> to format.</param>
        public void Info(String format, Object arg0, Object arg1, Object arg2)
        {
            if (infoEnabled)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, String.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
        public void Info(String format, params Object[] args)
        {
            if (infoEnabled)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, String.Format(format, args));
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Info(Func<String> messageBuilder)
        {
            if (infoEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, messageBuilder());
        }

        /// <summary>
        /// Writes an <value>INFO</value> diagnostic message if <value>IsInfoEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Info(Func<FormatMessageHandler, String> messageBuilder)
        {
            if (infoEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Information, NoIdentifier, messageBuilder(String.Format));
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public void Debug(Exception ex)
        {
            if (debugEnabled)
                traceSource.TraceData(TraceEventType.Verbose, NoIdentifier, ex);
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Debug(String message)
        {
            if (debugEnabled)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, message);
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg">The <see cref="Object"/> to format.</param>
        public void Debug(String format, Object arg)
        {
            if (debugEnabled)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, String.Format(format, arg));
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        public void Debug(String format, Object arg0, Object arg1)
        {
            if (debugEnabled)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, String.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        /// <param name="arg2">The third <see cref="Object"/> to format.</param>
        public void Debug(String format, Object arg0, Object arg1, Object arg2)
        {
            if (debugEnabled)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, String.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
        public void Debug(String format, params Object[] args)
        {
            if (debugEnabled)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, String.Format(format, args));
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Debug(Func<String> messageBuilder)
        {
            if (debugEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, messageBuilder());
        }

        /// <summary>
        /// Writes a <value>DEBUG</value> diagnostic message if <value>IsDebugEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Debug(Func<FormatMessageHandler, String> messageBuilder)
        {
            if (debugEnabled && messageBuilder != null)
                traceSource.TraceEvent(TraceEventType.Verbose, NoIdentifier, messageBuilder(String.Format));
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public void Trace(Exception ex)
        {
            if (traceEnabled)
                System.Diagnostics.Trace.WriteLine(ex, traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Trace(String message)
        {
            if (traceEnabled)
                System.Diagnostics.Trace.WriteLine(message, traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg">The <see cref="Object"/> to format.</param>
        public void Trace(String format, Object arg)
        {
            if (traceEnabled)
                System.Diagnostics.Trace.WriteLine(String.Format(format, arg), traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        public void Trace(String format, Object arg0, Object arg1)
        {
            if (traceEnabled)
                System.Diagnostics.Trace.WriteLine(String.Format(format, arg0, arg1), traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="arg0">The first <see cref="Object"/> to format.</param>
        /// <param name="arg1">The second <see cref="Object"/> to format.</param>
        /// <param name="arg2">The third <see cref="Object"/> to format.</param>
        public void Trace(String format, Object arg0, Object arg1, Object arg2)
        {
            if (traceEnabled)
                System.Diagnostics.Trace.WriteLine(String.Format(format, arg0, arg1, arg2), traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="format">A composite format <see cref="String"/>.</param>
        /// <param name="args">An <see cref="Object"/> array that contains zero or more objects to format.</param>
        public void Trace(String format, params Object[] args)
        {
            if (traceEnabled)
                System.Diagnostics.Trace.WriteLine(String.Format(format, args), traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Trace(Func<String> messageBuilder)
        {
            if (traceEnabled && messageBuilder != null)
                System.Diagnostics.Trace.WriteLine(messageBuilder(), traceSource.Name);
        }

        /// <summary>
        /// Writes a <value>TRACE</value> diagnostic message if <value>IsTraceEnabled</value> is <value>true</value>; otherwise ignored.
        /// </summary>
        /// <param name="messageBuilder">A <see cref="Func{String}"/> message builder.</param>
        public void Trace(Func<FormatMessageHandler, String> messageBuilder)
        {
            if (traceEnabled && messageBuilder != null)
                System.Diagnostics.Trace.WriteLine(messageBuilder(String.Format), traceSource.Name);
        }

        /// <summary>
        /// Start a new named logical operation.
        /// </summary>
        /// <param name="name">The logical operation name.</param>
        public IDisposable PushContext(String name)
        {
            return new LogicalOperationScope(traceSource, name, activityEnabled);
        }

        /// <summary>
        /// Start a new named logical operation.
        /// </summary>
        /// <param name="activityId">The activity identifier.</param>
        public IDisposable Transfer(Guid activityId)
        {
            return new ActivityScope(traceSource, activityId, activityEnabled);
        }
    }
}
