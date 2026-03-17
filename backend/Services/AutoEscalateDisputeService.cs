using backend.Models;
using Microsoft.EntityFrameworkCore;
using backend.Services.Notification;
namespace backend.Services
{
    public class AutoEscalateDisputeService : BackgroundService
    {
        private readonly ILogger<AutoEscalateDisputeService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;


        // Trong môi trường DEV: Chạy mỗi 1 phút để test
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        // Trong môi trường DEV: Hết hạn sau 3 phút (Production sẽ sửa thành TimeSpan.FromDays(3))
        private readonly TimeSpan _expireTime = TimeSpan.FromMinutes(3);

        public AutoEscalateDisputeService(ILogger<AutoEscalateDisputeService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AutoEscalateDisputeService has started.");

            //khi ứng dụng còn chạy là service này sẽ chạy
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //thực hiện nhiệm vụ =
                    await CheckAndEscalateDisputesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred validating disputes in AutoEscalateDisputeService.");
                }
                //tạm dừng 1 phút
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("AutoEscalateDisputeService is stopping.");
        }

        private async Task CheckAndEscalateDisputesAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                //lấy context
                var context = scope.ServiceProvider.GetRequiredService<CloneEbayDbContext>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var deadline = DateTime.Now.Subtract(_expireTime);

                var expiredDisputes = await context.Disputes
                    .Where(d => (d.Status == "Pending")
                                && d.SubmittedDate.HasValue
                                && d.SubmittedDate.Value <= deadline)
                    .ToListAsync();

                if (expiredDisputes.Any())
                {
                    _logger.LogInformation($"AutoEscalate: Found {expiredDisputes.Count} expired disputes. Escalating now...");

                    foreach (var dispute in expiredDisputes)
                    {
                        dispute.Status = "Escalated";

                        string autoComment = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] Hệ thống tự động đẩy lên Admin phân xử do quá hạn phản hồi.";
                        dispute.Comment = string.IsNullOrEmpty(dispute.Comment) ? autoComment : dispute.Comment + "\n" + autoComment;

                        _logger.LogInformation($"Auto-escalated Dispute ID: {dispute.Id} from User: {dispute.RaisedBy}");

                        await notificationService.SendNotificationAsync(
                            1, // admin id
                            $"Khiếu nại #{dispute.Id} đã được tự động đẩy lên Admin phân xử",
                            $"/Dispute/AllDisputes/{dispute.Id}"
                        );

                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation($"AutoEscalate: Successfully escalated {expiredDisputes.Count} disputes.");
                }
            }
        }

    }
}
