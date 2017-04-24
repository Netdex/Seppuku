# Seppuku
`This project is a work in progress.`

### What is it?
Seppuku is a self-hosted digital deadman switch installation with an extensible .NET module system.


### Build Instructions
1. Restore NuGet targets in `packages.config`
2. Use `msbuild` or `xbuild` to build the release target of Seppuku.sln


### Usage
1. Run seppuku.exe, it will generate a configuation file on first run.
2. Modify the configuration file to your heart's content.
3. Restart seppuku.exe and leave it running.

#### Reset
Make an HTTP GET request to `localhost:port (default 19007)` with uri `/reset/`

### Configuration
```xml
<ConfigBase>
  <GraceTime>[duration as xsd:duration, ex. "P30D" is 30 days]</GraceTime>
  
  <!-- this is internal, don't touch it please -->
  <FailureDate>[date of execution as DateTime string, ex. "2017-05-24T17:01:05.6525602-04:00"]</FailureDate>
</ConfigBase>
```

### Module Development
Modules are MEF contract based. See `Seppuku/Module/Internal/ModuleAlert.cs` for an example. 
For non-internal modules, compile them as a .dll file and place them in `Modules/` in the same 
directory as seppuku.exe.
