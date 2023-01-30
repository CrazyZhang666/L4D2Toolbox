using L4D2Toolbox.Data;

using Steamworks;
using Steamworks.Ugc;
using Steamworks.Data;

namespace L4D2Toolbox.Windows;

/// <summary>
/// UploadWindow.xaml 的交互逻辑
/// </summary>
public partial class UploadWindow
{
    private UploadData UploadData { get; }

    private const int MaxCloudFileChunkSize = 104857600;

    public UploadWindow(UploadData uploadData)
    {
        InitializeComponent();
        this.DataContext = this;

        UploadData = uploadData;
    }

    private void Window_Upload_Loaded(object sender, RoutedEventArgs e)
    {
        if (UploadData.IsPublish)
            UploadPublish(UploadData);
        else
            UploadUpdate(UploadData);
    }

    private void Window_Upload_Closing(object sender, CancelEventArgs e)
    {

    }

    /////////////////////////////////////////////////////////////////

    /// <summary>
    /// 增加日志信息
    /// </summary>
    /// <param name="log"></param>
    private void AddLogger(string log)
    {
        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            TextBox_Logger.AppendText($"[{DateTime.Now:HH:mm:ss.fff}] {log}\n");
            TextBox_Logger.ScrollToEnd();
        });
    }

    /////////////////////////////////////////////////////////////////

    /// <summary>
    /// 发布新Mod
    /// </summary>
    /// <param name="data"></param>
    private async void UploadPublish(UploadData data)
    {
        // 预览图
        var imgFile = new FileInfo(data.IMGPath);
        AddLogger($"正在处理预览图文件，文件大小：{imgFile.Length} 字节");

        // 预览图二进制文件
        AddLogger("正在读取预览图二进制文件...");
        var imgData = await File.ReadAllBytesAsync(data.IMGPath);
        AddLogger("读取预览图二进制文件成功");

        // 上传预览图文件到Steam云存储
        AddLogger("正在上传预览图文件到Steam云存储...");
        if (!SteamRemoteStorage.FileWrite(imgFile.Name, imgData))
        {
            AddLogger("上传预览图文件到Steam云存储失败，操作终止");
            ProgressBar_Upload.IsIndeterminate = false;
            return;
        }
        AddLogger("上传预览图文件到Steam云存储成功");

        /////////////////////////////////////////////////////////////////////////

        // VPK
        var vpkFile = new FileInfo(data.VPKPath);
        AddLogger($"正在处理VPK文件，文件大小：{vpkFile.Length} 字节");
        if (!await UploadVPK(vpkFile))
            return;

        /////////////////////////////////////////////////////////////////////////

        // 从Steam云存储发布
        AddLogger("正在从Steam云存储发布到求生之路2创意工坊中...");
        var result_t = await SteamRemoteStorage.PublishWorkshopFile(vpkFile.Name, imgFile.Name, data.Title, data.Description, data.Visibility, data.Tags);
        if (result_t.Value.Result == Result.OK)
        {
            AddLogger($"从Steam云存储发布到求生之路2创意工坊成功，物品Id：{result_t.Value.PublishedFileId.Value}");
        }
        else
        {
            AddLogger($"从Steam云存储发布到求生之路2创意工坊失败 {result_t.Value.Result}");
        }

        // 清理Steam云存储
        if (SteamRemoteStorage.FileDelete(vpkFile.Name))
            AddLogger($"删除Steam云存储文件 {vpkFile.Name} 成功");
        else
            AddLogger($"删除Steam云存储文件 {vpkFile.Name} 失败");

        if (SteamRemoteStorage.FileDelete(imgFile.Name))
            AddLogger($"删除Steam云存储文件 {imgFile.Name} 成功");
        else
            AddLogger($"删除Steam云存储文件 {imgFile.Name} 失败");

        AddLogger("发布新MOD操作完毕，请关闭此窗口");
        ProgressBar_Upload.IsIndeterminate = false;
    }

    /// <summary>
    /// 更新选中Mod信息
    /// </summary>
    /// <param name="data"></param>
    private async void UploadUpdate(UploadData data)
    {
        // 创建更新Mod请求
        var editor = new Editor(new PublishedFileId
        {
            Value = data.FileId
        });

        // 标题
        if (string.IsNullOrEmpty(data.Title))
            editor.WithTitle(data.Title);
        // 预览图
        if (string.IsNullOrEmpty(data.IMGPath))
            editor.WithPreviewFile(data.IMGPath);
        // 描述
        if (string.IsNullOrEmpty(data.Description))
            editor.WithDescription(data.Description);

        // 可见性
        if (data.Visibility == 0)
            editor.WithPublicVisibility();
        else if (data.Visibility == 1)
            editor.WithFriendsOnlyVisibility();
        else if (data.Visibility == 2)
            editor.WithPrivateVisibility();
        else if (data.Visibility == 3)
            editor.WithUnlistedVisibility();

        if (string.IsNullOrEmpty(data.VPKPath))
        {
            // VPK路径为空，代表玩家没有更新VPK文件

            // 提交更新请求
            var result = await editor.SubmitAsync();
            if (!result.Success)
            {
                AddLogger($"更新选中Mod信息失败 {result.Result}，操作终止");
                ProgressBar_Upload.IsIndeterminate = false;
                return;
            }
            else
            {
                AddLogger($"更新选中Mod信息成功 {result.Result}");

                AddLogger("更新选中Mod信息操作完毕，请关闭此窗口");
                ProgressBar_Upload.IsIndeterminate = false;
            }
        }
        else
        {
            // VPK路径不为空，代表玩家更新了VPK文件

            // 玩家更新了VPK文件
            if (!File.Exists(data.VPKPath))
            {
                AddLogger("Mod主体VPK文件不存在，操作终止");
                ProgressBar_Upload.IsIndeterminate = false;
                return;
            }

            /////////////////////////////////////////////////////////////////////////

            // VPK
            var vpkFile = new FileInfo(data.VPKPath);
            AddLogger($"正在处理VPK文件，文件大小：{vpkFile.Length} 字节");
            if (!await UploadVPK(vpkFile))
                return;

            /////////////////////////////////////////////////////////////////////////

            // 更新VPK文件和更新日志
            if (!SteamRemoteStorage.UpdatePublished(data.FileId, data.ChangeLog, vpkFile.Name))
            {
                AddLogger("更新已发布物品VPK文件成功");
            }
            else
            {
                AddLogger("更新已发布物品VPK文件失败 ");
            }

            // 清理Steam云存储
            if (SteamRemoteStorage.FileDelete(vpkFile.Name))
                AddLogger($"删除Steam云存储文件 {vpkFile.Name} 成功");
            else
                AddLogger($"删除Steam云存储文件 {vpkFile.Name} 失败");

            AddLogger("更新选中Mod信息操作完毕，请关闭此窗口");
            ProgressBar_Upload.IsIndeterminate = false;
        }
    }

    private async Task<bool> UploadVPK(FileInfo vpkFile)
    {
        // 检测VPK文件大小
        if (vpkFile.Length < MaxCloudFileChunkSize)
        {
            AddLogger("正在读取VPK二进制文件...");
            var vpkData = await File.ReadAllBytesAsync(vpkFile.FullName);
            AddLogger("读取VPK二进制文件成功");

            // 上传VPK文件到Steam云存储
            AddLogger("正在上传VPK文件到Steam云存储...");
            if (!SteamRemoteStorage.FileWrite(vpkFile.Name, vpkData))
            {
                AddLogger("上传VPK文件到Steam云存储失败，操作终止");
                ProgressBar_Upload.IsIndeterminate = false;
                return false;
            }
            AddLogger("上传VPK文件到Steam云存储成功");

            return true;
        }
        else
        {
            AddLogger("正在读取VPK二进制文件流...");
            using var fileStream = new FileStream(vpkFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            AddLogger("读取VPK二进制文件流成功");

            AddLogger("正在打开Steam云二进制文件写入流...");
            var writeHandle = SteamRemoteStorage.FileWriteStreamOpen(vpkFile.Name);
            if (writeHandle == UGCFileWriteStreamHandle_t.Invalid)
            {
                AddLogger("打开Steam云二进制文件写入流失败，操作终止");
                ProgressBar_Upload.IsIndeterminate = false;
                return false;
            }

            var byteCountRead = 1;
            var maxData = new byte[MaxCloudFileChunkSize];
            AddLogger("正在执行Steam云二进制文件写入流操作...");
            while ((byteCountRead = await fileStream.ReadAsync(maxData)) > 0)
            {
                if (!SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, maxData, byteCountRead))
                {
                    SteamRemoteStorage.FileWriteStreamClose(writeHandle);
                    AddLogger("上传VPK文件到Steam云存储失败，操作终止");
                    ProgressBar_Upload.IsIndeterminate = false;
                    return false;
                }
                else
                {
                    AddLogger($"Steam云二进制文件写入 {byteCountRead} 字节 成功");
                }
            }

            if (SteamRemoteStorage.FileWriteStreamClose(writeHandle))
            {
                AddLogger("关闭Steam云二进制文件写入流成功");
            }
            else
            {
                AddLogger("关闭Steam云二进制文件写入流失败，操作终止");
                ProgressBar_Upload.IsIndeterminate = false;
                return false;
            }

            AddLogger("上传VPK文件到Steam云存储成功");

            return true;
        }
    }
}
