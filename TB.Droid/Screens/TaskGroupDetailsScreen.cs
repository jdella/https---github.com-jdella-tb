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
using TaskBuddi.Droid.Adapters;
using System.Collections.Generic;

namespace TaskBuddi.Droid.Screens
{
	/// <summary>
	/// Task group details screen. Form to edit/delete Task.
	/// </summary>
	[Activity(Label = "Task Group Details")]			
	public class TaskGroupDetailsScreen : Activity
	{
		protected TaskGroup group = new TaskGroup();
		IList<Task> tasks;
		protected EditText newGroupName;

		/// <summary>
		/// Raises the create event. Sets action bar and view
		/// </summary>
		/// <param name="bundle">Bundle.</param>
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
			newGroupName = FindViewById<EditText>(Resource.Id.newGroupName);

			// Lookup TaskGroup ID passed from intent, if any
			var groupId = Intent.GetIntExtra("groupId", 0); //default
			if (groupId != 0) //existing
			{
				group = TaskGroupManager.GetTaskGroup(groupId);		
				tasks = TaskManager.GetTasksByGroup(groupId);

				newGroupName.Text = group.Name;

				var lv = FindViewById<ListView>(Resource.Id.groupDetailTasksList);
				//lv.Adapter = new TaskGroupListAdapter(this, group, tasks);
				lv.Adapter = new DetailedTaskAdapter(this, tasks, true);
			}
			else //new group
			{
				FindViewById<RelativeLayout>(Resource.Id.newGroupView)
                    .Visibility = ViewStates.Visible;
				
			}
		}

		/// <summary>
		/// Save this instance.
		/// </summary>
		protected void Save()
		{
			group.Name = newGroupName.Text;

			//EXISTING GROUP AND NAME NOT EMPTY
			if (group.ID != 0 && group.Name != "")
			{
				//group.Name = vName.Text;
				TaskGroupManager.SaveTaskGroup(group);
				foreach (var task in tasks)
				{
					TaskManager.SaveTask(task);
				}
			}
			// NEW GROUP AND NAME NOT EMPTY
			if (group.ID == 0 && newGroupName.Text != "")
			{
				//group.Name = newGroupName.Text;
				TaskGroupManager.SaveTaskGroup(group);
			}
			Finish();
		}

		/// <summary>
		/// Delete this instance.
		/// </summary>
		protected void Delete()
		{
			if (group.ID != 0)
			{
				foreach (var task in tasks)
				{
					TaskManager.DeleteTask(task.ID);
				}
				TaskGroupManager.DeleteTaskGroup(group.ID);
			}
			Finish();
		}

		/// <param name="menu">The options menu in which you place your items.</param>
		/// <summary>
		/// Initialize the contents of the Activity's standard options menu.
		/// </summary>
		/// <returns>To be added.</returns>
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_detailsscreen, menu);
			IMenuItem menuItem = menu.FindItem(Resource.Id.menu_delete);
			menuItem.SetTitle(group.ID == 0 ? "Cancel" : "Delete");

			return true;
		}

		/// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <returns>To be added.</returns>
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