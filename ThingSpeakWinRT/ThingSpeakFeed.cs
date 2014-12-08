using System;
using Newtonsoft.Json;

namespace ThingSpeakWinRT
{
    /// <summary>
    /// Class of a ThingSpeak Feed item
    /// </summary>
    public class ThingSpeakFeed
    {
        [JsonProperty(PropertyName = "channel_id")]
        public int ChannelId { get; set; }

        [JsonProperty(PropertyName = "field1")]
        public string Field1 { get; set; }

        [JsonProperty(PropertyName = "field2")]
        public string Field2 { get; set; }

        [JsonProperty(PropertyName = "field3")]
        public string Field3 { get; set; }

        [JsonProperty(PropertyName = "field4")]
        public string Field4 { get; set; }

        [JsonProperty(PropertyName = "field5")]
        public string Field5 { get; set; }

        [JsonProperty(PropertyName = "field6")]
        public string Field6 { get; set; }

        [JsonProperty(PropertyName = "field7")]
        public string Field7 { get; set; }

        [JsonProperty(PropertyName = "field8")]
        public string Field8 { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(PropertyName = "entry_id")]
        public int? EntryId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public decimal? Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public decimal? Longitude { get; set; }

        [JsonProperty(PropertyName = "elevation")]
        public int? Elevation { get; set; }

        [JsonProperty(PropertyName = "twitter")]
        public string Twitter { get; set; }

        [JsonProperty(PropertyName = "tweet")]
        public string Tweet { get; set; }
    }
}
