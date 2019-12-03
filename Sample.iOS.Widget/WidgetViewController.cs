using System;
using UIKit;
using NotificationCenter;
using Foundation;
using CoreGraphics;
using Sample.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sample.iOS.Widget
{
    [Register("WidgetViewController")]
    public class WidgetViewController:UIViewController,INCWidgetProviding
    {
        IWebApi _webApi;
        UITableView _tableView;
        WidgetTableViewSource _source;
        UITapGestureRecognizer _tapGesutre;

        public WidgetViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _webApi = new WebApi();

            ExtensionContext.SetWidgetLargestAvailableDisplayMode(NCWidgetDisplayMode.Expanded);

            _tableView = new UITableView(View.Frame, UITableViewStyle.Plain);
            _tableView.AllowsSelection = false;
            _tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _tableView.RowHeight = 52;
            _tableView.EstimatedRowHeight = 52;

            _source = new WidgetTableViewSource();
            _tableView.Source = _source;

            _tapGesutre = new UITapGestureRecognizer((obj) =>
            {
                ExtensionContext.OpenUrl(NSUrl.FromString("jp.kamusoft.sample://"), null);
            });

            View.AddGestureRecognizer(_tapGesutre);
            View.AddSubview(_tableView);

        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            View.RemoveGestureRecognizer(_tapGesutre);
            _tapGesutre.Dispose();
            _tapGesutre = null;
            _source.Dispose();
            _tableView.Dispose();
        }

        [Export("widgetActiveDisplayModeDidChange:withMaximumSize:")]
        public void WidgetActiveDisplayModeDidChange(NCWidgetDisplayMode activeDisplayMode, CoreGraphics.CGSize maxSize)
        {
            if (activeDisplayMode == NCWidgetDisplayMode.Compact)
            {
                PreferredContentSize = maxSize;
            }
            else
            {
                PreferredContentSize = _tableView.ContentSize;
            }
        }

        [Export("widgetPerformUpdateWithCompletionHandler:")]
        public async void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler)
        {
            try
            {
                var books = await _webApi.GetByKeyword("Xamarin", 4, 0);

                _source.Source.Clear();

                foreach (var item in books)
                {
                    _source.Source.Add(item);
                }

                _tableView.ReloadData();


                completionHandler(NCUpdateResult.NewData);
            }
            catch (Exception ex)
            {
                completionHandler(NCUpdateResult.Failed);
            }
        }
    }
}
