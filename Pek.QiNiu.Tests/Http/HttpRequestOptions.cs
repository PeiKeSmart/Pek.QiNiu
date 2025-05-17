using System.Collections.Specialized;
using System.Net;

using NUnit.Framework;

using Qiniu.Http;

namespace Pek.QiNiu.Tests.Http;

[TestFixture]
public class HttpRequestOptionsTests
{
    [Test]
    public void SetUrlTest()
    {
        var reqOpts = new Qiniu.Http.HttpRequestOptions();
        reqOpts.Url = "https://qiniu.com/index.html";

        var wReq = reqOpts.CreateHttpWebRequest();
        Assert.That(wReq.Address.ToString(), Is.EqualTo("https://qiniu.com/index.html"));
        wReq.Abort();

        reqOpts.Url = "https://www.qiniu.com/index.html";
        wReq = reqOpts.CreateHttpWebRequest();
        Assert.That(wReq.Address.ToString(), Is.EqualTo("https://www.qiniu.com/index.html"));
        wReq.Abort();
    }

    [Test]
    public void SetPropertiesTest()
    {
        var reqOpts = new Qiniu.Http.HttpRequestOptions();
        reqOpts.Url = "https://qiniu.com/index.html";

        reqOpts.AllowAutoRedirect = false; // default true
        reqOpts.AllowReadStreamBuffering = true; // default false
        reqOpts.AllowWriteStreamBuffering = false; // default true
                                                   // reqOpts.AutomaticDecompression = ;
                                                   // reqOpts.CachePolicy =;
                                                   // reqOpts.ClientCertificates = System.Security.Cryptography.X509Certificates.X509CertificateCollection;
        reqOpts.ConnectionGroupName = "qngroup"; // default ""
                                                 // reqOpts.ContinueDelegate =;
        reqOpts.ContinueTimeout = 360; // default 350
                                       // reqOpts.CookieContainer =;
                                       // reqOpts.Credentials =;
                                       // reqOpts.ImpersonationLevel = Delegation;
        reqOpts.KeepAlive = false; // default true
        reqOpts.MaximumAutomaticRedirections = 10; // default 50
        reqOpts.MaximumResponseHeadersLength = 32; // default 64
        reqOpts.MediaType = "video/mp4"; // default ""
        reqOpts.Method = "POST"; // default "GET"
        reqOpts.Pipelined = false; // default true
        reqOpts.PreAuthenticate = true; // default false
                                        // reqOpts.Proxy = System.Net.SystemWebProxy;
        reqOpts.ReadWriteTimeout = 200000; // default 300000
        reqOpts.SendChunked = true; // default false
                                    // reqOpts.ServerCertificateValidationCallback =;
        reqOpts.Timeout = 50000; // default 100000
        reqOpts.UnsafeAuthenticatedConnectionSharing = true; // default false
        reqOpts.UseDefaultCredentials = true; // default false

        var wReq = reqOpts.CreateHttpWebRequest();
        Assert.That(wReq.AllowAutoRedirect, Is.False);
        Assert.That(wReq.AllowReadStreamBuffering, Is.True);
        Assert.That(wReq.AllowWriteStreamBuffering, Is.False);
                                                                // Assert.AreEqual(, wReq.AutomaticDecompression); // default Null 
                                                                // Assert(, wReq.CachePolicy); // default Null
                                                                // Assert(, wReq.ClientCertificates); // default System.Security.Cryptography.X509Certificates.X509CertificateCollection
        Assert.That(wReq.ConnectionGroupName, Is.EqualTo("qngroup"));
                                                              // Assert(, wReq.ContinueDelegate); // default Null
        Assert.That(wReq.ContinueTimeout, Is.EqualTo(360));
                                                    // Assert(, wReq.CookieContainer); // default Null
                                                    // Assert(, wReq.Credentials); // default Null
                                                    // Assert(, wReq.ImpersonationLevel); // default Null
        Assert.That(wReq.KeepAlive, Is.False);
        Assert.That(wReq.MaximumAutomaticRedirections, Is.EqualTo(10));
        Assert.That(wReq.MaximumResponseHeadersLength, Is.EqualTo(32));
        Assert.That(wReq.MediaType, Is.EqualTo("video/mp4"));
        Assert.That(wReq.Method, Is.EqualTo("POST"));
        Assert.That(wReq.Pipelined, Is.False);
        Assert.That(wReq.PreAuthenticate, Is.True);
                                                     // Assert(, wReq.Proxy); // default System.Net.SystemWebProxy;
        Assert.That(wReq.ReadWriteTimeout, Is.EqualTo(200000));
        Assert.That(wReq.SendChunked, Is.True);
                                                 // Assert(, wReq.ServerCertificateValidationCallback); // default Null
        Assert.That(wReq.Timeout, Is.EqualTo(50000));
        Assert.That(wReq.UnsafeAuthenticatedConnectionSharing, Is.True);
        Assert.That(wReq.UseDefaultCredentials, Is.True);

        wReq.Abort();
    }

    [Test]
    public void SetHeadersTest()
    {
        var reqOpts = new Qiniu.Http.HttpRequestOptions();
        reqOpts.Url = "https://qiniu.com/index.html";

        reqOpts.Headers = new StringDictionary
            {
                { "Accept", "text/plain" },
                { "content-Type", "text/plain" },
                { "date", "Wed, 03 Aug 2011 04:00:00 GMT" },
                { "expect", "200-ok" },
                { "host", "qiniu.com" },
                { "if-modified-since", "Wed, 03 Aug 2011 04:00:00 GMT" },
                { "referer", "https://qiniu.com/" },
                { "transfer-encoding", "gzip" },
                { "user-agent", "qn-csharp-sdk" },
                { "X-Qiniu-A", "qn" }
            };

        reqOpts.SendChunked = true;

        var wReq = reqOpts.CreateHttpWebRequest();
        Assert.That(wReq.Accept, Is.EqualTo("text/plain"));
        Assert.That(wReq.ContentType, Is.EqualTo("text/plain"));
        Assert.That(wReq.Date, Is.EqualTo(DateTime.Parse("Wed, 03 Aug 2011 04:00:00 GMT")));
        Assert.That(wReq.Expect, Is.EqualTo("200-ok"));
        Assert.That(wReq.Host, Is.EqualTo("qiniu.com"));
        Assert.That(wReq.IfModifiedSince, Is.EqualTo(DateTime.Parse("Wed, 03 Aug 2011 04:00:00 GMT")));
        Assert.That(wReq.Referer, Is.EqualTo("https://qiniu.com/"));
        Assert.That(wReq.TransferEncoding, Is.EqualTo("gzip"));
        Assert.That(wReq.UserAgent, Is.EqualTo("qn-csharp-sdk"));
        Assert.That(wReq.Headers["X-Qiniu-A"], Is.EqualTo("qn"));

        wReq.Abort();
    }
}