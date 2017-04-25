# Seppuku
`This project is a work in progress.`

### What is it?
> A dead man's switch (for other names, see alternative names) is a switch that is automatically operated if the human operator becomes incapacitated, such as through death, loss of consciousness, or being bodily removed from control. Originally applied to switches on a vehicle or machine, it has since come to be used to describe other intangible uses like in computer software. [Wikipedia](https://en.wikipedia.org/wiki/Dead_man%27s_switch)

Seppuku is a self-hosted digital [deadman's switch](https://en.wikipedia.org/wiki/Dead_man%27s_switch) 
installation with an extensible .NET module system.

Consists of a continous countdown timer, which requires constant resets to avoid triggering, akin to a deadman's switch used 
in a train. If the timer is allowed to countdown because the owner is not able to reset the timer, then it will trigger a list 
of actions in the form of "modules", which can be added to Seppuku to extend its functionality.

For example, you could create a module which wipes your harddrive when the switch triggers, or sends a message to everyone 
in your contacts.


### Build Instructions
1. Restore NuGet targets in `packages.config`
2. Use `msbuild` or `xbuild` to build the release target of Seppuku.sln


### Usage
1. Run seppuku.exe, it will generate a configuation file on first run.
2. Modify the configuration file to your heart's content.
3. Restart seppuku.exe and leave it running.

### Web API
Seppuku also exposes a web api to query information about the deadman's switch, available at localhost:19007 by default (can be configured). All responses are returned in JSON. Look (here)[https://github.com/Netdex/Seppuku/blob/master/Seppuku/Endpoint/IndexEndpoint.cs] if you're curious about what data is returned. The following commands are available:

#### Reset
```
/reset/{secret}
```
Resets the deadman's switch, so that it will activate `{GRACE_PERIOD}` in the future. A secret key is required, which is generated on first run, and available in the configuration (you can change it to anything you want though).

#### Remaining Time
```
/remain/
```
Returns how much time is remaining in the counter.

#### Information
```
/
```
Some general information about the running Seppuku instance.


### Configuration
The template used for configuration is at https://github.com/Netdex/Seppuku/blob/master/Seppuku/Config/ConfigBase.cs, 
and is stored in seppuku.xml in the assembly directory. Modify the public properties to your heart's content.

### Module Development
Modules are MEF contract based. See `Seppuku/Module/Internal/ModuleAlert.cs` for an example. 
For non-internal modules, compile them as a .dll file and place them in a folder named `Modules/` in the same 
directory as seppuku.exe.
