﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.Services
{
    public class CourseService : ICourseService
    {
        #region Injection

        private readonly TopLearnDbContext _context;

        public CourseService(TopLearnDbContext context)
        {
            _context = context;
        }

        #endregion

        public int AddCourseFromAdmin(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.jpg";

            #region Check & Upload Image

            if (imgCourse != null && imgCourse.IsImage())
            {
                course.CourseImageName = Generator.Generator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/Image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                var imgReSizer = new ImageConvertor();
                var thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/Thumb", course.CourseImageName);

                imgReSizer.Image_resize(imagePath, thumbPath, 150);
            }


            #endregion

            #region Upload Demo

            if (courseDemo != null)
            {
                course.DemoFileName = Generator.Generator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                var demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/demoes", course.DemoFileName);
                using var stream = new FileStream(demoPath, FileMode.Create);
                courseDemo.CopyTo(stream);
            }

            #endregion

            _context.Courses.Add(course);
            _context.SaveChanges();

            return course.CourseId;
        }

        public List<CourseGroup> GetCourseGroups()
        {
            return _context.CourseGroups.Include(g => g.CourseGroups).AsNoTracking().ToList();
        }

        public List<SelectListItem> GetGroupForManageCourse()
        {
            return _context.CourseGroups.Where(x => x.ParentId == null)
                .Select(c => new SelectListItem
                {
                    Text = c.GroupTitle,
                    Value = c.GroupId.ToString()
                }).AsNoTracking().ToList();
        }

        public List<SelectListItem> GetLevels()
        {
            return _context.CourseLevels.Select(x => new SelectListItem
            {
                Text = x.LevelTitle,
                Value = x.LevelId.ToString()
            }).AsNoTracking().ToList();
        }

        public List<SelectListItem> GetStatues()
        {
            return _context.CourseStatus.Select(x => new SelectListItem
            {
                Text = x.StatusTitle,
                Value = x.StatusId.ToString()
            }).AsNoTracking().ToList();
        }

        public List<SelectListItem> GetSubGroupForManageCourse(int groupId)
        {
            return _context.CourseGroups.Where(x => x.ParentId == groupId)
                .Select(x => new SelectListItem
                {
                    Text = x.GroupTitle,
                    Value = x.GroupId.ToString()
                }).AsNoTracking().ToList();
        }

        public List<SelectListItem> GetTeachers()
        {
            return _context.UserRoles.Where(r => r.RoleId == 2)
                .Include(u => u.User)
                .Select(x => new SelectListItem
                {
                    Text = x.User.UserName,
                    Value = x.UserId.ToString()
                }).AsNoTracking().ToList();
        }

        public List<ShowCourseForAdminViewModel> GetCoursesForAdmin()
        {
            return _context.Courses
                .Include(u => u.User)
                .Select(c => new ShowCourseForAdminViewModel
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    PictureName = c.CourseImageName,
                    Teacher = c.User.UserName,
                    EpisodesCount = c.CourseEpisodes.Count,
                }).AsNoTracking().ToList();
        }

        public Course GetCourseDetails(int courseId)
        {
            return _context.Courses.Find(courseId);
        }

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.UpdateDate = DateTime.Now;

            #region Upload Image

            if (imgCourse != null && imgCourse.IsImage())
            {
                if (course.CourseImageName != "no-photo.jpg")
                {
                    string deleteimagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/Image", course.CourseImageName);
                    if (File.Exists(deleteimagePath))
                    {
                        File.Delete(deleteimagePath);
                    }

                    string deletethumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/Thumb", course.CourseImageName);
                    if (File.Exists(deletethumbPath))
                    {
                        File.Delete(deletethumbPath);
                    }
                }
                course.CourseImageName = Generator.Generator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/Image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Course/Thumb", course.CourseImageName);

                imgResizer.Image_resize(imagePath, thumbPath, 150);
            }


            #endregion

            #region Upload Demo

            if (courseDemo != null)
            {
                if (course.DemoFileName != null)
                {
                    string deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                    if (File.Exists(deleteDemoPath))
                    {
                        File.Delete(deleteDemoPath);
                    }
                }
                course.DemoFileName = Generator.Generator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                using (var stream = new FileStream(demoPath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }


            #endregion

            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public int AddEpisode(CourseEpisode episode, IFormFile episodeFile)
        {
            episode.EpisodeFileName = episodeFile.FileName;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFiles", episode.EpisodeFileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            episodeFile.CopyTo(stream);

            _context.CourseEpisodes.Add(episode);
            _context.SaveChanges();

            return episode.EpisodeId;
        }

        public bool CheckExistFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFiles", fileName);
            return File.Exists(filePath);
        }

        public List<CourseEpisode> GetCourseEpisodes(int courseId)
        {
            return _context.CourseEpisodes.Where(e => e.CourseId == courseId).AsNoTracking().ToList();
        }

        public CourseEpisode GetEpisodeById(int episodeId)
        {
            return _context.CourseEpisodes.Find(episodeId);
        }

        public void UpdateEpisode(CourseEpisode episode, IFormFile episodeFile)
        {
            if (episodeFile != null)
            {
                var deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFiles", episode.EpisodeFileName);
                File.Delete(deleteFilePath);

                episode.EpisodeFileName = episodeFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFiles", episode.EpisodeFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                episodeFile.CopyTo(stream);
            }

            _context.CourseEpisodes.Update(episode);
            _context.SaveChanges();
        }

        public int DeleteEpisode(int episodeId)
        {
            var episode = GetEpisodeById(episodeId);

            var deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFiles", episode.EpisodeFileName);
            File.Delete(deleteFilePath);

            var courseId = episode.CourseId;

            _context.CourseEpisodes.Remove(episode);
            _context.SaveChanges();

            return courseId;
        }

        public Tuple<List<ShowCourseListItemViewModel>, int> GetCourses(int pageId = 1, string filter = "", string getType = "all", string orderByType = "date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {
            if (take == 0)
                take = 8;

            IQueryable<Course> result = _context.Courses;

            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(c => c.CourseTitle.Contains(filter) || c.Tags.Contains(filter));
            }

            switch (getType)
            {
                case "all":
                    break;
                case "buy":
                    {
                        result = result.Where(c => c.CoursePrice != 0);
                        break;
                    }
                case "free":
                    {
                        result = result.Where(c => c.CoursePrice == 0);
                        break;
                    }

            }

            switch (orderByType)
            {
                case "date":
                    {
                        result = result.OrderByDescending(c => c.CreateDate);
                        break;
                    }
                case "updatedate":
                    {
                        result = result.OrderByDescending(c => c.UpdateDate);
                        break;
                    }
            }

            if (startPrice > 0)
            {
                result = result.Where(c => c.CoursePrice > startPrice);
            }

            if (endPrice > 0)
            {
                result = result.Where(c => c.CoursePrice < endPrice);
            }


            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (var group in selectedGroups)
                {
                    result = result.Where(c => c.GroupId == group || c.SubGroup == group);
                }
            }

            int skip = (pageId - 1) * take;

            var pageCount = result.Count() / take;

            var query = result.Select(c => new ShowCourseListItemViewModel()
            {
                CourseId = c.CourseId,
                PictureName = c.CourseImageName,
                CoursePrice = c.CoursePrice,
                CourseTitle = c.CourseTitle,
            }).AsNoTracking().Skip(skip).Take(take).ToList();

            return Tuple.Create(query, pageCount);
        }

        public ShowCourseViewModel GetCourseForShow(int courseId)
        {
            var course = _context.Courses
                .Include(e => e.CourseEpisodes)
                .Include(u => u.User)
                .Select(x => new ShowCourseViewModel
                {
                    CourseId = x.CourseId,
                    StatusId = x.StatusId,
                    LevelId = x.LevelId,
                    UserName = x.User.UserName,
                    CoursePrice = x.CoursePrice,
                    CourseImageName = x.CourseImageName,
                    CourseEpisodes = x.CourseEpisodes,
                    CourseDescription = x.CourseDescription,
                    CourseTitle = x.CourseTitle,
                    CreateDate = x.CreateDate,
                    Tags = x.Tags,
                    UpdateDate = x.UpdateDate,
                    DemoFileName = x.DemoFileName,
                    UserAvatar = x.User.UserAvatar,
                    UsersCount = x.UserCourses.Count
                }).FirstOrDefault(c => c.CourseId == courseId);

            if (course != null)
            {
                course.StatusTitle = _context.CourseStatus
                    .SingleOrDefault(s => s.StatusId == course.StatusId)?.StatusTitle;

                course.LevelTitle = _context.CourseLevels
                    .SingleOrDefault(l => l.LevelId == course.LevelId)?.LevelTitle;
            }

            return course;
        }

        public Tuple<List<CourseComment>, int> GetCourseComments(int courseId, int pageId = 1)
        {
            var take = 5;
            var skip = (pageId - 1) * take;
            int pageCount = _context.CourseComments.Count(c => c.CourseId == courseId) / take;

            if ((pageCount % 2) != 0)
            {
                pageCount += 1;
            }

            var comments = _context.CourseComments
            .Include(u => u.User)
            .Where(c => c.CourseId == courseId)
            .OrderByDescending(c => c.CreationDate)
            .Skip(skip).Take(take)
            .AsNoTracking().ToList();

            return Tuple.Create(comments, pageCount);
        }

        public void AddComment(CourseComment comment)
        {
            _context.CourseComments.Add(comment);
            _context.SaveChanges();
        }

        public List<ShowCourseListItemViewModel> GetPopularCourses()
        {
            return _context.Courses
                .Include(od => od.OrderDetails)
                .Where(od => od.OrderDetails.Any())
                .OrderByDescending(c => c.OrderDetails.Count)
                .Take(8)
                .Select(x => new ShowCourseListItemViewModel
                {
                    CourseId = x.CourseId,
                    CoursePrice = x.CoursePrice,
                    CourseTitle = x.CourseTitle,
                    PictureName = x.CourseImageName,
                }).ToList();
        }

        public void AddGroup(CourseGroup group)
        {
            _context.CourseGroups.Add(group);
            _context.SaveChanges();
        }

        public void UpdateGroup(CourseGroup group)
        {
            _context.CourseGroups.Update(group);
            _context.SaveChanges();
        }

        public CourseGroup GetGroupById(int groupId)
        {
            return _context.CourseGroups.Find(groupId);
        }

        public void AddVote(int userId, int courseId, bool vote)
        {
            var userVote = _context.CourseVotes.FirstOrDefault(v => v.UserId == userId && v.CourseId == courseId);

            if (userVote != null)
            {
                userVote.Vote = vote;
            }
            else
            {
                userVote = new CourseVote
                {
                    CourseId = courseId,
                    UserId = userId,
                    Vote = vote
                };

                _context.CourseVotes.Add(userVote);
            }

            _context.SaveChanges();
        }

        public Tuple<int, int> GetCourseVotes(int courseId)
        {
            var votes = _context.CourseVotes.Where(v => v.CourseId == courseId).Select(v => v.Vote);

            return Tuple.Create(votes.Count(v => v), votes.Count(v => !v));
        }
    }
}