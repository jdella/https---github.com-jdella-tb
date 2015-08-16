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
	public class TaskGroupListAdapter : BaseAdapter<TaskGroup>
	{
		protected Activity context = null;
		protected IList<TaskGroup> taskGroups = new List<TaskGroup>();
		protected IList<Task> tasks;
		protected LinearLayout vLayout;
		bool isGroupDetail = false;



		public TaskGroupListAdapter(Activity context, IList<TaskGroup> groups)
			: base()
		{
			//GET TASK GROUPS
			this.context = context;
			this.taskGroups = groups;
			//this.taskGroups = TaskGroupManager.GetTaskGroups();
		}

		//Constructor for Group Details Screen
		public TaskGroupListAdapter(Activity context, TaskGroup group, IList<Task> tasks)
			: base()
		{
			//GET TASK GROUPS
			this.context = context;
			this.taskGroups.Add(group);
			this.tasks = tasks;
			this.isGroupDetail = true;
		}
		
		//# INFLATE GROUP LIST VIEW
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			// Inflate layout
			var view = context.LayoutInflater.Inflate(Resource.Layout.TaskGroupListItem, null);
			vLayout = view.FindViewById<LinearLayout>(Resource.Id.vLayout);

			//#GET GROUP AND UI ELEMENTS
			var group = taskGroups[position];
			var groupId = group.ID;
			var vGroupName = view.FindViewById<TextView>(Resource.Id.vGroupName);
			var vGroupNameEdit = view.FindViewById<EditText>(Resource.Id.vGroupNameEdit);
			var groupClickable = view.FindViewById<FrameLayout>(Resource.Id.homeGroupClickableFrame);  
			var checkAll = view.FindViewById<ImageView>(Resource.Id.groupDetailCheckAll);
			
			//#SET VIEW MODEL			
			vGroupName.Text = group.Name;
			vGroupNameEdit.Text = group.Name;

			var groupColor = new Color(Resource.Color.tb_lightblue);
			groupClickable.SetBackgroundColor(groupColor);

			var inverseColor = new Color(255 - groupColor.R, 255 - groupColor.G, 255 - groupColor.B);
			vGroupName.SetTextColor(inverseColor);
			
			groupClickable.Click += (sender, e) =>
			{
				if (isGroupDetail)
				{
					//switch to edit
					vGroupName.Visibility = ViewStates.Gone;
					//var groupNameEdit = view.FindViewById<EditText>(Resource.Id.vGroupNameEdit);
					vGroupNameEdit.Visibility = ViewStates.Visible;
					vGroupNameEdit.Focusable = true;
					vGroupNameEdit.FocusableInTouchMode = true;
					vGroupNameEdit.RequestFocus();
					vGroupNameEdit.RequestFocusFromTouch();
				}
				else //go to Group Detail Screen
				{
					vGroupNameEdit.Focusable = false;
					vGroupNameEdit.FocusableInTouchMode = false;
					var taskGroupDetails = new Intent(context, typeof(TaskGroupDetailsScreen));
					taskGroupDetails.PutExtra("groupId", groupId);
					context.StartActivity(taskGroupDetails);
				}
			};

			//#GROUP DETAIL SCREEN SPECIFIC LOGIC
			if (isGroupDetail)
			{   
				var anyTasksNotDone = tasks.Any(x => x.Done == false);

				if (anyTasksNotDone)
					checkAll.SetImageResource(Resource.Drawable.ic_box_unticked);
				checkAll.Visibility = ViewStates.Visible;
				// if any Tasks incomplete, set all to Done, else set all to not Done
				checkAll.Click += (sender, e) =>
				{				
					foreach (var task in tasks)
					{
						task.Done = anyTasksNotDone;
					}
					//this.NotifyDataSetChanged();
				};                
				//Update Group Name (binding)
				//TODO: try MVVMCross instead of manual "binding"?
				vGroupNameEdit.AfterTextChanged += (sender, e) =>
				{
					group.Name = vGroupNameEdit.Text;
				};
			}

			//# INFLATE TASK LIST ITEMS
			InflateTaskItems(groupId);

			//ADD "NEW TASK" LINE
			AddNewTaskLine(groupId);

			return view;
		}

		private void InflateTaskItems(int groupId)
		{
			if (!isGroupDetail)
				tasks = TaskManager.GetTasksByGroup(groupId);

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
					taskName.SetTextAppearance(context, Resource.Style.homeTaskText_done);
					taskIcon.SetImageResource(Resource.Drawable.ic_box_ticked);
				}
				else
				{
					taskIcon.SetImageResource(Resource.Drawable.ic_box_unticked);
				}
				if (isGroupDetail && task.Notes != "")
				{
					var taskNotes = taskItem.FindViewById<TextView>(Resource.Id.taskListNotes);
					taskNotes.Text = task.Notes;
					taskNotes.Visibility = ViewStates.Visible;
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

		//# "ADD NEW TASK" LINE
		void AddNewTaskLine(int groupId)
		{
			var taskItem = context.LayoutInflater.Inflate(Resource.Layout.TaskListItem, null);
			var newTaskLine = taskItem.FindViewById<TextView>(Resource.Id.vName);
			newTaskLine.Text = "add new task";
			newTaskLine
                .SetTextAppearance(context, Resource.Style.homeTaskText_new);
			taskItem.FindViewById<ImageView>(Resource.Id.vCheck)
                .SetImageResource(Resource.Drawable.ic_plus_thick);

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