﻿using System;
using System.Collections.Generic;
using System.Linq;
using ItmoScheduleApiWrapper;
using ItmoScheduleApiWrapper.Models;

namespace ScheduleAggregator
{
    public class ScheduleItemProvider
    {
        public ScheduleItemProvider(List<String> groupList)
        {
            GroupList = groupList;
        }

        public List<string> GroupList { get; }
        private List<ScheduleItemModel> _scheduleItemModels;

        public void PrintLecture()
        {
            Print(GetItemsForGroup().Where(e => e.IsLecture()));
        }

        public void PrintPractice()
        {
            Print(GetItemsForGroup().Where(e => !e.IsLecture()));
        }


        public List<ScheduleItemModel> GetItemsForGroup()
        {
            if (_scheduleItemModels == null)
            {
                _scheduleItemModels = GroupList
                    .AsParallel()
                    .Select(GetScheduleItemModels)
                    .SelectMany(e => e)
                    .OrderBy(e => e.DataDay)
                    .ThenBy(e => e.DataWeek)
                    .ThenBy(e => e.StartTime)
                    .ThenBy(e => e.SubjectTitle)
                    .ThenBy(e => e.Group)
                    .ThenBy(e => e.Teacher)
                    .ToList();
            }

            return _scheduleItemModels;
        }

        private static IEnumerable<ScheduleItemModel> GetScheduleItemModels(String groupTitle)
        {
            var provider = new ItmoApiProvider();

            List<ScheduleItemModel> result = provider
                .ScheduleApi
                .GetGroupSchedule(groupTitle)
                .Result
                .Schedule;

            result.ForEach(s => s.Group = groupTitle);
            return result;
        }

        private static void Print(IEnumerable<ScheduleItemModel> items)
        {
            var scheduleItems = items
                .Select(e => (e.DataDay, e.DataWeek, e.Group, e.StartTime, e.SubjectTitle, e.Teacher))
                .GroupBy(e => (e.DataDay, e.DataWeek))
                .ToList();

            foreach (var tuple in scheduleItems)
            {
                Console.WriteLine($"\t\t{tuple.Key}");
                foreach (var valueTuple in tuple)
                {
                    Console.WriteLine(valueTuple);
                }
            }
        }


    }
}