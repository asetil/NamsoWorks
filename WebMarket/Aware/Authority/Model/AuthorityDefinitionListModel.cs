using System.Collections.Generic;
using Aware.Util.Lookup;

namespace Aware.Authority.Model
{
    public class AuthorityDefinitionListModel
    {
        public List<AuthorityDefinition> AuthorityDefinitions { get; set; }
        public List<Lookup> AuthorityTypeList { get; set; }
        public List<Lookup> AuthorityModeList { get; set; }
    }
}