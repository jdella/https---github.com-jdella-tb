using System;
using Android.Locations;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TaskBuddi.BL.Managers;
using TaskBuddi.BL;
using System.Threading;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Views;

namespace TaskBuddi.Droid
{
	using Android.App;
	using Android.Gms.Maps;
	using Android.Gms.Maps.Model;
	using Android.OS;
	using Android.Widget;
	using System.Net.Http;

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

		protected override void OnCreate(Bundle bundle)
		{
			// Init view
			base.OnCreate(bundle);
			//action bar
			RequestWindowFeature(WindowFeatures.ActionBar);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);
			//map
			SetContentView(Resource.Layout.MapLayout);
			InitMapFragment();
			InitTaskCategoryArrays();
		}

		// ActionBar tap event
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			//Back button
			Finish();
			return true;
		}

		protected override void OnResume()
		{
			base.OnResume();
			//Connect to Location client
			locClient = new LocationClient(this, this, this);
			locClient.Connect();

			SetupMapIfNeeded();
		}

		protected override void OnPause()
		{
			base.OnPause();
			if (locClient.IsConnected)
			{
				locClient.RemoveLocationUpdates(this);
				locClient.Disconnect();
			}
		}

		private void InitTaskCategoryArrays()
		{
			tasks = TaskManager.GetTasks().ToList();
			taskCategories = tasks.Where(x => x.Done == false)
                .Select(x => x.Category)
                .Distinct().ToArray();
			taskCatCount = new int[taskCategories.Length];
			for (var i = 0; i < taskCategories.Length; i++)
			{
				taskCatCount[i] = tasks.Count(x => x.Category == taskCategories[i]);
			}
		}

		private void InitMapFragment()
		{
			// get fragment
			_mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (_mapFragment == null)
			{
				GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
					.InvokeZoomControlsEnabled(true)
					.InvokeCompassEnabled(true);
                
				// get map
				FragmentTransaction fragTx = FragmentManager.BeginTransaction();
				_mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.vMap, _mapFragment, "map");
				fragTx.Commit();
			}
		}

		private void SetupMapIfNeeded()
		{
			if (_map == null)
			{
				_map = _mapFragment.Map;
			}
			if (_map != null)
			{
				_map.SetInfoWindowAdapter(new MapInfoWindowAdapter(LayoutInflater));
				RecenterMap();
				UpdateMarkers();
			}
		}

		void UpdateMarkers()
		{
			//Guard condition
			if (_map == null || _currentLocation == null)
				return;

			//...Now update map/markers...
			_map.Clear();

			// Gets location address string
			// GetDeviceLocationAddress();
           
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
						var jdata = JObject.Parse(reader.ReadToEnd());
						var results = jdata["results"];
						for (var loc = 0; loc < 3 && loc < results.Count(); loc++)
						{                                    
							var locMarker = new MarkerOptions();
							var locPos = new LatLng(Convert.ToDouble(results[loc]["geometry"]["location"]["lat"]), 
								                                  Convert.ToDouble(results[loc]["geometry"]["location"]["lng"]));
							locMarker.SetPosition(locPos);

							//pluralise "Task" ? (for info window title)
							var task_s = taskCatCount[i] == 1 ? " task" : " tasks";
							locMarker.SetTitle(results[loc]["name"] + " (" + taskCatCount[i] + task_s + ")");
							
							//set list of matched Location-Category Tasks as info window body
							var snippet = "";
							foreach (var task in tasks.Where(x=>x.Category == taskCategories[i]))
							{
								snippet += "\n" + task.Name;
							}
							locMarker.SetSnippet(snippet);

							//set marker
							locMarker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));
							_map.AddMarker(locMarker);
						}
					}
				}
			}
		}

		private void RecenterMap()
		{
			if (_map != null && _currentLocation != null)
			{
				CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(
					                                        new LatLng(_currentLocation.Latitude, _currentLocation.Longitude), 15);
				_map.MoveCamera(cameraUpdate);
			}
		}
            
		// Gets address string for current location.  Only used for testing may be useful otherwise...
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

		//LOCATION CLIENT INTERFACE METHODS
		public void OnLocationChanged(Location location)
		{
			if (_currentLocation == null || IsNotSameVicinity(location))
			{
				_currentLocation = location;
				UpdateMarkers();
			}
		}

		//Rudimentary method of checking if location has changed enouhh to update map markers
		private bool IsNotSameVicinity(Location newLocation)
		{
			var MIN_DISTANCE = 0.1;  //(equates roughly to 100m)

			//guard condition, safest to return true
			if (newLocation == null || _currentLocation == null)
				return true;

			//get absolute differences between lat/long values
			var latDiff = Math.Abs(newLocation.Latitude - _currentLocation.Latitude);
			var longDiff = Math.Abs(newLocation.Longitude - _currentLocation.Longitude);

			//check if change is big enough to warrant refreshing location markers
			if (latDiff > MIN_DISTANCE || longDiff > MIN_DISTANCE || (latDiff + longDiff) > MIN_DISTANCE)
				return true;
			//else
			return false;
		}

		public void OnConnected(Bundle bundle)
		{
			if (locClient.LastLocation != null)
			{
				_currentLocation = locClient.LastLocation;
				RecenterMap();
				UpdateMarkers();
			}
			LocationRequest locRequest = new LocationRequest();
			locRequest.SetPriority(LocationRequest.PriorityBalancedPowerAccuracy);
			locRequest.SetFastestInterval(500);
			locRequest.SetInterval(1000);

			locClient.RequestLocationUpdates(locRequest, this);
		}

		public void OnDisconnected()
		{
			//location client disconnected...let screen maintain current state...
		}

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
