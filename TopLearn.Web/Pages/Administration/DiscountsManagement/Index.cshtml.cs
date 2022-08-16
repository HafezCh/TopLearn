using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Web.Pages.Administration.DiscountsManagement
{
    public class IndexModel : PageModel
    {
        #region Injections

        private readonly IOrderService _orderService;

        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #endregion

        public List<Discount> Discounts { get; set; }

        public void OnGet()
        {
            Discounts = _orderService.GetDiscounts();
        }
    }
}
