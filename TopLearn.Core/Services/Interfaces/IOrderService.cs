using System.Collections.Generic;
using TopLearn.Core.DTOs.Order;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IOrderService
    {
        #region Order

        int AddOrder(string userName, int courseId);
        void UpdateOrderPrice(int orderId);
        void UpdateOrder(Order order);
        Order GetOrderForUserPanel(string userName, int orderId);
        Order GetOrderById(int orderId);
        bool IsFinallyOrder(string userName, int orderId);
        List<Order> GetOrders(string userName);
        bool IsUserInCourse(string userName, int courseId);

        #endregion

        #region Discount

        DiscountType UseDiscount(int orderId, string code);
        void UpdateDiscount(Discount discount);
        void AddDiscount(Discount discount);
        Discount GetDiscountById(int discountId);
        List<Discount> GetDiscounts();

        #endregion
    }
}