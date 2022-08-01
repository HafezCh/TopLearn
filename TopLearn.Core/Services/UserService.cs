using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Security;
using TopLearn.Core.Senders;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services
{
    public class UserService : IUserService
    {
        private readonly TopLearnDbContext _context;
        private readonly IViewRenderService _viewRenderService;

        public UserService(TopLearnDbContext context, IViewRenderService viewRenderService)
        {
            _context = context;
            _viewRenderService = viewRenderService;
        }

        public bool ActiveAccount(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(x => x.ActiveCode == activeCode);

            if (user == null || user.IsActive) return false;

            user.IsActive = true;
            user.ActiveCode = Generator.Generator.GenerateUniqCode();
            _context.SaveChanges();

            return true;
        }

        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public int BalanceUserWallet(string userName)
        {
            var userId = GetUserIdByUserName(userName);

            var deposit = _context.Wallets
                .Where(x => x.UserId == userId && x.TypeId == 1 && x.IsPay)
                .Select(x => x.Amount).ToList();

            var withdrawal = _context.Wallets
                .Where(x => x.UserId == userId && x.TypeId == 2)
                .Select(x => x.Amount).ToList();

            return deposit.Sum() - withdrawal.Sum();
        }

        public void ChangeUserPassword(string userName, string newPassword)
        {
            var user = GetByUserName(userName);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            UpdateUser(user);
        }

        public int ChargeWallet(string userName, int amount, string description, bool isPay = false)
        {
            var wallet = new Wallet
            {
                Amount = amount,
                Description = description,
                IsPay = isPay,
                CreationDate = DateTime.Now,
                TypeId = 1,
                UserId = GetUserIdByUserName(userName)
            };

            return AddWallet(wallet);
        }

        public bool CompareOldPassword(string userName, string oldPassword)
        {
            var pass = PasswordHelper.EncodePasswordMd5(oldPassword);

            return _context.Users.Any(x => x.UserName == userName && x.Password == pass);
        }

        public void EditProfile(string userName, EditUserProfileViewModel profile)
        {
            if (profile.NewAvatar != null)
            {
                string imagePath = "";
                if (profile.CurrentAvatar != "Default.gif")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", profile.CurrentAvatar);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                profile.CurrentAvatar = Generator.Generator.GenerateUniqCode() + Path.GetExtension(profile.NewAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", profile.CurrentAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    profile.NewAvatar.CopyTo(stream);
                }
            }
            var user = GetByUserName(userName);

            if (user.Email != profile.Email)
            {
                user.IsActive = false;
                var emailBody = _viewRenderService.RenderToStringAsync("_ActiveEmail", user);
                SendEmail.Send(profile.Email, "فعال سازی حساب کاربری", emailBody);
            }

            user.UserName = profile.UserName;
            user.Email = profile.Email;
            user.UserAvatar = profile.CurrentAvatar;

            UpdateUser(user);

        }

        public User GetByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(x => x.ActiveCode == activeCode);
        }

        public User GetByEmail(string email)
        {
            return _context.Users.SingleOrDefault(x => x.Email == email);
        }

        public User GetByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(x => x.UserName == userName);
        }

        public EditUserProfileViewModel GetDataForEditUserProfile(string userName)
        {
            return _context.Users.Select(x => new EditUserProfileViewModel
            {
                UserId = x.UserId,
                UserName = x.UserName,
                CurrentAvatar = x.UserAvatar,
                Email = x.Email
            }).Single(x => x.UserName == userName);
        }

        public SlideBarUserPanelViewModel GetSlideBarUserPanel(string userName)
        {
            return _context.Users.Select(x => new SlideBarUserPanelViewModel
            {
                UserName = x.UserName,
                Avatar = x.UserAvatar,
                RegisterDate = x.RegisterDate
            }).Single(x => x.UserName == userName);
        }

        public int GetUserIdByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(x => x.UserName == userName).UserId;
        }

        public InformationUserViewModel GetUserInformation(string userName)
        {
            var user = GetByUserName(userName);

            var info = new InformationUserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Wallet = BalanceUserWallet(user.UserName),
                RegisterDate = user.RegisterDate
            };

            return info;
        }

        public List<WalletViewModel> GetUserWallet(string userName)
        {
            var userId = GetUserIdByUserName(userName);

            return _context.Wallets
                .Where(x => x.UserId == userId && x.IsPay)
                .Select(x => new WalletViewModel
                {
                    Type = x.TypeId,
                    Amount = x.Amount,
                    Description = x.Description,
                    CreationDate = x.CreationDate
                }).AsNoTracking().OrderByDescending(x => x.CreationDate).ToList();
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public bool IsExistEmailForEditProfile(string email, int userId)
        {
            return _context.Users.Any(x => x.Email == email && x.UserId != userId);
        }

        public bool IsExistsEmail(string email)
        {
            return _context.Users.Any(x => x.Email == email);
        }

        public bool IsExistUserName(string userName)
        {
            return _context.Users.Any(x => x.UserName == userName);
        }

        public bool IsExistUserNameForEditProfile(string userName, int userId)
        {
            return _context.Users.Any(x => x.UserName == userName && x.UserId != userId);
        }

        public User LoginUser(LoginViewModel model)
        {
            var hashedPassword = PasswordHelper.EncodePasswordMd5(model.Password);
            var email = FixedText.FixEmail(model.Email);

            return _context.Users.SingleOrDefault(x => x.Email == email && x.Password == hashedPassword);
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
    }
}