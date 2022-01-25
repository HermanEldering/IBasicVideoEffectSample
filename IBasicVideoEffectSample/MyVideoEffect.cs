using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace IBasicVideoEffectSample {
    //[ComVisible(true)]
    //[Guid("6B0A6E4E-896C-4944-978B-43738D42AE24")]
    //[ProgId("IBasicVideoEffectSample.MyVideoEffect")]
    public sealed class MyVideoEffect: IBasicVideoEffect {
        public static string Version => "1";

        public bool IsReadOnly => true;
        public MediaMemoryTypes SupportedMemoryTypes => MediaMemoryTypes.GpuAndCpu;

        public bool TimeIndependent => true;

        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties {
            get {
                List<VideoEncodingProperties> props = new List<VideoEncodingProperties>();
                return props.AsReadOnly();
            }
        }

        public void Close(MediaEffectClosedReason reason) {
        }

        public void DiscardQueuedFrames() {
        }

        public void ProcessFrame(ProcessVideoFrameContext context) {
        }

        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device) {
        }

        public void SetProperties(IPropertySet configuration) {
        }
    }
}
