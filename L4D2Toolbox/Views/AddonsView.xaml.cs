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

        var addonList = GetAddonList();

        // 本地MOD
        foreach (var vpk in Directory.GetFiles(Globals.L4D2AddonsDir))
        {
            var file = new FileInfo(vpk);

            if (file.Extension != ".vpk")
                continue;

            var addonData = GetAddonData(vpk);

            var state = "\xe724";
            var stateColor = "#FF0000";
            if (addonList.ContainsKey(file.Name) && addonList[file.Name])
            {
                state = "\xe730";
                stateColor = "#00CC00";
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                AddonInfoLists.Add(new()
                {
                    Index = index++,
                    Title = addonData.Title,
                    Description = addonData.Description,
                    Author = addonData.Author,
                    State = state,
                    StateColor = stateColor,
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

            var addonData = GetAddonData(vpk);

            var state = "\xe724";
            var stateColor = "#FF0000";
            if (addonList.ContainsKey($"workshop\\{file.Name}") && addonList[$"workshop\\{file.Name}"])
            {
                state = "\xe730";
                stateColor = "#00CC00";
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                AddonInfoLists.Add(new()
                {
                    Index = index++,
                    Title = addonData.Title,
                    Description = addonData.Description,
                    Author = addonData.Author,
                    State = state,
                    StateColor = stateColor,
                    Source = "创意工坊",
                    FileSize = MiscUtil.ByteConverterMB((ulong)file.Length),
                    FileName = file.Name,
                });
            });
        }

        Button_RefushAddonsList.IsEnabled = true;
        NotifierHelper.Show(NotifierType.Success, "刷新玩家求生之路2MOD列表成功");

        AutoColumWidth();
    }

    private AddonData GetAddonData(string vpkPath)
    {
        var addonData = new AddonData
        {
            Title = "<无标题>",
            Description = "",
            Author = ""
        };

        using var package = new Package();
        package.Read(vpkPath);

        var addoninfo = package.FindEntry("addoninfo.txt");
        if (addoninfo != null)
        {
            package.ReadEntry(addoninfo, out byte[] output);

            var str = Encoding.UTF8.GetString(output);
            if (str.Contains('�'))
                str = Encoding.GetEncoding("GB2312").GetString(output);

            var lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.Contains("addonTitle ", StringComparison.OrdinalIgnoreCase))
                {
                    addonData.Title = GetAddonInfoValue(line, "addonTitle");
                }

                if (line.Contains("addonDescription ", StringComparison.OrdinalIgnoreCase))
                {
                    addonData.Description = GetAddonInfoValue(line, "addonDescription");
                }

                if (line.Contains("addonAuthor ", StringComparison.OrdinalIgnoreCase))
                {
                    addonData.Author = GetAddonInfoValue(line, "addonAuthor");
                }
            }
        }

        return addonData;
    }

    private string GetAddonInfoValue(string content, string key)
    {
        var temp = content.Replace("\"", "").Trim();
        if (temp.StartsWith(key, StringComparison.OrdinalIgnoreCase))
        {
            var value = temp.Replace(key, "", StringComparison.OrdinalIgnoreCase).Trim();
            var index = value.IndexOf("//");
            if (index != -1)
                value = value[..index].Trim();

            return value;
        }

        return string.Empty;
    }

    private Dictionary<string, bool> GetAddonList()
    {
        var addonList = new Dictionary<string, bool>();

        if (File.Exists(Globals.L4D2AddonListTxt))
        {
            foreach (var line in File.ReadAllLines(Globals.L4D2AddonListTxt))
            {
                if (line.Contains("AddonList"))
                    continue;

                if (line.StartsWith("{"))
                    continue;

                if (line.StartsWith("}"))
                    continue;

                var keyValueArray = Regex.Split(line, ".vpk");
                var key = keyValueArray[0].Replace("\"", "").Trim() + ".vpk";
                var value = keyValueArray[1].Replace("\"", "").Trim() == "1";
                addonList.Add(key, value);
            }
        }

        return addonList;
    }
}
