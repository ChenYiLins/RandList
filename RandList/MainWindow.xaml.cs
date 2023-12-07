using IniParser;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using RandList.Helpers;
using Windows.Graphics;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;

namespace RandList;

public sealed partial class MainWindow : WindowEx
{
    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private UISettings settings;

    // 获取程序运行路径
    public static readonly string PathApp = AppDomain.CurrentDomain.BaseDirectory;

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        Content = null;
        Title = "AppDisplayName".GetLocalized();

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event

        if (File.Exists(PathApp + "AppWindow.ini"))
        {
            var parser = new FileIniDataParser();
            var iniFile = parser.ReadFile(PathApp + "AppWindow.ini");

            if (iniFile["AppWindow"]["PositionX"] != "")
            {
                var rect = new RectInt32
                {
                    X = Convert.ToInt32(iniFile["AppWindow"]["PositionX"]),
                    Y = Convert.ToInt32(iniFile["AppWindow"]["PositionY"]),
                    Width = Convert.ToInt32(iniFile["AppWindow"]["SizeWidth"]),
                    Height = Convert.ToInt32(iniFile["AppWindow"]["SizeHeight"])
                };
                AppWindow.MoveAndResize(rect);
            }
            Closed += MainWindow_Closed;
        }

    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        // 保存窗口状态
        if (File.Exists(PathApp + "AppWindow.ini"))
        {
            var parser = new FileIniDataParser();
            var iniFile = parser.ReadFile(PathApp + "AppWindow.ini");

            iniFile["AppWindow"]["PositionX"] = AppWindow.Position.X.ToString();
            iniFile["AppWindow"]["PositionY"] = AppWindow.Position.Y.ToString();
            iniFile["AppWindow"]["SizeWidth"] = AppWindow.Size.Width.ToString();
            iniFile["AppWindow"]["SizeHeight"] = AppWindow.Size.Height.ToString();
            parser.WriteFile(PathApp + "AppWindow.ini", iniFile);
        }
    }
}
