using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms; // Necessário adicionar referência no projeto real ou usar System.Windows.Input
using BSOptimizerPro.Utils;

namespace BSOptimizerPro.Services
{
    public class SnapTapService
    {
        private NativeMethods.LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private bool _isActive = false;

        private bool _isAPressed = false;
        private bool _isDPressed = false;

        public SnapTapService()
        {
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_isActive) return;
            _hookID = SetHook(_proc);
            _isActive = true;
        }

        public void Stop()
        {
            if (!_isActive) return;
            NativeMethods.UnhookWindowsHookEx(_hookID);
            _isActive = false;
        }

        private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(13, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                uint msg = (uint)wParam;

                if (vkCode == (int)Keys.A)
                {
                    if (msg == 0x0100) // WM_KEYDOWN
                    {
                        _isAPressed = true;
                        if (_isDPressed) ReleaseKey(Keys.D); // Se apertou A e D estava seguro, solta o D
                    }
                    else if (msg == 0x0101) // WM_KEYUP
                    {
                        _isAPressed = false;
                    }
                }
                else if (vkCode == (int)Keys.D)
                {
                    if (msg == 0x0100) // WM_KEYDOWN
                    {
                        _isDPressed = true;
                        if (_isAPressed) ReleaseKey(Keys.A); // Se apertou D e A estava seguro, solta o A
                    }
                    else if (msg == 0x0101) // WM_KEYUP
                    {
                        _isDPressed = false;
                    }
                }
            }
            return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void ReleaseKey(Keys key)
        {
            NativeMethods.keybd_event((byte)key, 0, 0x0002, 0); // KEYEVENTF_KEYUP
        }
    }
}
