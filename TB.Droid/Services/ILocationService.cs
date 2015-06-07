using System;

namespace TaskBuddi.Droid
{
	// This class will provide the Interface to the the bindable location service
	// Not currently implemented
	public interface ILocationService
	{
		void LocationChanged(double longitude, double latitude);
	}
}

