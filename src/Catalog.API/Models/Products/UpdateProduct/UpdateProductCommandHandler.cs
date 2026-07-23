namespace Catalog.API.Models.Products.UpdateProduct
{
    public record UpdateProductCommand(
        string Name,
        string Description,
        List<string> Category,
        string ImageFiles,
        decimal Price) : ICommand<UpdateProductResult>;

    public record UpdateProductResult(bool IsSuccess);

    internal class UpdateProductCommandHandler(IDocumentSession documentSession)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            var products = await documentSession.Query<Product>().ToListAsync(cancellationToken);
            var product = products.FirstOrDefault(product =>
                product.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));

            if (product is null)
            {
                return new UpdateProductResult(false);
            }

            product.Description = request.Description;
            product.Category = request.Category;
            product.ImageFiles = request.ImageFiles;
            product.Price = request.Price;

            documentSession.Store(product);
            await documentSession.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }
    }
}
