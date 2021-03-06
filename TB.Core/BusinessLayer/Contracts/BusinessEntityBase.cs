using System;
using TaskBuddi.DL.SQLite;

namespace TaskBuddi.BL.Contracts
{
	/// <summary>
	/// Business entity base class. 
	/// Provides the ID property and implmenents the IBusiness interface.
	/// </summary>
	public abstract class BusinessEntityBase : IBusinessEntity
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
	}
}

