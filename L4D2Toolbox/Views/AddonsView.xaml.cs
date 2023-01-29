using L4D2Toolbox.Core;
using L4D2Toolbox.Data;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;

using ValvePak;

namespace L4D2Toolbox.Views;

/// <summary>
/// AddonsView.xaml 的交互逻辑
/// </summary>
public partial class AddonsView : UserControl
{
    /// <summary>
    /// 玩家求生之路2MOD列表动态集合
    /// </summary>
    public ObservableCollection<AddonInfo> AddonInfoLists { get; set; } = new();

    public AddonsView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.WindowClosingEvent += MainWindow_WindowClosingEvent;
    }

    /// <summary>
    /// 主窗口关闭事件
    /// </summary>
    private void MainWindow_WindowClosingEvent()
    {

    }

    /// <summary>
    /// 自动调整列宽
    /// </summary>
    private async void AutoColumWidth()
    {
        await Task.Delay(500);

        lock (this)
        {
            if (ListView_Addons.View is GridView view)
            {
                foreach (GridViewColumn gvc in view.Columns)
                {
                    gvc.Width = 100;
                    gvc.Width = double.NaN;
                }
            }
        }
    }

    /// <summary>
    /// 刷新求生之路2MOD列表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_RefushAddonsList_Click(object sender, RoutedEventArgs e)
    {
        ProcessUtil.ClearMemory();

        Button_RefushAddonsList.IsEnabled = false;
        NotifierHelper.Show(NotifierType.Notification, "正在刷新求生之路2MOD列表...");

        int index = 1;
        AddonInfoLists.Clear();

        // 本地MOD
        foreach (var vpk in Directory.GetFiles(Globals.L4D2AddonsDir))
        {
            var file = new FileInfo(vpk);

            if (file.Extension != ".vpk")
                continue;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                AddonInfoLists.Add(new()
                {
                    Index = index++,
                    Title = GetVPKTitle(vpk),
                    State = "\xe724",
                    StateColor = "#FF0000",
                    Source = "本地",
                    FileSize = MiscUtil.ByteConverterMB((ulong)file.Length),
                    FileName = file.Name,
                });
            });
        }

        // 创意工坊MOD
        foreach (var vpk in Directory.GetFiles(Globals.L4D2WorkshopDir))
        {
            var file = new FileInfo(vpk);

            if (file.Extension != ".vpk")
                continue;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                AddonInfoLists.Add(new()
                {
                    Index = index++,
                    Title = GetVPKTitle(vpk),
                    State = "\xe730",
                    Source = "创意工坊",
                    StateColor = "#00CC00",
                    FileSize = MiscUtil.ByteConverterMB((ulong)file.Length),
                    FileName = file.Name,
                });
            });
        }

        Button_RefushAddonsList.IsEnabled = true;
        NotifierHelper.Show(NotifierType.Success, "刷新玩家求生之路2MOD列表成功");

        AutoColumWidth();
    }

    private string GetVPKTitle(string vpkPath)
    {
        using var package = new Package();
        package.Read(vpkPath);

        var addoninfo = package.FindEntry("addoninfo.txt");
        if (addoninfo != null)
        {
            package.ReadEntry(addoninfo, out byte[] output);

            using var memoryStream = new MemoryStream(output);

            var streamReader = new StreamReader(memoryStream, Encoding.UTF8);
            if (streamReader.ReadToEnd().Contains('�'))
                streamReader = new StreamReader(memoryStream, Encoding.GetEncoding("GB2312"));

            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

            string line = string.Empty;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Trim().ToLower().StartsWith("addontitle"))
                {
                    streamReader.Close();
                    memoryStream.Close();

                    line = line.Replace("addontitle", "", StringComparison.OrdinalIgnoreCase).Trim();
                    var index = line.LastIndexOf("\"");
                    if (index != -1)
                    {
                        line = line[..index].Replace("\"", "").Trim();
                    }

                    return string.IsNullOrEmpty(line) ? "<无标题>" : line;
                }
            }
        }

        return "<无标题>";
    }
}
