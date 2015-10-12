using System;
using System.Linq;
using TaskBuddi.BL;
using System.Collections.Generic;
using TaskBuddi.DL.SQLite;
using TaskBuddi.BL.Contracts;

namespace TaskBuddi.DL
{
	/// <summary>
	// DB access abstraction layer using generics.
	/// </summary>
	public class TaskDatabase : SQLiteConnection
	{
		static object locker = new object();

		public TaskDatabase(string path)
			: base(path)
		{
			//for debugging
			//DropTable<Task>();
			//DropTable<TaskGroup>();
			
			// create the tables
			CreateTable<Task>();
			CreateTable<TaskGroup>();

			//AddDefaultGroup();  //for debuigging
		}

		//populate DB with default group - useful for debugging
		private int AddDefaultGroup()
		{
			DeleteItem<TaskGroup>(1);
			var tg = new TaskGroup();
			tg.ID = 1;
			tg.Name = "Default";
			SaveItem<TaskGroup>(tg);
			return 1;
		}

		private int DropTable<T>()
		{
			var map = GetMapping(typeof(T));
			var qry = String.Format("drop table if exists \"{0}\"", map.TableName);
			return Execute(qry);
		}

		public IEnumerable<T> GetItems<T>() where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return (from i in Table<T>()
				                    select i).ToList();
			}
		}

		public T GetItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return Table<T>().FirstOrDefault(x => x.ID == id);
			}
		}

		public int SaveItem<T>(T item) where T : IBusinessEntity
		{
			lock (locker)
			{
				if (item.ID != 0)
				{
					Update(item);
					return item.ID;
				}
				else
				{
					return Insert(item);
				}
			}
		}

		public int DeleteItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return Delete<T>(new T() { ID = id });
			}
		}
	}
}