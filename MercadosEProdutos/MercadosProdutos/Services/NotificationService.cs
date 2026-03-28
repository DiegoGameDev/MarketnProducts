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

    public async Task<ResultOperation> MarkIsRead(int id)
    {
        Alert alert = await _context.Alert.FirstOrDefaultAsync(x => x.ID == id);

        try
        {
            alert.IsRead = true;
            _context.Update(alert);
            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Notificação já foi lida, pelo usuário");
        }
        catch (DbUpdateException ex)
        {
            return ResultOperation.Fail("Notificação já foi lida, pelo usuário" + ex.InnerException?.Message);
        }
        
    }

    public async Task<ResultOperation<IEnumerable<Alert>>> NotificationsOfUser(User user)
    {
        var notifications = await _context.Alert.Where(x => x.TargetID == user.Id).ToListAsync();

        if (notifications == null)
            return ResultOperation<IEnumerable<Alert>>.Fail("Usuario não tem notificação ou usuário não existe");

        return ResultOperation<IEnumerable<Alert>>.Ok(notifications, "Notificações encontradas");
    }

    public async Task<ResultOperation> NotifyReviewers(Alert notification)
    {
        var reviewers = await _userManager.GetUsersInRoleAsync("Reviewer");

        if (!reviewers.Any())
            return ResultOperation.Fail("Nenhum reviewer encontrado");

        try
        {
            var notifications = reviewers.Select(r => new Alert
            {
                UserID = notification.UserID, // autor
                TargetID = r.Id,              // destinatário
                Title = notification.Title,
                Message = notification.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Alert.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return ResultOperation.Fail($"Erro ao salvar notificações: {ex.InnerException?.Message}");
        }

        return ResultOperation.Ok("Os analisadores receberam a notificação no app");
    }

    public async Task<ResultOperation> NotifyUser(Alert notification)
    {
        var user = await _userManager.Users.Where(x => x.Id == notification.TargetID).FirstOrDefaultAsync();

        if (user == null)
            return ResultOperation.Fail("Usuário destinatário não encontrado");
        
        notification.CreatedAt = DateTime.UtcNow;
        notification.IsRead = false;

        await _context.Alert.AddAsync(notification);
        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Notificação enviada ao usuário");
    }

    public async Task<ResultOperation> NotifyUsers(Alert notification)
    {
        var users = await _userManager.GetUsersInRoleAsync("Default");

        if (!users.Any())
            return ResultOperation.Fail("Nenhum usuário encontrado");

        try
        {
            var notifications = users.Select(r => new Alert
            {
                UserID = notification.UserID, // autor
                TargetID = r.Id,              // destinatário
                Title = notification.Title,
                Message = notification.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Alert.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return ResultOperation.Fail($"Erro ao salvar notificações: {ex.InnerException?.Message}");
        }

        return ResultOperation.Ok("Os analisadores receberam a notificação no app");
    }
}