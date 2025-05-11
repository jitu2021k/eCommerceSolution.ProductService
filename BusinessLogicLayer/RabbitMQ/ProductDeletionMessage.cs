namespace eCommerce.ProductsService.usinessLogicLayer.RabbitMQ
{
    public record ProductDeletionMessage(Guid ProductID, string? ProductName)
    {
    }
}
