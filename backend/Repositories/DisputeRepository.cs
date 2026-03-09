using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace backend.Repositories
{
    public class DisputeRepository : IDisputeRepository
    {
        private readonly CloneEbayDbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public DisputeRepository(CloneEbayDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
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

            // Cập nhật trạng thái của đơn hàng thành Dispute
            order.Status = "Disputed";
            _context.OrderTables.Update(order);

            await _context.Disputes.AddAsync(newDispute);
            await _context.SaveChangesAsync();
            return newDispute;
        }

        public Task DeleteDispute(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<DisputeDto>> GetAllDisputes(SieveModel sieveModel)
        {
            var query = _context.Disputes.Include(d => d.RaisedByNavigation).Include(d => d.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
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
                Resolution = d.Resolution,
                RaisedBy = d.RaisedByNavigation.Username,
                SellerResponse = d.SellerResponse,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            });
            // lọc dữ liệu mà chưa phân trang ( ví dụ có 20 item thì vẫn lấy hết chưa có skip (x) và take (x) item)
            var result = _sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            var totalCount = await result.CountAsync();
            // phân trang những dữ liệu ở bước trên ( ví dụ có 20 item thì lấy 10 item đầu tiên)
            var items = await _sieveProcessor.Apply(sieveModel, result, applyFiltering: false, applySorting: false).ToListAsync();

            return new PagedResult<DisputeDto>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? 10
            };
        }

        public async Task<PagedResult<DisputeDto>> GetDisputesByBuyerId(int buyerId, SieveModel sieveModel)
        {
            var query = _context.Disputes.Where(d => d.RaisedBy == buyerId).Include(d => d.RaisedByNavigation).Include(d => d.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product)
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
                Resolution = d.Resolution,
                SellerResponse = d.SellerResponse,
                RaisedBy = d.RaisedByNavigation.Username,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            });

            var result = _sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            var totalCount = await result.CountAsync();
            var items = await _sieveProcessor.Apply(sieveModel, result, applyFiltering: false, applySorting: false).ToListAsync();

            return new PagedResult<DisputeDto>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? 10
            };
        }

        public async Task<PagedResult<DisputeDto>> GetDisputesBySellerId(int sellerId, SieveModel sieveModel)
        {
            var query = _context.Disputes
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
                Resolution = d.Resolution,
                RaisedBy = d.RaisedByNavigation.Username,
                SellerResponse = d.SellerResponse,
                SellerName = d.Order.OrderItems.Select(oi => oi.Product != null && oi.Product.Seller != null ? oi.Product.Seller.Username : null).FirstOrDefault(),
                Images = d.DisputeImages.Select(i => new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    FilePath = i.FilePath,
                    FileExtension = i.FileExtension,
                    FileSizeInBytes = i.FileSizeInBytes ?? 0
                }).ToList()
            });

            var result = _sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            var totalCount = await result.CountAsync();
            var items = await _sieveProcessor.Apply(sieveModel, result, applyFiltering: false, applySorting: false).ToListAsync();

            return new PagedResult<DisputeDto>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = sieveModel.Page ?? 1,
                PageSize = sieveModel.PageSize ?? 10
            };
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
                Resolution = d.Resolution,
                SellerResponse = d.SellerResponse,
                RaisedBy = d.RaisedByNavigation.Username,
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

        public async Task UpdateDispute(Dispute dispute)
        {
            _context.Disputes.Update(dispute);
            await _context.SaveChangesAsync();
        }

        public async Task<Dispute?> GetDisputeEntityById(int id)
        {
            return await _context.Disputes
                .Include(d => d.Order)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(d => d.Id == id);
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