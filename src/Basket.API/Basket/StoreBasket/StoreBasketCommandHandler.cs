namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

    public record StoreBasketResult(string UserName);

    internal class StoreBasketCommandHandler(IBasketRepository repository)
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(
            StoreBasketCommand command,
            CancellationToken cancellationToken)
        {
            var basket = await repository.StoreBasket(command.Cart, cancellationToken);

            return new StoreBasketResult(basket.UserName);
        }
    }
}
