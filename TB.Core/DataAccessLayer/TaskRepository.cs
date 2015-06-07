using System;
using System.Collections.Generic;
using System.IO;
using TaskBuddi.BL;
using System.Linq;
using System.Collections;

//using Android.Media;
//using Android.Database.Sqlite;
//using Android.Database;
using TaskBuddi.DL.SQLite;

namespace TaskBuddi.DAL
{
	public class TaskRepository
	{
		DL.TaskDatabase db = null;
		protected static string dbLocation;
		protected static TaskRepository me;
		//TODO lock all access
		protected object theLock = new object();

		static TaskRepository()
		{
			me = new TaskRepository();
		}

		protected TaskRepository()
		{
			// instantiate the database
			dbLocation = DatabaseFilePath;
			db = new TaskBuddi.DL.TaskDatabase(dbLocation);
		}

		public static string DatabaseFilePath
		{
			get
			{
				var sqliteFilename = "TaskBuddiDB.db3";
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				var path = Path.Combine(libraryPath, sqliteFilename);
				return path;
			}
		}

		public static Task GetTask(int id)
		{
			return me.db.GetItem<Task>(id);
		}

		public static IEnumerable<Task> GetTasks()
		{
			return me.db.GetItems<Task>();
		}

		public static IEnumerable<Task> GetTasksByGroup(int groupId)
		{
			var res = me.db.GetItems<Task>().Where(t => t.GroupId == groupId);
			return res;
		}

		public static int SaveTask(Task item)
		{
			return me.db.SaveItem<Task>(item);
		}

		public static int DeleteTask(int id)
		{
			return me.db.DeleteItem<Task>(id);
		}

		//TASK GROUPS
		public static TaskGroup GetTaskGroup(int id)
		{
			return me.db.GetItem<TaskGroup>(id);
		}

		public static IEnumerable<TaskGroup> GetTaskGroups()
		{
			return me.db.GetItems<TaskGroup>();
		}

		//		public static IEnumerable<TaskGroup> GetRawTaskGroups()
		//		{
		//			return me.db.GetItems<TaskGroup>();
		//		}

		public static int SaveTaskGroup(TaskGroup group)
		{
			return me.db.SaveItem<TaskGroup>(group);
		}

		public static int DeleteTaskGroup(int id)
		{
			return me.db.DeleteItem<TaskGroup>(id);
		}
	}
}

