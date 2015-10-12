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
	///  Adapter to populate Task list items (used for both Homw Screen and Group Details Task Lists)
	/// </summary>
	public class DetailedTaskAdapter : BaseAdapter<Task>
	{
		protected Activity context = null;
		protected ListView taskListView;

		protected bool isGroupDetail = false;
		protected IList<Task> tasks = new List<Task>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskBuddi.Droid.Adapters.DetailedTaskAdapter"/> class.
		/// Populates Task list views.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="tasks">List of Group's Tasks.</param>
		/// <param name="isGroupDetail">If set to <c>true</c> is group detail.</param>
		public DetailedTaskAdapter(Activity context, IList<Task> tasks, bool isGroupDetail)
		{
			this.context = context;
			this.tasks = tasks;
			this.isGroupDetail = isGroupDetail;
		}

		/// <summary>
		/// *Adapter implementation method*
		/// Adapter method to inflate the view for each item in the list provided to the adapter.
		/// </summary>
		/// <returns>The inflated view.</returns>
		/// <param name="position">The position of the item within the adapter's data set</param>
		/// <param name="convertView">Recyclable view</param>
		/// <param name="parent">Parent viewgroup</param>
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			//Inflate view and get Task
			View view = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
			var task = tasks[position];

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			//NOT USED AS CAUSES CONFLICT BETWEEN ITEMS - ONLY GOOD FOR DISPLAYING STATIC LISTS
			//			if (convertView == null)
			//				view = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
			//			else
			//				view = convertView;

			// get view elements...
			var taskName = view.FindViewById<TextView>(Resource.Id.vName);
			var taskIcon = view.FindViewById<ImageView>(Resource.Id.vCheck);
			//...and populate with current Task details
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

			// if populating Group Details screen, then also display Task Notes if any... 
			if (isGroupDetail && task.Notes != "")
			{
				var taskNotes = view.FindViewById<TextView>(Resource.Id.taskListNotes);
				taskNotes.Text = task.Notes;
				taskNotes.Visibility = ViewStates.Visible;
			}

			// Set Click Event listeners on each Task item (to go to Details screen on tap)
			view.Click += (sender, e) =>
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
					taskName.SetTextAppearance(context, Resource.Style.homeTaskText);					
					taskIcon.SetImageResource(Resource.Drawable.ic_box_unticked);
				}
			};
			return view;                
		}

		/// <summary>
		/// *Adapter implmenentation method*
		/// Gets the <see cref="TaskBuddi.BL.Task"/> with the specified position in the adapter's data set.
		/// </summary>
		/// <param name="position">Task item Position (index)</param>
		public override Task this [int position]
		{
			get { return tasks[position]; }
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
		/// Returns how many Task items are in the data set represented by this Adapter.
		/// </summary>
		/// <value>Task count</value>
		public override int Count
		{
			get { return tasks.Count; }
		}

	}
}