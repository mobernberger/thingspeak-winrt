ThingSpeak WinRT Library
================

This is a WinRT Class library for Thingspeak. 

#### Dependencies
---

This library depends on Json.NET (http://james.newtonking.com/json). The greatest JSON library I know and a donation is really appreciated to the great work from James Newton-King.


### Example Usage

You have to instantiate the Class with the constructor and provide a bool value if you want to make all communications with or without SSL. 

```c#
var client = new ThingSpeakClient(false);
```
