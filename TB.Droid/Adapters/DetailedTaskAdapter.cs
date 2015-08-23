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
	//Adapter to populate Group GridView on Home Screen
	public class DetailedTaskAdapter : BaseAdapter<Task>
	{
		protected Activity context = null;
		protected ListView taskListView;

		protected bool isGroupDetail = false;
		protected IList<Task> tasks = new List<Task>();

		public DetailedTaskAdapter(Activity context, IList<Task> tasks, bool isGroupDetail)
		{
			this.context = context;
			this.tasks = tasks;
			this.isGroupDetail = isGroupDetail;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
			var task = tasks[position];

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
//			if (convertView == null)
//				view = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
//			else
//				view = convertView;
            
			var taskName = view.FindViewById<TextView>(Resource.Id.vName);
			var taskIcon = view.FindViewById<ImageView>(Resource.Id.vCheck);

			taskName.Text = task.Name;
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
			if (isGroupDetail && task.Notes != "")
			{
				var taskNotes = view.FindViewById<TextView>(Resource.Id.taskListNotes);
				taskNotes.Text = task.Notes;
				taskNotes.Visibility = ViewStates.Visible;
			}

			// Click Task item -> Go to Task Details
			view.Click += (sender, e) =>
			{
				var showDetails = new Intent(context, typeof(TaskDetailsScreen));
				showDetails.PutExtra("id", task.ID);
				context.StartActivity(showDetails);
			};
            
			// CLICK "TASK BOX" ICON TO COMPLETE TASK
			taskIcon.Click += (sender, e) =>
			{
				task.Done = !task.Done;
				TaskManager.SaveTask(task);
				if (task.Done)
				{
					taskName.Paint.Flags = PaintFlags.StrikeThruText;
					taskIcon.SetImageResource(Resource.Drawable.ic_box_ticked);
				}
				else
				{
					taskName.Paint.Flags = 0;
					taskIcon.SetImageResource(Resource.Drawable.ic_box_unticked);
				}
			};
			return view;                
		}

		/* COMMON METHODS */
		//#get Task
		public override Task this [int position]
		{
			get { return tasks[position]; }
		}
		//#get ID
		public override long GetItemId(int position)
		{
			return position;
		}
		//#get Count
		public override int Count
		{
			get { return tasks.Count; }
		}

	}
}