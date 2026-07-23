using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LogginBehavior<TRequest,
        TResponse>> logger) :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {

        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("[Empezamos] Manejo Peticion= {Request} - Respuesta={Response} - Respuesta data ={RequestData}",
                typeof(TRequest).Name, typeof(TResponse).Name, request);

            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3)
                logger.LogWarning("[Performance] La peticion {Request} toma {TimeTaken} segundos.",
                    typeof(TRequest).Name, timeTaken.Seconds);
            logger.LogInformation("[Final] Manejar {Request} with {Response}", typeof(IRequest).Name,
                typeof(TResponse).Name);
            return response;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}