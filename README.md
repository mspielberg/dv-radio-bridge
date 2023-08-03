# How To Use

1. Download and install [Virtual Audio Cable](https://vac.muzychenko.net).
1. Install the [AuRo Chrome extension](https://chrome.google.com/webstore/detail/auro-audio-output-device/hglnindfakmbhhkldompfjeknfapaceh).
1. Open a Chrome tab to Youtube, Spotify, etc.
1. Start some music playing.
1. Click the Extensions icon to the right of the address bar (puzzle piece icon).
1. Click the entry for `AuRo - audio output device router`.
1. Select `CABLE Input (VB-Audio Virtual Cable)`. Optionally, click `Save for this domain`.
1. You should now hear nothing.

Now to test your setup. This is optional.

1. Open the Windows Sound Control Panel. If on Windows 10 or later, there is a link to the Sound Control Panel on the right side of the Sound Settings window.
1. Select the Recording tab.
1. Scroll down to the `CABLE Output` device, right-click it and select Properties.
1. Select the `Listen` tab.
1. Select the `Listen to this device` checkbox and click `Apply`. You should now be hearing your music.
1. Deselect the `Listen to this device` checkbox and click `Apply`. The should now hear nothing.

Back to the game!

1. Load up Derail Valley, and grab your boombox.
1. Turn on the boombox, set it to Radio mode, and select the new "AudioBridge" radio station that appears after your other radio stations.
1. Enjoy the phat b34ts!

# How To Build

To build Radio Bridge from source, simply run the command `dotnet build RadioBridge.csproj`.
You'll need to do some setup first though in order to get that working.

## Assembly References

Create a filed called `Directory.Build.props` and paste the following code into it.
Adjust the paths as necessary to match your system's configuration.

```xml
<Project>
  <PropertyGroup>
    <Deterministic>true</Deterministic>
    <GenerateFullPaths>true</GenerateFullPaths>
  </PropertyGroup>

  <PropertyGroup>
    <AssemblySearchPaths>
      {CandidateAssemblyFiles};
      {HintPathFromItem};
      {TargetFrameworkDirectory};
      {RawFileName};
      C:\Program Files (x86)\Steam\steamapps\common\Derail Valley\DerailValley_Data\Managed\;
      C:\Program Files (x86)\Steam\steamapps\common\Derail Valley\DerailValley_Data\Managed\UnityModManager\
    </AssemblySearchPaths>
  </PropertyGroup>
</Project>
```
