package md5ff866c17d93e28b10d5700df9df82dbd;


public class LocationService_LocationBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("TaskBuddi.Droid.LocationService/LocationBinder, TB.Android, Version=1.0.5685.29095, Culture=neutral, PublicKeyToken=null", LocationService_LocationBinder.class, __md_methods);
	}


	public LocationService_LocationBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == LocationService_LocationBinder.class)
			mono.android.TypeManager.Activate ("TaskBuddi.Droid.LocationService/LocationBinder, TB.Android, Version=1.0.5685.29095, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
