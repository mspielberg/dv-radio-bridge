using DV.Radio;
using HarmonyLib;
using PlaylistsNET.Models;
using System.Linq;

namespace DvMod.RadioBridge
{
    [HarmonyPatch(typeof(PlaylistPlayer), nameof(PlaylistPlayer.TryGetPlaylist))]
    public static class TryGetPlaylistPatch
    {
        public static void Postfix(ref IBasePlaylist playlist, ref bool __result)
        {
            if (!__result || !(playlist is PlsPlaylist pls))
                return;
            PlsPlaylistEntry entry = new PlsPlaylistEntry()
            {
                Title = Main.mod!.Info.DisplayName,
                Path = $"http://localhost:{Main.settings.serverPort}",
                Nr = pls.PlaylistEntries.Last().Nr + 1,
            };
            pls.PlaylistEntries.Add(entry);
        }
    }
}
