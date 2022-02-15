using System.Collections.Generic;

namespace Dtmgrpc.DtmGImp
{
    public class TransBase
    {
        public string Gid { get; set; }

        public string TransType { get; set; }

        public string CustomData { get; set; }

        public bool WaitResult { get; set; }

        public long TimeoutToFail { get; set; }

        public long RetryInterval { get; set; }

        public List<string> PassthroughHeaders { get; set; }

        public Dictionary<string, string> BranchHeaders { get; set; }

        /// <summary>
        /// use in MSG/SAGA
        /// </summary>
        public List<Dictionary<string, string>> Steps { get; set; }

        /// <summary>
        /// used in MSG/SAGA
        /// </summary>
        public List<string> Payloads { get; set; }

        public List<byte[]> BinPayloads { get; set; }

        /// <summary>
        /// used in XA/TCC
        /// </summary>
        public BranchIDGen BranchIDGen { get; set; }

        /// <summary>
        /// used in XA/TCC
        /// </summary>
        public string Op { get; set; }

        /// <summary>
        /// used in MSG
        /// </summary>
        public string QueryPrepared { get; set; }

        public string Dtm { get; set; }

        public static TransBase NewTransBase(string gid, string transType, string dtm, string branchID)
        {
            return new TransBase
            {
                Gid = gid,
                TransType = transType,
                Dtm = dtm,
                BranchIDGen = new BranchIDGen(branchID),
            };
        }
    }
}
