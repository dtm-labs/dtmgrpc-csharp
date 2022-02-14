using Dtmgrpc.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dtmgrpc.Tests.Driver
{
    public class DefaultDtmDriverTests
    {
        [Fact]
        public void ParseServerMethod_Should_Succeed()
        {
            var d = new DefaultDtmDriver();

            var (server, serviceName, method, error) = d.ParseServerMethod("http://localhost:9999/dtmgimp.Dtm/Prepare");

            Assert.Equal("http://localhost:9999", server);
            Assert.Equal("dtmgimp.Dtm", serviceName);
            Assert.Equal("Prepare", method);
            Assert.Empty(error);
        }

        [Fact]
        public void ParseServerMethod_Should_Fail()
        {
            var d = new DefaultDtmDriver();

            var (server, serviceName, method, error) = d.ParseServerMethod("http://localhost:9999/Prepare");

            Assert.Empty(server);
            Assert.Empty(serviceName);
            Assert.Empty(method);
            Assert.NotEmpty(error);
        }
    }
}
