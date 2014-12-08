using System;
using Newtonsoft.Json;

namespace ThingSpeakWinRT
{
    /// <summary>
    /// Class of a ThingSpeak Channel item
    /// </summary>
    public class ThingSpeakChannel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "field1")]
        public string Field1 { get; set; }

        [JsonProperty(PropertyName = "field2")]
        public string Field2 { get; set; }

        [JsonProperty(PropertyName = "field3")]
        public string Field3 { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public decimal? Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public decimal? Longitude { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "last_entry_id")]
        public int LastEntryId { get; set; }
    }
}
