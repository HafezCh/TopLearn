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
        //public TimeSpan TotalTime { get; set; }
    }
}
