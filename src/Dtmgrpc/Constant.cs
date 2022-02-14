namespace Dtmgrpc
{
    internal class Constant
    {
        internal static readonly string TYPE_TCC = "tcc";
        internal static readonly string TYPE_SAGA = "saga";
        internal static readonly string TYPE_MSG = "msg";

        internal class Op
        {
            internal static readonly string Submit = "Submit";
            internal static readonly string Prepare = "Prepare";
            internal static readonly string Abort = "Abort";
        }

        internal class Md
        {
            internal static readonly string Gid = "dtm-gid";
            internal static readonly string TransType = "dtm-trans_type";
            internal static readonly string BranchId = "dtm-branch_id";
            internal static readonly string Op = "dtm-op";
            internal static readonly string Dtm = "dtm-dtm";
        }
    }
}
