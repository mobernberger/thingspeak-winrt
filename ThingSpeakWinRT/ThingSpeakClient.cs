using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace ThingSpeakWinRT
{
    /// <summary>
    /// Thingspeak Client Class
    /// </summary>
    public class ThingSpeakClient
    {
        #region Fields

        // host name and update path
        private static string _thingSpeakHost;
        private string _requestUri;

        // max values
        private const int ThingSpeakMaxStatus = 140;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sslRequired">HTTPS connection requested</param>
        public ThingSpeakClient(bool sslRequired)
        {
            _thingSpeakHost = sslRequired ? "https://api.thingspeak.com" : "http://api.thingspeak.com";
        }

        /// <summary>
        /// Update a feed in a channel
        /// </summary>
        /// <param name="writeApiKey">Write API Key for the channel to update</param>
        /// <param name="feedEntry">Data entry for updating channel</param>
        /// <returns>Updated feedEntry. The EntryId is null when an error occurs</returns>
        public async Task<ThingSpeakFeed> UpdateFeedAsync(string writeApiKey, ThingSpeakFeed feedEntry)
        {
            // check for write API Key
            if (string.IsNullOrEmpty(writeApiKey))
            {
                throw new ArgumentNullException("writeApiKey", "You must specify a write API Key");
            }

            // check at leaset one field value not empty
            if (String.IsNullOrEmpty(feedEntry.Field1) && String.IsNullOrEmpty(feedEntry.Field2) &&
                String.IsNullOrEmpty(feedEntry.Field3) && String.IsNullOrEmpty(feedEntry.Field4) &&
                String.IsNullOrEmpty(feedEntry.Field5) && String.IsNullOrEmpty(feedEntry.Field6) &&
                String.IsNullOrEmpty(feedEntry.Field7) && String.IsNullOrEmpty(feedEntry.Field8))
            {
                throw new ArgumentNullException("feedEntry", "You must specify one field at least");
            }

            // check status message
            if ((feedEntry.Status != null) && (feedEntry.Status.Length > ThingSpeakMaxStatus))
            {
                throw new ArgumentException("status", "Max status length is " + ThingSpeakMaxStatus);
            }

            // check twitter account and message
            if (((feedEntry.Twitter == null) && (feedEntry.Tweet != null)) ||
                ((feedEntry.Twitter != null) && (feedEntry.Tweet == null)))
            {
                throw new ArgumentException("twitter and tweet parameters must be both valued");
            }

            // build body
            var body = String.Empty;
            // fields
            if (!String.IsNullOrEmpty(feedEntry.Field1))
            {
                body += "field1=" + feedEntry.Field1 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field2))
            {
                body += "field2=" + feedEntry.Field2 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field3))
            {
                body += "field3=" + feedEntry.Field3 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field4))
            {
                body += "field4=" + feedEntry.Field4 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field5))
            {
                body += "field5=" + feedEntry.Field5 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field6))
            {
                body += "field6=" + feedEntry.Field6 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field7))
            {
                body += "field7=" + feedEntry.Field7 + "&";
            }
            if (!String.IsNullOrEmpty(feedEntry.Field8))
            {
                body += "field8=" + feedEntry.Field8 + "&";
            }

            // location
            if (feedEntry.Latitude != null && feedEntry.Longitude != null && feedEntry.Elevation != null)
            {
                body += "&lat=" + feedEntry.Latitude + "&long=" + feedEntry.Longitude + "&elevation=" +
                        feedEntry.Elevation;
            }

            // status
            if (feedEntry.Status != null)
            {
                body += "&status=" + feedEntry.Status;
            }

            // twitter
            if ((feedEntry.Twitter != null) && (feedEntry.Tweet != null))
            {
                body += "&twitter=" + feedEntry.Twitter + "&tweet=" + feedEntry.Tweet;
            }

            //Create URI
            _requestUri = _thingSpeakHost + "/update.json";

            // build HTTP request
            string request = "api_key=" + writeApiKey + "&";
            request += body;

            using (var httpClient = new HttpClient())
            {
                var stringContent = new HttpStringContent(request, UnicodeEncoding.Utf8,
                    "application/x-www-form-urlencoded");
                var httpResponse = await httpClient.PostAsync(new Uri(_requestUri), stringContent);

                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    feedEntry.EntryId =
                        Convert.ToInt32(
                            JsonConvert.DeserializeObject<ThingSpeakFeed>(httpResponse.Content.ToString()).EntryId);
                }
                else
                {
                    feedEntry.EntryId = null;
                }

                return feedEntry;
            }
        }

        /// <summary>
        /// Read last feed in a Channel feed with a specific field. Returns null when an error happens.
        /// </summary>
        /// <param name="readApiKey">Read API Key for the channel to read (null if channel is public)</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="fieldId">Field ID</param>
        /// <param name="status">Include status update in feed</param>
        /// <param name="location">Include latitude, longitude and elevation in feed</param>
        /// <returns>ThingSpeakData Type of the last data entry</returns>
        public async Task<ThingSpeakFeed> ReadLastFieldFeedAsync(string readApiKey, int channelId, int fieldId,
            bool status = false, bool location = false)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/channels/" + channelId + "/fields/" + fieldId + "/last.json" +
                          ConstructQueryString(status, location);

            //Start request
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse;

                if (readApiKey == null)
                {
                    httpResponse = await httpClient.GetAsync(new Uri(_requestUri));
                }
                else
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_requestUri),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("X-THINGSPEAKAPIKEY", readApiKey);
                    httpResponse = await httpClient.SendRequestAsync(request);
                }


                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    var entry = JsonConvert.DeserializeObject<ThingSpeakFeed>(httpResponse.Content.ToString());
                    if (entry.CreatedAt != null)
                    {
                        entry.CreatedAt = entry.CreatedAt.Value.ToLocalTime();
                    }
                    return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Read all feeds in a Channel with a specified field
        /// </summary>
        /// <param name="readApiKey">Read API Key for the channel to read (null if channel is public)</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="fieldId">Field ID</param>
        /// <param name="status">Include status update in feed</param>
        /// <param name="location">Include latitude, longitude and elevation in feed</param>
        /// <returns>List of all data entries read</returns>
        public async Task<ThingSpeakData> ReadFieldsAsync(string readApiKey, int channelId, int fieldId,
            bool status = false,
            bool location = false)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/channels/" + channelId + "/fields/" + fieldId + ".json" +
                          ConstructQueryString(status, location);

            //Start request
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse;

                if (readApiKey == null)
                {
                    httpResponse = await httpClient.GetAsync(new Uri(_requestUri));
                }
                else
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_requestUri),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("X-THINGSPEAKAPIKEY", readApiKey);
                    httpResponse = await httpClient.SendRequestAsync(request);
                }


                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    var entry = JsonConvert.DeserializeObject<ThingSpeakData>(httpResponse.Content.ToString());
                    foreach (var feed in entry.Feeds.Where(feed => feed.CreatedAt != null))
                    {
                        feed.CreatedAt = feed.CreatedAt.Value.ToLocalTime();
                    }
                    return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Read all feeds in a Channel
        /// </summary>
        /// <param name="readApiKey">Read API Key for the channel to read (null if channel is public)</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="status">Include status update in feed</param>
        /// <param name="location">Include latitude, longitude and elevation in feed</param>
        /// <returns>List of all data entries read</returns>
        public async Task<ThingSpeakData> ReadAllFeedsAsync(string readApiKey, int channelId, bool status = false,
            bool location = false)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/channels/" + channelId + "/feeds.json" +
                          ConstructQueryString(status, location);

            //Start request
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse;

                if (readApiKey == null)
                {
                    httpResponse = await httpClient.GetAsync(new Uri(_requestUri));
                }
                else
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_requestUri),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("X-THINGSPEAKAPIKEY", readApiKey);
                    httpResponse = await httpClient.SendRequestAsync(request);
                }


                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    var entry = JsonConvert.DeserializeObject<ThingSpeakData>(httpResponse.Content.ToString());
                    foreach (var feed in entry.Feeds.Where(feed => feed.CreatedAt != null))
                    {
                        feed.CreatedAt = feed.CreatedAt.Value.ToLocalTime();
                    }
                    return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Read last feed in a channel
        /// </summary>
        /// <param name="readApiKey">Read API Key for the channel to read (null if channel is public)</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="status">Include status update in feed</param>
        /// <param name="location">Include latitude, longitude and elevation in feed</param>
        /// <returns>List of all data entries read</returns>
        public async Task<ThingSpeakFeed> ReadLastFeedAsync(string readApiKey, int channelId, bool status = false,
            bool location = false)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/channels/" + channelId + "/feeds/last.json" +
                          ConstructQueryString(status, location);

            //Start request
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse;

                if (readApiKey == null)
                {
                    httpResponse = await httpClient.GetAsync(new Uri(_requestUri));
                }
                else
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_requestUri),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("X-THINGSPEAKAPIKEY", readApiKey);
                    httpResponse = await httpClient.SendRequestAsync(request);
                }


                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    var entry = JsonConvert.DeserializeObject<ThingSpeakFeed>(httpResponse.Content.ToString());
                    if (entry.CreatedAt != null)
                    {
                        entry.CreatedAt = entry.CreatedAt.Value.ToLocalTime();
                    }
                    return entry;
                }
                return null;
            }
        }


        /// <summary>
        /// Read status updates for a specified Channel
        /// </summary>
        /// <param name="readApiKey">Read API Key for the channel to read (null if channel is public)</param>
        /// <param name="channelId">Channel ID</param>
        /// <returns>List of all data entries read with only status update</returns>
        public async Task<ThingSpeakData> ReadStatusUpdateAsync(string readApiKey, int channelId)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/channels/" + channelId + "/status.json";

            //Start request
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse;

                if (readApiKey == null)
                {
                    httpResponse = await httpClient.GetAsync(new Uri(_requestUri));
                }
                else
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_requestUri),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("X-THINGSPEAKAPIKEY", readApiKey);
                    httpResponse = await httpClient.SendRequestAsync(request);
                }


                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    var entry = JsonConvert.DeserializeObject<ThingSpeakData>(httpResponse.Content.ToString());
                    foreach (var feed in entry.Feeds.Where(feed => feed.CreatedAt != null))
                    {
                        feed.CreatedAt = feed.CreatedAt.Value.ToLocalTime();
                    }
                    return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Read specific feed in a Channel
        /// </summary>
        /// <param name="readApiKey">Read API Key for the channel to read (null if channel is public)</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="entryId">Channel ID</param>
        /// <param name="status">Include status update in feed</param>
        /// <param name="location">Include latitude, longitude and elevation in feed</param>
        /// <returns>List of all data entries read</returns>
        public async Task<ThingSpeakFeed> ReadFeedAsync(string readApiKey, int channelId, int entryId, bool status = false,
            bool location = false)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/channels/" + channelId + "/feeds/" + entryId + ".json" +
                          ConstructQueryString(status, location);

            //Start request
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse;

                if (readApiKey == null)
                {
                    httpResponse = await httpClient.GetAsync(new Uri(_requestUri));
                }
                else
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(_requestUri),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("X-THINGSPEAKAPIKEY", readApiKey);
                    httpResponse = await httpClient.SendRequestAsync(request);
                }


                if (httpResponse.StatusCode == HttpStatusCode.Ok)
                {
                    var entry = JsonConvert.DeserializeObject<ThingSpeakFeed>(httpResponse.Content.ToString());
                    if (entry.CreatedAt != null)
                    {
                        entry.CreatedAt = entry.CreatedAt.Value.ToLocalTime();
                    }
                    return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Send Twitter Message via ThingTweet
        /// </summary>
        /// <param name="apiKey">API Key for the ThingTweet Account</param>
        /// <param name="status">The status message you want to send</param>
        /// <returns>Returns true or false</returns>
        public async Task<bool> SendThingTweetAsync(string apiKey, string status)
        {
            //Check if Twitter Status is too long
            if (status.Length > 139)
            {
                throw new ArgumentException("Your Twitter Status is too long", "status");
            }

            //Create URI
            _requestUri = _thingSpeakHost + "/apps/thingtweet/1/statuses/update";

            //Start request
            using (var httpClient = new HttpClient())
            {
                var request = "api_key=" + apiKey + "&status=" + status;
                var stringContent = new HttpStringContent(request, UnicodeEncoding.Utf8,
                    "application/x-www-form-urlencoded");
                var httpResponse = await httpClient.PostAsync(new Uri(_requestUri), stringContent);
                return httpResponse.IsSuccessStatusCode;
            }
        }

        /// <summary>
        /// Send a Reqest via ThingHTTP
        /// </summary>
        /// <param name="apiKey">API Key for the ThingTweet Account</param>
        /// <param name="message">The message you want to send</param>
        /// <returns>Returns true or false</returns>
        public async Task<bool> SendThingHttpAsync(string apiKey, string message)
        {
            //Create URI
            _requestUri = _thingSpeakHost + "/apps/thinghttp/send_request";

            //Start request
            using (var httpClient = new HttpClient())
            {
                var request = "api_key=" + apiKey + "&message=" + message;
                var stringContent = new HttpStringContent(request, UnicodeEncoding.Utf8,
                    "application/x-www-form-urlencoded");
                var httpResponse = await httpClient.PostAsync(new Uri(_requestUri), stringContent);
                return httpResponse.IsSuccessStatusCode;
            }
        }


        private static string ConstructQueryString(bool status, bool location)
        {
            var queryString = String.Empty;

            if (status)
                queryString += "?status=true";

            if (!location) return queryString;
            if (queryString != String.Empty)
                queryString += "&";
            else
                queryString += "?";

            queryString += "location=true";
            return queryString;
        }
    }
}
