using L4D2Toolbox.Core;
using L4D2Toolbox.Steam;
using L4D2Toolbox.Utils;

using Steamworks;

namespace L4D2Toolbox;

/// <summary>
/// LoadWindow.xaml 的交互逻辑
/// </summary>
public partial class LoadWindow
{
    public LoadWindow()
    {
        InitializeComponent();
    }

    private void Window_Load_Loaded(object sender, RoutedEventArgs e)
    {
        this.DataContext = this;

        Task.Run(async () =>
        {
            try
            {
                // 创建文件夹
                Directory.CreateDirectory(Globals.OutputDir);
                Directory.CreateDirectory(Globals.ConfigDir);

                if (!File.Exists(".\\AppData.bin"))
                {
                    // 释放数据文件
                    MiscUtil.ExtractResFile("L4D2Toolbox.Files.AppData.zip", ".\\AppData.bin");

                    if (!Directory.Exists(Globals.AppDataDir))
                    {
                        // 解压数据文件
                        using var archive = ZipFile.OpenRead(".\\AppData.bin");
                        archive.ExtractToDirectory(Globals.AppDataDir);
                    }
                }

                if (!File.Exists(".\\steam_api64.dll"))
                {
                    // 释放数据文件
                    MiscUtil.ExtractResFile("L4D2Toolbox.Files.steam_api64.dll", ".\\steam_api64.dll");
                }

                // 检测Steam进程
                if (!Workshop.Init(out string log))
                {
                    LoadLogger(log, Brushes.Red);
                    return;
                }

                var dir = Workshop.GetL4D2InstallDir();
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    Globals.L4D2MainDir = dir;
                    Globals.L4D2MainExec = $"{Globals.L4D2MainDir}\\left4dead2.exe";
                }

                /////////////////////////////////////////////////////////////////////

                await Task.Delay(500);

                this.Dispatcher.Invoke(() =>
                {
                    var mainWindow = new MainWindow();
                    // 转移主程序控制权
                    Application.Current.MainWindow = mainWindow;
                    // 显示主窗口
                    mainWindow.Show();

                    // 关闭初始化窗口
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                LoadLogger(ex.Message, Brushes.Red);
                return;
            }
        });
    }

    private void Window_Load_Closing(object sender, CancelEventArgs e)
    {

    }

    private void LoadLogger(string log, SolidColorBrush solidColor)
    {
        this.Dispatcher.Invoke(() =>
        {
            TextBlock_LoadLogger.Text = log;
            TextBlock_LoadLogger.Foreground = solidColor;
        });
    }

    private void Button_ExitApp_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
