using System;
using System.Collections.Specialized;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.Legacy; // Ensure this is present for ClassicAssert

using Qiniu.Http;
using Qiniu.Util;

namespace Pek.QiNiu.Tests.Http;

[TestFixture]
public class HttpManagerTests
{
    private static HttpManager httpManager = new HttpManager();
    private static MethodInfo? dynMethod = httpManager.GetType().GetMethod("addAuthHeaders", // Added ? for nullable
         BindingFlags.NonPublic | BindingFlags.Instance);
    private static Mac mac = new Mac("ak", "sk");

    [TearDown]
    public void EachTeardown()
    {
        Environment.SetEnvironmentVariable("DISABLE_QINIU_TIMESTAMP_SIGNATURE", null);
    }

    [Test]
    public void DisableQiniuTimestampSignatureDefaultTest()
    {
        var headers = new StringDictionary(); // Changed to var
        var auth = new Auth(mac); // Changed to var
        dynMethod?.Invoke(httpManager, new object[] { headers, auth }); // Added ?. for null conditional

        ClassicAssert.IsTrue(headers.ContainsKey("X-Qiniu-Date")); // Changed to ClassicAssert.IsTrue
    }

    [Test]
    public void DisableQiniuTimestampSignatureTest()
    {
        var headers = new StringDictionary(); // Changed to var
        var auth = new Auth(mac, new AuthOptions // Changed to var
        {
            DisableQiniuTimestampSignature = true
        });
        dynMethod?.Invoke(httpManager, new object[] { headers, auth }); // Added ?. for null conditional

        ClassicAssert.IsFalse(headers.ContainsKey("X-Qiniu-Date")); // Changed to ClassicAssert.IsFalse
    }

    [Test]
    public void DisableQiniuTimestampSignatureEnvTest()
    {
        Environment.SetEnvironmentVariable("DISABLE_QINIU_TIMESTAMP_SIGNATURE", "true");

        var headers = new StringDictionary(); // Changed to var
        var auth = new Auth(mac); // Changed to var
        dynMethod?.Invoke(httpManager, new object[] { headers, auth }); // Added ?. for null conditional

        ClassicAssert.IsFalse(headers.ContainsKey("X-Qiniu-Date")); // Changed to ClassicAssert.IsFalse
    }

    [Test]
    public void DisableQiniuTimestampSignatureEnvBeIgnoredTest()
    {
        Environment.SetEnvironmentVariable("DISABLE_QINIU_TIMESTAMP_SIGNATURE", "true");

        var headers = new StringDictionary(); // Changed to var
        var auth = new Auth(mac, new AuthOptions // Changed to var
        {
            DisableQiniuTimestampSignature = false
        });
        dynMethod?.Invoke(httpManager, new object[] { headers, auth }); // Added ?. for null conditional

        ClassicAssert.IsTrue(headers.ContainsKey("X-Qiniu-Date")); // Changed to ClassicAssert.IsTrue
    }
}