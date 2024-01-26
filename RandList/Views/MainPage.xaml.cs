using System.Reflection;
using IniParser;
using IniParser.Exceptions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RandList.Helpers;
using RandList.ViewModels;
using Windows.ApplicationModel;

namespace RandList.Views;

public sealed partial class MainPage : Page
{
    private static readonly string PathApp = AppDomain.CurrentDomain.BaseDirectory; // 获取程序运行路径

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();

        // 获取"List.ini"内所有名单，并加入到子菜单之中
        if (File.Exists(PathApp + "List.ini"))
        {
            LoadIniFile();
        }
        else
        {
            ShowWarning("WarningWords_NoList", FileMenuBar);
            GenerateStartMenuItem.IsEnabled = false;
        }
    }

    private async void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        var selectedFlyoutItem = sender as MenuFlyoutItem;
        if (selectedFlyoutItem == GenerateStartMenuItem)
        {
            MainPageGenerateDialogOfTextBox.Text = "";
            await GenerateDialog.ShowAsync();
        }
        else if (selectedFlyoutItem == AboutAboutMenuItem)
        {
            MainPageAboutDialogOfTextBox.Text = "MainPage_Dialog_About_TextBlock".GetLocalized() + GetVersionDescription();
            await AboutDialog.ShowAsync();
        }
        else if (selectedFlyoutItem == FileImportMenuItem)
        {
            var importDialog = new Dialog.ImportDialog
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ViewModel.ElementTheme
            };
            await importDialog.ShowAsync();
            LoadIniFile();
        }
        else if (selectedFlyoutItem == FileEditListMenuItem)
        {
            var editListDialog = new Dialog.EditListDialog
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ViewModel.ElementTheme
            };
            await editListDialog.ShowAsync();
            LoadIniFile();
        }
    }

    private void MainPageGenerateDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var parser = new FileIniDataParser();
        var iniFile = parser.ReadFile(PathApp + "List.ini");

        var selectedItem = FileListMenuItem.Items.Cast<RadioMenuFlyoutItem>().FirstOrDefault(item => item.IsChecked);
        if (selectedItem == null) return;

        // 异常情况提示
        // 没有填写生成数量
        if (MainPageGenerateDialogOfTextBox.Text == "")
        {
            ShowWarning("WarningWords_NoGenerateNum", GenerateMenuBar);
            return;
        }
        // 生成数量超出名单数
        else if (Convert.ToInt32(MainPageGenerateDialogOfTextBox.Text) > Convert.ToInt32(iniFile[selectedItem.Text]["NUM"]))
        {
            ShowWarning("WarningWords_BiggerGenerateNum", GenerateMenuBar);
            return;
        }

        // 不同生成模式判断
        if (EditModeRandomRadioMenuItem.IsChecked)
        {
            MainPageTextBlock.Text = "";

            var randomNum = new List<int>();
            for (var i = 0; i < Convert.ToInt32(MainPageGenerateDialogOfTextBox.Text); i++)
            {
                var random = new Random();
                var newRandNum = random.Next(1, Convert.ToInt32(iniFile[selectedItem.Text]["NUM"]) + 1);
                // 随机起点顺序，while循环判断是否生成了重复的随机数，避免产生相同名单
                while (randomNum.Contains(newRandNum))
                {
                    newRandNum = random.Next(1, Convert.ToInt32(iniFile[selectedItem.Text]["NUM"]) + 1);
                }
                randomNum.Add(newRandNum);
                // 通过对分割符菜单选项判断，确立不同的分割符
                if (EditSymbolSpaceRadioMenuItem.IsChecked)
                {
                    MainPageTextBlock.Text += " " + iniFile[selectedItem.Text][Convert.ToString(randomNum[i])];
                }
                else if (EditSymbolNewlineRadioMenuItem.IsChecked)
                {
                    MainPageTextBlock.Text += iniFile[selectedItem.Text][Convert.ToString(randomNum[i])] + "\n";
                }
            }
        }
        else if (EditModeFixedRadioMenuItem.IsChecked)
        {
            MainPageTextBlock.Text = "";

            // 固定起点顺序，先行初始化随机数，并从这个随机数开始累加
            int selectNum;
            var random = new Random();
            var newRandNum = random.Next(1, Convert.ToInt32(iniFile[selectedItem.Text]["NUM"]) + 1);
            for (var i = 0; i < Convert.ToInt32(MainPageGenerateDialogOfTextBox.Text); i++)
            {
                if (newRandNum + i <= Convert.ToInt32(iniFile[selectedItem.Text]["NUM"]))
                {
                    selectNum = newRandNum + i;
                }
                else
                {
                    selectNum = newRandNum + i - Convert.ToInt32(iniFile[selectedItem.Text]["NUM"]);
                }
                // 通过对分割符菜单选项判断，确立不同的分割符
                if (EditSymbolSpaceRadioMenuItem.IsChecked)
                {
                    MainPageTextBlock.Text += " " + iniFile[selectedItem.Text][Convert.ToString(selectNum)];
                }
                else if (EditSymbolNewlineRadioMenuItem.IsChecked)
                {
                    MainPageTextBlock.Text += iniFile[selectedItem.Text][Convert.ToString(selectNum)] + "\n";
                }
            }
        }
    }

    private void LoadIniFile()
    {
        var parser = new FileIniDataParser();
        try
        {
            var iniFile = parser.ReadFile(PathApp + "List.ini");

            FileListMenuItem.Items.Clear();

            for (var i = 0; i < iniFile.Sections.Count; i++)
            {
                var item = new RadioMenuFlyoutItem
                {
                    Text = iniFile.Sections.ElementAt(i).SectionName,
                    GroupName = "List",
                    IsChecked = i == 0
                };
                FileListMenuItem.Items.Add(item);
            }
        }
        catch (ParsingException ex)
        {
            if (ex.HResult == -2146233088) ShowWarning("WarningWords_DuplicateSections", FileMenuBar);
        }

    }

    private void ShowWarning(string warningKey, FrameworkElement frameworkElement)
    {
        WarningTeachingTip.Subtitle = warningKey.GetLocalized();
        WarningTeachingTip.Target = frameworkElement;
        WarningTeachingTip.IsOpen = true;
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{version.Major}.{version.Minor}.{version.Build}";
    }
}
