namespace Dtmgrpc
{
    internal class Constant
    {
        internal static readonly string TYPE_TCC = "tcc";
        internal static readonly string TYPE_SAGA = "saga";
        internal static readonly string TYPE_MSG = "msg";

        internal static readonly string ResultFailure = "FAILURE";
        internal static readonly string ResultSuccess = "SUCCESS";
        internal static readonly string ResultOngoing = "ONGOING";

        /// <summary>
        /// error of DUPLICATED for only msg
        /// if QueryPrepared executed before call. then DoAndSubmit return this error
        /// </summary>
        internal static readonly string ResultDuplicated = "DUPLICATED";

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

        internal class Barrier
        {
            internal static readonly string DBTYPE_MYSQL = "mysql";
            internal static readonly string DBTYPE_POSTGRES = "postgres";
            internal static readonly string DBTYPE_SQLSERVER = "sqlserver";
            internal static readonly string PG_CONSTRAINT = "uniq_barrier";
            internal static readonly string MSG_BARRIER_REASON = "rollback";
            internal static readonly string MSG_BRANCHID = "00";
            internal static readonly string MSG_BARRIER_ID = "01";
        }
    }
}
