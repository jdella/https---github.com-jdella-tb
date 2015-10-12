using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using TaskBuddi.BL;
using Android.Views;
using Android.Widget;
using TaskBuddi.Droid.Screens;
using TaskBuddi.Droid.Adapters;
using Android.Locations;
using System.Linq;
using System.Text;
using System;
using TaskBuddi.BL.Managers;

namespace TaskBuddi.Droid.Screens
{
	/// <summary>
	/// Home screen.  Main screen of the application
	/// </summary>
	[Activity(Label = "TaskBuddi", MainLauncher = true)]			
	public class HomeScreen : Activity
	{
		protected GridView vGroupGrid;
		protected TextView vDebug;
		protected ListView listView1;

		/// <summary>
		/// Raises the create event.  Sets view layout
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{       
			// ACTION BAR & MAIN SCREEN LAYOUT
			base.OnCreate(bundle);
			RequestWindowFeature(WindowFeatures.ActionBar);
			SetContentView(Resource.Layout.HomeScreen);

			//get Task Group List view Element
			listView1 = FindViewById<ListView>(Resource.Id.listView1);
		}

		/// <summary>
		/// Raises the resume event.  Populate Task Groups
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();  
			//Get groups and populate Task Group lists
			var taskGroups = TaskGroupManager.GetTaskGroups();
			listView1.Adapter = new TaskGroupListAdapter(this, taskGroups); 

		}

		/// <param name="menu">Layout for menu</param>
		/// <summary>
		/// Initialize the contents of the Activity's standard options menu.
		/// </summary>
		/// <returns>True</returns>
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_homescreen, menu);
			return true;
		}

		/// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <returns>True/false for success/fail</returns>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
			// add group
				case Resource.Id.menu_add_taskGroup:
					StartActivity(typeof(TaskGroupDetailsScreen));
					return true;
			// show map
				case Resource.Id.menu_show_map:
					StartActivity(typeof(MapScreen));
					return true;

				default:
					return base.OnOptionsItemSelected(item);
			}

		}
	}
}