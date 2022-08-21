using System;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.Course
{
    public class CourseVote
    {
        [Key]
        public int VoteId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool Vote { get; set; }
        public DateTime VoteDate { get; set; } = DateTime.Now;

        #region Relations

        public User.User User { get; set; }
        public Course Course { get; set; }

        #endregion
    }
}