// See https://aka.ms/new-console-template for more information
//using IBasicVideoEffectSample;

using Windows.Media.Effects;

Console.WriteLine($"Hello, World! ");// {typeof(MyVideoEffect).AssemblyQualifiedName}");


var vefdef = new VideoEffectDefinition("IBasicVideoEffectSample.MyVideoEffect");

Console.WriteLine($"No exception! 🥳");