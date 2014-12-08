using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace ThingSpeakWinRT
{
    /// <summary>
    /// Class of a ThingSpeak Data item which you get back when you ask for all entries in a channel
    /// </summary>
    public class ThingSpeakData
    {
        [JsonProperty(PropertyName = "channel")]
        public ThingSpeakChannel Channel { get; set; }

        [JsonProperty(PropertyName = "feeds")]
        public Collection<ThingSpeakFeed> Feeds { get; set; }
    }
}
