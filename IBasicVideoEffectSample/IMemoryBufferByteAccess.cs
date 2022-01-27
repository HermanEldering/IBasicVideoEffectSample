using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IBasicVideoEffectSample {
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[ContractVersion(typeof(UniversalApiContract), 65536)]
    //[global::System.Runtime.InteropServices.Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    //[WinRT.WindowsRuntimeType("Windows.Foundation.UniversalApiContract")]
    unsafe interface IMemoryBufferByteAccess {
        void GetBuffer(out byte* buffer, out uint capacity);
    }
}
