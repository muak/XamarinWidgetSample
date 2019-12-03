using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sample.Models
{
    public class WebBook
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
    }

    public interface IWebApi
    {
        Task<IEnumerable<WebBook>> GetByKeyword(string keyword, int count = 30, int startIndex = 0);
        Task<byte[]> GetThumbnail(string url);
    }

    public class WebApi:IWebApi
    {
        public static readonly string BooksBasePath = "https://www.googleapis.com/books/v1/volumes?q=";
        public static readonly string BooksSearchByTitle = BooksBasePath + "intitle:";

        internal static HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(180)
        };

        public WebApi()
        {
        }

        public async Task<byte[]> GetThumbnail(string url)
        {
            return await httpClient.GetByteArrayAsync(url).ConfigureAwait(false);
        }


        public async Task<IEnumerable<WebBook>> GetByKeyword(string keyword,int count = 30, int startIndex = 0)
        {
            var encodedKeyword = System.Web.HttpUtility.UrlEncode(keyword);
            var url = $"{BooksSearchByTitle}{encodedKeyword}&maxResults={count}&startIndex={startIndex}";

            var result = await RequestObserver<GoogleResponse>(url);

            if (result.TotalItems == 0 || result.Items == null)
            {
                return new List<WebBook>();
            }

            // 画像がないやつはスキップ
            var webBooks = result.Items.Where(x => x.VolumeInfo.ImageLinks != null)
                .Select(x => new WebBook {
                     Title = x.VolumeInfo.Title,
                     Thumbnail = x.VolumeInfo.ImageLinks.Thumbnail.Replace("http://","https://") 
                });

            return webBooks;
        }

        internal async Task<ReturnT> RequestObserver<ReturnT>(string url, int retryCount = 2)
        {
            var sub = Observable.Create<ReturnT>(async observer =>
            {
                var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    observer.OnNext(JsonConvert.DeserializeObject<ReturnT>(content));
                    observer.OnCompleted();
                }
                else
                {
                    // リトライ
                    throw new Exception("ServerError Retry");
                }

                // HttpClient自体の例外もリトライ対象（キャッチはしない）

                return Disposable.Empty;
            }).Retry(retryCount + 1); // 最大この数値分リトライを含めて実行する 0だと1回も実行されないので注意

            ReturnT result = default;

            try
            {
                result = await sub.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // リトライしても解決しなかった場合は一旦例外とする
                throw ex;
            }

            return result;
        }
    }
}
