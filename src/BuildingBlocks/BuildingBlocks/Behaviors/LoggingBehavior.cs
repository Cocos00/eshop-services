using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "[Start] Handling {Request} with response {Response}. Data: {@RequestData}",
                typeof(TRequest).Name,
                typeof(TResponse).Name,
                request);

            var timer = Stopwatch.StartNew();
            var response = await next();
            timer.Stop();

            if (timer.Elapsed > TimeSpan.FromSeconds(3))
            {
                logger.LogWarning(
                    "[Performance] Request {Request} took {ElapsedMilliseconds} ms.",
                    typeof(TRequest).Name,
                    timer.ElapsedMilliseconds);
            }

            logger.LogInformation(
                "[End] Handled {Request} with response {Response}",
                typeof(TRequest).Name,
                typeof(TResponse).Name);

            return response;
        }
    }
}
