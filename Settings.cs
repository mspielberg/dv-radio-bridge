using UnityModManagerNet;

namespace DvMod.RadioBridge
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("HTTP server port")]
        public int serverPort = 7100;
        [Draw("Enable logging")]
        public bool enableLogging = false;

        public readonly string? version = Main.mod?.Info.Version;
        
        public Settings()
        {
        }

        public void OnChange()
        {
        }
    }
}
