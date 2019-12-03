using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Sample.Models;

namespace Sample.iOS.Widget
{
    public class WidgetTableViewSource:UITableViewSource
    {
        public List<WebBook> Source { get; set; } = new List<WebBook>();

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var reusableCell = tableView.DequeueReusableCell("sampleCell");
            if (reusableCell == null)
            {
                reusableCell = new UITableViewCell(UITableViewCellStyle.Default, "sampleCell");
                reusableCell.TextLabel.Font = reusableCell.TextLabel.Font.WithSize(12);
            }

            var book = Source[indexPath.Row];
            reusableCell.TextLabel.Text = book.Title;

            var mainScale = (float)UIScreen.MainScreen.Scale;
            Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("In get image:" + book.Thumbnail);
                using var url = NSUrl.FromString(book.Thumbnail);
                using var data = NSData.FromUrl(url);
                var image = UIImage.LoadFromData(data);
                System.Diagnostics.Debug.WriteLine(image);
                image = image.Scale(new CGSize(30, 40), mainScale);
                BeginInvokeOnMainThread(() =>
                {
                    reusableCell.ImageView.Image = image;
                    reusableCell.SetNeedsLayout();
                });
            });

            return reusableCell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Source.Count;
        }

    }
}
