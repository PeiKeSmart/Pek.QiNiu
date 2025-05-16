using Pek.Configs;

using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Pek.QiNiu.Extensions;

public class QiniuCloud
{
    private static readonly String? Ak = OssSetting.Current.QiNiu.AccessKey;
    private static readonly String? Sk = OssSetting.Current.QiNiu.SecretKey;
    private static readonly String? Bucket = OssSetting.Current.QiNiu.Bucket;  //空间名
    private static readonly String? BasePath = OssSetting.Current.QiNiu.BasePath;
    private static readonly String? Domain = OssSetting.Current.QiNiu.Domain;

    /// <summary>
    /// 获取存储区域
    /// </summary>
    /// <returns></returns>
    public static Zone GetZone
    {
        get
        {
            switch (OssSetting.Current.QiNiu.Zone)
            {
                default:
                case "华南":
                    return Zone.ZONE_CN_South;

                case "华东":
                    return Zone.ZONE_CN_East;

                case "华北":
                    return Zone.ZONE_CN_North;

                case "北美":
                    return Zone.ZONE_US_North;

                case "东南亚":
                    return Zone.ZONE_AS_Singapore;
            }
        }
    }

    /// <summary>
    /// 根据前缀获得文件列表
    /// </summary>
    /// <param name="prefix">指定前缀，只有资源名匹配该前缀的资源会被列出</param>
    /// <param name="marker">上一次列举返回的位置标记，作为本次列举的起点信息</param>
    /// <returns></returns>
    public static CloudFile List(string prefix = "case", string marker = "")
    {
        var model = new CloudFile();
        Mac mac = new Mac(Ak, Sk);
        // 设置存储区域
        Config config = new Config
        {
            Zone = GetZone,
            UseHttps = false
        };
        BucketManager bucketManager = new BucketManager(mac, config);
        // 指定目录分隔符，列出所有公共前缀（模拟列出目录效果）
        string delimiter = "";
        // 本次列举的条目数，范围为1-1000
        int limit = 20;
        prefix = BasePath + prefix;
        ListResult listRet = bucketManager.ListFiles(Bucket, prefix, marker, limit, delimiter);
        model.Code = listRet.Code;
        model.Page = listRet.Result?.Marker;
        if (model.Code != 200)
        {
            model.Message = listRet.Text;
        }
        if (listRet.Code == (int)HttpCode.OK)
        {
            var list = new List<ListInfo>();
            foreach (var item in listRet.Result?.Items!)
            {
                list.Add(new ListInfo
                {
                    Name = Domain + item.Key,
                    Size = item.Fsize,
                    Type = item.MimeType,
                    Time = item.PutTime.ToDateTime(),
                });
            }
            model.list = list;
        }
        return model;
    }

    /// <summary>
    /// 删除云端图片
    /// </summary>
    /// <param name="filename">文件名称</param>
    /// <returns></returns>
    public static CloudFile Delete(string filename)
    {
        var model = new CloudFile();
        Mac mac = new Mac(Ak, Sk);
        // 设置存储区域
        Config config = new Config
        {
            Zone = GetZone,
            UseHttps = false
        };
        BucketManager bucketManager = new BucketManager(mac, config);
        // 文件名
        filename = filename.Replace(Domain!, "");
        HttpResult deleteRet = bucketManager.Delete(Bucket, filename);
        model.Code = deleteRet.Code;
        if (model.Code != 200)
        {
            model.Message = deleteRet.Text;
        }
        return model;
    }

    /// <summary>
    /// 文件上传
    /// </summary>
    /// <param name="filepath">文件物理地址</param>
    public static CloudFile UploadFile(string prefix = "else", string filepath = "")
    {
        var model = new CloudFile();
        try
        {
            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            Mac mac = new Mac(Ak, Sk);
            string saveKey = BasePath + prefix;
            string localFile = filepath;
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = Bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            //putPolicy.DeleteAfterDays = 1;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Qiniu.Util.Auth.CreateUploadToken(mac, jstr);
            //设置上传域名区域
            Config config = new Config
            {
                Zone = GetZone,
                UseHttps = false
            };
            UploadManager um = new UploadManager(config);
            HttpResult result = um.UploadFile(localFile, saveKey, token, null);
            model.Code = result.Code;
            if (model.Code != 200)
            {
                model.Message = result.Text;
            }
        }
        catch (Exception ex)
        {
            model.Code = 500;
            model.Message = ex.Message;
        }
        return model;
    }

    /// <summary>
    /// 文件上传
    /// </summary>
    /// <param name="data">待上传的数据</param>
    public static CloudFile UploadData(string prefix = "else", byte[]? data = null)
    {
        var model = new CloudFile();
        try
        {
            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            Mac mac = new Mac(Ak, Sk);
            string saveKey = BasePath + prefix;
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = Bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            //putPolicy.DeleteAfterDays = 1;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Qiniu.Util.Auth.CreateUploadToken(mac, jstr);
            //设置上传域名区域
            Config config = new Config
            {
                Zone = GetZone,
                UseHttps = false
            };
            UploadManager um = new UploadManager(config);
            HttpResult result = um.UploadData(data, saveKey, token, null);
            model.Code = result.Code;
            if (model.Code != 200)
            {
                model.Message = result.Text;
            }
        }
        catch (Exception ex)
        {
            model.Code = 500;
            model.Message = ex.Message;
        }
        return model;
    }

    /// <summary>
    /// 获取Token
    /// </summary>
    /// <returns></returns>
    public static CloudFile GetToken()
    {
        var model = new CloudFile();
        try
        {
            Mac mac = new Mac(Ak, Sk);
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = Bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            //putPolicy.DeleteAfterDays = 1;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Qiniu.Util.Auth.CreateUploadToken(mac, jstr);
            model.Token = token;
            model.Page = BasePath;
        }
        catch (Exception ex)
        {
            model.Code = 500;
            model.Message = ex.Message;
        }
        return model;
    }

}