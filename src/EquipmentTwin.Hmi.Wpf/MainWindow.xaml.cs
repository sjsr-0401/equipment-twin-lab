using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using EquipmentTwin.Hmi.Wpf.ViewModels;

namespace EquipmentTwin.Hmi.Wpf;

public partial class MainWindow : Window
{
    private readonly ScreenshotCaptureRequest? screenshotRequest;
    private bool screenshotCaptured;

    public MainWindow()
    {
        screenshotRequest = ScreenshotCaptureRequest.Parse(Environment.GetCommandLineArgs());
        if (screenshotRequest is not null)
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
        }

        InitializeComponent();

        var viewModel = new OperatorConsoleViewModel();
        if (screenshotRequest is not null)
        {
            PrepareCaptureState(viewModel, screenshotRequest);
        }

        DataContext = viewModel;

        if (screenshotRequest is not null)
        {
            if (screenshotRequest.State == "alarm")
            {
                WorkspaceTabs.SelectedIndex = 1;
            }

            WindowState = System.Windows.WindowState.Normal;
            Width = 1600;
            Height = 900;
            ShowInTaskbar = false;
            ShowActivated = false;
            ContentRendered += CaptureScreenshotOnContentRendered;
        }
    }

    private async void CaptureScreenshotOnContentRendered(object? sender, EventArgs e)
    {
        if (screenshotCaptured || screenshotRequest is null)
        {
            return;
        }

        screenshotCaptured = true;
        ContentRendered -= CaptureScreenshotOnContentRendered;

        var errorPath = $"{screenshotRequest.OutputPath}.error.txt";
        try
        {
            File.Delete(errorPath);
            await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
            await Task.Delay(250);
            await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
            await Dispatcher.InvokeAsync(static () => { }, DispatcherPriority.ApplicationIdle);

            SaveRootLayoutPng(screenshotRequest.OutputPath);
            Application.Current.Shutdown(0);
        }
        catch (Exception exception)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(errorPath)!);
            File.WriteAllText(errorPath, exception.ToString());
            Application.Current.Shutdown(1);
        }
    }

    private static void PrepareCaptureState(
        OperatorConsoleViewModel viewModel,
        ScreenshotCaptureRequest request)
    {
        viewModel.ResetCommand.Execute(null);

        switch (request.State)
        {
            case "load":
                break;
            case "process":
                Advance(viewModel, 1);
                break;
            case "alarm":
                viewModel.SelectedFaultScenario = "precursor-dose-timeout";
                viewModel.FaultReplayCommand.Execute(null);
                break;
            case "transfer-out":
                Advance(viewModel, viewModel.Steps.Count - 2);
                break;
            case "complete":
                Advance(viewModel, viewModel.Steps.Count - 1);
                break;
            default:
                throw new InvalidOperationException($"Unsupported capture state: {request.State}");
        }

        if (request.Language == "ko")
        {
            viewModel.ToggleLanguageCommand.Execute(null);
        }
    }

    private static void Advance(OperatorConsoleViewModel viewModel, int count)
    {
        for (var index = 0; index < count; index++)
        {
            viewModel.StepForwardCommand.Execute(null);
        }
    }

    private void SaveRootLayoutPng(string outputPath)
    {
        RootLayout.UpdateLayout();

        var width = Math.Max(1, (int)Math.Ceiling(RootLayout.ActualWidth));
        var height = Math.Max(1, (int)Math.Ceiling(RootLayout.ActualHeight));
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(RootLayout);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        using var stream = File.Create(outputPath);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        encoder.Save(stream);
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private sealed record ScreenshotCaptureRequest(string OutputPath, string State, string Language)
    {
        public static ScreenshotCaptureRequest? Parse(string[] arguments)
        {
            var outputPath = ReadValue(arguments, "--capture-screenshot");
            if (outputPath is null)
            {
                return null;
            }

            var state = (ReadValue(arguments, "--capture-state") ?? "process").ToLowerInvariant();
            if (state is not ("load" or "process" or "alarm" or "transfer-out" or "complete"))
            {
                throw new ArgumentException($"Unsupported --capture-state value: {state}");
            }

            var language = (ReadValue(arguments, "--capture-language") ?? "ko").ToLowerInvariant();
            if (language is not ("en" or "ko"))
            {
                throw new ArgumentException($"Unsupported --capture-language value: {language}");
            }

            return new ScreenshotCaptureRequest(Path.GetFullPath(outputPath), state, language);
        }

        private static string? ReadValue(string[] arguments, string option)
        {
            for (var index = 0; index < arguments.Length; index++)
            {
                if (!string.Equals(arguments[index], option, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (index + 1 >= arguments.Length || arguments[index + 1].StartsWith("--", StringComparison.Ordinal))
                {
                    throw new ArgumentException($"{option} requires a value.");
                }

                return arguments[index + 1];
            }

            return null;
        }
    }
}
