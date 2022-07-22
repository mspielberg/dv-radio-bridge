using System;
using System.Net;
using UnityEngine;
using System.Threading.Tasks;

namespace DvMod.RadioBridge
{
    public class HttpServer : MonoBehaviour
    {
        private static GameObject? rootObject;
        private readonly HttpListener listener = new HttpListener();

        public async void Start()
        {
            if (!listener.IsListening)
            {
                listener.Prefixes.Add($"http://*:{Main.settings.serverPort}/");
                Main.DebugLog(() => $"Starting HTTP server on port {Main.settings.serverPort}");
                listener.Start();
            }

            while (listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync().ConfigureAwait(true);
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            HandleRequest(context);
                        }
                        catch (Exception e)
                        {
                            Main.DebugLog(() => $"Exception while handling HTTP request ({context.Request.Url}): {e}");
                            context.Response.Close();
                        }
                    });
                }
                catch (ObjectDisposedException e) when (e.ObjectName == "listener")
                {
                    // ignore when OnDestroy() is called to shutdown the server
                }
            }
        }

        public void OnDestroy()
        {
            if (listener.IsListening)
            {
                Main.DebugLog(() => "Stopping HTTP server");
                listener.Stop();
                listener.Prefixes.Clear();
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            var response = context.Response;
            response.ContentType = "audio/mpeg";
            response.AddHeader("icy-br", "320");
            response.AddHeader("icy-name", "RadioBridge");
            response.AddHeader("Server", $"RadioBridge/{Main.mod!.Info.Version}");
            //var fileOutput = File.Create(Path.Combine(Main.mod!.Path, $"output-{DateTime.Now:yyyyMMddTHHmmss}.mp3"));
            //var streams = new SplitStream(fileOutput, response.OutputStream);
            //var recording = new Recording(streams);
            var recording = new Recording(response.OutputStream);
            recording.BeginCapture();
        }

        public static void Create()
        {
            if (rootObject == null)
            {
                rootObject = new GameObject();
                GameObject.DontDestroyOnLoad(rootObject);
                rootObject.AddComponent<HttpServer>();
            }
        }

        public static void Destroy()
        {
            // ensure server shuts down immediately, not at the end of the frame
            GameObject.DestroyImmediate(rootObject);
            rootObject = null;
        }
    }
}
