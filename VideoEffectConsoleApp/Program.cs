// See https://aka.ms/new-console-template for more information
//using IBasicVideoEffectSample;

using IBasicVideoEffectSample;

using VideoEffectConsoleApp;

using Windows.Foundation.Collections;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Storage;

class Program {
    [STAThread]
    async static Task Main(string[] args) {
        Console.WriteLine($"Initializing...");// {typeof(MyVideoEffect).AssemblyQualifiedName}");

        if (args.Length == 0) {
            var baseDir = Environment.CurrentDirectory;

            args = new[] {
                //@"D:\Media\Metris\Other projects\2016\TouchScreens\videos\14-5-2021\00041.MTS",
                Path.Combine(baseDir, @"00042.mp4"),
                Path.Combine(baseDir, @"output.mp4")
            };
        }

        var ratpadEffect = new RatpadVideoEffect();
        Action<IntPtr, TimeSpan, int, int, int> action = ratpadEffect.Process;

        var properties = new PropertySet();
        properties.Add(MyVideoEffect.SimpleFilterKey, action);

        var vefdef = new VideoEffectDefinition(typeof(MyVideoEffect).FullName, properties);

        var videoInput = await StorageFile.GetFileFromPathAsync(args[0]);
        var outputFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(args[1]));
        var videoOutput = await outputFolder.CreateFileAsync(Path.GetFileName(args[1]), CreationCollisionOption.ReplaceExisting);


        //create clip from file
        var clip = await MediaClip.CreateFromFileAsync(videoInput);
        clip.VideoEffectDefinitions.Add(vefdef);

        // Uncomment next line to limit processing to first 30 seconds
        if (clip.OriginalDuration > TimeSpan.FromSeconds(30)) clip.TrimTimeFromEnd = clip.OriginalDuration - TimeSpan.FromSeconds(30);

        //create composition
        var compositor = new MediaComposition();
        compositor.Clips.Add(clip);

        Console.WriteLine($"Rendering...");
        var result = await compositor.RenderToFileAsync(videoOutput, MediaTrimmingPreference.Precise);

        Console.WriteLine($"Done!");
    }
}