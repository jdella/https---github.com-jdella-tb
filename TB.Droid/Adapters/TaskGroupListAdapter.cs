using System;
using System.Collections.Generic;
using Android.Widget;
using TaskBuddi.BL;
using TaskBuddi.BL.Managers;
using Android.App;
using Android;
using Android.Views;
using Android.Content;
using TaskBuddi.Droid.Screens;
using Android.Graphics;
using System.Linq;

namespace TaskBuddi.Droid.Adapters
{

	/// <summary>
	//Adapter to populate the Home Screen view.
	/// </summary>
	public class TaskGroupListAdapter : BaseAdapter<TaskGroup>
	{
		protected Activity context = null;
		protected IList<TaskGroup> taskGroups = new List<TaskGroup>();
		protected IList<Task> tasks;
		protected LinearLayout vLayout;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskBuddi.Droid.Adapters.TaskGroupListAdapter"/> class.
		/// Populates the Home Screen view.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="groups">List of Task Groups.</param>
		public TaskGroupListAdapter(Activity context, IList<TaskGroup> groups)
		{
			this.context = context;
			this.taskGroups = groups;
		}

		/// <summary>
		/// *Adapter implementation method*
		/// Adapter method to inflate the view for each Group.
		/// </summary>
		/// <returns>The inflated view.</returns>
		/// <param name="position">The position of the item within the adapter's data set</param>
		/// <param name="convertView">Recyclable view</param>
		/// <param name="parent">Parent viewgroup</param>
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			// Inflate layout
			var view = context.LayoutInflater.Inflate(Resource.Layout.TaskGroupListItem, null);
			vLayout = view.FindViewById<LinearLayout>(Resource.Id.vLayout);

			// Get current group and id
			var group = taskGroups[position];
			var groupId = group.ID;

			// Get view elements....
			var vGroupName = view.FindViewById<TextView>(Resource.Id.vGroupName);
			var groupClickable = view.FindViewById<FrameLayout>(Resource.Id.homeGroupClickableFrame);  
			var checkAll = view.FindViewById<ImageView>(Resource.Id.groupDetailCheckAll);
			
			// set Group name...
			vGroupName.Text = group.Name;

			// set background colour and calculates the inverse for the text... ensures high contrast
			// this will be useful if custom colors are implemented
			var groupColor = new Color(Resource.Color.tb_lightblue);
			groupClickable.SetBackgroundColor(groupColor);
			var inverseColor = new Color(255 - groupColor.R, 255 - groupColor.G, 255 - groupColor.B);
			vGroupName.SetTextColor(inverseColor);

			// Set Click Event listeners on Group's clickable region (name) to go to Group Details screen
			groupClickable.Click += (sender, e) =>
			{
				var taskGroupDetails = new Intent(context, typeof(TaskGroupDetailsScreen));
				taskGroupDetails.PutExtra("groupId", groupId);
				context.StartActivity(taskGroupDetails);
			};

			// Inflate Task list item views
			InflateTaskItems(groupId);
			//TODO: Modify DetailedTaskAdapter for use here aswell instead of inflating views manually...
			//          tasks = TaskManager.GetTasksByGroup(groupId);
			//          var lv = view.FindViewById<ListView>(Resource.Id.homeGroupTaskListView);
			//          lv.Adapter = new DetailedTaskAdapter(context, tasks, true);

			// add "new task" line..
			AddNewTaskLine(groupId);

			return view;
		}

		/// <summary>
		/// Inflates the Group's Task list items
		/// </summary>
		/// <param name="groupId">Group ID</param>
		private void InflateTaskItems(int groupId)
		{
			// get the Group's Tasks
			tasks = TaskManager.GetTasksByGroup(groupId);

			//Inflate view for each Task
			foreach (var task in tasks)
			{
				View taskItem;
				TextView taskName;
				ImageView taskIcon;

				// get view elements...
				taskItem = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
				taskName = taskItem.FindViewById<TextView>(Resource.Id.vName);
				taskIcon = taskItem.FindViewById<ImageView>(Resource.Id.vCheck);
				// ...and populate with Task details
				taskName.Text = task.Name;
				// set Task font/styling based on status
				if (task.Done)
				{
					taskName.Paint.Flags = PaintFlags.StrikeThruText;
					taskName.SetTextAppearance(context, Resource.Style.homeTaskText_done);
					taskIcon.SetImageResource(Resource.Drawable.ic_box_ticked);
				}
				else
				{
					taskIcon.SetImageResource(Resource.Drawable.ic_box_unticked);
				}

				// Set Click Event listeners on each Task item (to go to Details screen on tap)
				taskItem.Click += (sender, e) =>
				{
					var showDetails = new Intent(context, typeof(TaskDetailsScreen));
					showDetails.PutExtra("id", task.ID);
					context.StartActivity(showDetails);
				};
				// Set Click Event listeners on the Task item's 'completed checkbox' ...
				taskIcon.Click += (sender, e) =>
				{
					// toggle done status and save immediately
					task.Done = !task.Done;
					TaskManager.SaveTask(task);
					// set font/display styling to indicate updated status
					if (task.Done)
					{
						taskName.Paint.Flags = PaintFlags.StrikeThruText;
						taskIcon.SetImageResource(Resource.Drawable.ic_box_ticked);
					}
					else
					{
						taskName.Paint.Flags = 0;  //remove strikethrough
						taskName.SetTextAppearance(context, Resource.Style.homeTaskText);       
						taskIcon.SetImageResource(Resource.Drawable.ic_box_unticked);
					}
				};
				vLayout.AddView(taskItem);
			}
		}

		/// <summary>
		/// Adds the new task line.
		/// </summary>
		/// <param name="groupId">Group ID.</param>
		void AddNewTaskLine(int groupId)
		{
			// inflate Task item view and set text
			var taskItem = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
			var newTaskLine = taskItem.FindViewById<TextView>(Resource.Id.vName);
			newTaskLine.Text = "add new task";

			// set custom styling
			newTaskLine
                .SetTextAppearance(context, Resource.Style.homeTaskText_new);
			taskItem.FindViewById<ImageView>(Resource.Id.vCheck)
                .SetImageResource(Resource.Drawable.ic_plus_thick);

			// Set Click Event listeners to add new task		
			taskItem.Click += (sender, e) =>
			{
				var showDetails = new Intent(context, typeof(TaskDetailsScreen));
				showDetails.PutExtra("groupId", groupId);
				context.StartActivity(showDetails);
			};
			//add to end of Group task list
			vLayout.AddView(taskItem); 
		}

		/// <summary>
		/// *Adapter implmenentation method*
		/// Gets the <see cref="TaskBuddi.BL.TaskGroup"/> with the specified position in the adapter's data set.
		/// </summary>
		/// <param name="position">Item Position (index)</param>
		public override TaskGroup this [int position]
		{
			get { return taskGroups[position]; }
		}

		/// <param name="position">The position of the item within the adapter's data set whose row id we want.</param>
		/// <summary>
		/// *Adapter implmenentation method*
		/// Get the row id associated with the specified position in the list.
		/// </summary>
		/// <returns>Position</returns>
		public override long GetItemId(int position)
		{
			return position;
		}

		/// <summary>
		/// *Adapter implmenentation method*
		/// Returns how many Groups are in the data set represented by this Adapter.
		/// </summary>
		/// <value>Group count</value>
		public override int Count
		{
			get { return taskGroups.Count; }
		}

	}
}