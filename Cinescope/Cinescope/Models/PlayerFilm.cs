using Aware.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinescope.Web.Models
{
    public class PlayerFilm : BaseEntity
    {
        public int FilmId { get; set; }
        
        public int PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [ForeignKey("FilmId")]
        public virtual Film Film { get; set; }
    }
}
