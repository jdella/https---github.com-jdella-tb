using System;
using Android.Gms.Maps;
using Android.Views;
using Android.Gms.Maps.Model;
using Android.Widget;

namespace TaskBuddi.Droid
{
	public class MapInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
	{
		private View view = null;
		private LayoutInflater inflater;

		public MapInfoWindowAdapter(LayoutInflater inflater)
		{
			this.inflater = inflater;
		}

		public View GetInfoContents(Marker p0)
		{
			if (view == null)
			{
				view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
			}

			var title = view.FindViewById<TextView>(Resource.Id.title);
			var snippet = view.FindViewById<TextView>(Resource.Id.snippet);

			title.Text = p0.Title;
			snippet.Text = p0.Snippet;

			return view;
		}

		public View GetInfoWindow(Marker p0)
		{
			return null;
		}
	}
}

