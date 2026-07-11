using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EquipmentTwin.Hmi.Wpf.Services;

internal static class WindowScreenshotService
{
    private const int TargetWidth = 1600;
    private const int TargetHeight = 900;

    public static void SavePng(Window window, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(window);

        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            throw new InvalidOperationException("The WPF window handle is not available.");
        }

        if (!GetClientRect(handle, out var clientRect))
        {
            throw new InvalidOperationException("The WPF client area could not be measured.");
        }

        var clientOrigin = new NativePoint();
        if (!ClientToScreen(handle, ref clientOrigin))
        {
            throw new InvalidOperationException("The WPF client origin could not be located.");
        }

        var clientWidth = clientRect.Right - clientRect.Left;
        var clientHeight = clientRect.Bottom - clientRect.Top;
        var workingArea = System.Windows.Forms.Screen.FromHandle(handle).WorkingArea;
        clientWidth = Math.Min(clientWidth, workingArea.Right - clientOrigin.X);
        clientHeight = Math.Min(clientHeight, workingArea.Bottom - clientOrigin.Y);
        if (clientWidth <= 0 || clientHeight <= 0)
        {
            throw new InvalidOperationException("The visible WPF client area is empty.");
        }

        using var clientBitmap = new System.Drawing.Bitmap(
            clientWidth,
            clientHeight,
            PixelFormat.Format32bppArgb);
        using (var graphics = System.Drawing.Graphics.FromImage(clientBitmap))
        {
            graphics.CopyFromScreen(
                clientOrigin.X,
                clientOrigin.Y,
                0,
                0,
                new System.Drawing.Size(clientWidth, clientHeight),
                System.Drawing.CopyPixelOperation.SourceCopy);
        }

        using var outputBitmap = Resize(clientBitmap);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        outputBitmap.Save(outputPath, ImageFormat.Png);
    }

    private static System.Drawing.Bitmap Resize(System.Drawing.Bitmap source)
    {
        var output = new System.Drawing.Bitmap(TargetWidth, TargetHeight, PixelFormat.Format32bppArgb);
        output.SetResolution(96, 96);

        using var graphics = System.Drawing.Graphics.FromImage(output);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.DrawImage(
            source,
            new System.Drawing.Rectangle(0, 0, TargetWidth, TargetHeight),
            0,
            0,
            source.Width,
            source.Height,
            System.Drawing.GraphicsUnit.Pixel);

        return output;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(IntPtr window, out NativeRect rect);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ClientToScreen(IntPtr window, ref NativePoint point);
}
