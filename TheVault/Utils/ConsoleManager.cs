using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace TheVault.Utils
{
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32DllName = "kernel32.dll";
        private const string User32DllName = "user32.dll";
        private const int SwpNoZOrder = 0x4;
        private const int SwpNoActivate = 0x10;
        
        public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;
    
        #region DLL Imports
    
        [DllImport(Kernel32DllName)]
        private static extern bool AllocConsole();
    
        [DllImport(Kernel32DllName)]
        private static extern bool FreeConsole();
    
        [DllImport(Kernel32DllName)]
        private static extern IntPtr GetConsoleWindow();
        
        [DllImport(User32DllName)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);
    
        #endregion DLL imports
        
        #region Public methods
        
        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show(int left, int top)
        {
            //#if DEBUG
            if (HasConsole) return;
            AllocConsole();
            InvalidateOutAndError();
            SetConsoleInformation(left, top);
            //#endif
        }
    
        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible.
        /// Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            //#if DEBUG
            if (!HasConsole) return;
            SetOutAndErrorNull();
            FreeConsole();
            //#endif
        }

        public static void WriteLine(string value)
        {
            Console.WriteLine($@"[DEBUG] {value}");
        }
    
        #endregion Public methods
        
        #region Private methods
        
        private static void InvalidateOutAndError()
        {
            var type = typeof(Console);
            var _out = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            var error = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            var initializeStdOutError = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
    
            Debug.Assert(_out != null);
            Debug.Assert(error != null);
            Debug.Assert(initializeStdOutError != null);
    
            _out.SetValue(null, null);
            error.SetValue(null, null);
    
            initializeStdOutError.Invoke(null, new object[] { true });
        }
    
        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
        
        private static void SetConsoleInformation(int x, int y)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            // ReSharper disable once LocalizableElement
            Console.Title = "TheVault Output";
            
            var height = 300;
            var width = 700;
            Console.BufferWidth = 80;
            Console.BufferHeight = 300;
            SetWindowPos(GetConsoleWindow(), 
                        IntPtr.Zero, x, y, 
                        width, height, 
                        SwpNoZOrder | SwpNoActivate);
        }
        
        #endregion Private methods
    } 
}