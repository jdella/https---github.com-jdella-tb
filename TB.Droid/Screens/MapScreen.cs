using System;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

using TaskBuddi.BL.Managers;
using TaskBuddi.BL;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Views;

namespace TaskBuddi.Droid
{
	using Android.App;
	using Android.Gms.Maps;
	using Android.Gms.Maps.Model;
	using Android.OS;

	/// <summary>
	/// Task Map screen.  Implements iLocation and IGooglePlayServicesClientCOnnection listeners for location updates.
	/// </summary>
	[Activity(Label = "Task Map")]
	public class MapScreen : Activity,  
    IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener , Android.Gms.Location.ILocationListener
	{
		protected GoogleMap _map;
		protected MapFragment _mapFragment;
		protected MarkerOptions marker;
		
		protected List<Task> tasks;
		protected string[] taskCategories;
		protected int[] taskCatCount;
		
		protected Location _currentLocation;
		protected LocationManager _locationManager;
		LocationClient locClient;

		/// <summary>
		/// Raises the create event.  Initialises layout, map and action bar.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{
			// Init view
			base.OnCreate(bundle);
			// action bar
			RequestWindowFeature(WindowFeatures.ActionBar);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);
			// init map
			SetContentView(Resource.Layout.MapLayout);
			InitMapFragment();
			InitTaskCategoryArrays();
		}

		/// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <returns>True</returns>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			//  Currently only back option
			Finish();
			return true;
		}

		/// <summary>
		/// Raises the resume event.  Initialises location client and sets up Map.
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();
			//Connect to Location client
			locClient = new LocationClient(this, this, this);
			locClient.Connect();

			SetupMapIfNeeded();
		}

		/// <summary>
		///  Removes location updates and disconnects location client on pause.
		/// (Re-established on resume).
		/// </summary>
		protected override void OnPause()
		{
			base.OnPause();
			if (locClient.IsConnected)
			{
				locClient.RemoveLocationUpdates(this);
				locClient.Disconnect();
			}
		}

		/// <summary>
		/// Creates  corresponding arrays for distinct task categories for active tasks
		/// and how many tasks in each category.
		/// </summary>
		private void InitTaskCategoryArrays()
		{
			// Get all distinct task categories for incomplete tasks
			tasks = TaskManager.GetTasks().Where(x => x.Done == false).ToList();
			taskCategories = tasks.Select(x => x.Category)
                .Distinct().ToArray();

			// Count how many tasks per each category and store in separate array
			// this is used to display numbert of tasks in the map marker info windows
			taskCatCount = new int[taskCategories.Length];
			for (var i = 0; i < taskCategories.Length; i++)
			{
				taskCatCount[i] = tasks.Count(x => x.Category == taskCategories[i]);
			}
		}

		/// <summary>
		/// Initialises the map fragment and sets map options
		/// </summary>
		private void InitMapFragment()
		{
			// get fragment
			_mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (_mapFragment == null)
			{
				// set map options
				GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
					.InvokeZoomControlsEnabled(true)
					.InvokeCompassEnabled(true);
                
				// load map into fragment
				FragmentTransaction fragTx = FragmentManager.BeginTransaction();
				_mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.vMap, _mapFragment, "map");
				fragTx.Commit();
			}
		}

		/// <summary>
		/// Sets up  the map if needed.
		/// </summary>
		private void SetupMapIfNeeded()
		{
			// initialise map if needed
			if (_map == null)
			{
				_map = _mapFragment.Map;
			}
			// Centre map on current (or last known) location and update markers
			if (_map != null)
			{
				_map.SetInfoWindowAdapter(new MapInfoWindowAdapter(LayoutInflater));
				RecenterMap();
				UpdateMarkers();
			}
		}

		/// <summary>
		/// Updates map markers.
		/// </summary>
		void UpdateMarkers()
		{
			//Guard condition
			if (_map == null || _currentLocation == null)
				return;

			//...Now update map/markers...
			_map.Clear();

			// Gets location address string - not currently used
			// GetDeviceLocationAddress();
           
			// get current latitude and longitude coords
			var currentLocation = new LatLng(_currentLocation.Latitude, _currentLocation.Longitude);
			
			// Set Current location marker
			marker = new MarkerOptions();
			marker.SetPosition(currentLocation);
			marker.SetTitle("Current");
			marker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));
			_map.AddMarker(marker);

			//TODO - experimental Task Location filter options. Radius/Keywords etc
			//const int RADIUS = 500;

			// loop thru all active task categories and plot Task Locations
			for (var i = 0; i < taskCategories.Length; i++)
			{       
				//Gooogle Places API request string (using Server API Key set up in Google Developer Console)
				var req = HttpWebRequest.Create(
					                      "https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
					                      "location=" + _currentLocation.Latitude + "," + _currentLocation.Longitude +
					//"&radius=" + RADIUS + 
					                      "&rankby=distance" +
					                      "&types=" + taskCategories[i] +
					//"&keyword=" + tasks[i].Keywords +
					                      "&key=AIzaSyAoj2j9AtFF08nWRUdiN577shLpvPIhz-E");

				//set headers
				req.ContentType = "application/json";
				req.Method = "GET";

				//make request and process response
				using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
				{
					if (res.StatusCode != HttpStatusCode.OK)
					{
						Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", res.StatusCode);
						return;
					}
					//Process JSON object with data for Task Locations
					using (var reader = new StreamReader(res.GetResponseStream()))
					{
						// get Json data
						var jdata = JObject.Parse(reader.ReadToEnd());
						var results = jdata["results"];
						// map 3 closest locations per category
						for (var loc = 0; loc < 3 && loc < results.Count(); loc++)
						{     
							// get  lat and long coords for location and set position for marker
							var locMarker = new MarkerOptions();
							var locPos = new LatLng(Convert.ToDouble(results[loc]["geometry"]["location"]["lat"]), 
								                                  Convert.ToDouble(results[loc]["geometry"]["location"]["lng"]));
							locMarker.SetPosition(locPos);

							//pluralise "Task"? (for info window title)
							var task_s = taskCatCount[i] == 1 ? " task" : " tasks";
							locMarker.SetTitle(results[loc]["name"] + " (" + taskCatCount[i] + task_s + ")");
							
							//set list of matched Location-Category Tasks as info window body
							var snippet = "";
							foreach (var task in tasks.Where(x=>x.Category == taskCategories[i]))
							{
								snippet += "\n" + task.Name;
							}
							locMarker.SetSnippet(snippet);

							//set marker on map
							locMarker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));
							_map.AddMarker(locMarker);
						}
					}
				}
			}
		}

		/// <summary>
		/// Recenters the map. 
		/// </summary>
		private void RecenterMap()
		{
			if (_map != null && _currentLocation != null)
			{
				CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(
					                                        new LatLng(_currentLocation.Latitude, _currentLocation.Longitude), 15);
				_map.MoveCamera(cameraUpdate);
			}
		}
            
		//  Only used for testing may be useful otherwise...
		/// <summary>
		/// Gets address string for current location. 
		/// </summary>
		async void GetDeviceLocationAddress()
		{
			// debug result string
			var log = "No location";
			if (_currentLocation != null)
			{
				// Google returns a list of addresses
				Geocoder geocoder = new Geocoder(this);
				IList<Address> addressList = await geocoder
                    .GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10); //max results
				// Get first
				Address address = addressList.FirstOrDefault();
				if (address != null)
				{
					//build address string
					StringBuilder deviceAddress = new StringBuilder();
					for (int i = 0; i < address.MaxAddressLineIndex; i++)
					{
						deviceAddress.Append(address.GetAddressLine(i))
                            .AppendLine(",");
					}
					log = deviceAddress.ToString();
				}
				else
					log = "Unable to determine the address.";
			}
		}

		//Rudimentary method of checking if location has changed enough to update map markers
		//TODO: tweak and improve
		/// <summary>
		/// Determines whether the current location is in same vicinity the specified new Location.
		/// </summary>
		/// <returns><c>true</c> if this instance is not in same vicinity  of the specified new Location; otherwise, <c>false</c>.</returns>
		/// <param name="newLocation">New location.</param>
		private bool IsNotSameVicinity(Location newLocation)
		{
			var MIN_DISTANCE = 0.1;  //(equates roughly to 100m)

			//guard condition, safest to return true
			if (newLocation == null || _currentLocation == null)
				return true;

			//get absolute (positive values) difference between lat/long values
			var latDiff = Math.Abs(newLocation.Latitude - _currentLocation.Latitude);
			var longDiff = Math.Abs(newLocation.Longitude - _currentLocation.Longitude);

			//check if change is big enough to warrant refreshing location markers
			if (latDiff > MIN_DISTANCE || longDiff > MIN_DISTANCE || (latDiff + longDiff) > MIN_DISTANCE)
				return true;
			//else
			return false;
		}

		/// <summary>
		/// *Location Client implementation method*
		/// Raises the location changed event.
		/// Updates markers
		/// </summary>
		/// <param name="location">Location.</param>
		public void OnLocationChanged(Location location)
		{
			if (_currentLocation == null || IsNotSameVicinity(location))
			{
				_currentLocation = location;
				UpdateMarkers();
			}
		}

		/// <summary>
		/// *Location Client implementation method*
		/// Raises the connected event.
		/// Centres map, sets markers and registers location update requests.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		public void OnConnected(Bundle bundle)
		{
			if (locClient.LastLocation != null)
			{
				_currentLocation = locClient.LastLocation;
				RecenterMap();
				UpdateMarkers();
			}
			// location request settings
			LocationRequest locRequest = new LocationRequest();
			// This priority checks if a location request has already been made recently by another application
			// if not then makes the request itself
			locRequest.SetPriority(LocationRequest.PriorityBalancedPowerAccuracy);
			locRequest.SetFastestInterval(500);  // minimum interval for receiving location updates
			locRequest.SetInterval(1000);          // request interval (in ms)

			locClient.RequestLocationUpdates(locRequest, this);
		}

		/// <summary>
		/// *Location Client implementation method*
		/// Raises the disconnected event.
		/// </summary>
		public void OnDisconnected()
		{
			//location client disconnected...let screen maintain current state...
		}

		/// <summary>
		/// *Location Client implementation method*
		/// Raises the connection failed event when trying to initialise the location client.
		/// In this case, attempts to initialise the map based on the last known location.
		/// </summary>
		/// <param name="p0">Connection result enum (error message)</param>
		void IGooglePlayServicesClientOnConnectionFailedListener.OnConnectionFailed(ConnectionResult p0)
		{
			// Get last known location if possible and use that to at least initiate the screen...
			if (locClient.LastLocation != null)
			{
				_currentLocation = locClient.LastLocation;
				RecenterMap();
				UpdateMarkers();
			}
		}
	}
}
