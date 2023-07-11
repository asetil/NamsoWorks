using System.Linq;
using System.Collections.Generic;
using Aware.Payment.Model;

namespace Aware.ECommerce.Model
{
    public class InstallmentViewModel
    {
        public int DrawMode { get; set; }
        public List<InstallmentInfo> Installments { get; set; }
        public List<PosDefinition> PosList { get; set; }
        public decimal Total { get; set; }
        public List<InstallmentInfo> AvailableInstallments
        {
            get
            {
                if (Installments != null && PosList != null)
                {
                    return Installments.Where(i => PosList.Any(p => p.ID == i.PosID)).ToList(); ;
                }
                return Installments;
            }
        }
    }
}