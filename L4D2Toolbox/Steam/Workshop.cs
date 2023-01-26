using L4D2Toolbox.Core;
using L4D2Toolbox.Data;
using L4D2Toolbox.Utils;

using Steamworks;
using Steamworks.Ugc;
using Steamworks.Data;

namespace L4D2Toolbox.Steam;

public static class Workshop
{
    /// <summary>
    /// 初始化Steamworks
    /// </summary>
    /// <returns></returns>
    public static bool Init(out string log)
    {
        if (Client.IsRun())
        {
            try
            {
                SteamClient.Init(Globals.AppID);

                log = "初始化Steamworks成功";
                return true;
            }
            catch (Exception ex)
            {
                log = $"Steamworks初始化失败，请重启程序并检查Steam状态\n异常信息 : {ex.Message}";
                return false;
            }
        }
        else
        {
            log = "未发现Steam进程，请先启动Steam客户端";
            return false;
        }
    }

    /// <summary>
    /// 结束Steamworks
    /// </summary>
    public static void ShutDown()
    {
        SteamClient.Shutdown();
    }

    /// <summary>
    /// 获取Mod可见性
    /// </summary>
    /// <param name="isPublic"></param>
    /// <param name="isFriendsOnly"></param>
    /// <param name="isPrivate"></param>
    /// <param name="isUnlisted"></param>
    /// <returns></returns>
    public static string GetPublicState(bool isPublic, bool isFriendsOnly, bool isPrivate, bool isUnlisted)
    {
        // 当前可见性： 公开
        if (isPublic)
            return "公开";
        // 当前可见性： 仅限好友该物品仅对您、您的好友和管理员可见。
        if (isFriendsOnly)
            return "仅限好友";
        // 当前可见性： 隐藏该物品仅对您、管理员和被标记为创作者的用户可见。
        if (isPrivate)
            return "隐藏";
        // 当前可见性： 非公开此物品对所有人可见，但不会在搜索中或您的个人资料里显示。
        if (isUnlisted)
            return "非公开";

        return "其他";
    }

    /// <summary>
    /// 获取Mod标志字符串
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static string GetTagsContent(string[] tags)
    {
        for (int i = 0; i < tags.Length; i++)
            tags[i] = MiscUtil.UpperCaseFirstChar(tags[i]);

        return string.Join(", ", tags);
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 获取玩家Steam昵称
    /// </summary>
    /// <returns></returns>
    public static string GetUserName()
    {
        return SteamClient.Name;
    }

    /// <summary>
    /// 获取玩家Steam数字Id
    /// </summary>
    /// <returns></returns>
    public static ulong GetUserSteamId()
    {
        return SteamClient.SteamId.Value;
    }

    /// <summary>
    /// 获取求生之路2安装目录
    /// </summary>
    /// <returns></returns>
    public static string GetL4D2InstallDir()
    {
        return SteamApps.AppInstallDir(Globals.AppID);
    }

    /// <summary>
    /// 获取求生之路2玩家创意工坊物品列表
    /// </summary>
    public static async Task<List<ItemInfo>> GetUserPublished()
    {
        var itemInfos = new List<ItemInfo>();

        var published = Query.ItemsReadyToUse.WhereUserPublished().SortByUpdateDate();

        int page = 1, index = 1;
        ResultPage? result;

        do
        {
            result = await published.GetPageAsync(page++);

            foreach (var item in result.Value.Entries)
            {
                itemInfos.Add(new()
                {
                    Index = index++,
                    Id = item.Id.Value,
                    PreviewImage = item.PreviewImageUrl,
                    Title = item.Title.Replace("\n", " "),
                    Description = item.Description,
                    Url = item.Url,
                    FileSize = MiscUtil.ByteConverterMB(item.FileSize),
                    IsPublic = item.IsPublic,
                    IsFriendsOnly = item.IsFriendsOnly,
                    IsPrivate = item.IsPrivate,
                    IsUnlisted = item.IsUnlisted,
                    PublicState = GetPublicState(item.IsPublic, item.IsFriendsOnly, item.IsPrivate, item.IsUnlisted),
                    Updated = MiscUtil.FormatDateTime(item.Updated),
                    Created = MiscUtil.FormatDateTime(item.Created),
                    Tags = item.Tags,
                    TagsContent = GetTagsContent(item.Tags),
                    Owner = item.Owner.Name,
                    NumUniqueWebsiteViews = item.NumUniqueWebsiteViews,
                    NumSubscriptions = item.NumSubscriptions,
                    NumFavorites = item.NumFavorites
                });
            }
        } while (result.Value.ResultCount == 50);

        return itemInfos;
    }

    /// <summary>
    /// 删除创意工坊物品
    /// </summary>
    /// <param name="id"></param>
    public static async void DeletePublishedFile(ulong id)
    {
        var fileId = new PublishedFileId
        {
            Value = id
        };

        var result = await SteamUGC.DeleteFileAsync(fileId);
        if (result)
            MsgBoxUtil.Information($"物品ID {fileId} 删除成功");
        else
            MsgBoxUtil.Error($"物品ID {fileId} 删除失败");
    }
}