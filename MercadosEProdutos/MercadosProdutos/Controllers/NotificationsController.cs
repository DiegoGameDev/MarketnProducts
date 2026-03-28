using DBModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MercadosProdutos.Controllers
{
    [Authorize(Roles = "Default")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _service;
        private readonly IMarketSession session;

        public NotificationsController(INotificationService notificationService,
            IMarketSession marketSession)
        {
            _service = notificationService;
            session = marketSession;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User user = session.GetSession();
            var alert = await _service.NotificationsOfUser(user);
            return View(alert.Data);
        }

        [HttpPost]
        public async Task<IActionResult> IsRead(int id)
        {
            var result = await _service.MarkIsRead(id);
            Console.WriteLine(result.Message);
            return RedirectToAction("Index", "Notifications");
        }
    }
}
