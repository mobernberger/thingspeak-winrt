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

After you have created a Channel on the ThingSpeak Website you could add a an entry inside the channel
