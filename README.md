
[![Build status](https://ci.appveyor.com/api/projects/status/sa0tjdyyfev17uq4?svg=true)](https://ci.appveyor.com/project/antoinebidault/memorycacheredisbackplane-net)
![Nuget](https://img.shields.io/nuget/v/MemoryCacheRedisBackplane.Net)

![Logo](/MemoryCacheRedisBackplane.Net/MemoryCacheRedisBackplane.Net.png)

# MemoryCacheRedisBackplane.Net

An Asp.Net Core Memory cache invalidation (.Net Standard 2.0)


## Purpose

The purpose of this library is to keep using the native IMemoryCache that is very fast and reliable for each instance of your application (server farm). The Redis backplane is just used for communication between applications using the Pub/Sub of events.

![Purpose](/purpose.png)

## Simple use


In your startup.cs
```CSharp
services.AddMemoryCache();
services.AddMemoryCacheRedisBackplane(Configuration.GetConnectionString("Redis"));
```

Then in your controllers, if you need to invalidate the cache :
```CSharp
  public TestController(IMemoryCacheInvalidator cacheInvalidator)
  {
      _cacheInvalidator = cacheInvalidator;
  }

  [HttpPost("invalidate")]
  public IActionResult Invalidate()
  {
      // This will invalidate the cache through all the app instances
      _cacheInvalidator.InvalidateAsync(_cacheKey);
      return Ok();
  }
```
