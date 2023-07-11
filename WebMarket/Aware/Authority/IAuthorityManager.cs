using System.Collections.Generic;
using Aware.Authority.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.Util.Enums;

namespace Aware.Authority
{
    public interface IAuthorityManager
    {
        bool Check(AuthorityType authorityType, int relationID, int relationType = (int)RelationTypes.User);
        AuthorityDefinition GetAuthorityDefinition(int id);
        List<AuthorityDefinition> GetAuthorityDefinitions(bool enableQuota);
        List<AuthorityDefinition> GetAuthorityUsages(int relationID, int relationType, bool enableQuota);
        List<AuthorityUsage> GetAuthorityUsages(int relationID, int relationType);
        Result SaveAuthority(AuthorityDefinition model);
        Result DeleteAuthority(int authorityID);
        Result SaveAuthorityUsage(int relationID, int relationType, List<AuthorityUsage> authorities, bool enableQuota);
    }
}
