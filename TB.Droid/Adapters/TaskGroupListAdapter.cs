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

namespace TaskBuddi.Droid.Adapters
{
	//Adapter to populate Group GridView on Home Screen
	public class TaskGroupListAdapter : BaseAdapter<TaskGroup>
	{
		protected Activity context = null;
		protected IList<TaskGroup> taskGroups = new List<TaskGroup>();
		protected LinearLayout vLayout;

		public TaskGroupListAdapter(Activity context)
			: base()
		{
			//GET TASK GROUPS
			this.context = context;
			this.taskGroups = TaskGroupManager.GetTaskGroups();
		}
		
		//# INFLATE GROUP LIST VIEW
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			// Inflate layout
			var view = context.LayoutInflater.Inflate(Resource.Layout.TaskGroupListItem, null);
			vLayout = view.FindViewById<LinearLayout>(Resource.Id.vLayout);

			//#Set Group Name and Clicks
			var group = taskGroups[position];
			var vGroupName = view.FindViewById<TextView>(Resource.Id.vGroupName);
			var groupClickable = view.FindViewById<FrameLayout>(Resource.Id.homeGroupClickableFrame);   
			
			var groupId = group.ID;			
			vGroupName.Text = group.Name;

			var groupColor = new Color(Resource.Color.tb_lightblue);
			groupClickable.SetBackgroundColor(groupColor);

			var inverseColor = new Color(255 - groupColor.R, 255 - groupColor.G, 255 - groupColor.B);
			vGroupName.SetTextColor(inverseColor);
			
			groupClickable.Click += (sender, e) =>
			{
				var taskGroupDetails = new Intent(context, typeof(TaskGroupDetailsScreen));
				taskGroupDetails.PutExtra("groupId", groupId);
				context.StartActivity(taskGroupDetails);
			};
            
			//# INFLATE TASK LIST ITEMS
			InflateTaskItems(groupId);

			//ADD "NEW TASK" LINE
			AddNewTaskLine(groupId);

			return view;
		}

		private void InflateTaskItems(int groupId)
		{
			var tasks = TaskManager.GetTasksByGroup(groupId);
			foreach (var task in tasks)
			{
				View taskItem;
				TextView taskName;
				ImageView taskIcon;

				// Inflate layout and bind data
				taskItem = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
				taskName = taskItem.FindViewById<TextView>(Resource.Id.vName);
				taskIcon = taskItem.FindViewById<ImageView>(Resource.Id.vCheck);

				taskName.Text = task.Name;
				if (task.Done)
				{
					taskName.Paint.Flags = PaintFlags.StrikeThruText;
					taskIcon.SetImageResource(Resource.Drawable.ic_box_ticked);
				}
				else
				{
					taskIcon.SetImageResource(Resource.Drawable.ic_box_unticked);
				}

				//#Click Task item -> Go to Task Details
				taskItem.Click += (sender, e) =>
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
				vLayout.AddView(taskItem);
			}
		}

		void AddNewTaskLine(int groupId)
		{
			var taskItem = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
			var newTaskText = taskItem.FindViewById<TextView>(Resource.Id.vName);
			newTaskText.Text = "add new task";
			newTaskText.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
			newTaskText.SetTextColor(new Color(Resource.Color.shadeygrey));
			taskItem.FindViewById<ImageView>(Resource.Id.vCheck).SetImageResource(Resource.Drawable.ic_plus_thick);

			//#Click Task item -> Task Details
			taskItem.Click += (sender, e) =>
			{
				var showDetails = new Intent(context, typeof(TaskDetailsScreen));
				showDetails.PutExtra("groupId", groupId);
				context.StartActivity(showDetails);
			};
			vLayout.AddView(taskItem); 
		}

		/* ADAPTER OVERRIDE METHODS */
		//#get group
		public override TaskGroup this [int position]
		{
			get { return taskGroups[position]; }
		}
		//#get id
		public override long GetItemId(int position)
		{
			return position;
		}
		//#get count
		public override int Count
		{
			get { return taskGroups.Count; }
		}

	}
}