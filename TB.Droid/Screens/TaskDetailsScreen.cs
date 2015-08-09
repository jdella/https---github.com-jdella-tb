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
		protected List<TaskGroup> groupList;
		protected string[] categoryValues;
		//Controls
		protected EditText vName;
		protected Spinner tdAssignedTo;
		protected Spinner vCategory;
		protected EditText vKeywords;
		protected EditText vNotes;
		protected CheckBox vDone;

		protected override void OnCreate(Bundle bundle)
		{
			// Init Screen
			base.OnCreate(bundle);
			InitViewControls();						
			categoryValues = Resources.GetStringArray(Resource.Array.locCategoryValues);
			
			//Load Task Details, if any
			var taskId = Intent.GetIntExtra("id", 0); //default
			if (taskId != 0)
			{
				LoadTask(taskId);
			}
			else
			{
				var groupId = Intent.GetIntExtra("groupId", 0); //default
				if (groupId != 0)
				{
					var idArray = groupList.Select(g => g.ID).ToArray();
					tdAssignedTo.SetSelection(Array.IndexOf(idArray, groupId));
				}
			}
		}

		private void InitViewControls()
		{
			// action bar
			RequestWindowFeature(WindowFeatures.ActionBar);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);

			// grab view controls
			SetContentView(Resource.Layout.TaskDetails);
			vName = FindViewById<EditText>(Resource.Id.vName);
			vCategory = FindViewById<Spinner>(Resource.Id.catSpinner);
			vKeywords = FindViewById<EditText>(Resource.Id.vKeywords);
			vNotes = FindViewById<EditText>(Resource.Id.tdNotes);
			vDone = FindViewById<CheckBox>(Resource.Id.vDone);
			// Init TaskGroups spinner 
			groupList = TaskGroupManager.GetTaskGroups().OrderBy(g => g.Name).ToList(); 
			tdAssignedTo = FindViewById<Spinner>(Resource.Id.tdAssignedTo);
			tdAssignedTo.Adapter = new GroupSpinnerAdapter(this, groupList);
		}

		private void LoadTask(int taskId)
		{
			//get Task
			task = TaskManager.GetTask(taskId);
			
			// Load task details
			vName.Text = task.Name;
			vNotes.Text = task.Notes;
			vKeywords.Text = task.Keywords;
			vCategory.SetSelection(Array.IndexOf(categoryValues, task.Category));

			var idArray = groupList.Select(g => g.ID).ToArray();
			tdAssignedTo.SetSelection(Array.IndexOf(idArray, task.GroupId));

			vDone.Checked = task.Done;
			//vDone.Visibility = task.Done ? ViewStates.Visible : ViewStates.Gone;
		}

		protected void Save()
		{
			task.Name = vName.Text;
			task.GroupId = (Int32)tdAssignedTo.SelectedItemId;
			task.Category = categoryValues[vCategory.SelectedItemPosition];
			task.Keywords = vKeywords.Text;
			task.Notes = vNotes.Text;
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