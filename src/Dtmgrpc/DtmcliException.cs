using System;

namespace Dtmgrpc
{
    public class DtmcliException : Exception
    {
        public DtmcliException(string message)
            : base(message)
        {
        }
    }
}
