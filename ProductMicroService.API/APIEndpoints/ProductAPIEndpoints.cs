using eCommerce.BusinessLogicLayer.DTO;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;

namespace eCommerce.ProductMicroService.API.APIEndpoints
{
    public static class ProductAPIEndpoints
    {
        public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products",async (IProductsService productsService) =>
            {
                List<ProductResponse?> products =  await productsService.GetProducts();
                return Results.Ok(products);
            });

            app.MapGet("/api/products/search/product-id/{ProductID:guid}", async (IProductsService productsService,Guid ProductID) =>
            {
                //await Task.Delay(100);
                //throw new NotImplementedException();

                ProductResponse? product = await productsService.GetProductByCondition(temp=>temp.ProductID == ProductID);
                if(product == null)
                    return Results.NotFound();

                return Results.Ok(product);
            });

            app.MapGet("/api/products/search/{SearchString}", async (IProductsService productsService, 
                                                                                string SearchString) =>
            {
                List<ProductResponse> productsByProductName = await productsService
                            .GetProductsByCondition(temp => temp.ProductName != null
                             && temp.ProductName.Contains(SearchString,StringComparison.OrdinalIgnoreCase));

                List<ProductResponse> productsByCategory = await productsService
                           .GetProductsByCondition(temp => temp.Category != null
                            && temp.Category.Contains(SearchString, StringComparison.OrdinalIgnoreCase));

                var products = productsByProductName.Union(productsByCategory);

                return Results.Ok(products);
            });

            app.MapPost("/api/products", async (IProductsService productsService,
                                                IValidator<ProductAddRequest> productAddRequestvalidator,
                                                ProductAddRequest productAddRequest) =>
            {
                ValidationResult validationResult =  await productAddRequestvalidator.ValidateAsync(productAddRequest);

                if(!validationResult.IsValid)
                {
                    Dictionary<string, string[]> errors = validationResult.Errors.GroupBy(temp => temp.PropertyName)
                                                    .ToDictionary(grp => grp.Key, 
                                                           grp => grp.Select(err => err.ErrorMessage).ToArray());
                    return Results.ValidationProblem(errors);
                }

               var addedProductResponse =  await productsService.AddProduct(productAddRequest);

                if (addedProductResponse != null)
                {
                    return Results.Created($"/api/products/search/product-id/{addedProductResponse.ProductID}", addedProductResponse);
                }
                else
                    return Results.Problem("Error in adding product");
            });

            app.MapPut("/api/products", async (IProductsService productsService,
                                               IValidator<ProductUpdateRequest> productUpdateRequestvalidator,
                                               ProductUpdateRequest productUpdateRequest) =>
            {
                ValidationResult validationResult = await productUpdateRequestvalidator.ValidateAsync(productUpdateRequest);

                if (!validationResult.IsValid)
                {
                    Dictionary<string, string[]> errors = validationResult.Errors.GroupBy(temp => temp.PropertyName)
                                                    .ToDictionary(grp => grp.Key,
                                                           grp => grp.Select(err => err.ErrorMessage).ToArray());
                    return Results.ValidationProblem(errors);
                }

                var updatedProductResponse = await productsService.UpdateProduct(productUpdateRequest);

                if (updatedProductResponse != null)
                {
                    return Results.Ok(updatedProductResponse);
                }
                else
                    return Results.Problem("Error in updating product");
            });

            app.MapDelete("/api/products/{ProductID:guid}", async (IProductsService productsService,
                                                                    Guid ProductID) =>
            {
                
                bool isDeleted = await productsService.DeleteProduct(ProductID);

                if (isDeleted)
                {
                    return Results.Ok(true);
                }
                else
                    return Results.Problem("Error in deleting product");
            });



            return app;
        }
    }
}
