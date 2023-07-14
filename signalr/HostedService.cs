// <copyright file="HostedService.cs" company="VMware Inc.">
//  Copyright (c) 2023 VMware Inc. All rights reserved.
//  This product is protected by copyright and intellectual property laws in the United States and other countries as well as by international treaties.
//  VMWare products may be covered by one or more patents listed at http://www.vmware.com/go/patents.
// </copyright>

namespace signalr
{
    /// <summary>
    /// Implement long running service with dependency injection and logging.
    /// </summary>
    public class HostedService : IHostedService
    {
        private readonly ILogger<HostedService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IHostedService"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HostedService(ILogger<HostedService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">token.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting HostedService...");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">token.</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stoping HostedService...");

            return Task.CompletedTask;
        }
    }
}
