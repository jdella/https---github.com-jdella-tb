using System;

namespace TaskBuddi.Droid
{
	/// <summary>
	/// This class will provide the Interface to the the bindable location service
	/// NOT USED - currently abandoned in favour of Fused Location Provider services.
	/// </summary>
	public interface ILocationService
	{
		void LocationChanged(double longitude, double latitude);
	}
}

