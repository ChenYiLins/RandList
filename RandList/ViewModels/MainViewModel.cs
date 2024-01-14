using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IniParser;
using Microsoft.UI.Xaml;
using RandList.Contracts.Services;
using RandList.Helpers;
using Windows.ApplicationModel;

namespace RandList.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _mode;

    [ObservableProperty]
    private string _symbol;

    [ObservableProperty]
    private string _language;

    [ObservableProperty]
    private string _versionDescription;

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public ICommand SwitchEditCommand
    {
        get;
    }

    public ICommand SwitchLanguageCommand
    {
        get;
    }

    public MainViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;

        var PathApp = AppDomain.CurrentDomain.BaseDirectory;
        if (File.Exists(PathApp + "AppWindow.ini"))
        {
            var parser = new FileIniDataParser();
            var iniFile = parser.ReadFile(PathApp + "AppWindow.ini");
            _mode = iniFile["AppWindow"]["Mode"];
            _symbol = iniFile["AppWindow"]["Symbol"];
        }
        else
        {
            _mode = "null";
            _symbol = "null";
        }

        // TODO: If someday Microsoft fix the program that unpackaged can't use PrimaryLanguageOverride.
        // https://github.com/microsoft/WindowsAppSDK/issues/1687
        if (RuntimeHelper.IsMSIX)
        {
            _language = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
        }
        else
        {
            _language = "zh-cn";
        }

        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });

        SwitchEditCommand = new RelayCommand<string>(
            (param) =>
            {
                if (param != null)
                {
                    var word = param.Split('_');
                    var PathApp = AppDomain.CurrentDomain.BaseDirectory;
                    if (File.Exists(PathApp + "AppWindow.ini"))
                    {
                        var parser = new FileIniDataParser();
                        var iniFile = parser.ReadFile(PathApp + "AppWindow.ini");

                        iniFile["AppWindow"][word[0]] = word[1];
                        parser.WriteFile(PathApp + "AppWindow.ini", iniFile);
                    }
                }
            });

        SwitchLanguageCommand = new RelayCommand<string>(
            (param) =>
            {
                if (param != null)
                {
                    if (RuntimeHelper.IsMSIX)
                    {
                        Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = param;
                    }
                    else
                    {

                    }
                }
            });
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

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}";
    }

}