using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtmgrpc.Driver
{
    public interface IDtmDriver
    {
        string GetName();

        void RegisterGrpcResolver();

        void RegisterGrpcService(string target, string endpoint);

        (string server, string serviceName, string method, string error) ParseServerMethod(string url);
    }
}
