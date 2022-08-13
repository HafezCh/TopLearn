using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopLearn.DataLayer.Entities.Course
{
    public class CourseGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Display(Name = "عنوان گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string GroupTitle { get; set; }
        [Display(Name = "حذف شده ؟")]
        public bool IsRemoved { get; set; }

        [Display(Name = "گروه اصلی")]
        public int? ParentId { get; set; }

        #region Relations

        [ForeignKey("ParentId")]
        public virtual List<CourseGroup> CourseGroups { get; set; }

        [InverseProperty("CourseGroup")]
        public virtual List<Course> Courses { get; set; }

        [InverseProperty("SGroup")]
        public virtual List<Course> SubGroup { get; set; }

        #endregion
    }
}