using System;

namespace TaskBuddi
{
	public class LocationResult
	{
		public Geometry geometry;
		public string name, icon, id, place_id, reference, scope, vicinity;
		public string[] types;

		public LocationResult()
		{
            
		}
	}

	public class Geometry
	{
		public Loc Location;

		public Geometry()
		{            
		}
	}

	public class Loc
	{
		public decimal lat;
		public decimal lng;

		public Loc()
		{
		}

	}
}

