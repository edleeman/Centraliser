using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CentraliserLogicTest.Services
{
    public class CentraliseService : Form
    {
        public CentraliseService()
        {
            var id = 0;
            GetDesktop();
            GetCurrentWindow();
            RegisterHotKey(this.Handle, id, 8, Keys.C.GetHashCode());
            GetWindowRect(_desktop, ref _desktopRect);
            GetWindowRect(_currentWindow, ref _windowRect);
        }

        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
            /// contents of the client area are saved and copied back into the client area after the window is sized or 
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }


        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        const int WM_GETTEXT = 0xD;

        IntPtr _currentWindow;
        IntPtr _desktop;
        Rect _windowRect = new Rect();
        Rect _desktopRect = new Rect();

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == Keys.C.GetHashCode())
            {
                Console.WriteLine("HotkeyPressed");
                Calculate();
            }
        }

        public void GetDesktop()
        {
            _desktop = GetDesktopWindow();
        }

        public void GetCurrentWindow()
        {
            _currentWindow = GetForegroundWindow();
        }

        public int GetScreenHeight()
        {
            return _desktopRect.Bottom - _desktopRect.Top;
        }

        public int GetScreenWidth()
        {
            return _desktopRect.Right - _desktopRect.Left;
        }

        public int GetWindowHeight()
        {
            return _windowRect.Bottom - _windowRect.Top;
        }

        public int GetWindowWidth()
        {
            return _windowRect.Right - _windowRect.Left;
        }

        public decimal GetXPosition()
        {
            return _windowRect.Left;
        }

        public decimal GetYPosition()
        {
            return _windowRect.Top;
        }

        public string GetWindowTitle()
        {
            StringBuilder sb = new StringBuilder(60000);
            SendMessage(_currentWindow, WM_GETTEXT, (IntPtr)sb.Capacity, sb);

            return sb.ToString();
        }

        public void Calculate()
        {
            Console.WriteLine($"Window Title: {GetWindowTitle()}");
            Console.WriteLine($"Window Height: {GetWindowHeight()}");
            Console.WriteLine($"Window Width: {GetWindowWidth()}");
            Console.WriteLine($"Desktop Width: {GetScreenHeight()}");
            Console.WriteLine($"Desktop Width: {GetScreenWidth()}");

            Console.WriteLine($"Window X: {GetXPosition()}");
            Console.WriteLine($"Window Y: {GetYPosition()}");
        }

        public void Centralise()
        {
            var screenWidth = GetScreenWidth();
            var screenHeight = GetScreenHeight();
            var windowWidth = GetWindowWidth();
            var windowHeight = GetWindowHeight();

            int centerX = (screenWidth - windowWidth) / 2;
            int centerY = (screenHeight - windowHeight) / 2;

            SetWindowPos(_currentWindow, (IntPtr)0, centerX, centerY, windowWidth, windowHeight, SetWindowPosFlags.ShowWindow);
        }
    }
}
