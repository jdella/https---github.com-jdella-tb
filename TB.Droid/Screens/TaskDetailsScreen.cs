using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskBuddi.BL;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using Android.Util;
using TaskBuddi.BL.Managers;
using Android.Nfc;
using Android.Database;

//using Android.Support.V4.Widget;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using TaskBuddi.DL;
using TaskBuddi.Droid.Adapters;
using Java.Lang;

namespace TaskBuddi.Droid.Screens
{
	[Activity(Label = "Task Details")]			
	public class TaskDetailsScreen : Activity
	{
		protected Task task = new Task();
		//Controls
		protected EditText vName;
		protected EditText vNotes;
		protected CheckBox vDone;
		protected Spinner tdAssignedTo;

		protected override void OnCreate(Bundle bundle)
		{
			// Init Screen
			base.OnCreate(bundle);
			// action bar
			RequestWindowFeature(WindowFeatures.ActionBar);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);
			// view/controls
			SetContentView(Resource.Layout.TaskDetails);
			vName = FindViewById<EditText>(Resource.Id.vName);
			vNotes = FindViewById<EditText>(Resource.Id.tdNotes);
			vDone = FindViewById<CheckBox>(Resource.Id.vDone);

			// Init TaskGroups spinner 
			var groupList = TaskGroupManager
                .GetTaskGroups()
                .OrderBy(g => g.Name)
                .ToList(); 
			tdAssignedTo = FindViewById<Spinner>(Resource.Id.tdAssignedTo);
			tdAssignedTo.Adapter = new GroupSpinnerAdapter(this, groupList);			

			//Load TaskGroup Details, if any
			var taskId = Intent.GetIntExtra("id", 0); //default
			if (taskId != 0)
			{
				//get Task
				task = TaskManager.GetTask(taskId);
				// Load task details
				vName.Text = task.Name;
				vNotes.Text = task.Notes;
				vDone.Visibility = task.Done ? 
                    ViewStates.Visible : ViewStates.Gone;
				// set spinner to current group
				var idArray = groupList.Select(g => g.ID).ToArray();
				tdAssignedTo.SetSelection(Array.IndexOf(idArray, task.GroupId));
			}
		}

		protected void Save()
		{
			task.Name = vName.Text;
			task.Notes = vNotes.Text;
			task.GroupId = (Int32)tdAssignedTo.SelectedItemId;
			task.Done = vDone.Checked;
			TaskManager.SaveTask(task);
		}

		protected void CancelDelete()
		{
			if (task.ID != 0)
			{
				TaskManager.DeleteTask(task.ID);
			}
		}

		// MENU METHODS
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_detailsscreen, menu);
			// Cancel if New
			menu.FindItem(Resource.Id.menu_delete)
                .SetTitle(task.ID == 0 ? "Cancel" : "Delete");  
			return true;
		}

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

				default: 
					break;
			}
			Finish();
			return true;
		}
	}
}