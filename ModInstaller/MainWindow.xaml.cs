using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ModInstaller;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainPageModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();

        _viewModel = DataContext as MainPageModel;

        if (!DesignerProperties.GetIsInDesignMode(this))
            _viewModel?.Init();

    }

    private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // _viewModel?.SearchMod(SearchTermTextBox.Text);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _ = AppShell.Instance.LoadConfig("https://raw.githubusercontent.com/vr-commiter/HapticModList/refs/heads/main/version.json");
    }

}