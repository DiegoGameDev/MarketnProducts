using System.Linq.Expressions;
using DBModel;
using Results;

namespace Services;

public interface INotificationService
{
    Task<ResultOperation> NotifyReviewers(Alert notification);
    Task<ResultOperation> NotifyUsers(Alert notification);
    Task<ResultOperation> NotifyUser(Alert notification);
    Task<ResultOperation<IEnumerable<Alert>>> NotificationsOfUser(User user);
    Task<ResultOperation> MarkIsRead(int id);
}