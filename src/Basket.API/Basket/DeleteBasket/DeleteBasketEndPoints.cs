namespace Basket.API.Basket.DeleteBasket
{
    public class DeleteBasketEndPoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new DeleteBasketCommand(userName));

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { Message = $"Basket '{userName}' was not found." });
            })
            .WithName("DeleteBasket")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Eliminar carrito")
            .WithDescription("Elimina el carrito de compras de un usuario.");
        }
    }
}
