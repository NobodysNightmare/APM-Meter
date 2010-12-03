using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace APM_Meter
{
    class KeyHookInterface
    {
        private const int WH_KEYBOARD = 2;
        private const int WH_MOUSE = 7;

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(int idHook);

        public static bool Started {get; private set;}

        private static IntPtr DllInst;
        private static IntPtr KeyboardProc;
        private static IntPtr MouseProc;

        private static int KeyboardHook;
        private static int MouseHook;

        public static void StartHook()
        {
            if (Started)
                return;

            DllInst = LoadLibrary("APMKeyHook.dll");
            if(DllInst == IntPtr.Zero)
                throw new Exception("Couldn't load APMKeyHook.dll");

            KeyboardProc = GetProcAddress(DllInst, "?KeyboardProc@APMKeyHook@@SGJHIJ@Z");
            MouseProc = GetProcAddress(DllInst, "?MouseProc@APMKeyHook@@SGJHIJ@Z");
            if (KeyboardProc == IntPtr.Zero || MouseProc == IntPtr.Zero)
                throw new Exception("Couldn't find procedures in the dll");

            KeyboardHook = SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, DllInst, 0);
            MouseHook = SetWindowsHookEx(WH_MOUSE, MouseProc, DllInst, 0);
            if(KeyboardHook == 0 || MouseHook == 0)
                throw new Exception("Couldn't register hooks.");

            Started = true;
        }

        public static void StopHook()
        {
            if (!Started)
                return;

            UnhookWindowsHookEx(KeyboardHook);
            UnhookWindowsHookEx(MouseHook);
            FreeLibrary(DllInst);

            Started = false;
        }

        public static void ToogleHook()
        {
            if (Started)
                StopHook();
            else
                StartHook();
        }
    }
}
