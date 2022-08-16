using System;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.Course
{
    public class CourseComment
    {
        [Key]
        public int CommentId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        [MaxLength(700)]
        public string Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsAdminRead { get; set; }

        #region Relations

        public Course Course { get; set; }
        public User.User User { get; set; }

        #endregion
    }
}
