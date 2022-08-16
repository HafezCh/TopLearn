using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class MyOrderController : Controller
    {
        #region Injections

        private readonly IOrderService _orderService;

        public MyOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #endregion

        public IActionResult Index()
        {
            var orders = _orderService.GetOrders(User.Identity.Name);

            return View(orders);
        }

        public IActionResult ShowOrder(int id, string type = "", bool isFinally = false)
        {
            var order = _orderService.GetOrderForUserPanel(User.Identity.Name, id);

            if (order == null) return NotFound();

            ViewBag.IsFinally = isFinally;
            ViewBag.DiscountType = type;

            return View(order);
        }

        public IActionResult FinallyOrder(int id)
        {
            if (_orderService.IsFinallyOrder(User.Identity.Name, id))
            {
                return Redirect($"/UserPanel/MyOrder/ShowOrder?id={id}&isFinally=true");
            }

            return BadRequest();
        }

        [HttpPost]
        public IActionResult UseDiscount(int orderId, string code)
        {
            var type = _orderService.UseDiscount(orderId, code);

            return Redirect($"/UserPanel/MyOrder/ShowOrder?id={orderId}&type={type}");
        }
    }
}
