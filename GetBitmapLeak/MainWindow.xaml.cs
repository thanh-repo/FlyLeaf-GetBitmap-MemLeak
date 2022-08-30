using FlyleafLib;
using FlyleafLib.MediaPlayer;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GetBitmapLeak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player Player;
        EngineConfig engineConfig;
        Config playerConfig;
        public MainWindow()
        {
            ThreadPool.GetMinThreads(out int workers, out int ports);
            ThreadPool.SetMinThreads(workers + 6, ports + 6);
            engineConfig = DefaultEngineConfig();
            Engine.Start(engineConfig);
            playerConfig = DefaultConfig();
            InitializeComponent();
            Player = new Player(playerConfig);
            playerConfig.Decoder.MaxVideoFrames = 60;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.AddFirewallRule();
            FlyleafPlayer.Player = Player;
            Player.VideoView.WinFormsHost.Focus();
            Player.OpenAsync("Heal The World (Official Video)-(480p).mp4");
            Task.Run(() =>
            {
                ExtractFrame();
            });
        }

        private void ExtractFrame()
        {
            while (true)
            {
                if (Player.Status == Status.Playing)
                {
                    Bitmap Frame = Player.renderer.GetBitmap();
                    if (Frame != null)
                    {
                        //Do something with frame;
                        Frame.Dispose();
                    }
                }
                Task.Delay(500);
            }
        }

        private EngineConfig DefaultEngineConfig()
        {
            EngineConfig engineConfig = new EngineConfig();

            engineConfig.PluginsPath = ":Plugins";
            engineConfig.FFmpegPath = ":FFmpeg";
            engineConfig.HighPerformaceTimers
                                        = false;
            engineConfig.UIRefresh = true;

#if RELEASE
            engineConfig.LogOutput      = "Flyleaf.FirstRun.log";
            engineConfig.LogLevel       = LogLevel.Debug;
            engineConfig.FFmpegDevices  = true;
#else
            engineConfig.LogOutput = ":debug";
            engineConfig.LogLevel = LogLevel.Debug;
            engineConfig.FFmpegLogLevel = FFmpegLogLevel.Warning;
#endif

            return engineConfig;
        }

        private Config DefaultConfig()
        {
            Config config = new();
            config.Subtitles.SearchLocal = true;
            config.Video.GPUAdapter = ""; // Set it empty so it will include it when we save it

            return config;
        }
    }
}
