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

//TODO refactor two group adapters?
namespace TaskBuddi.Droid.Adapters
{
	public class GroupSpinnerAdapter : BaseAdapter<TaskGroup>
	{
		protected Activity context = null;
		protected IList<TaskGroup> taskGroups = new List<TaskGroup>();

		public GroupSpinnerAdapter(Activity context, IList<TaskGroup> groups)
			: base()
		{
			this.context = context;
			//this.taskGroups = TaskGroupManager.GetTaskGroups();
			this.taskGroups = groups;
		}

		//# Set View Template
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view;
			var group = taskGroups[position];

			//Reuse inflated view if possible
			if (convertView == null)
				view = context.LayoutInflater.Inflate(Resource.Layout.TaskGroupListItem, null);
			else
				view = convertView;

			//#Set view model
			var vGroupName = view.FindViewById<TextView>(Resource.Id.vGroupName);
			vGroupName.Text = group.Name;

			//TODO move to xml?
			//view.FindViewById<RelativeLayout>(Resource.Id.vMain).SetPadding(20, 20, 20, 20);

			return view;
		}
            
		/* COMMON METHODS */
		//#get group
		public override TaskGroup this [int position]
		{
			get { return taskGroups[position]; }
		}
		//#get id
		public override long GetItemId(int position)
		{
			return taskGroups[position].ID;
			//return position;
		}
		//#get count
		public override int Count
		{
			get { return taskGroups.Count; }
		}

	}
}