﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.Course
{
    public class CourseLevel
    {
        [Key]
        public int LevelId { get; set; }

        [MaxLength(150)]
        [Required]
        public string LevelTitle { get; set; }

        #region Relations

        public List<Course> Courses { get; set; }

        #endregion
    }
}