using CommandLine;
using System.Windows;

namespace ModInstaller;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        new AppShell();
        Parser.Default.ParseArguments<AppShellOptions>(e.Args)
                   .WithParsed<AppShellOptions>(o =>
                   {
                       AppShell.Instance.SetRunOptions(o);
                   });

        base.OnStartup(e);
    }
}