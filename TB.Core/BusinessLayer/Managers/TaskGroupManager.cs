using System;
using System.Collections.Generic;
using TaskBuddi.BL;
using TaskBuddi.DAL;

//using Android.Database;
using TaskBuddi.DL.SQLite;

namespace TaskBuddi.BL.Managers
{
    public static class TaskGroupManager
    {
        static TaskGroupManager()
        {			
        }

        public static TaskGroup GetTaskGroup(int id)
        {
            return TaskRepository.GetTaskGroup(id);
        }

        public static IList<TaskGroup> GetTaskGroups()
        {
            return new List<TaskGroup>(TaskRepository.GetTaskGroups());
        }

        public static IEnumerable<TaskGroup> GetRawTaskGroups()
        {
            return new List<TaskGroup>(TaskRepository.GetRawTaskGroups());
        }

        public static int SaveTaskGroup(TaskGroup group)
        {
            return TaskRepository.SaveTaskGroup(group);
        }

        public static int DeleteTaskGroup(int id)
        {
            return TaskRepository.DeleteTaskGroup(id);
        }
		
    }
}