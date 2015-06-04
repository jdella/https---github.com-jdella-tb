using System;
using System.Collections.Generic;
using TaskBuddi.BL;

namespace TaskBuddi.BL.Managers
{
	public static class TaskManager
	{
		static TaskManager()
		{
		}

		public static Task GetTask(int id)
		{
			return DAL.TaskRepository.GetTask(id);
		}

		public static IList<Task> GetTasks()
		{
			return new List<Task>(DAL.TaskRepository.GetTasks());
		}

		public static IList<Task> GetTasksByGroup(int id)
		{
			return new List<Task>(DAL.TaskRepository.GetTasksByGroup(id));
		}

		public static int SaveTask(Task item)
		{
			return DAL.TaskRepository.SaveTask(item);
		}

		public static int DeleteTask(int id)
		{
			return DAL.TaskRepository.DeleteTask(id);
		}
		
	}
}