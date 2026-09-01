using Microsoft.UI.Xaml.Controls;

namespace Lumenhop.Pages;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();
        VersionText.Text = string.Format(Loc.Get("About_Version"), AppInfo.Version);
        MarkdownLite.Render(ChangelogView, ChangelogStore.Read());
    }
}
