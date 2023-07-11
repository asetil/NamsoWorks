using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.Payment.Model
{
    public class PosDefinitionDetailModel
    {
        public PosDefinition PosDefinition { get; set; }
        public List<Lookup> PosTypeList { get; set; }
        public List<Lookup> PaymentMethodList { get; set; }
    }
}