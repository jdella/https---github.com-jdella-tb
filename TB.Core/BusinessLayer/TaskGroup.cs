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
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Name { get; set; }

		//TODO: Implement Group Types (eg. ordered/unorderd)
		public string GroupType { get; set; }

		//@Override
		public override string ToString()
		{
			return Name;
		}
		
	}
}

