using System;
using TaskBuddi.BL.Contracts;
using TaskBuddi.DL.SQLite;

namespace TaskBuddi.BL
{
	/// <summary>
	/// Represents a Task.
	/// </summary>
	public class Task : IBusinessEntity
	{
		public Task()
		{
		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Name { get; set; }

		public string Category { get; set; }

		public string Notes { get; set; }

		public bool Done { get; set; }

		[Indexed]
		public int GroupId { get; set; }

	}
}

