namespace Pek.QiNiu.Extensions;

public class QiniuListParmDto
{
    public String? prefix { get; set; }

    public String? marker { get; set; }
}

public class QiniuDelParmDto
{
    public String? filename { get; set; }
}

public class QiniuDelByPathParmDto
{
    public String? prefix { get; set; }

    public String? filepath { get; set; }
}