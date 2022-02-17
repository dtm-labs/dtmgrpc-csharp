using System;

namespace Dtmgrpc
{
    public class DtmException : Exception
    {
        public DtmException(string message)
            : base(message)
        {
        }

        public const string ErrFailure = "FAILURE";
        public const string ErrOngoing = "ONGOING";
        public const string ErrDuplicated = "DUPLICATED";
    }

    public class DtmDuplicatedException : DtmException
    {
        public DtmDuplicatedException(string message = ErrDuplicated)
            : base(message)
        {
        }
    }

    public class DtmOngingException : DtmException
    {
        public DtmOngingException(string message = ErrOngoing)
            : base(message)
        {
        }
    }

    public class DtmFailureException : DtmException
    {
        public DtmFailureException(string message = ErrFailure)
            : base(message)
        {
        }
    }
}
