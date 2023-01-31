namespace L4D2Toolbox.Helper;

public static class HttpHelper
{
    private static readonly HttpClient client = new();

    /// <summary>
    /// 获取url内容
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> DownloadString(string url)
    {
        return await client.GetStringAsync(url);
    }

    /// <summary>
    /// 下载网络图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="savePath"></param>
    public static async Task DownloadImags(string url, string savePath)
    {
        var urlContents = await client.GetByteArrayAsync(url);
        using var stream = new FileStream(savePath, FileMode.Create);
        stream.Write(urlContents);
        stream.Close();
    }
}
