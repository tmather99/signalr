// <copyright file="TraceIdEnricher.cs" company="VMware Inc.">
//  Copyright (c) 2023 VMware Inc. All rights reserved.
//  This product is protected by copyright and intellectual property laws in the United States and other countries as well as by international treaties.
//  VMWare products may be covered by one or more patents listed at http://www.vmware.com/go/patents.
// </copyright>

namespace signalr.SerilogExtensions
{

    using System.Diagnostics;
    using Serilog.Core;
    using Serilog.Events;

    /// <summary>
    /// The trace id enricher.
    /// </summary>
    public class TraceIdEnricher : ILogEventEnricher
    {
        /// <summary>
        /// The trace id property name.
        /// </summary>
        public const string traceIdProperty = "traceId";

        /// <summary>
        /// The span id property name.
        /// </summary>
        public const string spanIdProperty = "spanId";

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceIdEnricher"/> class.
        /// </summary>
        public TraceIdEnricher()
        {
        }

        /// <summary>
        /// Enriches the log event message.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="propertyFactory">The property factory.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current != null)
            {
                AddProperty(logEvent, propertyFactory, traceIdProperty, Activity.Current?.TraceId.ToHexString());
                AddProperty(logEvent, propertyFactory, spanIdProperty, Activity.Current?.SpanId.ToHexString());
            }
        }

        /// <summary>
        /// Adds the property to the event message.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="propertyFactory">The property factory.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value.</param>
        static void AddProperty(LogEvent logEvent, ILogEventPropertyFactory propertyFactory, string propertyName, string? value)
        {
            if (value != null)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(propertyName, value));
            }
        }
    }
}