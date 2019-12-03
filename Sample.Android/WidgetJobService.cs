using System;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Appwidget;

namespace Sample.Droid
{
    [Service(Name = "jp.kamusoft.sample.widgetscheduler", Permission = "android.permission.BIND_JOB_SERVICE")]
    public class WidgetJobService:JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            Task.Run(() =>
            {
                var ids = @params.Extras.GetIntArray(AppWidget.WIDGET_BUNDLE);
                System.Diagnostics.Debug.WriteLine($"OnStartJob:{ids} {DateTime.Now}");
                AppWidgetManager.GetInstance(this.ApplicationContext).NotifyAppWidgetViewDataChanged(ids, Resource.Id.widget_listview);
                
                JobFinished(@params, false);
            });

            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
           return false;
        }
    }
}
