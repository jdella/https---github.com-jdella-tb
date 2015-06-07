using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using TaskBuddi.BL;
using TaskBuddi.BL.Managers;
using Android.Test.Suitebuilder;

namespace TaskBuddi.Droid.Screens
{
	/// <summary>
	/// Task group details screen.
	/// Form to edit/delete Task.
	/// </summary>
	[Activity(Label = "Task Group Details")]			
	public class TaskGroupDetailsScreen : Activity
	{
		protected TaskGroup group = new TaskGroup();
		//Controls
		protected EditText vName;

		protected override void OnCreate(Bundle bundle)
		{
			//Init Screen
			base.OnCreate(bundle);
			// action bar
			RequestWindowFeature(WindowFeatures.ActionBar);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);
			// view/controls
			SetContentView(Resource.Layout.TaskGroupDetails);
			vName = FindViewById<EditText>(Resource.Id.vName);  

			// Lookup TaskGroup ID passed from intent, if any
			var groupId = Intent.GetIntExtra("groupId", 0); //default
			if (groupId != 0)
			{
				group = TaskGroupManager.GetTaskGroup(groupId);		
				vName.Text = group.Name ?? "";
			}
		}

		//# Save
		protected void Save()
		{
			group.Name = vName.Text;
			TaskGroupManager.SaveTaskGroup(group);
			Finish();
		}
		//# Delete (or Cancel if new object)
		protected void Delete()
		{
			if (group.ID != 0)
			{
				TaskGroupManager.DeleteTaskGroup(group.ID);
			}
			Finish();
		}
		//# Menu
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_detailsscreen, menu);
			IMenuItem menuItem = menu.FindItem(Resource.Id.menu_delete);
			menuItem.SetTitle(group.ID == 0 ? "Cancel" : "Delete");

			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.menu_save:
					Save();
					return true;

				case Resource.Id.menu_delete:
					Delete();
					return true;

				default: 
					Finish();
					//return base.OnOptionsItemSelected(item);
					return true;
			}
		}

	}
}