namespace Catalog.API.Models.Products.DeleteProduct
{
    public record DeleteProductCommand(string Name) : ICommand<DeleteProductResult>;

    public record DeleteProductResult(bool IsSuccess);

    internal class DeleteProductCommandHandler(IDocumentSession documentSession)
        : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(
            DeleteProductCommand request,
            CancellationToken cancellationToken)
        {
            var products = await documentSession.Query<Product>().ToListAsync(cancellationToken);
            var product = products.FirstOrDefault(product =>
                product.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));

            if (product is null)
            {
                return new DeleteProductResult(false);
            }

            documentSession.Delete(product);
            await documentSession.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(true);
        }
    }
}
