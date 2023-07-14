namespace HealthChecks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    /// <summary>
    /// Signalr Health Check.
    /// </summary>
    public class SignalrHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrHealthCheck"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public SignalrHealthCheck()
        {
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
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(description: "exception", e));
            }
        }
    }
}