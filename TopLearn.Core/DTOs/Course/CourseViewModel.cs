using System;
using System.Collections.Generic;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.DTOs.Course
{
    public class ShowCourseForAdminViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string PictureName { get; set; }
        public string Teacher { get; set; }
        public int EpisodesCount { get; set; }
    }

    public class ShowCourseListItemViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int CoursePrice { get; set; }
        public string PictureName { get; set; }
        //public long TotalTime { get; set; }
    }

    public class ShowCourseViewModel
    {
        public int CourseId { get; set; }
        public int StatusId { get; set; }
        public int LevelId { get; set; }
        public string CourseTitle { get; set; }
        public string StatusTitle { get; set; }
        public string LevelTitle { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public int UsersCount { get; set; }
        public string CourseDescription { get; set; }
        public int CoursePrice { get; set; }
        public string Tags { get; set; }
        public string CourseImageName { get; set; }
        public string DemoFileName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<CourseEpisode> CourseEpisodes { get; set; }
    }
}
