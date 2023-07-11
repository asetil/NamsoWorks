using System.Collections.Generic;
using Aware.Util.View;

namespace Aware.Language.Model
{
    public class LanguageValueDisplayModel
    {
        public int RelationID { get; set; }
        public int RelationType { get; set; }
        public List<Language> LanguageList { get; set; }
        public List<LanguageValue> ValueList { get; set; }
        public FieldHelper FieldHelper { get; set; }
    }
}
