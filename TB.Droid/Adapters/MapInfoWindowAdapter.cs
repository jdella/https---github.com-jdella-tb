using System;
using Android.Gms.Maps;
using Android.Views;
using Android.Gms.Maps.Model;
using Android.Widget;

namespace TaskBuddi.Droid
{
	/// <summary>
	/// Adapter to inflate info windows on Task Map markers when they are tapped.
	/// </summary>
	public class MapInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
	{
		private View view = null;
		private LayoutInflater inflater;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskBuddi.Droid.MapInfoWindowAdapter"/> class.
		/// </summary>
		/// <param name="inflater">Inflater.</param>
		public MapInfoWindowAdapter(LayoutInflater inflater)
		{
			this.inflater = inflater;
		}

		/// <summary>
		/// Sets the info window contents.
		/// </summary>
		/// <returns>The info contents.</returns>
		/// <param name="p0">The activated (tapped) marker</param>
		public View GetInfoContents(Marker p0)
		{
			//inflate view if required
			if (view == null)
			{
				view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
			}
			// get view elements...
			var title = view.FindViewById<TextView>(Resource.Id.title);
			var snippet = view.FindViewById<TextView>(Resource.Id.snippet);
			// ...and populate with marker's details
			title.Text = p0.Title;
			snippet.Text = p0.Snippet;

			return view;
		}

		/// <summary>
		/// *Implementation method*
		/// Gets the info window.
		/// </summary>
		/// <returns>The info window.</returns>
		/// <param name="p0">P0.</param>
		public View GetInfoWindow(Marker p0)
		{
			return null;
		}
	}
}

