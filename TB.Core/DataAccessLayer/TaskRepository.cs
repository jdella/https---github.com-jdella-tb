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
				// TODO Rename db file
				var sqliteFilename = "TaskDB.db3";
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

		//TODO: still needed?? OLD IMPLEMENTATION
		//		public static IEnumerable<TaskGroup> GetTasksByGroup()
		//		{
		//			var tasks = me.db.GetItems<Task>();
		//			var groups = me.db.GetItems<TaskGroup>();
		//
		//			//List<List<Task>> groupedTasks = new List<List<Task>>();
		//			//List<TaskGroup> taskGroups = new List<TaskGroup>();
		//
		//			//List groupIds = new List ();
		////			foreach (var group in groups) {
		////				groupIds.add(group.ID);
		////			}
		//
		//			foreach (var group in groups)
		//			{
		//				//List<Task> groupTasks = new List<Task>();
		////                if (tasks != null)
		////                {
		////                    foreach (var task in tasks)
		////                    {
		////                        //me.db.DeleteItem<Task> (task.ID);
		////                        //if (task.groupId == group.ID) {
		////                        group.Tasks.Add(task);
		////                        //}
		////                    }
		////                }
		//				//groupedTasks.Add(groupTasks);
		//
		//			}
		//			return groups;
		//		}

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

		public static IEnumerable<TaskGroup> GetRawTaskGroups()
		{
			// var cn = new SQLiteConnection(dbLocation);

			//var cmd = cn.CreateCommand();
                
			//return cmd.CommandText="Select * from [TaskGroup]";

//            var t = me.db.Query("Select * from [TaskGroup]", null);
//               
//            ICursor c = (ICursor)t;


			return me.db.GetItems<TaskGroup>();
		}

		public static int SaveTaskGroup(TaskGroup group)
		{
			// Set ID if new
//            if (group.ID == 0)
//            {
//                var lastId = me.db.Table<TaskGroup>().OrderByDescending(g => g.ID).First().ID;
//                group.ID = lastId + 1;
//            }
			return me.db.SaveItem<TaskGroup>(group);
		}

		public static int DeleteTaskGroup(int id)
		{
			return me.db.DeleteItem<TaskGroup>(id);
		}
	}
}

