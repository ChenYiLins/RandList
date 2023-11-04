using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using RandList.Helpers;
using Windows.Graphics;
using Windows.UI.ViewManagement;

namespace RandList;

public sealed partial class MainWindow : WindowEx
{
    //**************************
    //定义SetWindowLong函数
    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static partial int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);
    //**************************
    //定义GetWindowLong函数
    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static partial int GetWindowLong(IntPtr hWnd, int nIndex);
    //**************************
    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    //**************************

    //**************************
    //定义全局变量
    public static class GlobalVar
    {
        public static IntPtr hWnd;
    }
    //**************************
    //定义常量
    public const int GWL_STYLE = -16;
    public const long MAXIMIZEBOX = 0x00010000L;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 19;
    //**************************

    public MainWindow()
    {
        InitializeComponent();
        //*************************************************************************************
        // Use 'this' rather than 'window' as variable if this is about the current window.获取窗口句柄
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        GlobalVar.hWnd = hWnd;
        Debug.WriteLine("***************");//测试代码
        Debug.WriteLine("hWnd:" + Convert.ToString(hWnd));//测试代码
        Debug.WriteLine("***************");//测试代码
        //*************************************************************************************
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        //*************************************************************************************
        long style = GetWindowLong(hWnd, GWL_STYLE); //获取窗口风格
        style &= ~(MAXIMIZEBOX); //禁用最大化按钮
        SetWindowLong(hWnd, GWL_STYLE, style); //设置窗口风格
        AppWindow.MoveAndResize(new RectInt32 { X = 100, Y = 100, Width = 600, Height = 400 });
        //*************************************************************************************
        var uiSettings = new UISettings();
        var color = uiSettings.GetColorValue(UIColorType.Background);

        var useImmersiveDarkMode = 0;
        if (color == Microsoft.UI.Colors.Black)
        {
            // Dark mode is enabled
            useImmersiveDarkMode = 1;
            DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int));

        }
        else
        {
            // Light mode is enabled
            useImmersiveDarkMode = 0;
            DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int));
        }
        //*************************************************************************************

    }
}
