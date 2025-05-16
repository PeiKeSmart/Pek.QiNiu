using System.Collections.Generic;
using System.Collections.Specialized; // Added for StringDictionary

using NUnit.Framework;
using NUnit.Framework.Legacy; // Ensure this is present

using Qiniu.Http;

namespace Pek.QiNiu.Tests.Http;

class RecorderMiddleware : IMiddleware
{
    private readonly List<string> _orderRecorder;

    private readonly string _label;

    public RecorderMiddleware(List<string> orderRecorder, string label)
    {
        _orderRecorder = orderRecorder;
        _label = label;
    }

    public HttpResult Send(Qiniu.Http.HttpRequestOptions req, DNextSend next)
    {
        _orderRecorder.Add("bef_" + _label + _orderRecorder.Count);
        var result = next(req);
        _orderRecorder.Add("aft_" + _label + _orderRecorder.Count);
        return result;
    }
}

[TestFixture]
public class MiddlewareTests
{
    [Test]
    public void SendWithMiddlewareTest()
    {
        var httpManager = new HttpManager(true);

        var orderRecorder = new List<string>();

        var middlewares = new List<IMiddleware>
            {
                new RecorderMiddleware(orderRecorder, "A"),
                new RecorderMiddleware(orderRecorder, "B")
            };

        var resp = httpManager.Get("https://example.com/index.html", new StringDictionary(), "", middlewares); // Changed second and third arguments

        Assert.That(resp.Code, Is.EqualTo((int)HttpCode.OK), resp.ToString()); // Changed to Assert.That
        CollectionAssert.AreEqual(
            new List<string>
            {
                    "bef_A0",
                    "bef_B1",
                    "aft_B2",
                    "aft_A3"
            },
            orderRecorder
        );
    }

    [Test]
    public void RetryDomainsMiddlewareTest()
    {

        var httpManager = new HttpManager(true);

        var orderRecorder = new List<string>();

        var middlewares = new List<IMiddleware>
            {
                new RetryDomainsMiddleware(
                    new List<string>
                    {
                        "unavailable.csharpsdk.qiniu.com",
                        "example.com"
                    },
                    3
                ),
                new RecorderMiddleware(orderRecorder, "A")
            };

        var resp = httpManager.Get("https://fake.csharpsdk.qiniu.com/index.html", new StringDictionary(), "", middlewares); // Changed second and third arguments

        Assert.That(resp.Code, Is.EqualTo((int)HttpCode.OK), resp.ToString()); // Changed to Assert.That

        CollectionAssert.AreEqual(
            new List<string>
            {
                    // fake.csharpsdk.qiniu.com
                    "bef_A0",
                    "aft_A1",
                    "bef_A2",
                    "aft_A3",
                    "bef_A4",
                    "aft_A5",
                    // unavailable.csharpsdk.qiniu.com
                    "bef_A6",
                    "aft_A7",
                    "bef_A8",
                    "aft_A9",
                    "bef_A10",
                    "aft_A11",
                    // qiniu.com
                    "bef_A12",
                    "aft_A13"
            },
            orderRecorder
        );
    }
}