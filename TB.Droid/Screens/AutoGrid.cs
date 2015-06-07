
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TaskBuddi.Droid
{
	[Register("taskbuddi.droid.AutoGrid")] 
	public class AutoGrid : GridView
	{

		private string TAG = "AutoGridView";
		private int numColumnsID;
		private int previousFirstVisible;
		private int numColumns = 1;

		public AutoGrid(Context context)
			: base(context)
		{
			//base(context);
		}

		public AutoGrid(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			Initialize(attrs);
		}

		public AutoGrid(Context context, IAttributeSet attrs, int defStyle)
			: base(context, attrs, defStyle)
		{
			Initialize(attrs);
		}

		/**
     * Sets the numColumns based on the attributeset
     */
		private void Initialize(IAttributeSet attrs)
		{
			// Read numColumns out of the AttributeSet
			int count = attrs.AttributeCount;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					var name = attrs.GetAttributeName(i);
					if (name != null && name.Equals("numColumns"))
					{
						// Update columns
						this.numColumnsID = attrs.GetAttributeResourceValue(i, 1);
						UpdateColumns();
						break;
					}
				}
			}
		}

		/**
     * Reads the amount of columns from the resource file and
     * updates the "numColumns" variable
     */
		private void UpdateColumns()
		{
			this.numColumns = Context.Resources.GetInteger(numColumnsID);
		}

		// @Override
		public override void SetNumColumns(int numColumns)
		{
			this.numColumns = numColumns;
			base.SetNumColumns(numColumns);
			SetSelection(previousFirstVisible);
		}

		//@Override
		protected override void OnLayout(bool changed, int leftPos, int topPos, int rightPos, int bottomPos)
		{
			base.OnLayout(changed, leftPos, topPos, rightPos, bottomPos);
			SetHeights();    
		}

		// @Override
		protected override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			UpdateColumns();
			SetNumColumns(this.numColumns);
		}

		// @Override
		protected override void OnScrollChanged(int newHorizontal, int newVertical, int oldHorizontal, int oldVertical)
		{
			// Check if the first visible position has changed due to this scroll
			int firstVisible = FirstVisiblePosition;
			if (previousFirstVisible != firstVisible)
			{
				// Update position, and update heights
				previousFirstVisible = firstVisible;
				SetHeights();
			}

			base.OnScrollChanged(newHorizontal, newVertical, oldHorizontal, oldVertical);
		}

		/**
     * Sets the height of each view in a row equal to the height of the tallest view in this row.
     * @param firstVisible The first visible position (adapter order)
     */
		private void SetHeights()
		{
			IAdapter adapter = this.Adapter;

			if (adapter != null)
			{
				for (int i = 0; i < ChildCount; i += numColumns)
				{
					// Determine the maximum height for this row
					int maxHeight = 0;
					for (int j = i; j < i + numColumns; j++)
					{
						View view = GetChildAt(j);
						if (view != null && view.Height > maxHeight)
						{
							maxHeight = view.Height;
						}
					}
					// Set max height for each element in this row
					if (maxHeight > 0)
					{
						for (int j = i; j < i + numColumns; j++)
						{
							View view = GetChildAt(j);
							if (view != null && view.Height != maxHeight)
							{
								view.SetMinimumHeight(maxHeight);
							}
						}
					}
				}
			}
		}


	}
}
