using System.Linq.Expressions;
using DBContext;
using DBModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Results;
using ViewComponents;

namespace Services;

public class NotificationService : INotificationService
{
    private readonly UserManager<User> _userManager;
    private readonly MarketDBContext _context;

    public NotificationService(UserManager<User> userRepository, MarketDBContext context)
    {
        _userManager = userRepository;
        _context = context;
    }

    public async Task<ResultOperation> NotifyReviewers(Notification notification)
    {
        var reviewers = await _userManager.GetUsersInRoleAsync("Reviewer");

        if (!reviewers.Any())
            return ResultOperation.Fail("Nenhum reviewer encontrado");

        var notifications = reviewers.Select(r => new Notification
        {
            UserID = notification.UserID, // autor
            TargetID = r.Id,              // destinatário
            Title = notification.Title,
            Content = notification.Content,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Os analisadores receberam a notificação no app");
    }

    public Task<ResultOperation> NotifyUsers(Notification notification)
    {
        throw new NotImplementedException();
    }
}