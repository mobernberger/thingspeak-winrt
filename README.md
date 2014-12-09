ThingSpeak WinRT Library
================

This is a WinRT Class library for Thingspeak. 

#### Dependencies
---

This library depends on Json.NET (http://james.newtonking.com/json). The greatest JSON library I know and a donation is really appreciated to the great work from James Newton-King.

#### Prerequisites
---

You need a ThingSpeak Account (https://thingspeak.com/) to get started with the library.


### Example Usage

You have to instantiate the Class with the constructor and provide a bool value if you want to make all communications with or without SSL. After that you are ready to get started.

```c#
var client = new ThingSpeakClient(false);
```

##### Create an feed inside a Channel

After you have created a Channel on the ThingSpeak Website you could add a new feed inside the channel with the following commands:´
```c#
var dataFeed = new ThingSpeakFeed { Field1 = "58.27", Field2 = "32.59" };
dataFeed = await client.UpdateFeedAsync("<Your Write API Key>", dataFeed);
```
If there occurs any error, the dataFeed.Id entry will be null. If everyhting works ok, the Id of the entry will be what Id is returned from ThingSpeak.

##### Get all feeds from a Channel

```c#
var feeds = await client.ReadAllFeedsAsync("<Your Read API Key>", <Your Channel Id>);
```
This command returns an item of the type "ThingSpeakData" which contains a Channel item and a Collection of all feed items which are in the channel.

##### Get specific feed from a Channel

```c#
var feed = await client.ReadFeedAsync("<Your Read API Key>", <Your Channel Id>, <Your Feed Id>);
```
This command returns the specified feed in the channel which is of the type "ThingSpeakFeed".

##### Get last feed from a Channel

```c#
var feeds = await client.ReadLastFeedAsync("<Your Read API Key>", <Your Channel Id>);
```
This command returns the last feed in the channel which is of the type "ThingSpeakFeed".

##### Get all feeds for the specified field from a Channel

```c#
var feeds = await client.ReadFieldsAsync("<Your Read API Key>", <Your Channel Id>, <The id of the field>);
```
This command returns an item of the type "ThingSpeakData" which contains a Channel item and a Collection of all feed items which are in the channel but only the specified field isn't null.

##### Get last feed for the specified field from a Channel

```c#
var feeds = await client.ReadLastFieldFeedAsync("<Your Read API Key>", <Your Channel Id>, <The id of the field>);
```
This command returns the last feed in the channel which is of the type "ThingSpeakFeed", where only the specified field isn't null.

##### Get the status for the specified Channel and all feeds within

```c#
var feeds = await client.ReadStatusUpdateAsync("<Your Read API Key>", <Your Channel Id>);
```
This command returns an item of the type "ThingSpeakData" which contains a Channel item and a Collection of all feed items which are in the channel but only the status isn't null if anything is there.

##### Send a Status Update via a ThingTweet Account

You need to first setup a ThingTweet Account and link it to your Twitter account.

```c#
var tweet = await client.SendThingTweetAsync("<Your ThingTweet API Key>", "Your Twitter Status");
```
This command returns a bool which is true when the command was successfully sent to the ThingTweet API. You should now see the Twitter Status Update.

##### Send a ThingHTTP request

You need to setup a ThingHTTP Account.
```c#
var tweet = await client.SendThingHttpAsync("<Your ThingHTTP API Key>", "Your Message");
```
This command returns a bool which is true when the command was successfully sent to the ThingHTTP API. 


### Notes:
* If your channel is public you have to pass null instead of the Read API Key.
