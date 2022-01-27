using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Storage;

using Path = System.IO.Path;

namespace VideoEffectWpfApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            Title = "Sample video effect";
            Task.Run(ProcessVideo);
        }

        private async Task ProcessVideo() {
            var args = new[] {
                //@"D:\Media\Metris\Other projects\2016\TouchScreens\videos\14-5-2021\00041.MTS",
                @"C:\Users\Herman Eldering\Videos\Captures\Hello Texture 2020-05-11 00-34-52_Trim.mp4",
                @"D:\Media\Metris\Other projects\2016\TouchScreens\videos\14-5-2021\00042.output.avi"
            };

            var vefdef = new VideoEffectDefinition("IBasicVideoEffectSample.MyVideoEffect");

            var task = Task.Run<MediaClip>(async () => {
                var videoInput = await StorageFile.GetFileFromPathAsync(args[0]);
                //var videoInput = await StorageFile.CreateStreamedFileAsync("streamfile", dataRequested, null);

                //void dataRequested(StreamedFileDataRequest request) {
                //    request.WriteAsync(
                //}


                //create clip from file
                var task = MediaClip.CreateFromFileAsync(videoInput);



                while (task.Status == Windows.Foundation.AsyncStatus.Started) {
                    await Task.Delay(100);
                }

                MediaClip clip = task.GetResults();
                return clip;
            });

            var outputFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(args[1]));
            var videoOutput = await outputFolder.CreateFileAsync(Path.GetFileName(args[1]), CreationCollisionOption.ReplaceExisting);

            var clip = task.Result;
            clip.VideoEffectDefinitions.Add(vefdef);
            if (clip.OriginalDuration > TimeSpan.FromSeconds(30)) clip.TrimTimeFromEnd = clip.OriginalDuration - TimeSpan.FromSeconds(30);

            //create composition
            var compositor = new MediaComposition();
            compositor.Clips.Add(clip);

            _media. = compositor;

            //var result = await compositor.RenderToFileAsync(videoOutput, MediaTrimmingPreference.Precise);

            Console.WriteLine($"No exception! 🥳");

            Dispatcher.Invoke(() => Title = "Done");
        }
    }
}
