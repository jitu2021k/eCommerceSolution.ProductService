using eCommerce.DataAccessLayer.Entities;
using eCommerce.DataAccessLayer.Context;
using eCommerce.DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductsRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Product?> AddProduct(Product product)
        {
            _applicationDbContext.Products.Add(product);
            await _applicationDbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            var product = _applicationDbContext.Products.FirstOrDefault(p=>p.ProductID==productID);
            if (product != null)
            {
                _applicationDbContext.Remove(product);
                int affectedRows = await _applicationDbContext.SaveChangesAsync();
                return affectedRows>0;
            }
            return false;
        }

        public Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return _applicationDbContext.Products.FirstOrDefaultAsync(conditionExpression);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _applicationDbContext.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _applicationDbContext.Products.Where(conditionExpression).ToListAsync();
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            var existingProduct = _applicationDbContext.Products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.Category = product.Category;
                existingProduct.UnitPrice = product.UnitPrice;
                existingProduct.QuantityInStock = product.QuantityInStock;
                
                await _applicationDbContext.SaveChangesAsync();
                return existingProduct;
            }
            return null;
        }
    }
}
