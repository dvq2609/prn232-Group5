using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CloneEbayDbContext _context;
        public ProductRepository(CloneEbayDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products.Include(p => p.Seller).Include(p => p.Category).ToListAsync();
            return products;
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _context.Products.Include(p => p.Seller).Include(p => p.Category).Include(p => p.Reviews).ThenInclude(r => r.Reviewer).FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }
    }
}