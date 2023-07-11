using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aware.Payment.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CardInfo
    {
        [JsonProperty(PropertyName = "bin")]
        public string BinNumber { get; set; }

        [JsonProperty(PropertyName = "bank")]
        public int BankType { get; set; }

        [JsonProperty(PropertyName = "card")]
        public int CardType { get; set; }

        [JsonProperty(PropertyName = "brand")]
        public int Brand { get; set; }

        [JsonProperty(PropertyName = "pos")]
        public int PosType { get; set; }

        [JsonProperty(PropertyName = "credit")]
        public bool IsCredit { get; set; }

        public int PosID { get; set; }
        public decimal OrderTotal { get; set; }
        public List<InstallmentInfo> Installments { get; set; }
    }
}