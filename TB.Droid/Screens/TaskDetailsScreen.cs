using System;
using System.Collections.Generic;
using System.Linq;
using TaskBuddi.BL;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Views;
using TaskBuddi.BL.Managers;
using TaskBuddi.Droid.Adapters;


namespace TaskBuddi.Droid.Screens
{
	/// <summary>
	/// Task details screen.  Used for editing exisitng and new Tasks.
	/// </summary>
	[Activity(Label = "Task Details")]			
	public class TaskDetailsScreen : Activity
	{
		protected Task task = new Task();
		protected List<TaskGroup> groupList;
		protected string[] categoryValues;
		
		// View elements
		protected EditText vName;
		protected Spinner tdAssignedTo;
		protected Spinner vCategory;
		protected EditText vNotes;
		//protected EditText vKeywords;  //disabled

		/// <summary>
		/// Raises the create event.  
		/// Initialises view.  Handles both existing and new Tasks.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{
			// Init view
			base.OnCreate(bundle);
			InitViewControls();	

			// get list of task/location categories
			categoryValues = Resources.GetStringArray(Resource.Array.locCategoryValues);
			
			//Load Task Details, if existing task
			var taskId = Intent.GetIntExtra("id", 0); // 0 = default value if no id passed
			if (taskId != 0)
			{
				LoadTask(taskId);
			}
			else // if being passed 'groupId', it is a new Task for that group...
			{
				var groupId = Intent.GetIntExtra("groupId", 0); // 0 = default value if no id passed
				if (groupId != 0)
				{  //... so set the Group selector (spinner) to that Group 
					var idArray = groupList.Select(g => g.ID).ToArray();
					tdAssignedTo.SetSelection(Array.IndexOf(idArray, groupId));
				}
			}
		}

		/// <summary>
		/// Initialises the view.
		/// </summary>
		private void InitViewControls()
		{
			// action bar
			RequestWindowFeature(WindowFeatures.ActionBar);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);

			// get view elements
			SetContentView(Resource.Layout.TaskDetails);
			vName = FindViewById<EditText>(Resource.Id.vName);
			vCategory = FindViewById<Spinner>(Resource.Id.catSpinner);
			vNotes = FindViewById<EditText>(Resource.Id.tdNotes);
			//vKeywords = FindViewById<EditText>(Resource.Id.vKeywords);  //disabled
      
			// populate Group selector (spinner)
			groupList = TaskGroupManager.GetTaskGroups().OrderBy(g => g.Name).ToList(); 
			tdAssignedTo = FindViewById<Spinner>(Resource.Id.tdAssignedTo);
			tdAssignedTo.Adapter = new GroupSpinnerAdapter(this, groupList);
		}

		/// <summary>
		/// Loads the task details.
		/// </summary>
		/// <param name="taskId">Task ID</param>
		private void LoadTask(int taskId)
		{
			//get Task
			task = TaskManager.GetTask(taskId);
			
			// Load task details
			vName.Text = task.Name;
			vNotes.Text = task.Notes;
			vCategory.SetSelection(Array.IndexOf(categoryValues, task.Category));  //set category spinner
			//vKeywords.Text = task.Keywords;  // disabled

			// sets Group spinner to Task's current Group
			var idArray = groupList.Select(g => g.ID).ToArray();
			tdAssignedTo.SetSelection(Array.IndexOf(idArray, task.GroupId));
		}

		/// <summary>
		/// Save the Task.
		/// </summary>
		protected void Save()
		{
			// set Task values from inputs
			task.Name = vName.Text;
			task.GroupId = (Int32)tdAssignedTo.SelectedItemId;
			task.Category = categoryValues[vCategory.SelectedItemPosition];
			//task.Keywords = vKeywords.Text;  //disabled
			task.Notes = vNotes.Text;
			//save
			TaskManager.SaveTask(task);
		}

		/// <summary>
		/// Handles 'Delete' action.  If new Task, then just cancels.
		/// </summary>
		protected void CancelDelete()
		{
			if (task.ID != 0)
			{
				TaskManager.DeleteTask(task.ID);
			}
		}

		/// <param name="menu">The options menu in which you place your items.</param>
		/// <summary>
		/// Initialize the contents of the Activity's standard options menu.
		/// </summary>
		/// <returns>True</returns>
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_detailsscreen, menu);
			
			// sets Cancel/Delete text depending if Task is new/existing
			menu.FindItem(Resource.Id.menu_delete)
                .SetTitle(task.ID == 0 ? "Cancel" : "Delete");  
			return true;
		}

		/// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <returns>True (and exits screen)</returns>
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