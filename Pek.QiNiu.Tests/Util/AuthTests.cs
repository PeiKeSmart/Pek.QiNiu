using System.Collections.Specialized;

using NUnit.Framework;

using Pek.QiNiu.Tests.TestHelpers;

using Qiniu.Util;

namespace Pek.QiNiu.Tests.Util
{
    /// <summary>
    /// 认证测试类
    /// </summary>
    [TestFixture]
    public class AuthTests
    {
        private Mac _mac;
        private Auth _auth;

        [SetUp]
        public void Setup()
        {
            _mac = TestConfig.GetTestMac();
            _auth = new Auth(_mac);
        }

        [Test]
        public void CreateUploadToken_ShouldReturnNonEmptyToken()
        {
            // Arrange
            var putPolicy = TestConfig.GetTestPutPolicy();
            string jsonBody = putPolicy.ToJsonString();

            // Act
            string token = Auth.CreateUploadToken(_mac, jsonBody);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        [Test]
        public void CreateDownloadToken_ShouldReturnNonEmptyToken()
        {
            // Arrange
            string url = "http://test-bucket.qiniudn.com/test.jpg";

            // Act
            string token = Auth.CreateDownloadToken(_mac, url);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        [Test]
        public void CreateManageToken_ShouldReturnNonEmptyToken()
        {
            // Arrange
            string url = "http://rs.qiniu.com/stat/test-bucket/test.jpg";
            byte[] body = Array.Empty<byte>();

            // Act
            string token = Auth.CreateManageToken(_mac, url, body);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
            Assert.That(token, Does.StartWith("QBox "));
        }

        private static Mac mac = new Mac("ak", "sk");
        private static Auth auth = new Auth(mac);

        [TestCaseSource(typeof(SignatureV2DataClass), nameof(SignatureV2DataClass.TestCases))]
        public string CreateManageTokenV2Test(string method, string url, StringDictionary headers,
            string body)
        {
            return auth.CreateManageTokenV2(method, url, headers, body);
        }

        [Test]
        public void CreateManageTokenV2Test()
        {
            string actual = auth.CreateManageTokenV2("GET", "http://rs.qbox.me");
            Assert.AreEqual("Qiniu ak:bgfeAqx6xXMIXA232e8ocxfhINc=", actual);
        }
    }
}
