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

namespace TaskBuddi.Droid
{
	using Android.App;
	using Android.Gms.Maps;
	using Android.Gms.Maps.Model;
	using Android.OS;
	using Android.Widget;
	using System.Net.Http;

	[Activity(Label = "Task Map")]
	public class MapScreen : Activity, ILocationListener
	{
		protected GoogleMap _map;
		protected MapFragment _mapFragment;
		protected MarkerOptions marker;
		
		protected List<Task> tasks;
		protected string[] taskCategories;
		protected int[] taskCatCount;

		protected TextView vDebug;
		
		protected Location _currentLocation;
		protected LocationManager _locationManager;
		protected string _locationProvider;

		bool markersUpdated = false;

		//** TODO Bind to Background Location Service implmentation**//
		//protected LocationService _locationService;
		//protected IServiceConnection serviceConnection = new IServiceConnection();
		//bool _isBound;

		protected override void OnCreate(Bundle bundle)
		{
			// Init vieww
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.MapLayout);
			//vDebug = FindViewById<TextView>(Resource.Id.vDebug);

			InitMapFragment();
			InitLocationManager();
			InitTaskCategoryArrays();

			//** TODO Bind to Background Location Service implmentation**//
			//_locationManager = GetSystemService(Context.LocationService) as LocationManager;
			//StartService(new Intent(this, typeof(LocationService)));
			//var intent = new Intent(this, typeof(LocationService));
			//BindService(intent, _locationService, Bind.AutoCreate);
		}

