using L4D2Toolbox.Core;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;

namespace L4D2Toolbox.Views;

/// <summary>
/// GameFontView.xaml 的交互逻辑
/// </summary>
public partial class GameFontView : UserControl
{
    private const string fontINI = $"{Globals.MacTypeDir}\\L4D2.ini";

    public GameFontView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.WindowClosingEvent += MainWindow_WindowClosingEvent;

        TextBox_SampleText.AppendText("让大家翘首以盼的的《求生之路 2》（L4D2）以僵尸大灾难为背景，是 2008 年最受欢迎且屡获殊荣的合作游戏《求生之路》的续集。\n\n");
        TextBox_SampleText.AppendText("这个第一人称射击恐怖合作动作游戏将带领玩家和好友穿过美国南部的城市、沼泽和墓地，从萨凡纳到新奥尔良，沿途经过五个漫长的战役。\n\n");
        TextBox_SampleText.AppendText("玩家将扮演四名新生还者中的一名，装备有种类繁多、数量惊人的经典及先进武器。 除了枪支之外，玩家还有机会用各种可制造屠杀的近战武器在感染者上泄愤，例如电锯、斧头、甚至是致命的平底锅。");

        // 读取对应配置文件
        var CustomFontName = IniHelper.ReadValue("Font", "CustomFontName");
        var CustomRunArgs = IniHelper.ReadValue("Font", "CustomRunArgs");

        TextBox_CustomFontName.Text = CustomFontName;
        TextBox_CustomRunArgs.Text = CustomRunArgs;

        if (string.IsNullOrWhiteSpace(CustomRunArgs))
            TextBox_CustomRunArgs.Text = "-steam -novid -language schinese";

        Task.Run(() =>
        {
            // 获取系统所有字体
            var fonts = new List<string>();
            foreach (var family in Fonts.SystemFontFamilies)
            {
                foreach (var item in family.FamilyNames)
                {
                    fonts.Add(item.Value);
                }
            }

            fonts.Sort();
            fonts.ForEach(font =>
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    ListBox_Fonts.Items.Add(font);
                });
            });

            var index = fonts.IndexOf(CustomFontName);
            if (index != -1)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ListBox_Fonts.SelectedIndex = index;
                });
            }
        });
    }

    /// <summary>
    /// 主窗口关闭事件
    /// </summary>
    private void MainWindow_WindowClosingEvent()
    {
        SaveConfig();
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    private void SaveConfig()
    {
        IniHelper.WriteValue("Font", "CustomFontName", TextBox_CustomFontName.Text.Trim());
        IniHelper.WriteValue("Font", "CustomRunArgs", TextBox_CustomRunArgs.Text.Trim());
    }

    private void WriteCustonFontName(string fontName)
    {
        IniHelper.WriteValue("FontSubstitutes@L4D2", "SimSun", fontName, fontINI);
        IniHelper.WriteValue("FontSubstitutes@L4D2", "NSimSun", fontName, fontINI);
        IniHelper.WriteValue("FontSubstitutes@L4D2", "Tahoma", fontName, fontINI);
    }

    private void Button_RunL4D2ByMacLoader_Click(object sender, RoutedEventArgs e)
    {
        var fontName = TextBox_CustomFontName.Text.Trim();
        if (!string.IsNullOrWhiteSpace(fontName))
        {
            WriteCustonFontName(fontName);
        }

        ProcessUtil.OpenExecWithArgs(Globals.FontLoaderExec, $"\"{Globals.L4D2MainExec}\" {TextBox_CustomRunArgs.Text.Trim()}");
    }
}
