# Pandv.Config
A config keeper for etcd v3 
## Quick start

### Install package

* Package Manager

```
Install-Package Pandv.Config -Version 0.0.1.2
```
* .NET CLI

```
dotnet add package Pandv.Config --version 0.0.1.2
```

### Simple use example

``` csharp

// if reloadOnChange == true, the all key/value will keep same with etcd v3 which's keys under $"{configName}:{systemName}:"
// else only once get value which's keys under $"{configName}:{systemName}:" 
var config = new ConfigurationBuilder()
    .UsePandv(systemName, reloadOnChange, Endpoints, User, Pwd, configName)
    .Build();

// get value 
// etcd v3 key is $"{configName}:{systemName}:test1"
var value = config["test1"];

// set value
// if reloadOnChange == true, "NewValue" will set to etcd v3
// else "NewValue" will not set to etcd v3
config["test4"] = "NewValue";
```

### api doc

Main api doc please see 

[https://fs7744.github.io/pandv.config/api/Pandv.Config.html](https://fs7744.github.io/pandv.config/api/Pandv.Config.html)
