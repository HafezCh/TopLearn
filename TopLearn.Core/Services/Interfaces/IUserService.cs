using System.Collections.Generic;
using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region User

        bool IsExistUserName(string userName);
        bool IsExistsEmail(string email);
        int AddUser(User user);
        User LoginUser(LoginViewModel model);
        User GetByEmail(string email);
        User GetByActiveCode(string activeCode);
        User GetByUserName(string userName);
        void UpdateUser(User user);
        bool ActiveAccount(string activeCode);
        int GetUserIdByUserName(string userName);

        #endregion

        #region UserPanel

        InformationUserViewModel GetUserInformation(string userName);
        SlideBarUserPanelViewModel GetSlideBarUserPanel(string userName);
        EditUserProfileViewModel GetDataForEditUserProfile(string userName);
        bool IsExistUserNameForEditProfile(string userName, int userId);
        bool IsExistEmailForEditProfile(string email, int userId);
        void EditProfile(string userName, EditUserProfileViewModel profile);
        bool CompareOldPassword(string userName, string oldPassword);
        void ChangeUserPassword(string userName, string newPassword);

        #endregion

        #region Wallet

        int BalanceUserWallet(string userName);
        List<WalletViewModel> GetUserWallet(string userName);
        int ChargeWallet(string userName, int amount, string description, bool isPay = false);
        int AddWallet(Wallet wallet);
        Wallet GetWalletByWalletId(int walletId);
        void UpdateWallet(Wallet wallet);

        #endregion
    }
}