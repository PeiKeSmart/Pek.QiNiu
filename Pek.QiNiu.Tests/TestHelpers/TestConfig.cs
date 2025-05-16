using Qiniu.Storage;
using Qiniu.Util;

namespace Pek.QiNiu.Tests.TestHelpers
{
    /// <summary>
    /// 测试配置辅助类
    /// </summary>
    public static class TestConfig
    {
        /// <summary>
        /// 获取测试用的Mac（密钥）
        /// </summary>
        /// <returns>Mac实例</returns>
        public static Mac GetTestMac()
        {
            return new Mac("test-access-key", "test-secret-key");
        }

        /// <summary>
        /// 获取测试用的Config（配置）
        /// </summary>
        /// <returns>Config实例</returns>
        public static Config GetTestConfig()
        {
            return new Config
            {
                Zone = Zone.ZONE_CN_East,
                UseHttps = true,
                UseCdnDomains = false,
                ChunkSize = ChunkUnit.U4096K,
                MaxRetryTimes = 3
            };
        }

        /// <summary>
        /// 获取测试用的PutPolicy（上传策略）
        /// </summary>
        /// <param name="bucket">存储空间名称</param>
        /// <returns>PutPolicy实例</returns>
        public static PutPolicy GetTestPutPolicy(string bucket = "test-bucket")
        {
            var putPolicy = new PutPolicy
            {
                Scope = bucket
            };
            putPolicy.SetExpires(3600);
            return putPolicy;
        }
    }
}
