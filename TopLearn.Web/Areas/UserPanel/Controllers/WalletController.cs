using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IUserService _userService;

        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("UserPanel/Wallet")]
        public IActionResult Index()
        {
            ViewBag.WalletList = _userService.GetUserWallet(User.Identity.Name);
            return View();
        }

        [HttpPost("UserPanel/Wallet")]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ChargeWalletViewModel command)
        {
            var userName = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                ViewBag.WalletList = _userService.GetUserWallet(userName);
                return View(command);
            }

            var walletId = _userService.ChargeWallet(userName, command.Amount, "شارژ حساب");

            // Online Payment

            var payment = new ZarinpalSandbox.Payment(command.Amount);

            var res = payment.PaymentRequest("شارژ کیف پول", "https://localhost:5001/OnlinePayment/" + walletId);

            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }

            ViewBag.WalletList = _userService.GetUserWallet(userName);
            return View();
        }
    }
}
