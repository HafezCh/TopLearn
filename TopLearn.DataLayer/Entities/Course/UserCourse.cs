using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.Course
{
    public class UserCourse
    {
        [Key]
        public int UC_Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        #region Relations

        public virtual User.User User { get; set; }
        public virtual Course Course { get; set; }

        #endregion
    }
}