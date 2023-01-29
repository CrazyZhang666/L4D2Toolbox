using L4D2Toolbox.Core;
using L4D2Toolbox.Utils;
using L4D2Toolbox.Helper;
using Steamworks.Ugc;

namespace L4D2Toolbox.Views;

/// <summary>
/// ToolkitView.xaml 的交互逻辑
/// </summary>
public partial class ToolkitView : UserControl
{
    public ToolkitView()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    private void DropHelper(DragEventArgs e, string extePath, string[] fileExte)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (fileExte.Contains(Path.GetExtension(fileNames[0]).ToLower()))
                ProcessUtil.OpenExecWithArgs(extePath, $"\"{fileNames[0]}\"");
            else
                NotifierHelper.Show(NotifierType.Warning, $"当前拖放的目标文件非 {string.Join(", ", fileExte)} 格式，操作取消");
        }
    }

    private void Button_VPKUnpack_Drop(object sender, DragEventArgs e)
    {
        DropHelper(e, Globals.VPKExec, new string[] { ".vpk" });
    }

    private void Button_VPKPack_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (Directory.Exists(fileNames[0]))
            {
                if (File.Exists($"{fileNames[0]}\\addoninfo.txt"))
                {
                    Compile.RunL4D2DevExec(Globals.VPKExec, $"\"{fileNames[0]}\"");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Warning, "未发现 addoninfo.txt 文件，操作取消");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "当前拖放的目标非文件夹，操作取消");
            }
        }
    }

    private void Button_RunGCFScape_Drop(object sender, DragEventArgs e)
    {
        DropHelper(e, Globals.GCFScapeExec, new string[] { ".vpk" });
    }

    private void Button_RunVTFEdit_Drop(object sender, DragEventArgs e)
    {
        DropHelper(e, Globals.VTFEditExec, new string[] { ".vtf" });
    }

    private void Button_RunBSPSource_Drop(object sender, DragEventArgs e)
    {
        DropHelper(e, Globals.BSPSourceExec, new string[] { ".bsp" });
    }

    private void Button_RunHammer_Drop(object sender, DragEventArgs e)
    {
        DropHelper(e, Globals.HammerExec, new string[] { ".vmf" });
    }

    private void Button_RunCrowbar_Drop(object sender, DragEventArgs e)
    {
        DropHelper(e, Globals.CrowbarExec, new string[] { ".vpk", ".mdl" });
    }

    /////////////////////////////////////////////////////////////////////////

    [RelayCommand]
    private void ButtonClick(string name)
    {
        switch (name)
        {
            case "VPK解包":
                {
                    var fileDialog = new OpenFileDialog
                    {
                        Title = "选择要解包的VPK文件",
                        RestoreDirectory = true,
                        DefaultExt = ".vpk",
                        Filter = "VPK文件 (*.vpk)|*.vpk",
                        ValidateNames = true,
                        AddExtension = true,
                        CheckFileExists = false,
                        Multiselect = false
                    };

                    if (fileDialog.ShowDialog() == true)
                    {
                        ProcessUtil.OpenExecWithArgs(Globals.VPKExec, $"\"{fileDialog.FileName}\"");
                    }
                }
                break;
            case "VPK打包":
                {
                    var fileDialog = new OpenFileDialog
                    {
                        Title = "选择要打包的VPK资源文件夹",
                        RestoreDirectory = true,
                        DefaultExt = ".txt",
                        FileName = "addoninfo.txt",
                        Filter = "文本文件 (*.txt)|*.txt",
                        ValidateNames = true,
                        AddExtension = true,
                        CheckFileExists = false,
                        Multiselect = false
                    };

                    if (fileDialog.ShowDialog() == true)
                    {
                        Compile.RunL4D2DevExec(Globals.VPKExec, $"\"{Path.GetDirectoryName(fileDialog.FileName)}\"");
                    }
                }
                break;
            case "GCFScape":
                ProcessUtil.OpenExecWithArgs(Globals.GCFScapeExec);
                break;
            case "VTFEdit":
                ProcessUtil.OpenExecWithArgs(Globals.VTFEditExec);
                break;
            case "BSPSource":
                ProcessUtil.OpenExecWithArgs(Globals.BSPSourceExec);
                break;
            case "Hammer":
                ProcessUtil.OpenExecWithArgs(Globals.HammerExec);
                break;
            case "Crowbar":
                ProcessUtil.OpenExecWithArgs(Globals.CrowbarExec);
                break;
            ///////////////////////
            case "区域格式-英语美国":
                ProcessUtil.OpenExecWithArgs("rundll32.exe", "shell32.dll,Control_RunDLL intl.cpl,,0");
                Win32.SetComboboxSelectIndex(501);
                break;
            case "区域格式-中文中国":
                ProcessUtil.OpenExecWithArgs("rundll32.exe", "shell32.dll,Control_RunDLL intl.cpl,,0");
                Win32.SetComboboxSelectIndex(0);
                break;
            ///////////////////////
            case "求生之路2目录":
                ProcessUtil.OpenLink(Globals.L4D2MainDir);
                break;
            case "Addons目录":
                ProcessUtil.OpenLink(Globals.L4D2AddonsDir);
                break;
            case "Maps目录":
                ProcessUtil.OpenLink(Globals.L4D2MapsDir);
                break;
        }
    }
}
