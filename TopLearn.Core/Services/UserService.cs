using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
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

        public int AddUserFromAdmin(CreateUserFromAdminViewModel user)
        {
            var newUser = new User
            {
                UserName = user.UserName,
                RegisterDate = DateTime.Now,
                IsActive = true,
                Email = FixedText.FixEmail(user.Email),
                ActiveCode = Generator.Generator.GenerateUniqCode(),
                Password = PasswordHelper.EncodePasswordMd5(user.Password)
            };

            #region UploadAvatar


            if (user.UserAvatar != null)
            {
                newUser.UserAvatar = Generator.Generator.GenerateUniqCode() + Path.GetExtension(user.UserAvatar.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", newUser.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    user.UserAvatar.CopyTo(stream);
                }
            }
            else
            {
                newUser.UserAvatar = "Default.gif";
            }

            #endregion

            return AddUser(newUser);
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
            var user = GetByUserName(userName);

            if (profile.NewAvatar != null)
            {
                var newAvatarPath = SaveUserAvatar(profile.CurrentAvatar, profile.NewAvatar);
                user.UserAvatar = newAvatarPath;
            }

            if (user.Email != profile.Email)
            {
                user.IsActive = false;
                var emailBody = _viewRenderService.RenderToStringAsync("_ActiveEmail", user);
                SendEmail.Send(profile.Email, "فعال سازی حساب کاربری", emailBody);
            }

            user.UserName = profile.UserName;
            user.Email = profile.Email;

            UpdateUser(user);

        }

        public void EditUserFromAdmin(EditUserFromAdminViewModel editModel)
        {
            var user = GetUserByUserId(editModel.UserId);
            user.UserName = editModel.UserName;

            if (!string.IsNullOrWhiteSpace(editModel.Password))
                user.Password = PasswordHelper.EncodePasswordMd5(editModel.Password);

            #region Edit Avatar

            if (editModel.UserAvatar != null)
            {
                var newAvatarPath = SaveUserAvatar(editModel.CurrentAvatar, editModel.UserAvatar);
                user.UserAvatar = newAvatarPath;
            }

            #endregion

            if (user.Email != editModel.Email)
            {
                user.IsActive = false;
                var emailBody = _viewRenderService.RenderToStringAsync("_ActiveEmail", user);
                SendEmail.Send(editModel.Email, "فعال سازی حساب کاربری", emailBody);
            }

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

        public UserForAdminViewModel GetRemovedUsers(int pageId = 1, string emailFilter = "", string userNameFilter = "")
        {
            IQueryable<User> query = _context.Users.IgnoreQueryFilters().Where(x => x.IsRemoved);

            if (!string.IsNullOrWhiteSpace(emailFilter))
                query = query.Where(x => x.Email.Contains(emailFilter));

            if (!string.IsNullOrWhiteSpace(userNameFilter))
                query = query.Where(x => x.UserName.Contains(userNameFilter));

            // Show Item In Page

            int take = 5;
            int skip = (pageId - 1) * take;

            var model = new UserForAdminViewModel
            {
                CurrentPage = pageId,
                PageCount = query.Count() / take,
                Users = query.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList()
            };

            return model;
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

        public User GetUserByUserId(int userId)
        {
            return _context.Users.SingleOrDefault(x => x.UserId == userId);
        }

        public EditUserFromAdminViewModel GetUserDetailsForEditFromAdmin(int userId)
        {
            return _context.Users.Select(x => new EditUserFromAdminViewModel
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                CurrentAvatar = x.UserAvatar,
                UserRoles = x.UserRoles.Select(x => x.RoleId).ToList()
            }).Single(x => x.UserId == userId);
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

        public UserForAdminViewModel GetUsers(int pageId = 1, string emailFilter = "", string userNameFilter = "")
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrWhiteSpace(emailFilter))
                query = query.Where(x => x.Email.Contains(emailFilter));

            if (!string.IsNullOrWhiteSpace(userNameFilter))
                query = query.Where(x => x.UserName.Contains(userNameFilter));

            // Show Item In Page

            int take = 5;
            int skip = (pageId - 1) * take;

            var model = new UserForAdminViewModel
            {
                CurrentPage = pageId,
                PageCount = query.Count() / take,
                Users = query.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList()
            };

            return model;
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

        public void RemoveUser(int userId)
        {
            var user = GetUserByUserId(userId);
            user.IsRemoved = true;
            UpdateUser(user);
        }

        public void RestoreUser(int userId)
        {
            var user = _context.Users.IgnoreQueryFilters().Single(u => u.UserId == userId);
            user.IsRemoved = false;
            UpdateUser(user);
        }

        public string SaveUserAvatar(string currentAvatar, IFormFile newAvatar)
        {
            string imagePath = "";
            if (currentAvatar != "Default.gif")
            {
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", currentAvatar);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            currentAvatar = Generator.Generator.GenerateUniqCode() + Path.GetExtension(newAvatar.FileName);
            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", currentAvatar);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                newAvatar.CopyTo(stream);
            }

            return currentAvatar;
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