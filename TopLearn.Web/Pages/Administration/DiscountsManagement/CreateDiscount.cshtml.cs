using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Convertors;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Web.Pages.Administration.DiscountsManagement
{
    public class CreateDiscountModel : PageModel
    {
        #region Injections

        private readonly IOrderService _orderService;

        public CreateDiscountModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #endregion

        [BindProperty] public Discount Discount { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string stDate = "", string edDate = "")
        {
            if (stDate != "")
                Discount.StartDate = stDate.ToGeorgianDateTime();

            if (edDate != "")
                Discount.EndDate = edDate.ToGeorgianDateTime();

            if (!ModelState.IsValid) return Page();

            _orderService.AddDiscount(Discount);

            return RedirectToPage("./Index");
        }
    }
}
