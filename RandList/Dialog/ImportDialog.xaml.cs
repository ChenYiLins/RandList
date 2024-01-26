using IniParser;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RandList.Helpers;

namespace RandList.Dialog;

public sealed partial class ImportDialog : ContentDialog
{
    private static readonly string PathApp = AppDomain.CurrentDomain.BaseDirectory; // 获取程序运行路径

    private bool canHideDialog;

    public ImportDialog()
    {
        InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (ListNameTextBox.Text == "")
        {
            ShowWarning("WarningWords_NoListName", ListNameTextBox);
            canHideDialog = false;
            return;
        }

        var keyArray = ListContentTextBox.Text.Split([' ', ',', '，', ';', '；']);
        if (keyArray.Length == 1)
        {
            ShowWarning("WarningWords_InvalidContent", ListContentTextBox);
            canHideDialog = false;
            return;
        }
        else
        {
            var parser = new FileIniDataParser();
            var iniFile = parser.ReadFile(PathApp + "List.ini");
            iniFile.Sections.AddSection(ListNameTextBox.Text);
            iniFile[ListNameTextBox.Text].AddKey("NUM");
            iniFile[ListNameTextBox.Text]["NUM"] = keyArray.Length.ToString();
            for (var i = 0; i < keyArray.Length; i++)
            {
                iniFile[ListNameTextBox.Text].AddKey((i + 1).ToString());
                iniFile[ListNameTextBox.Text][(i + 1).ToString()] = keyArray[i];
            }
            parser.WriteFile(PathApp + "List.ini", iniFile);
            ShowWarning("WarningWords_SuccessfullyImport", ListContentTextBox);
            canHideDialog = false;
            return;
        }
    }

    private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        canHideDialog = true;
    }

    private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (canHideDialog == false) args.Cancel = true;
    }

    private void ShowWarning(string warningKey, FrameworkElement frameworkElement)
    {
        WarningTeachingTip.Subtitle = warningKey.GetLocalized();
        WarningTeachingTip.Target = frameworkElement;
        WarningTeachingTip.IsOpen = true;
    }
}
