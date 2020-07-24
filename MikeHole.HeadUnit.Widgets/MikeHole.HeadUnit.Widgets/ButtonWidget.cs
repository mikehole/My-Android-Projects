using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Views.View;

namespace MikeHole.HeadUnit.Widgets
{
	[Activity]
	[BroadcastReceiver(Label = "@string/button_widget_name")]
	[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	[MetaData("android.appwidget.provider", Resource = "@xml/button_widget_provider")]
	class ButtonWidget : AppWidgetProvider
	{
		private static string AnnouncementClick = "AnnouncementClickTag";		
		
		public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
			var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(ButtonWidget)).Name);
			
			appWidgetManager.UpdateAppWidget(me, BuildRemoteViews(context, appWidgetIds));
		}

		private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
		{
			// Retrieve the widget layout. This is a RemoteViews, so we can't use 'FindViewById'
			var widgetView = new RemoteViews(context.PackageName, Resource.Layout.button_widget_main);

			RegisterClicks(context, appWidgetIds, widgetView);



			return widgetView;
		}

		private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
		{
			var intent = new Intent(context, typeof(ButtonWidget));
			intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
			intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

			// Register click event for the Background
			var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
			
			widgetView.SetOnClickPendingIntent(Resource.Id.btn_scan_qr, piBackground);			
			
			widgetView.SetOnClickPendingIntent(Resource.Id.btn_scan_qr,
				GetPendingSelfIntent(context, AnnouncementClick));			
		}
		
		private PendingIntent GetPendingSelfIntent(Context context, string action)
		{
			var intent = new Intent(context, typeof(ButtonWidget));
			intent.SetAction(action);
			return PendingIntent.GetBroadcast(context, 0, intent, 0);
		}

		public override void OnReceive(Context context, Intent intent)
		{
			base.OnReceive(context, intent);

			// Check if the click is from the "Announcement" button
			if (AnnouncementClick.Equals(intent.Action))
			{
				var spotifyIntent = new Intent(Intent.ActionView);
				spotifyIntent.SetFlags(ActivityFlags.NewTask);
				spotifyIntent.SetData( Android.Net.Uri.Parse("spotify:playlist:5y8pKT4T2L6GTxIPOfIMXu:play"));
				spotifyIntent.PutExtra(Intent.ExtraReferrer, Android.Net.Uri.Parse("android-app://" + context.PackageName));
				context.StartActivity(spotifyIntent);			
			}
		}
	}
}