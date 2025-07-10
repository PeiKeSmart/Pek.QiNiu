namespace Qiniu.Util
{
    /// <summary>
    /// 账户访问控制(密钥)
    /// </summary>
    /// <remarks>
    /// 初始化密钥AK/SK
    /// </remarks>
    /// <param name="accessKey">AccessKey</param>
    /// <param name="secretKey">SecretKey</param>
    public class Mac(String? accessKey, String? secretKey)
    {
        /// <summary>
        /// 密钥-AccessKey
        /// </summary>
        public String? AccessKey { set; get; } = accessKey;

        /// <summary>
        /// 密钥-SecretKey
        /// </summary>
        public String? SecretKey { set; get; } = secretKey;
    }
}