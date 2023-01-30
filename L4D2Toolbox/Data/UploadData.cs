namespace L4D2Toolbox.Data;

public class UploadData
{
    public bool IsPublish { get; set; }
    public string IMGPath { get; set; }
    public string VPKPath { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Visibility { get; set; }
    public List<string> Tags { get; set; }
    public string ChangeLog { get; set; }
    public ulong FileId { get; set; }
}
