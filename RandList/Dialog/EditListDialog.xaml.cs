using System.Text;
using IniParser;
using IniParser.Exceptions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RandList.Helpers;

namespace RandList.Dialog;

public sealed partial class EditListDialog : ContentDialog
{
    private static readonly string PathApp = AppDomain.CurrentDomain.BaseDirectory; // 获取程序运行路径

    public EditListDialog()
    {
        InitializeComponent();

        LoadIniFile();
    }

    private async void CommandBar_Click(object sender, RoutedEventArgs e)
    {
        var clickButton = sender as AppBarButton;
        if (clickButton == DeleteCommandBar)
        {
            ListView.Items.Remove(ListView.SelectedItem);
            if (ListView.Items.Count > 0) ListView.SelectedIndex = 0;
            SaveIniFile();

            StatusBarText.Text = "WarningWords_Saved".GetLocalized();
        }
        else if (clickButton == ImportCommandBar)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.FileTypeFilter.Add(".ini");
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var listFileStr = File.ReadAllText(PathApp + "List.ini") + "\n" + File.ReadAllText(file.Path);
                File.WriteAllText(PathApp + "List.ini", listFileStr, Encoding.UTF8);
            }
            LoadIniFile();
            StatusBarText.Text = "WarningWords_Saved".GetLocalized();
        }
        else if (clickButton == SaveCommandBar)
        {
            SaveIniFile();
            ShowWarning("WarningWords_SaveSuccess", SaveCommandBar);
            StatusBarText.Text = "WarningWords_Saved".GetLocalized();
        }
    }

    private void CloseDialogButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        StatusBarText.Text = "WarningWords_Unsaved".GetLocalized();
    }

    private void SaveIniFile()
    {
        File.WriteAllText(PathApp + "ListBackup.ini", "");

        var parser = new FileIniDataParser();
        var iniFile = parser.ReadFile(PathApp + "List.ini");
        var backupIniFile = parser.ReadFile(PathApp + "ListBackup.ini");
        for (var i = 0; i < ListView.Items.Count; i++)
        {
            var listItem = (ListViewItem)ListView.Items[i];
            if (iniFile.Sections.ContainsSection(listItem.Content.ToString()))
            {
                backupIniFile.Sections.AddSection(listItem.Content.ToString());
                backupIniFile.Sections.SetSectionData(listItem.Content.ToString(), iniFile.Sections.GetSectionData(listItem.Content.ToString()));
            }
        }
        parser.WriteFile(PathApp + "ListBackup.ini", backupIniFile);
        File.WriteAllText(PathApp + "List.ini", "", Encoding.UTF8);
        File.WriteAllText(PathApp + "List.ini", File.ReadAllText(PathApp + "ListBackup.ini"), Encoding.UTF8);
    }

    private void LoadIniFile()
    {
        var parser = new FileIniDataParser();
        try
        {
            var iniFile = parser.ReadFile(PathApp + "List.ini");

            ListView.Items.Clear();

            for (var i = 0; i < iniFile.Sections.Count; i++)
            {
                var item = new ListViewItem
                {
                    Content = iniFile.Sections.ElementAt(i).SectionName,
                };
                ListView.Items.Add(item);
            }
            ListView.SelectedIndex = 0;
        }
        catch (ParsingException ex)
        {
            if (ex.HResult == -2146233088) ShowWarning("WarningWords_DuplicateSections", ListView);
        }

    }

    private void ShowWarning(string warningKey, FrameworkElement frameworkElement)
    {
        WarningTeachingTip.Subtitle = warningKey.GetLocalized();
        WarningTeachingTip.Target = frameworkElement;
        WarningTeachingTip.IsOpen = true;
    }

}
