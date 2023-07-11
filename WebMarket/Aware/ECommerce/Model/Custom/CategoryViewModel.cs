using System.Collections.Generic;
using Aware.File.Model;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace Aware.ECommerce.Model.Custom
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public FileGalleryModel FileGallery { get; set; }
        public string HierarchyInfo { get; set; }
        public bool HasMultiLanguage { get; set; }
        public bool AllowEdit { get; set; }
        public List<Lookup> StatusList { get; set; }
    }
}