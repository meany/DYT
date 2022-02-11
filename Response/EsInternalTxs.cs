using System;
using System.Collections.Generic;

namespace dm.DYT.Response
{
    public class EsInternalTxs
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<EsInternalTxsResult> Result { get; set; }
    }

    public class EsInternalTxsResult
    {
        public string BlockNumber { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Value { get; set; }
        public string ContractAddress { get; set; }
        public string Input { get; set; }
        public string Type { get; set; }
        public string Gas { get; set; }
        public string GasUsed { get; set; }
        public string IsError { get; set; }
        public string ErrCode { get; set; }
    }
}
