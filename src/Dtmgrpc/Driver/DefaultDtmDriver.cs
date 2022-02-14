namespace Dtmgrpc.Driver
{
    public class DefaultDtmDriver : IDtmDriver
    {
        private static readonly int PathPartCount = 3;
        private static readonly string DefaultName = "default";

        public string GetName() => DefaultName;

        public (string server, string serviceName, string method, string error) ParseServerMethod(string url)
        {
            try
            {
                var uri = new Uri(url);

                var host = uri.Host;
                var port = uri.Port;
                var scheme = uri.Scheme;
                var path = uri.AbsolutePath;

                // /servicename/method
                var arr = path.Split('/');

                if (arr.Length < PathPartCount) return (string.Empty, string.Empty, string.Empty, $"bad url: {url}.");

                return ($"{scheme}://{host}:{port}", arr[1], arr[2], "");
            }
            catch (Exception ex)
            {
                return (string.Empty, string.Empty, string.Empty, ex.Message);
            }
        }

        public void RegisterGrpcResolver()
        {
        }

        public void RegisterGrpcService(string target, string endpoint)
        {
        }
    }
}
