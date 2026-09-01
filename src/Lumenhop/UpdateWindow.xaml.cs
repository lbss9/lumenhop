using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;

namespace Lumenhop;

public sealed partial class UpdateWindow : Window
{
    public UpdateWindow(UpdateOffer offer)
    {
        InitializeComponent();
        SystemBackdrop = new DesktopAcrylicBackdrop();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        AppWindow.Resize(new SizeInt32(460, 620));
        CenterOnWorkArea();
        if (AppWindow.Presenter is OverlappedPresenter presenter)
            presenter.IsMaximizable = false;

        VersionText.Text = string.Format(Loc.Get("Update_VersionLine"), offer.Version);
        MarkdownLite.Render(ChangelogView, offer.Notes);
        RootGrid.RequestedTheme = App.Main?.RootTheme ?? ElementTheme.Default;
    }

    private async void OnInstall(object sender, RoutedEventArgs e)
    {
        InstallButton.IsEnabled = false;
        LaterButton.IsEnabled = false;
        Spinner.IsActive = true;
        StatusText.Text = Loc.Get("Update_Downloading");
        var result = await UpdateService.DownloadAndApplyAsync();
        if (result == UpdateCheckKind.Failed)
        {
            Spinner.IsActive = false;
            InstallButton.IsEnabled = true;
            LaterButton.IsEnabled = true;
            StatusText.Text = Loc.Get("Update_Failed");
        }
    }

    private void OnLater(object sender, RoutedEventArgs e) => Close();

    private void CenterOnWorkArea()
    {
        var work = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest).WorkArea;
        var x = work.X + (work.Width - 460) / 2;
        var y = work.Y + (work.Height - 620) / 2;
        AppWindow.Move(new PointInt32(x, y));
    }
}
