using Aware.Model;
using System.Collections.Generic;

namespace Cinescope.Web.Models
{
    public class Film : BaseEntity
    {
        public string Adi { get; set; }

        public string Aciklama { get; set; }

        public string Afis { get; set; }

        public decimal ImdbPuani { get; set; }

        public int YapimYili { get; set; }

        public string YapimSirketi { get; set; }

        public decimal Sure { get; set; }

        public string FragmanAdresi { get; set; }

        public string IzlemeAdresi { get; set; }

        public List<Player> Players;
    }
}
