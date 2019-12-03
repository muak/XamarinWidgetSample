using System;
using Android.App;
using Android.App.Job;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Sample.Droid
{
    [BroadcastReceiver(Label = "Xamarin Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidget_provider")]
    public class AppWidget : AppWidgetProvider
    {
        public const string ACTION_REFRESH = "jp.kamusoft.sample.widget.ACTION_REFRESH";
        public const string ACTION_SELECTED = "jp.kamusoft.sample.widget.ACTION_SELECTED";
        public const string WIDGET_BUNDLE = "widget_bundle";

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            System.Diagnostics.Debug.WriteLine("Widget OnUpdate");
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
            appWidgetManager.UpdateAppWidget(me, BuildRemoteViews(context, appWidgetIds));

            var scheduler = context.GetSystemService(Context.JobSchedulerService) as JobScheduler;

            var jobName = new ComponentName(context, Java.Lang.Class.FromType(typeof(WidgetJobService)).Name);
            var jobInfo = new JobInfo.Builder(1, jobName);
            jobInfo.SetBackoffCriteria(5000, BackoffPolicy.Linear);
            jobInfo.SetPersisted(true);
            jobInfo.SetPeriodic(900000);
            jobInfo.SetRequiredNetworkType(NetworkType.Any);
            jobInfo.SetRequiresCharging(false);

            var bundle = new PersistableBundle();
            bundle.PutIntArray(WIDGET_BUNDLE, appWidgetIds);

            jobInfo.SetExtras(bundle);

            var job = jobInfo.Build();
            scheduler.Schedule(job);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
            switch (intent.Action)
            {
                case ACTION_SELECTED:
                    var dummyIntent = new Intent(context, typeof(MainActivity));
                    dummyIntent.SetFlags(ActivityFlags.SingleTop);
                    dummyIntent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(dummyIntent);
                    break;
                case ACTION_REFRESH:
                    var appWidgetIds = intent.GetIntArrayExtra(ACTION_REFRESH);
                    if (appWidgetIds != null && appWidgetIds.Length > 0)
                    {
                        AppWidgetManager.GetInstance(Application.Context).NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.widget_listview);
                    }
                    break;
            }
        }

        RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            var intent = new Intent(context, Java.Lang.Class.FromType(typeof(WidgetScheduleService)));
            widgetView.SetRemoteAdapter(Resource.Id.widget_listview, intent);

            RegisterClicks(context, appWidgetIds, widgetView);

            return widgetView;
        }

        void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
        {
            var intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(ACTION_SELECTED);

            var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            widgetView.SetPendingIntentTemplate(Resource.Id.widget_listview, piBackground);

            SetRefreshPendingIntent(context, widgetView, appWidgetIds);
        }

        void SetRefreshPendingIntent(Context ctx, RemoteViews rv, int[] appWidgetIds)
        {
            var refreshIntent = new Intent(ctx, typeof(AppWidget));
            refreshIntent.SetAction(ACTION_REFRESH);
            refreshIntent.PutExtra(ACTION_REFRESH, appWidgetIds);

            PendingIntent btnClickPendingIntent = PendingIntent.GetBroadcast(
                ctx,
                0,
                refreshIntent,
                PendingIntentFlags.UpdateCurrent
            );

            rv.SetOnClickPendingIntent(Resource.Id.widget_refresh, btnClickPendingIntent);
        }
    }
}
