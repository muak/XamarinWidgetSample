using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sample.Models
{
    [JsonObject]
    public class GoogleResponse
    {
        [JsonProperty(nameof(Items))]
        public List<GoogleBook> Items { get; set; }
        [JsonProperty(nameof(TotalItems))]
        public int TotalItems { get; set; }
    }

    [JsonObject]
    public class GoogleBook
    {
        [JsonProperty(nameof(VolumeInfo))]
        public VolumeInfo VolumeInfo { get; set; }
    }

    [JsonObject]
    public class VolumeInfo
    {
        [JsonProperty(nameof(Title))]
        public string Title { get; set; }
        [JsonProperty(nameof(PageCount))]
        public int PageCount { get; set; }
        [JsonProperty(nameof(ImageLinks))]
        public ImageLinks ImageLinks { get; set; }
    }

    [JsonObject]
    public class ImageLinks
    {
        [JsonProperty(nameof(SmallThumbnail))]
        public string SmallThumbnail { get; set; }
        [JsonProperty(nameof(Thumbnail))]
        public string Thumbnail { get; set; }
    }
}
