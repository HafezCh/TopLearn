using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.DataLayer.Entities.Order
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "مقادیر این فیلد الزامی است")]
        [MaxLength(150, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string DiscountCode { get; set; }

        [Display(Name = "درصد تخفیف")]
        [Required(ErrorMessage = "مقادیر این فیلد الزامی است")]
        public int DiscountRate { get; set; }

        public int? UsableCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        #region Relations

        public List<UserDiscountCode> UserDiscountCodes { get; set; }

        #endregion
    }
}
