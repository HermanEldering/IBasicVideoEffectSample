using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace IBasicVideoEffectSample {
    public sealed class FrameInfo {
        public TimeSpan Time;
        public int Width, Height, Stride;
    }

    //[ComVisible(true)]
    //[Guid("6B0A6E4E-896C-4944-978B-43738D42AE24")]
    //[ProgId("IBasicVideoEffectSample.MyVideoEffect")]
    public sealed class MyVideoEffect: IBasicVideoEffect {
        public static string Version => "1";
        public const string SimpleFilterKey = nameof(SimpleFilterKey);

        private Action<IntPtr, TimeSpan, int, int, int> _simpleEffect;

        public bool IsReadOnly => false;
        public MediaMemoryTypes SupportedMemoryTypes => MediaMemoryTypes.Cpu;

        public bool TimeIndependent => true;

        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties {
            get {
                var encodingProperties = new VideoEncodingProperties();
                encodingProperties.Subtype = "ARGB32";
                return new List<VideoEncodingProperties>() { encodingProperties };

                // If the list is empty, the encoding type will be ARGB32.
                // return new List<VideoEncodingProperties>();
            }
        }

        public void Close(MediaEffectClosedReason reason) {
        }

        public void DiscardQueuedFrames() {
        }

        public void ProcessFrame(ProcessVideoFrameContext context) {
            context.InputFrame.SoftwareBitmap.CopyTo(context.OutputFrame.SoftwareBitmap);

            if (_simpleEffect is not null) {
                using (GetBufferPtr(context.OutputFrame, out var outBuffer, out var bufferLayout)) {
                    var info = new FrameInfo() {
                        Time = context.InputFrame.RelativeTime ?? TimeSpan.MinValue,
                        Width = bufferLayout.Width,
                        Height = bufferLayout.Height,
                        Stride = bufferLayout.Stride,
                    };

                    //_simpleEffect.Process(outBuffer, info);
                    _simpleEffect(outBuffer, info.Time, info.Width, info.Height, info.Stride);
                    return;
                }
            }

            using (GetBuffer(context.InputFrame, out var inBuffer, out var bufferLayout))
            using (GetBuffer(context.OutputFrame, out var outBuffer, out var _)) {
                
                for (int i = 0; i < bufferLayout.Height / 2; i++) {
                    for (int j = 0; j < bufferLayout.Width; j++) {

                        byte value = (byte)((float)j / bufferLayout.Width * 255);

                        int bytesPerPixel = 4;
                        //if (encodingProperties.Subtype != "ARGB32") {
                        //    // If you support other encodings, adjust index into the buffer accordingly
                        //}
                        var fadeValue = 0.5f;

                        int idx = bufferLayout.StartIndex + bufferLayout.Stride * i + bytesPerPixel * j;

                        outBuffer[idx + 0] = (byte)(fadeValue * (float)inBuffer[idx + 0]);
                        outBuffer[idx + 1] = (byte)(fadeValue * (float)inBuffer[idx + 1]);
                        outBuffer[idx + 2] = (byte)(255 - fadeValue * (float)inBuffer[idx + 2]);
                        outBuffer[idx + 3] = inBuffer[idx + 3];
                    }
                }
            }

            //object temp = null;
            //var method = reference.GetType().GetMethod("As");
            //var genmethod = method.MakeGenericMethod(typeof(IMemoryBufferByteAccess));
            //var result = genmethod.Invoke(reference, Array.Empty<object>());


            //var bufbyte = ((IMemoryBufferByteAccess)result);
            //bufbyte.GetBuffer(out dataInBytes, out capacity);
            //using (var buffer = context.OutputFrame.SoftwareBitmap.LockBuffer(BitmapBufferAccessMode.Write)) {
            //    using (var inbuffer = context.InputFrame.SoftwareBitmap.LockBuffer(BitmapBufferAccessMode.Read)) {
            //        buffer.
            //    }
            //}
        }

        private static unsafe IDisposable GetBuffer(VideoFrame frame, out Span<byte> buffer, out BitmapPlaneDescription bufferLayout) {
            var softwareBitmap = frame.SoftwareBitmap;
            var lockbuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
            var reference = lockbuffer.CreateReference();
            var rtObject = (WinRT.IWinRTObject)reference;
            var nativeObject = rtObject.NativeObject;
            var byteAccess = nativeObject.AsInterface<IMemoryBufferByteAccess>();

            byte* dataInBytes;
            uint capacity;
            byteAccess.GetBuffer(out dataInBytes, out capacity);

            bufferLayout = lockbuffer.GetPlaneDescription(0);
            buffer = new Span<byte>(dataInBytes + bufferLayout.StartIndex, bufferLayout.Stride * bufferLayout.Height);

            return new CompositeDisposable(reference, softwareBitmap);
        }

        private static unsafe IDisposable GetBufferPtr(VideoFrame frame, out IntPtr buffer, out BitmapPlaneDescription bufferLayout) {
            var softwareBitmap = frame.SoftwareBitmap;
            var lockbuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
            var reference = lockbuffer.CreateReference();
            var rtObject = (WinRT.IWinRTObject)reference;
            var nativeObject = rtObject.NativeObject;
            var byteAccess = nativeObject.AsInterface<IMemoryBufferByteAccess>();

            byte* dataInBytes;
            uint capacity;
            byteAccess.GetBuffer(out dataInBytes, out capacity);

            bufferLayout = lockbuffer.GetPlaneDescription(0);
            //buffer = new Span<byte>(dataInBytes + bufferLayout.StartIndex, bufferLayout.Stride * bufferLayout.Height);
            buffer = new IntPtr(dataInBytes + bufferLayout.StartIndex);

            return new CompositeDisposable(reference, softwareBitmap);
        }

        public unsafe void ProcessFrameX(ProcessVideoFrameContext context) {
            using (var buffer = context.OutputFrame.SoftwareBitmap.LockBuffer(BitmapBufferAccessMode.Write)) {
                using (var reference = buffer.CreateReference()) {
                    byte* dataInBytes;
                    uint capacity;
                    var bufbyte = ((IMemoryBufferByteAccess)reference);
                    bufbyte.GetBuffer(out dataInBytes, out capacity);

                    BitmapPlaneDescription bufferLayout = buffer.GetPlaneDescription(0);
                    for (int i = 0; i < bufferLayout.Height / 2; i++) {
                        for (int j = 0; j < bufferLayout.Width; j++) {

                            byte value = (byte)((float)j / bufferLayout.Width * 255);

                            int bytesPerPixel = 4;
                            //if (encodingProperties.Subtype != "ARGB32") {
                            //    // If you support other encodings, adjust index into the buffer accordingly
                            //}
                            var fadeValue = 0.5f;

                            int idx = bufferLayout.StartIndex + bufferLayout.Stride * i + bytesPerPixel * j;

                            dataInBytes[idx + 0] = (byte)(fadeValue * (float)dataInBytes[idx + 0]);
                            dataInBytes[idx + 1] = (byte)(fadeValue * (float)dataInBytes[idx + 1]);
                            //dataInBytes[idx + 2] = (byte)(fadeValue * (float)dataInBytes[idx + 2]);
                            dataInBytes[idx + 3] = dataInBytes[idx + 3];
                        }
                    }
                }
            }
        }

        private VideoEncodingProperties encodingProperties;
        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device) {
            this.encodingProperties = encodingProperties;
        }

        
        private IPropertySet configuration;
        public void SetProperties(IPropertySet configuration) {
            this.configuration = configuration;
            if (configuration.TryGetValue(SimpleFilterKey, out var effect)) _simpleEffect = (Action<IntPtr, TimeSpan, int, int, int>)effect;
        }
    }
}
