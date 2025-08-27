using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace UserSetting.Controls.CustomUI;

/// <summary>
/// A custom window base class which extends the system Window class that can be used in WPF with C#.
/// </summary>
/// <example>
/// Example usage:
/// <code>
/// MaximizeWindowBase window = new MaximizeWindowBase();
/// window.Show();
/// </code>
/// </example>
/// <remarks>
/// It overrides the OnSourceInitialized method to handle the initialization of the window and adds a hook to the window procedure to handle messages.
/// It also provides a method to adjust the size and position of the window when it is maximized to fit the work area of the monitor it is on.
/// Main functionalities:
/// - Extends the Window class to create a custom window class.
/// - Overrides the OnSourceInitialized method to handle the initialization of the window and add a hook to the window procedure.
/// - Provides a method (WmGetMinMaxInfo) to adjust the size and position of the window when it is maximized to fit the work area of the monitor it is on.
/// - Defines several struct types (POINT, MinMaxInfo, MonitorInfo, RECT) to represent window and monitor information.
/// </remarks>
/// <seealso cref="Window"/>
/// <reference>
/// "https://stackoverflow.com/a/2588909/5352166"
/// </reference>
public class MaximizeWindowBase : Window
{
    /// <summary>
    /// Method that overrides the base method in the Window class. Initializes the window source and adds a hook to the window procedure.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var handle = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
    }

    /// <summary>
    /// Handles messages sent to a window.
    /// </summary>
    /// <param name="hwnd">The handle of the window that received the message.</param>
    /// <param name="msg">The message identifier.</param>
    /// <param name="wParam">Additional message parameter.</param>
    /// <param name="lParam">Additional message parameter.</param>
    /// <param name="handled">A boolean value indicating whether the message has been handled.</param>
    /// <returns>The return value of the method, which is always IntPtr.Zero.</returns>
    private static IntPtr WindowProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        switch (msg)
        {
            case 0x0024:
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
                break;
        }

        return IntPtr.Zero;
    }

    /// <summary>
    /// Adjusts the size and position of a window when it is maximized to fit the work area of the monitor it is on.
    /// </summary>
    /// <param name="hwnd">The handle of the window.</param>
    /// <param name="lParam">A pointer to the structure containing window information.</param>
    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        var mmi = (MinMaxInfo)Marshal.PtrToStructure(lParam, typeof(MinMaxInfo))!;

        // Adjust the maximized size and position to fit the work area of the correct monitor
        var MONITOR_DEFAULT_TONE_REST = 0x00000002;
        var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULT_TONE_REST);

        if (monitor != IntPtr.Zero)
        {
            MonitorInfo monitorInfo = new();
            GetMonitorInfo(monitor, monitorInfo);
            var rcWorkArea = monitorInfo.rcWork;
            var rcMonitorArea = monitorInfo.rcMonitor;
            mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
            mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
            mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
            mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
        }

        Marshal.StructureToPtr(mmi, lParam, true);
    }


    /// <summary>
    /// Represents a point in a two-dimensional coordinate system.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// The x coordinate of the point.
        /// </summary>
        public int x;

        /// <summary>
        /// The y coordinate of the point.
        /// </summary>
        public int y;

        /// <summary>
        /// Initializes a new instance of the POINT struct with the specified x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// Defines two struct types, MinMaxInfo and MonitorInfo, using the StructLayout attribute to specify the layout of the struct members in memory.
    /// </summary>
    /// <example>
    /// <code>
    /// MinMaxInfo minMaxInfo = new MinMaxInfo();
    /// MonitorInfo monitorInfo = new MonitorInfo();
    /// </code>
    /// </example>
    [StructLayout(LayoutKind.Sequential)]
    public struct MinMaxInfo
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    /// <summary>
    /// Represents monitor information using the StructLayout attribute to specify the layout of the struct members in memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MonitorInfo
    {
        public int cbSize = Marshal.SizeOf(typeof(MonitorInfo));

        public readonly RECT rcMonitor = new();

        public readonly RECT rcWork = new();

        public int dwFlags = 0;
    }

    /// <summary>
    /// Represents a rectangle with four integer properties: left, top, right, and bottom.
    /// Provides properties and methods to calculate the width, height, and emptiness of the rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct RECT
    {

        public int left;

        public int top;

        public int right;

        public int bottom;


        private static readonly RECT Empty = new();

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        public readonly int Width
        {
            get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
        }

        /// <summary>
        /// Gets the height of the rectangle.
        /// </summary>
        public readonly int Height
        {
            get { return bottom - top; }
        }


        /// <summary>
        /// Initializes a new instance of the RECT struct with the specified coordinates.
        /// </summary>
        /// <param name="left">The left coordinate of the rectangle.</param>
        /// <param name="top">The top coordinate of the rectangle.</param>
        /// <param name="right">The right coordinate of the rectangle.</param>
        /// <param name="bottom">The bottom coordinate of the rectangle.</param>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }


        /// <summary>
        /// Initializes a new instance of the RECT struct with the coordinates from another RECT instance.
        /// </summary>
        /// <param name="rcSrc">The source RECT instance.</param>
        public RECT(RECT rcSrc)
        {
            left = rcSrc.left;
            top = rcSrc.top;
            right = rcSrc.right;
            bottom = rcSrc.bottom;
        }

        /// <summary>
        /// Gets a value indicating whether the rectangle is empty.
        /// </summary>
        public readonly bool IsEmpty
        {
            get
            {
                //Todo: BugFix on Bidi OS (hebrew arabic) left > right
                return left >= right || top >= bottom;
            }
        }
        /// <summary>
        /// Return a user-friendly representation of this struct
        /// </summary>
        public readonly override string ToString()
        {
            if (this == Empty) { return "RECT {Empty}"; }
            return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
        }

        /// <summary>
        /// Determine if 2 RECT are equal (deep compare)
        /// </summary>
        public readonly override bool Equals(object? obj)
        {
            if (obj is not Rect) { return false; }
            return this == (RECT)obj;
        }

        /// <summary>
        /// Return the HashCode for this struct (not guaranteed to be unique)
        /// </summary>
        public readonly override int GetHashCode()
        {
            return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
        }


        /// <summary>
        /// Determine if 2 RECT are equal (deep compare)
        /// </summary>
        public static bool operator ==(RECT rect1, RECT rect2)
        {
            return rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom;
        }

        /// <summary>
        /// Determine if 2 RECT are different(deep compare)
        /// </summary>
        public static bool operator !=(RECT rect1, RECT rect2)
        {
            return !(rect1 == rect2);
        }

    }

    [DllImport("user32")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

    [DllImport("User32")]
    private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
}
