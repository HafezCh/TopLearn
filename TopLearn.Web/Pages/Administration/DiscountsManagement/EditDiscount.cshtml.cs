using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Convertors;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Web.Pages.Administration.DiscountsManagement
{
    public class EditDiscountModel : PageModel
    {
        #region Injections

        private readonly IOrderService _orderService;

        public EditDiscountModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #endregion

        [BindProperty] public Discount Discount { get; set; }

        public void OnGet(int id)
        {
            Discount = _orderService.GetDiscountById(id);
        }

        public IActionResult OnPost(string stDate, string edDate)
        {
            if (stDate != null)
                Discount.StartDate = stDate.ToGeorgianDateTime();

            if (edDate != null)
                Discount.EndDate = edDate.ToGeorgianDateTime();

            if (!ModelState.IsValid) return Page();

            _orderService.UpdateDiscount(Discount);

            return RedirectToPage("./Index");
        }
    }
}
