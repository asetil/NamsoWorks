using Aware.Model;
using System;
using System.Collections.Generic;

namespace Cinescope.Web.Models
{
    public class Player : BaseEntity
    {
        public string Name { get; set; }

        public string BirthPlace { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public string Photo { get; set; }

        public string ShortDesc { get; set; }

        public List<Film> Films;
    }
}
