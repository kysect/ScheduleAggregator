﻿using ScheduleAggregator.DataModels.Entities;
using ScheduleAggregator.DataModels.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleAggregator.DataModels.Services
{
    public class SemesterSubjectService
    {
        private UnitOfWork _uof;
        public SemesterSubjectService(UnitOfWork uof)
        {
            _uof = uof;
        }
        public SemesterSubject Create(Subject subject, uint lecture, uint practise, uint laboratory)
        {
            if (_uof.SemesterSubjects.Get(_ => _.Subject == subject) != null)
                throw new Exception("The SemesterSubject already exists");

            var labourIntensity = new LabourIntensity() { Laboratory = laboratory, Lecture = lecture, Practise = practise};
            var Out = new SemesterSubject() { Subject = subject, LabourIntensity = labourIntensity };
            _uof.SemesterSubjects.Create(Out);
            return Out;
        }
        public void Update(SemesterSubject semesterSubject)
        {
            _uof.SemesterSubjects.Update(semesterSubject);
        }

        #region SameOperations

        public SemesterSubject FindByID(int id)
        {
            return _uof.SemesterSubjects.FindById(id);
        }

        public IEnumerable<SemesterSubject> Get()
        {
            return _uof.SemesterSubjects.Get();
        }
        public void Remove(SemesterSubject studyCourse)
        {
            _uof.SemesterSubjects.Remove(studyCourse);
        }
        #endregion 
    }
}