		protected override void OnResume()
		{
			base.OnResume();
			//start location listeners
			if (!_locationProvider.Equals(string.Empty) && _locationProvider != null)
			{
				//_locationManager.RequestLocationUpdates(_locationProvider, 6000, 50, this);
				_locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 5000, 0, this);
			}
			SetupMapIfNeeded();
		}

		private void InitTaskCategoryArrays()
		{
			tasks = TaskManager.GetTasks().ToList();
			taskCategories = tasks.Select(x => x.Category).Distinct().ToArray();
			taskCatCount = new int[taskCategories.Length];
			for (var i = 0; i < taskCategories.Length; i++)
			{
				taskCatCount[i] = tasks.Count(x => x.Category == taskCategories[i]);
			}
		}

		protected void InitLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);

			// set accuracy for location requests
			Criteria criteria = new Criteria();
			criteria.Accuracy = Accuracy.Fine;

			//todo choose one method here
			// Get suitable provider from OS
			var best = _locationManager.GetBestProvider(criteria, false);
			IList<string> acceptableLocationProviders = _locationManager
                .GetProviders(criteria, false);
			if (acceptableLocationProviders.Any())
				_locationProvider = acceptableLocationProviders.First();
			else
				_locationProvider = string.Empty; //none found
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
				_map.SetInfoWindowAdapter(new MapInfoWindowAdapter(LayoutInflater));
				RecenterMap();
				UpdateMarker();
			}
		}
		//#############//
		// MAP UPDATES //
		//*************//
		void UpdateMarker()
		{
			if (_map != null && _currentLocation != null)
			{
				//todo debugging...
				markersUpdated = true;

				// outputs device location address to screen for debugging
				//GetDeviceLocationAddress();
				_map.Clear();

				// Current location marker
				marker = new MarkerOptions();
				var pos = new LatLng(_currentLocation.Latitude, _currentLocation.Longitude);
				marker.SetPosition(pos);
				marker.SetTitle("Current");
				marker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));
				_map.AddMarker(marker);

				//TODO - filter options? & set search constants? & implement keywords?
				// loop thru all active task categories
				const int RADIUS = 500;
				if (true)
				{
//					foreach (var t in tasks)
//					{       
//						ThreadPool.QueueUserWorkItem(o => GetTaskLocations(t));
//						//GetTaskLocations(t);						
//					}
					for (var i = 0; i < taskCategories.Length; i++)
					{                    
						var req = HttpWebRequest.Create("https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
							                            "location=" + _currentLocation.Latitude + "," + _currentLocation.Longitude +
							                            //"&radius=" + RADIUS + 
							                            "&rankby=distance" +
							                            "&types=" + taskCategories[i] +
							                            //"&keyword=" + tasks[i].Keywords +
							                            "&key=AIzaSyAoj2j9AtFF08nWRUdiN577shLpvPIhz-E");
						req.ContentType = "application/json";
						req.Method = "GET";
						using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
						{
							if (res.StatusCode != HttpStatusCode.OK)
								Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", res.StatusCode);
							using (var reader = new StreamReader(res.GetResponseStream()))
							{
								//var data = reader.ReadToEnd();
								//TODO - if (string.IsNullOrWhiteSpace(data)) null check ?
								var jdata = JObject.Parse(reader.ReadToEnd());
								var results = jdata["results"];
								for (var l = 0; l < 3 && l < results.Count(); l++) //jdata["results"])
								{                                    
									var locMarker = new MarkerOptions();
									var lpos = new LatLng(Convert.ToDouble(results[l]["geometry"]["location"]["lat"]), 
										                                      Convert.ToDouble(results[l]["geometry"]["location"]["lng"]));
									locMarker.SetPosition(lpos);
									var task_s = taskCatCount[i] == 1 ? " task" : " tasks";
									locMarker.SetTitle(results[l]["name"] + " (" + taskCatCount[i] + task_s + ")");
									var snippet = ""; //"Category: " + taskCategories[i];
									foreach (var task in tasks.Where(x=>x.Category == taskCategories[i]))
									{
										snippet += "\n" + task.Name;
									}
									locMarker.SetSnippet(snippet);
									//TODO colour enum / set different colours on markers...
									//eg by Group / Priority / No. of Tasks / Category - make this an option?
									locMarker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));
									_map.AddMarker(locMarker);
								}
							}
						}
					}
				}
			}
		}

		async void GetTaskLocations(Task t)
		{
			var req = HttpWebRequest.Create("https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
				                   "location=" + _currentLocation.Latitude + "," + _currentLocation.Longitude +
                //"&radius=" + RADIUS + 
				                   "&rankby=distance" +
				                   "&types=" + t.Category +
				                   "&keyword=" + t.Keywords +
				                   "&key=AIzaSyAoj2j9AtFF08nWRUdiN577shLpvPIhz-E");
			req.ContentType = "application/json";
			req.Method = "GET";
			using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
			{
				if (res.StatusCode != HttpStatusCode.OK)
					Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", res.StatusCode);
				using (var reader = new StreamReader(res.GetResponseStream()))
				{
					//var data = reader.ReadToEnd();
					//TODO - if (string.IsNullOrWhiteSpace(data)) null check ?
					var jdata = JObject.Parse(reader.ReadToEnd());
					var results = jdata["results"];
					for (var l = 0; l < 3; l++) //jdata["results"])
					{
						var locMarker = new MarkerOptions();
						var lpos = new LatLng(Convert.ToDouble(results[l]["geometry"]["location"]["lat"]), 
							                             Convert.ToDouble(results[l]["geometry"]["location"]["lng"]));
						locMarker.SetPosition(lpos);
						locMarker.SetTitle(results[l]["name"].ToString());// + " (" + taskCatCount[i] + ")");
						var BR = System.Environment.NewLine;
						var snippet = "Category: " + t.Category + BR + t.Name + BR;
						//                                    foreach (var task in tasks.Where(x=>x.Category == taskCategories[i]))
						//                                    {
						//                                        snippet += "\n- " + task.Name;
						//                                    }
						locMarker.SetSnippet(snippet);
						//TODO colour enum / set different colours on markers...
						//eg by Group / Priority / No. of Tasks / Category - make this an option?
						locMarker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));
						_map.AddMarker(locMarker);
					}
				}
			}
		}

		//TODO only recentre on init?? and have button to manually center on location...
		private void RecenterMap()
		{
			if (_map != null && _currentLocation != null)
			{
				CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(
					                                        new LatLng(_currentLocation.Latitude, _currentLocation.Longitude), 15);
				_map.MoveCamera(cameraUpdate);
			}
		}
            
		// outputs device location address to screen for debugging
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
			// log result to screen
			vDebug.Text = System.DateTime.Now.TimeOfDay + " : " + log;
		}

		//LOCATION PROVIDER EVENTS
		//TODO need qualifier here for when to update markers - distance constraint
		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;
			if (_currentLocation != null)
			{
				RecenterMap();
				if (!markersUpdated)
					UpdateMarker();
			}
		}

		public void OnProviderDisabled(string provider)
		{
			
		}

		public void OnProviderEnabled(string provider)
		{
			
		}

		/// <param name="provider">the name of the location provider associated with this
		///  update.</param>
		/// <summary>
		/// Raises the status changed event.
		/// </summary>
		/// <param name="status">Status.</param>
		/// <param name="extras">Extras.</param>
		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
            
		}
	}
}
