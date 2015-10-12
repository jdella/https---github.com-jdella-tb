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
	/// Group Details screen.
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
			var groupId = Intent.GetIntExtra("groupId", 0); // 0 = default value if none passed

			if (groupId != 0) //if existing Group
			{
				// get Group details and set to view
				group = TaskGroupManager.GetTaskGroup(groupId);	
				newGroupName.Text = group.Name;
               
				// get list of Group's assigned tasks
				GetGroupTasks();               
			}
		}

		/// <summary>
		/// Raises the resume event.
		/// Refreshes list of current Tasks assigned to Group.
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();    
			GetGroupTasks();
		}

		/// <summary>
		/// Gets the Group's assigned tasks and populates the list in the view.
		/// </summary>
		protected void  GetGroupTasks()
		{
			if (group != null)  //guard condition
			{
				var showNotes = true;
				tasks = TaskManager.GetTasksByGroup(group.ID);
				var lv = FindViewById<ListView>(Resource.Id.groupDetailTasksList);
				lv.Adapter = new DetailedTaskAdapter(this, tasks, showNotes);
			}
		}

		/// <summary>
		/// Save the Group as well as all Tasks to capture any changes made from this screen.
		/// (Namely changing Task's 'Completed' status.
		/// </summary>
		protected void Save()
		{
			// Guard condition for blank Group Names
			if (string.IsNullOrWhiteSpace(newGroupName.Text))
			{
				return;
			}
			// set Group Name from input and save
			group.Name = newGroupName.Text;
			TaskGroupManager.SaveTaskGroup(group);

			//if existing Group, save all assigned Tasks
			if (group.ID != 0)
			{
				foreach (var task in tasks)
				{
					TaskManager.SaveTask(task);
				}
			}
		}

		/// <summary>
		/// Delete the Group
		/// </summary>
		protected void CancelDelete()
		{
			//  Delete only if existing.  New Groups just exit screen
			if (group.ID != 0)
			{
				foreach (var task in tasks)
				{
					TaskManager.DeleteTask(task.ID);
				}
				TaskGroupManager.DeleteTaskGroup(group.ID);
			}
		}

		/// <param name="menu">The options menu in which you place your items.</param>
		/// <summary>
		/// Initialize the contents of the Activity's standard options menu.
		/// </summary>
		/// <returns>True</returns>
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			// Inflate menu layout for this screen
			MenuInflater.Inflate(Resource.Menu.menu_detailsscreen, menu);
			
			// set "Cancel"/"Delete" text depending of if Group is new/existing
			IMenuItem menuItem = menu.FindItem(Resource.Id.menu_delete);
			menuItem.SetTitle(group.ID == 0 ? "Cancel" : "Delete");
			return true;
		}

		/// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in the options menu is tapped.
		/// </summary>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.menu_save:
					Save();
					break;
				case Resource.Id.menu_delete:
					CancelDelete();
					break;
			}
			Finish();  // exit screen (end activity)
			return true;
		}

	}
}