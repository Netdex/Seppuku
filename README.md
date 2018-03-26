# Seppuku
`This project is a work in progress.`

![](http://i.imgur.com/A55ZajQ.png)

### What is it?
> A dead man's switch (for other names, see alternative names) is a switch that is automatically operated if the human operator becomes incapacitated, such as through death, loss of consciousness, or being bodily removed from control. Originally applied to switches on a vehicle or machine, it has since come to be used to describe other intangible uses like in computer software. [Wikipedia](https://en.wikipedia.org/wiki/Dead_man%27s_switch)

Seppuku is a self-hosted digital [deadman's switch](https://en.wikipedia.org/wiki/Dead_man%27s_switch) 
installation with an extensible .NET module system.

It intends to follow the principles outlined in [this blog post](https://blog.netdex.cf/2017/creating-a-deadmans-switch/).

Consists of a continous countdown timer, which requires constant resets to avoid triggering, akin to a deadman's switch used 
in a train. If the timer is allowed to countdown because the owner is not able to reset the timer, then it will trigger a list 
of actions in the form of "modules", which can be added to Seppuku to extend its functionality.

For example, you could create a module which wipes your harddrive when the switch triggers, or sends a message to everyone 
in your contacts.

## FAQ

**Q**: I'll be dead anyways, why would I care about what happens after I die?<br>
**A**: I just write software, get your philosophy outta here

**Q**: How should I run this?<br>
**A**: Rent a Linux box and run it with Mono as a daemon or something of that ilk.

### Build Instructions
1. Restore NuGet targets in `packages.config`
2. Use `msbuild` or `xbuild` to build the release target of Seppuku.sln

### Usage
1. Run seppuku.exe, it will generate a configuration files on first run.
2. Modify the configuration file to your heart's content.
3. Modify the module configuration files in Configuration/.
4. Restart seppuku.exe and leave it running.

### Web API
Seppuku also exposes a web API through ModuleWebAPI to query information about the deadman's switch, available at localhost:19007 by default (can be configured). All responses are returned in JSON. Look [here](https://github.com/Netdex/Seppuku/blob/master/Seppuku/Module/Internal/Endpoint/IndexEndpoint.cs) if you're curious about what data is returned. The following commands are available (and probably more since I don't document quickly):

#### Reset
```
GET /reset/{token}
```
Resets the deadman's switch, so that it will activate `Configuration.GracePeriod` in the future. A token is required, which is derived with the following algorithm:

`sha1(Configuration.Secret + DateTime.UtcNow.Date)`

`Configuration.Secret` is generated on first launch and is in the configuration file, and also printed when the program is run. This is appended to today's date so that the token changes everyday. It is then hashed with sha1 so that the secret is more difficult to derive. This is not the most secure implementation possible, and may possibly be changed in the future.

#### Remaining Time
```
GET /remain/
```
Returns how much time is remaining in the counter.

#### Information
```
GET /
```
Some general information about the running Seppuku instance.

#### Debug
```
GET /debug/trigger
```
Forces the deadman's switch to fail without requiring a token. Only available in the Debug target.

```
GET /debug/reset
```
Resets the deadman's switch without requiring a token. Only available in the Debug target.

### Configuration
On first execution, `seppuku.xml` will be generated. Look through it to see what you can modify. The default configuration properties are available at https://github.com/Netdex/Seppuku/blob/master/Seppuku/Config/Conf.cs#L30 (this link might not lead to the right code).

Each individual module also has it's own configuration, stored at `/Configuration`. Each module specifies its default 
configuration through the constructor.

### Module Development
Modules are MEF contract based. See `Seppuku/Module/Internal/ModuleAlert.cs` for an example. 
For non-internal modules, compile them as a .dll file and place them in a folder named `Modules/` in the same 
directory as seppuku.exe.

#### Included Modules
##### Seppuku.Module.Internal.ModuleAlert
Prints out a message to the console whenever an event occurs.

##### Seppuku.Module.Internal.ModuleWebAPI
Exposes the web API mentioned above.

##### Seppuku.Module.ModuleGetRequest
Sends GET requests to multiple configurable endpoints when the switch fails.

##### Seppuku.Module.Proxy
Proxies requests to a specifed endpoint to one of two configurable endpoints. The endpoint chosen depends on whether the dead man's switch has been triggered or not. Good for sinkholing domains when the dead man's switch expires.
