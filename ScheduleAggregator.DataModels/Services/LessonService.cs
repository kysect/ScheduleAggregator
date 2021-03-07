using ScheduleAggregator.DataModels.Entities;
using ScheduleAggregator.DataModels.Enums;
using ScheduleAggregator.DataModels.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleAggregator.DataModels.Services
{
    public class LessonService
    {
        private UnitOfWork _uof;
        public LessonService(UnitOfWork uof)
        {
            _uof = uof;
        }
        public Guid Create(Guid subjectID, LessonType lessonType, Guid groupID, Guid teacherID, Guid roomID, TimeSlot timeSlot, DaySlot daySlot, WeekType weekType)
        {
            var LessonsInThisTime = _uof.Lessons.Get(_ => _.DaySlot == daySlot && _.TimeSlot == timeSlot);

            if (LessonsInThisTime.Where(_ => _.Room.Id == roomID).Any())
                throw new Exception("There is already lesson in this room");

            if (LessonsInThisTime.Where(_ => _.Teacher.Id == teacherID).Any())
                throw new Exception("This teacher already has lesson in this time");

            if (LessonsInThisTime.Where(_ => _.Group.Id == groupID).Any())
                throw new Exception("This group already has lesson in this time");

            var Out = new Lesson() {
                Subject = _uof.SemesterSubjects.FindById(subjectID),
                LessonType = lessonType,
                Group = _uof.StudyGroups.FindById(groupID),
                Teacher = _uof.Teachers.FindById(teacherID),
                Room = _uof.Rooms.FindById(roomID),
                TimeSlot = timeSlot,
                DaySlot = daySlot,
                WeekType = weekType
            };
            _uof.Lessons.Create(Out);
            return Out.Id;
        }

        public void AssignLessonToTeacher(Guid teacherID, Guid lessonID)
        {
            var teacher = _uof.Teachers.FindById(teacherID);
            var lesson = _uof.Lessons.FindById(lessonID);

            if(teacher.Lessons.Where(_ => _.DaySlot == lesson.DaySlot && _.TimeSlot == lesson.TimeSlot).Any())
                throw new Exception("This teacher already has lesson in this time");

            if (!teacher.Lessons.Exists(_ => _.Id == lesson.Id))
            {
                teacher.Lessons.Add(lesson);
                _uof.Teachers.Update(teacher);
            }
        }

        #region SameOperations
        public Lesson FindByID(Guid id)
        {
            return _uof.Lessons.FindById(id);
        }

        public IEnumerable<Lesson> Get()
        {
            return _uof.Lessons.Get();
        }

        public IEnumerable<Lesson> Get(Func<Lesson, bool> predicate)
        {
            return _uof.Lessons.Get(predicate);
        }

        public void Remove(Guid lessonID)
        {
            _uof.Lessons.Remove(_uof.Lessons.FindById(lessonID));
        }
        #endregion
    }
}
