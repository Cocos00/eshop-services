namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketRequest(ShoppingCart Cart);

    public record StoreBasketResponse(string UserName);

    public class StoreBasketEndPoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket", async (StoreBasketRequest request, ISender sender) =>
            {
                var result = await sender.Send(new StoreBasketCommand(request.Cart));

                return Results.Created($"/basket/{result.UserName}", new StoreBasketResponse(result.UserName));
            })
            .WithName("StoreBasket")
            .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Guardar carrito")
            .WithDescription("Crea o actualiza el carrito de compras de un usuario.");
        }
    }
}
