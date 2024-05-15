using Asp.Versioning;
using HPlusSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/v{version:apiVersion}/products")]
    [Route("api/products")]
    [ApiController]
    public class ProductsV1Controller : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsV1Controller(ShopContext shopContext)
        {
            _shopContext = shopContext;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products;

            if (queryParameters.MinPrice != null) 
            {
                products = products.Where(
                    p => p.Price >=  queryParameters.MinPrice.Value);
            }

            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price <= queryParameters.MaxPrice.Value);
            }

            if(!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(
                    p => p.Sku.Equals(queryParameters.Sku, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                         p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if(typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(
                        queryParameters.SortBy,
                        queryParameters.SortOrder);
                }
            }

            

            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return  Ok( await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product is null) {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction(
                "GetProduct", 
                new { id = product.Id }, 
                product
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }
            _shopContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_shopContext.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if(product is null)
            {
                return NotFound();
            }

            _shopContext.Remove(product);

            await _shopContext.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery] int[] id)
        {
            var products = await _shopContext.Products.Where(p => id.Contains(p.Id)).ToArrayAsync();

            if (products is null || products.Length != id.Length)
            {
                return NotFound();
            }

            _shopContext.RemoveRange(products);

            await _shopContext.SaveChangesAsync();

            return Ok(products);
        }
    
    }


    [ApiVersion("2.0")]
    //[Route("api/v{version:apiVersion}/products")]
    [Route("api/products")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsV2Controller(ShopContext shopContext)
        {
            _shopContext = shopContext;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products.Where(p => p.IsAvailable == true);

            if (queryParameters.MinPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value);
            }

            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price <= queryParameters.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(
                    p => p.Sku.Equals(queryParameters.Sku, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                         p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(
                        queryParameters.SortBy,
                        queryParameters.SortOrder);
                }
            }



            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            _shopContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_shopContext.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            _shopContext.Remove(product);

            await _shopContext.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery] int[] id)
        {
            var products = await _shopContext.Products.Where(p => id.Contains(p.Id)).ToArrayAsync();

            if (products is null || products.Length != id.Length)
            {
                return NotFound();
            }

            _shopContext.RemoveRange(products);

            await _shopContext.SaveChangesAsync();

            return Ok(products);
        }

    }
}
