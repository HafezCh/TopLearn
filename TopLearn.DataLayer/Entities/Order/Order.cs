using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.Order
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrderSum { get; set; }

        public bool IsFinally { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        #region Relations

        public virtual User.User User { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }

        #endregion
    }
}
