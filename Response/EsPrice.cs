using System;
using System.Collections.Generic;
using System.Text;

namespace dm.DYT.Response
{
    public class EsPrice
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public EsPriceResult Result { get; set; }
    }

    public class EsPriceResult
    {
        public string EthBtc { get; set; }
        public string EthBtc_Timestamp { get; set; }
        public string EthUsd { get; set; }
        public string EthUsd_Timestamp { get; set; }
    }
}
