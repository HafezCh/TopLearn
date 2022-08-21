using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TopLearn.Core.DTOs.Course;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.Services.Interfaces
{
    public interface ICourseService
    {
        #region Group

        List<CourseGroup> GetCourseGroups();
        List<SelectListItem> GetGroupForManageCourse();
        List<SelectListItem> GetSubGroupForManageCourse(int groupId);
        List<SelectListItem> GetTeachers();
        List<SelectListItem> GetLevels();
        List<SelectListItem> GetStatues();
        CourseGroup GetGroupById(int groupId);
        void AddGroup(CourseGroup group);
        void UpdateGroup(CourseGroup group);

        #endregion

        #region Course

        Tuple<List<ShowCourseListItemViewModel>, int> GetCourses(int pageId = 1, string filter = "", string getType = "all",
            string orderByType = "date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0);
        int AddCourseFromAdmin(Course course, IFormFile imgCourse, IFormFile courseDemo);
        List<ShowCourseForAdminViewModel> GetCoursesForAdmin();
        List<ShowCourseListItemViewModel> GetPopularCourses();
        Course GetCourseDetails(int courseId);
        ShowCourseViewModel GetCourseForShow(int courseId);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);

        #endregion

        #region Episode

        List<CourseEpisode> GetCourseEpisodes(int courseId);
        int AddEpisode(CourseEpisode episode, IFormFile episodeFile);
        bool CheckExistFile(string fileName);
        CourseEpisode GetEpisodeById(int episodeId);
        void UpdateEpisode(CourseEpisode episode, IFormFile episodeFile);
        int DeleteEpisode(int episodeId);

        #endregion

        #region Comment

        Tuple<List<CourseComment>, int> GetCourseComments(int courseId, int pageId = 1);
        void AddComment(CourseComment comment);

        #endregion

        #region Course Vote

        void AddVote(int userId, int courseId, bool vote);
        Tuple<int, int> GetCourseVotes(int courseId);

        #endregion
    }
}