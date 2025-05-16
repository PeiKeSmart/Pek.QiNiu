namespace Pek.QiNiu.Extensions;

public class ListInfo
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public String? Name { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public Int64 Size { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public String? Type { get; set; }

    /// <summary>
    /// 上传时间
    /// </summary>
    public DateTime Time { get; set; }
}

public class CloudFile
{
    public Int32 Code { get; set; } = 200;
    public String? Message { get; set; }
    public String? Page { get; set; } = "";
    public String? Token { get; set; }
    public List<ListInfo>? list { get; set; }
}