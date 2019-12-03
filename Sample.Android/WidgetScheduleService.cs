using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Widget;
using Sample.Models;
using System.Linq;
using Android.Graphics;

namespace Sample.Droid
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    public class WidgetScheduleService:RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new WidgetScheduleFactory(this.ApplicationContext);
        }
    }

    public class WidgetScheduleFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        public int Count => _source.Count;

        public bool HasStableIds => true;

        public RemoteViews LoadingView => null;

        public int ViewTypeCount => 1;

        List<WebBook> _source;
        Context _context;
        IWebApi _webApi;

        public WidgetScheduleFactory(Context context)
        {
            _context = context;
            _webApi = new WebApi();
        }

        public long GetItemId(int position)
        {
            return position;
        }

        public RemoteViews GetViewAt(int position)
        {
            if (_source.Count == 0)
            {
                return null;
            }

            var book = _source[position];

            var remoteViews = new RemoteViews(_context.PackageName, Resource.Layout.WidgetCell);

            remoteViews.SetTextViewText(Resource.Id.widgetcell_title, book.Title);

            var data = _webApi.GetThumbnail(book.Thumbnail).Result;
            var image = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            remoteViews.SetImageViewBitmap(Resource.Id.widgetcell_image, image);

            var intent = new Intent(); // TODO: セルごとにアクションを変えたい場合などはこのIntendにデータをセットする
            remoteViews.SetOnClickFillInIntent(Resource.Id.widgetcell_container, intent);

            return remoteViews;
        }

        public void OnCreate()
        {
            System.Diagnostics.Debug.WriteLine("OnCreate:");
        }

        public void OnDataSetChanged()
        {
            System.Diagnostics.Debug.WriteLine("OnDataSetChanged:");
            _source = GetData();
            System.Diagnostics.Debug.WriteLine(_source);
        }

        List<WebBook> GetData()
        {
            return _webApi.GetByKeyword("Xamarin", 4, 0).Result.ToList();
        }


        public void OnDestroy()
        {

        }
    }
}
