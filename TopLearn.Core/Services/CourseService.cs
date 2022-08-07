using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public List<CourseGroup> GetCourseGroups()
        {
            return _context.CourseGroups.AsNoTracking().ToList();
        }
    }
}