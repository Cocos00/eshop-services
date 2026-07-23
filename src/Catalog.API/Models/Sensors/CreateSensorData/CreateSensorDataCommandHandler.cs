namespace Catalog.API.Models.Sensors.CreateSensorData
{
    public record CreateSensorDataCommand(double Distance) : ICommand<CreateSensorDataResult>;

    public record CreateSensorDataResult(Guid Id);

    internal class CreateSensorDataCommandHandler(IDocumentSession documentSession)
        : ICommandHandler<CreateSensorDataCommand, CreateSensorDataResult>
    {
        public async Task<CreateSensorDataResult> Handle(
            CreateSensorDataCommand request,
            CancellationToken cancellationToken)
        {
            var sensor = new Sensor
            {
                Id = Guid.NewGuid(),
                Distance = request.Distance,
                Timestamp = DateTime.UtcNow
            };

            documentSession.Store(sensor);
            await documentSession.SaveChangesAsync(cancellationToken);

            return new CreateSensorDataResult(sensor.Id);
        }
    }
}
