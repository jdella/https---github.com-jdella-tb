using System;
using TaskBuddi.BL.Contracts;
using TaskBuddi.DL.SQLite;
using System.Collections.Generic;

namespace TaskBuddi.BL
{
	/// <summary>
	/// Represents a TaskGroup.
	/// </summary>
	public class TaskGroup : IBusinessEntity
	{
		public TaskGroup()
		{
		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Name { get; set; }
		//TODO implement order/unordered
		public string GroupType { get; set; }

		//@Override
		public override string ToString()
		{
			return Name;
		}
		
	}
}

