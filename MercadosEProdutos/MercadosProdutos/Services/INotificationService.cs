using System.Linq.Expressions;
using DBModel;
using Results;

namespace Services;

public interface INotificationService
{
    Task<ResultOperation> NotifyReviewers(Notification notification);
    Task<ResultOperation> NotifyUsers(Notification notification);
}