namespace signalr.HealthChecks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    /// <summary>
    /// Signalr Health Check.
    /// </summary>
    public class SignalrHealthCheck : IHealthCheck
    {
        private readonly ILogger<SignalrHealthCheck> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrHealthCheck"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SignalrHealthCheck(ILogger<SignalrHealthCheck> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Health Check.
        /// </summary>
        /// <param name="context">context.</param>
        /// <param name="cancellationToken">token.</param>
        /// <returns>result.</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("SignalR health check passed.");
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Task.FromResult(HealthCheckResult.Unhealthy(description: "exception", e));
            }
        }
    }
}