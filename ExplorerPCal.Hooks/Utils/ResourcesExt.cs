using EasyHook;
using System;

namespace ExplorerPCal.Hooks.Utils
{
    public static class ResourcesExt
    {
        public static void ForceDispose(this IDisposable resource)
        {
            if (resource == null)
                return;

            try
            {
                resource.Dispose();
                resource = null;
            }
            catch
            { }
        }

        public static LocalHook RegisterHook(this object callback, string moduleName, string method, Delegate interceptor)
        {
            var handle = NativeAPI.LoadLibrary(moduleName);
            if (handle == IntPtr.Zero)
                return null;

            var hook = LocalHook.Create(
                InTargetProc: LocalHook.GetProcAddress(moduleName, method),
                InNewProc: interceptor,
                InCallback: callback);

            // all threads are intercepted                
            hook.ThreadACL.SetExclusiveACL(new Int32[1]);

            NativeAPI.CloseHandle(handle);
            return hook;
        }
    }
}