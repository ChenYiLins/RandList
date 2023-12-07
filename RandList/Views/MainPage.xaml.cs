using IniParser;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using RandList.ViewModels;

namespace RandList.Views;

public sealed partial class MainPage : Page
{
    // 获取程序运行路径
    public static readonly string PathApp = AppDomain.CurrentDomain.BaseDirectory;

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
            var parser = new FileIniDataParser();
            var iniFile = parser.ReadFile(PathApp + "List.ini");

            for (var i = 0; i < iniFile.Sections.Count; i++)
            {
                var item = new RadioMenuFlyoutItem
                {
                    Text = iniFile.Sections.ElementAt(i).SectionName
                };
                MenuFlyoutSubItemFileList.Items.Add(item);
            }
        }
    }

    private async void MenuFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var selectedFlyoutItem = sender as MenuFlyoutItem;
        if (selectedFlyoutItem == MenuFlyoutItemSpawOfStartSpaw)
        {
            MainPageSpawDialogOfTextBox.Text = "";
            await MainPageSpawDialog.ShowAsync();
        }
        else if (selectedFlyoutItem == MenuFlyoutItemAboutOfAbout)
        {
            await MainPageAboutDialog.ShowAsync();
        }
    }

    private void MainPageSpawDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var parser = new FileIniDataParser();
        var iniFile = parser.ReadFile(PathApp + "List.ini");
        var isAnyItemSelected = false;
        var selectedItemText = string.Empty;
        // 遍历名单菜单项，获取所有名单名称
        foreach (var items in MenuFlyoutSubItemFileList.Items.Cast<RadioMenuFlyoutItem>())
        {
            if (items.IsChecked)
            {
                isAnyItemSelected = true;
                selectedItemText = items.Text;
                break;
            }
        }
        if (RadioMenuFlyoutItemEditModeOfOne.IsChecked && isAnyItemSelected && MainPageSpawDialogOfTextBox.Text != "" && Convert.ToInt32(MainPageSpawDialogOfTextBox.Text) < Convert.ToInt32(iniFile[selectedItemText]["NUM"]))
        {
            MainPageTextBlock.Text = "";

            var randomNum = new int[100];
            for (var i = 0; i < Convert.ToInt32(MainPageSpawDialogOfTextBox.Text); i++)
            {
                var random = new Random();
                var newRandNum = random.Next(1, Convert.ToInt32(iniFile[selectedItemText]["NUM"]) + 1);
                // 随机起点顺序，while循环判断是否生成了重复的随机数，避免产生相同名单
                while (Array.IndexOf(randomNum, newRandNum) != -1)
                {
                    newRandNum = random.Next(1, Convert.ToInt32(iniFile[selectedItemText]["NUM"]) + 1);
                }
                randomNum[i] = newRandNum;
                // 通过对分割符菜单选项判断，确立不同的分割符
                if (RadioMenuFlyoutItemEditSymbolOfOne.IsChecked)
                {
                    MainPageTextBlock.Text = MainPageTextBlock.Text + " " + iniFile[selectedItemText][Convert.ToString(randomNum[i])];
                }
                else if (RadioMenuFlyoutItemEditSymbolOfTwo.IsChecked)
                {
                    MainPageTextBlock.Text = MainPageTextBlock.Text + iniFile[selectedItemText][Convert.ToString(randomNum[i])] + "\n";
                }
            }
        }
        else if (RadioMenuFlyoutItemEditModeOfTwo.IsChecked && isAnyItemSelected && MainPageSpawDialogOfTextBox.Text != "" && Convert.ToInt32(MainPageSpawDialogOfTextBox.Text) < Convert.ToInt32(iniFile[selectedItemText]["NUM"]))
        {
            MainPageTextBlock.Text = "";

            // 固定起点顺序，先行初始化随机数，并从这个随机数开始累加
            int selectNum;
            var random = new Random();
            var newRandNum = random.Next(1, Convert.ToInt32(iniFile[selectedItemText]["NUM"]) + 1);
            for (var i = 0; i < Convert.ToInt32(MainPageSpawDialogOfTextBox.Text); i++)
            {
                if (newRandNum + i <= Convert.ToInt32(iniFile[selectedItemText]["NUM"]))
                {
                    selectNum = newRandNum + i;
                }
                else
                {
                    selectNum = newRandNum + i - Convert.ToInt32(iniFile[selectedItemText]["NUM"]);
                }
                // 通过对分割符菜单选项判断，确立不同的分割符
                if (RadioMenuFlyoutItemEditSymbolOfOne.IsChecked)
                {
                    MainPageTextBlock.Text = MainPageTextBlock.Text + " " + iniFile[selectedItemText][Convert.ToString(selectNum)];
                }
                else if (RadioMenuFlyoutItemEditSymbolOfTwo.IsChecked)
                {
                    MainPageTextBlock.Text = MainPageTextBlock.Text + iniFile[selectedItemText][Convert.ToString(selectNum)] + "\n";
                }
            }
        }

        // 异常情况提示
        // 没有选择任何名单
        if (isAnyItemSelected == false)
        {
            var resourceLoader = new ResourceLoader();
            MainPageTextBlock.Text = resourceLoader.GetString("WarningWords_NoChoiceList");
        }
        // 没有填写生成数量
        else if (MainPageSpawDialogOfTextBox.Text == "")
        {
            var resourceLoader = new ResourceLoader();
            MainPageTextBlock.Text = resourceLoader.GetString("WarningWords_NoSpawNum");
        }
        // 生成数量超出名单数
        else if (Convert.ToInt32(MainPageSpawDialogOfTextBox.Text) > Convert.ToInt32(iniFile[selectedItemText]["NUM"]))
        {
            var resourceLoader = new ResourceLoader();
            MainPageTextBlock.Text = resourceLoader.GetString("WarningWords_BiggerSpawNum");
        }
    }
}
