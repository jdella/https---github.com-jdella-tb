using System;
using System.Linq;
using TaskBuddi.BL;
using System.Collections.Generic;
using TaskBuddi.DL.SQLite;
using TaskBuddi.BL.Contracts;

namespace TaskBuddi.DL
{
	/// <summary>
	/// TaskDatabase builds on SQLite.Net and represents a specific database, in our case, the Task DB.
	/// It contains methods for retrieval and persistance as well as db creation, all based on the 
	/// underlying ORM.
	/// </summary>
	public class TaskDatabase : SQLiteConnection
	{
		static object locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskBuddi.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public TaskDatabase(string path)
			: base(path)
		{
			//DropTable<Task>();
			//DropTable<TaskGroup>();
			// create the tables
			CreateTable<Task>();
			CreateTable<TaskGroup>();
			//AddDefaultGroup();
		}

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

		public List<T> GetRawQuery<T>(string qry) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				//var map = GetMapping(typeof(T));
				//var qry = "Select * From " + map.TableName;
				return Query<T>(qry);
			}
		}

		public IEnumerable<T> GetItems<T>() where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return (from i in Table<T>()
				                    select i).ToList();
			}
		}

		public IEnumerable<T> GetRawItems<T>() where T : IBusinessEntity, new()
		{
			lock (locker)
			{
//                var q = new SQLiteQueryBuilder();
//                var c = q.Query(me.db, null, null, null, null, null, "Name");



				return (from i in Table<T>()
				                    select i).ToList();
			}
		}

		public T GetItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return Table<T>().FirstOrDefault(x => x.ID == id);
				// Following throws NotSupportedException - thanks aliegeni
				//return (from i in Table<T> ()
				//        where i.ID == id
				//        select i).FirstOrDefault ();
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
#if NETFX_CORE
                return Delete(new T() { ID = id });
#else
				return Delete<T>(new T() { ID = id });
#endif
			}
		}
	}
}