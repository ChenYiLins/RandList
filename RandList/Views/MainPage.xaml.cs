using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using IniParser;
using IniParser.Model;
using Microsoft.UI.Xaml.Controls;

using RandList.ViewModels;

namespace RandList.Views;

public sealed partial class MainPage : Page
{
    public static string Path_App = System.AppDomain.CurrentDomain.BaseDirectory;//获取程序运行路径
    public static string? IniSection;
    public static bool ThreadCannel;
    Thread? thread;

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        //*************************************************************************************
        if (File.Exists(Path_App + "List.ini"))
        {
            //为ComboBox添加成员
            var parser = new FileIniDataParser();
            IniData iniFile = parser.ReadFile(Path_App + "List.ini");
            for (var i = 0; i < iniFile.Sections.Count; i++)
            {
                MainPageComboBox.Items.Add(iniFile.Sections.ElementAt(i).SectionName);
            }
        }
        else
        {
            MainPageButton.IsEnabled = false;
            App.MainWindow.Title = "未找到有效名单";
        }
        //*************************************************************************************

    }

    private async void MainPageButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //*************************************************************************************
        //创建线程
        thread = new Thread(MainPageTextBoxChangeText);
        ThreadCannel = false;
        //*************************************************************************************
        if (MainPageComboBox.SelectedIndex == -1)
        {
            MainPageDialog.Content = "未选择名单";
            await MainPageDialog.ShowAsync();
        }
        else
        {

            if (Convert.ToString(MainPageButton.Content) == "开始")
            {
                MainPageButton.Content = "停止";
                MainPageComboBox.IsEnabled = false;
                IniSection = Convert.ToString(MainPageComboBox.SelectedItem);
                thread.Start();

            }
            else
            {
                MainPageButton.Content = "开始";
                MainPageComboBox.IsEnabled = true;
                ThreadCannel = true;             
            }
        }
    }

    public async void MainPageTextBoxChangeText()
    {
        var parser = new FileIniDataParser();
        IniData iniFile = parser.ReadFile(Path_App + "List.ini");
        await Task.Run(() =>
        {
            while (true)
            {
                for (var i = 0; i < Convert.ToInt32(iniFile[IniSection]["Size"]); i++)
                {
                    if (ThreadCannel == true)
                    {

                        ThreadCannel = false;
                        return;
                    }
                    Thread.Sleep(20);
                     MainPageTextBox.DispatcherQueue.TryEnqueue(() =>
                     {
                         //Debug.WriteLine(i);//测试代码
                         MainPageTextBox.Text = iniFile[IniSection][Convert.ToString(i)];
                     });
                }
            }
        });
    }

    #region API函数声明
    [DllImport("kernel32")]//返回0表示失败，非0为成功
    private static extern long WritePrivateProfileString(string section, string key,
      string val, string filePath);
    [DllImport("kernel32")]//返回取得字符串缓冲区的长度
    private static extern long GetPrivateProfileString(string section, string key,
      string def, StringBuilder retVal, int size, string filePath);
    #endregion
    #region 读Ini文件
    /// <summary>
    /// 读取ini文件内容的方法
    /// </summary>
    /// <param name="Section">ini文件的节名</param>
    /// <param name="Key">ini文件对应节下的键名</param>
    /// <param name="NoText">ini文件对应节对应键下无内容时返回的值</param>
    /// <param name="iniFilePath">该ini文件的路径</param>
    /// <returns></returns>
    public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
    {
        if (File.Exists(iniFilePath))
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
            return temp.ToString();
        }
        else
        {
            return String.Empty;
        }
    }
    #endregion
    #region 写Ini文件
    /// <summary>
    /// 将内容写入指定的ini文件中
    /// </summary>
    /// <param name="Section">ini文件中的节名</param>
    /// <param name="Key">ini文件中的键</param>
    /// <param name="Value">要写入该键所对应的值</param>
    /// <param name="iniFilePath">ini文件路径</param>
    /// <returns></returns>
    public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
    {
        if (File.Exists(iniFilePath))
        {
            var OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            if (OpStation == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region 取文本左边
    public static string GetLeft(string str, int n)//取文本左边
    {
        var Temp = str.Substring(0, n);
        return Temp;
    }
    #endregion
    #region 取文本右边
    public static string GetRight(string str, int n)//取文本右边
    {
        var Temp = str.Substring(str.Length - n);
        return Temp;
    }
    #endregion

}
