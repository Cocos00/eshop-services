namespace Catalog.API.Models.Products.CreateProduct
{
    public record CreateProductRequest(string Name, string Description,
        List<string> Category, string ImageFiles, decimal Price);

    public record CreateProductResponse(Guid Id);
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();

                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{response.Id}", response);
            })
                .WithName("Crear Producto")
                .Produces<CreateProductResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Crear un nuevo producto")
                .WithDescription("Crea a nuevo producto y se retorna el identificador de la entidad");
        }
    }
}
