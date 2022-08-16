using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TopLearn.Core.DTOs.Order;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Order;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services
{
    public class OrderService : IOrderService
    {
        #region Injections

        private readonly IUserService _userService;
        private readonly TopLearnDbContext _context;

        public OrderService(IUserService userService, TopLearnDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        #endregion

        public void AddDiscount(Discount discount)
        {
            _context.Discounts.Add(discount);
            _context.SaveChanges();
        }

        public int AddOrder(string userName, int courseId)
        {
            var userId = _userService.GetUserIdByUserName(userName);

            var order = _context.Orders.FirstOrDefault(o => o.UserId == userId && !o.IsFinally);

            var course = _context.Courses.Find(courseId);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    CreationDate = DateTime.Now,
                    IsFinally = false,
                    OrderSum = course.CoursePrice,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            Count = 1,
                            CourseId = course.CourseId,
                            UnitPrice = course.CoursePrice,
                            Price = course.CoursePrice
                        }
                    }
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            else
            {
                var detail =
                    _context.OrderDetails.FirstOrDefault(d =>
                        d.OrderId == order.OrderId && d.CourseId == course.CourseId);

                if (detail != null)
                {
                    detail.Count += 1;
                    detail.Price = detail.UnitPrice * detail.Count;
                    _context.OrderDetails.Update(detail);
                }
                else
                {
                    detail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        Count = 1,
                        CourseId = courseId,
                        UnitPrice = course.CoursePrice,
                        Price = course.CoursePrice
                    };
                    _context.OrderDetails.Add(detail);
                }

                _context.SaveChanges();
                UpdateOrderPrice(order.OrderId);
            }

            return order.OrderId;
        }

        public Discount GetDiscountById(int discountId)
        {
            return _context.Discounts.Find(discountId);
        }

        public List<Discount> GetDiscounts()
        {
            return _context.Discounts.AsNoTracking().ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
        }

        public Order GetOrderForUserPanel(string userName, int orderId)
        {
            var userId = _userService.GetUserIdByUserName(userName);

            return _context.Orders
                .Include(od => od.OrderDetails)
                .ThenInclude(c => c.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
        }

        public List<Order> GetOrders(string userName)
        {
            var userId = _userService.GetUserIdByUserName(userName);

            return _context.Orders.Where(o => o.UserId == userId).AsNoTracking().ToList();
        }

        public bool IsFinallyOrder(string userName, int orderId)
        {
            var userId = _userService.GetUserIdByUserName(userName);

            var order = _context.Orders
                .Include(od => od.OrderDetails)
                .ThenInclude(u => u.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null || order.IsFinally)
            {
                return false;
            }

            if (_userService.BalanceUserWallet(userName) >= order.OrderSum)
            {
                order.IsFinally = true;
                _userService.AddWallet(new Wallet
                {
                    UserId = userId,
                    Amount = order.OrderSum,
                    CreationDate = DateTime.Now,
                    Description = "فاکتور شماره #" + order.OrderId,
                    IsPay = true,
                    TypeId = 2
                });
                _context.Orders.Update(order);

                foreach (var detail in order.OrderDetails)
                {
                    _context.UserCourses.Add(new UserCourse
                    {
                        CourseId = detail.CourseId,
                        UserId = userId
                    });
                }

                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool IsUserInCourse(string userName, int courseId)
        {
            var userId = _userService.GetUserIdByUserName(userName);

            return _context.UserCourses.Any(c => c.UserId == userId && c.CourseId == courseId);
        }

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void UpdateOrderPrice(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            order.OrderSum = _context.OrderDetails.Where(o => o.OrderId == order.OrderId).Sum(s => s.Price);
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public DiscountType UseDiscount(int orderId, string code)
        {
            var discount = _context.Discounts.FirstOrDefault(d => d.DiscountCode == code);

            if (discount == null)
                return DiscountType.NotFound;

            if (discount.StartDate != null && discount.StartDate > DateTime.Now)
                return DiscountType.ExpireDate;

            if (discount.EndDate != null && discount.EndDate < DateTime.Now)
                return DiscountType.ExpireDate;

            if (discount.DiscountCode != null && discount.UsableCount < 1)
                return DiscountType.Finished;

            var order = GetOrderById(orderId);

            if (_context.UserDiscountCodes.Any(ud => ud.UserId == order.UserId && ud.DiscountId == discount.DiscountId))
                return DiscountType.UserUsed;

            var discountPrice = (order.OrderSum * discount.DiscountRate) / 100;
            order.OrderSum -= discountPrice;

            UpdateOrder(order);

            if (discount.UsableCount != null)
                discount.UsableCount -= 1;

            UpdateDiscount(discount);

            _context.UserDiscountCodes.Add(new UserDiscountCode
            {
                UserId = order.UserId,
                DiscountId = discount.DiscountId
            });

            _context.SaveChanges();

            return DiscountType.Success;
        }
    }
}