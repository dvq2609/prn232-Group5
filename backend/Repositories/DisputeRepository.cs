using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class DisputeRepository : IDisputeRepository
    {
        private readonly CloneEbayDbContext _context;

        public DisputeRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<Dispute> AddDispute(DisputeCreateDto disputeDto, int currentUserId)
        {
            var order = await _context.OrderTables.FindAsync(disputeDto.OrderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            if (order.BuyerId != currentUserId)
            {
                throw new Exception("You are not the buyer of this order");
            }
            if (await HasPendingDisputeAsync(order.Id))
            {
                throw new Exception("You have already raised a dispute for this order");
            }
            var newDispute = new Dispute
            {
                OrderId = disputeDto.OrderId,
                Description = disputeDto.Description,
                RaisedBy = currentUserId,
                Status = "Pending",
                SubmittedDate = DateTime.Now,
            };
            await _context.Disputes.AddAsync(newDispute);
            await _context.SaveChangesAsync();
            return newDispute;
        }

        public Task DeleteDispute(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DisputeDto>> GetAllDisputes()
        {
            var dispute = await _context.Disputes.Include(d => d.RaisedByNavigation).Include(d => d.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Select(d => new DisputeDto
            {
                DisputeId = d.Id,
                OrderId = d.OrderId,
                Status = d.Status,
                UserDispute = d.RaisedByNavigation.Username,
                ProductTitle = d.Order.OrderItems.Select(oi => oi.Product.Title).FirstOrDefault(),
                Description = d.Description,
                SubmittedDate = d.SubmittedDate,
                SolvedDate = d.SolvedDate,
                Comment = d.Comment,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            })
            .ToListAsync();
            return dispute;
        }

        public async Task<IEnumerable<DisputeDto>> GetDisputesByBuyerId(int buyerId)
        {
            var disputes = await _context.Disputes.Where(d => d.RaisedBy == buyerId).Include(d => d.RaisedByNavigation).Include(d => d.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Select(d => new DisputeDto
            {
                DisputeId = d.Id,
                OrderId = d.OrderId,
                Status = d.Status,
                UserDispute = d.RaisedByNavigation.Username,
                ProductTitle = d.Order.OrderItems.Select(oi => oi.Product.Title).FirstOrDefault(),
                Description = d.Description,
                SubmittedDate = d.SubmittedDate,
                SolvedDate = d.SolvedDate,
                Comment = d.Comment,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            })
            .ToListAsync();
            return disputes;
        }

        public async Task<IEnumerable<DisputeDto>> GetDisputesBySellerId(int sellerId)
        {
            var disputes = await _context.Disputes
            .Include(d => d.RaisedByNavigation)
            .Include(d => d.Order)
            .ThenInclude(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(d => d.Order.OrderItems.Any(oi => oi.Product.SellerId == sellerId))
            .Select(d => new DisputeDto
            {
                DisputeId = d.Id,
                OrderId = d.OrderId,
                Status = d.Status,
                UserDispute = d.RaisedByNavigation.Username,
                ProductTitle = d.Order.OrderItems.Select(oi => oi.Product.Title).FirstOrDefault(),
                Description = d.Description,
                SubmittedDate = d.SubmittedDate,
                SolvedDate = d.SolvedDate,
                Comment = d.Comment,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            }).ToListAsync();
            return disputes;
        }

        public async Task<DisputeDto> GetDisputeById(int id)
        {
            var dispute = await _context.Disputes.Include(d => d.RaisedByNavigation).Include(d => d.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Select(d => new DisputeDto
            {
                DisputeId = d.Id,
                OrderId = d.OrderId,
                Status = d.Status,
                UserDispute = d.RaisedByNavigation.Username,
                ProductTitle = d.Order.OrderItems.Select(oi => oi.Product.Title).FirstOrDefault(),
                Description = d.Description,
                SubmittedDate = d.SubmittedDate,
                SolvedDate = d.SolvedDate,
                Comment = d.Comment,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            })
            .FirstOrDefaultAsync(d => d.DisputeId == id);
            return dispute;
        }

        public Task UpdateDispute(Dispute dispute)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> HasPendingDisputeAsync(int orderId)
        {
            return await _context.Disputes
                .AnyAsync(d => d.OrderId == orderId && d.Status == "Pending");
        }

        public async Task AddDisputeImages(List<DisputeImage> images)
        {
            if (images != null && images.Any())
            {
                await _context.DisputeImages.AddRangeAsync(images);
                await _context.SaveChangesAsync();
            }
        }
    }
}