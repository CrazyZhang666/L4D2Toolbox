namespace L4D2Toolbox.Helper;

public static class HttpHelper
{
    private static readonly HttpClient client = new();

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
