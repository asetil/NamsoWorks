using Aware.Regional.Model;
using Aware.Util.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMarket.Admin.Models
{
    public class RegionDisplayModel
    {
        public List<Region> RegionList { get; set; }
        public RegionType Type { get; set; }
        public int ParentID { get; set; }
    }
}