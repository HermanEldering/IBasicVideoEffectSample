using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IBasicVideoEffectSample;

using SkiaSharp;

namespace VideoEffectConsoleApp {
    internal class RatpadVideoEffect {
        readonly static TimeSpan _progressInterval = TimeSpan.FromSeconds(10);

        SKBitmap _bitmap;
        SKImageInfo _bitmapInfo;
        TimeSpan _progressTime = _progressInterval;

        public RatpadVideoEffect() {
            _bitmap = new SKBitmap();
            _bitmapInfo = _bitmap.Info;
        }

        public void Process(IntPtr framebuffer, TimeSpan time, int width, int height, int stride) {
            if (time > _progressTime) {
                Console.WriteLine($"Time: {_progressTime}");
                _progressTime += _progressInterval;
            }

            if (_bitmap.Width != width || _bitmap.Height != height) {
                _bitmapInfo = new SKImageInfo() {
                    Width = width,
                    Height = height,
                    ColorType = SKColorType.Bgra8888,
                    AlphaType = SKAlphaType.Opaque
                };
            }
            _bitmap.InstallPixels(_bitmapInfo, framebuffer);
            using (SKCanvas canvas = new SKCanvas(_bitmap)) {
                ProcessWithSkia(canvas, time);
            }
        }

        public void ProcessWithSkia(SKCanvas canvas, TimeSpan time) {
            // there is a line in program.cs that can be commented/uncommented to process the whole video or just the first 30 seconds

            using SKPaint textPaint = new SKPaint {
                TextSize = 48, Color = SKColors.Red,
                Typeface = SKTypeface.FromFamilyName(
                    "Arial",
                    SKFontStyleWeight.Bold,
                    SKFontStyleWidth.Normal,
                    SKFontStyleSlant.Upright)
            };

            canvas.DrawText($"Hello world! The time is {time}", _bitmap.Width / 4, _bitmap.Height / 2, textPaint);
            canvas.DrawRect(0, _bitmap.Height, (int)(time.TotalMinutes * _bitmap.Width), -20, textPaint);
        }
    }
}
