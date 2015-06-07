using System;

namespace TaskBuddi.Droid
{
	public interface ILocationService
	{
		void LocationChanged(double longitude, double latitude);
	}
}

