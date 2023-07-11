using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Authority.Model;
using Aware.Cache;
using Aware.Data;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.Authority
{
    public class AuthorityManager : IAuthorityManager
    {
        private readonly ICacher _cacher;
        private readonly IRepository<AuthorityDefinition> _authorityRepository;
        private readonly IRepository<AuthorityUsage> _authorityUsageRepository;
        private IEnumerable<AuthorityDefinition> _definitions;
        private readonly ILogger _logger;

        public AuthorityManager(ICacher cacher, IRepository<AuthorityDefinition> authorityRepository, IRepository<AuthorityUsage> authorityUsageRepository, ILogger logger)
        {
            _cacher = cacher;
            _authorityRepository = authorityRepository;
            _authorityUsageRepository = authorityUsageRepository;
            _logger = logger;
        }

        public IEnumerable<AuthorityDefinition> Definitions
        {
            get
            {
                if (_definitions == null)
                {
                    LoadDefinitions();
                }
                return _definitions;
            }
        }

        public AuthorityDefinition GetAuthorityDefinition(int id)
        {
            return _authorityRepository.Get(id);
        }

        public List<AuthorityDefinition> GetAuthorityDefinitions(bool enableQuota)
        {
            try
            {
                if (enableQuota)
                {
                    return _authorityRepository.Where(i => i.ID > 0).ToList();
                }
                return _authorityRepository.Where(i => i.ID > 0 && i.Mode == AuthorityMode.Single).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error("CommonService > GetAuthorityDefinitions - Failed", ex);
                return null;
            }
        }

        public bool Check(AuthorityType authorityType, int relationID, int relationType)
        {
            if (relationID > 0)
            {
                var definition = Definitions.FirstOrDefault(i => i.Type == authorityType);
                if (definition != null)
                {
                    var usage = GetAuthorityUsage(authorityType, relationID, relationType);
                    if (definition.Mode == AuthorityMode.Single && usage != null)
                    {
                        return true;
                    }

                    if (definition.Mode == AuthorityMode.WithQuota && usage != null)
                    {
                        return usage.Quota > 0 && usage.ExpireDate > DateTime.Now;
                    }
                }
            }
            return false;
        }

        public List<AuthorityDefinition> GetAuthorityUsages(int relationID, int relationType, bool enableQuota)
        {
            try
            {
                var authorities = _authorityUsageRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType && i.Status == Statuses.Active).ToList();
                var definitions = GetAuthorityDefinitions(enableQuota);
                definitions = definitions.Select(i =>
                {
                    i.Usage = authorities.FirstOrDefault(a => a.DefinitionID == (int)i.Type);
                    return i;
                }).OrderBy(o => o.Mode).ToList();
                return definitions;
            }
            catch (Exception ex)
            {
                _logger.Error("CommonService > GetAuthorityUsages - Failed for relID:{0}, relType:{1}", ex, relationID, relationType);
                return null;
            }
        }

        public List<AuthorityUsage> GetAuthorityUsages(int relationID, int relationType)
        {
            try
            {
                var authorities = _authorityUsageRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType && i.Status == Statuses.Active).ToList();
                return authorities;
            }
            catch (Exception ex)
            {
                _logger.Error("CommonService > GetAuthorityUsages2 - Failed for relID:{0}, relType:{1}", ex, relationID, relationType);
                return null;
            }
        }

        public Result SaveAuthority(AuthorityDefinition model)
        {
            try
            {
                if (model != null)
                {
                    var existingDefinitions = _authorityRepository.Where(i => i.Type == model.Type).ToList();
                    if (existingDefinitions != null && existingDefinitions.Any(i => i.ID != model.ID))
                    {
                        return Result.Error(Resource.Authority_ValueExists);
                    }

                    if (model.ID <= 0)
                    {
                        _authorityRepository.Add(model);
                        return Result.Success(model, Resource.General_Success);
                    }

                    var definition = _authorityRepository.Get(model.ID);
                    if (definition == null) { throw new Exception("AuthorityDefinition Not Found"); }

                    Mapper.Map(ref definition, model);
                    _authorityRepository.Update(definition);
                    return Result.Success(definition, Resource.General_Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("CommonService > SaveAuthority - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result DeleteAuthority(int authorityID)
        {
            try
            {
                if (authorityID > 0)
                {
                    var result = _authorityRepository.Delete(authorityID);
                    if (result)
                    {
                        return Result.Success(null, Resource.General_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("CommonService > DeleteAuthority - Failed", ex);
            }
            return Result.Error(Resource.General_Error);
        }

        public Result SaveAuthorityUsage(int relationID, int relationType, List<AuthorityUsage> authorities, bool enableQuota)
        {
            var guid = _authorityUsageRepository.StartTransaction();
            try
            {
                authorities = authorities ?? new List<AuthorityUsage>();
                if (relationID > 0)
                {
                    var definitions = GetAuthorityDefinitions(enableQuota);
                    var persistUsages = _authorityUsageRepository.Where(i => i.RelationID == relationID && i.RelationType == relationType)
                                            .ToList().Where(a => definitions.Any(d => (int)d.Type == a.DefinitionID)).ToList();

                    var deletedUsages = persistUsages.Where(i => i.Status == Statuses.Active && authorities.All(a => a.DefinitionID != i.DefinitionID)).ToList();
                    if (deletedUsages.Any())
                    {
                        foreach (var authorityUsage in deletedUsages)
                        {
                            authorityUsage.Status = Statuses.Deleted;
                            _authorityUsageRepository.Update(authorityUsage, false);
                        }
                    }

                    var updateUsages = persistUsages.Where(i => authorities.Any(a => a.DefinitionID == i.DefinitionID)).ToList();
                    if (updateUsages.Any())
                    {
                        foreach (var authorityUsage in updateUsages)
                        {
                            var authority = authorities.FirstOrDefault(i => i.DefinitionID == authorityUsage.DefinitionID) ?? new AuthorityUsage();
                            authorityUsage.Quota = authority.Quota;
                            authorityUsage.ExpireDate = authority.ExpireDate > DateTime.MinValue ? authority.ExpireDate : new DateTime(2000, 1, 1);
                            authorityUsage.Status = Statuses.Active;
                            _authorityUsageRepository.Update(authorityUsage, false);
                        }
                    }

                    var newUsages = authorities.Where(i => persistUsages.All(a => a.DefinitionID != i.DefinitionID)).ToList();
                    if (newUsages.Any())
                    {
                        foreach (var authority in newUsages)
                        {
                            authority.RelationID = relationID;
                            authority.RelationType = relationType;
                            authority.DateCreated = DateTime.Now;
                            authority.ExpireDate = authority.ExpireDate > DateTime.MinValue ? authority.ExpireDate : new DateTime(2000, 1, 1);
                            authority.Status = Statuses.Active;
                            _authorityUsageRepository.Add(authority, false);
                        }
                    }
                    _authorityUsageRepository.Save();
                }

                _authorityUsageRepository.Commit(guid);
                return Result.Success(null, Resource.General_Success);
            }
            catch (Exception ex)
            {
                _logger.Error("CommonService > SaveAuthorityUsage - Failed", ex);
                _authorityUsageRepository.Rollback(guid);
            }
            return Result.Error(Resource.General_Error);
        }

        private void LoadDefinitions()
        {
            try
            {
                _definitions = GetAuthorityDefinitions(true);
            }
            catch (Exception ex)
            {
                _logger.Error("AuthorityManager > LoadDefinitions - Failed", ex);
                _definitions = new List<AuthorityDefinition>();
            }
        }

        private AuthorityUsage GetAuthorityUsage(AuthorityType authorityType, int relationID, int relationType)
        {
            try
            {
                var cacheKey = string.Format("AuthorityUsage_{0}_{1}", relationID, relationType);
                var result = _cacher.Get<string>(cacheKey);
                if (string.IsNullOrEmpty(result))
                {
                    var usages = GetAuthorityUsages(relationID, relationType);
                    result = Common.Serialize(usages);
                    _cacher.Add(cacheKey, result);
                    return usages.FirstOrDefault(i => i.DefinitionID == (int)authorityType);
                }

                var authorityUsages = result.DeSerialize<IEnumerable<AuthorityUsage>>();
                return authorityUsages.FirstOrDefault(i => i.DefinitionID == (int)authorityType);
            }
            catch (Exception ex)
            {
                _logger.Error("AuthorityManager > GetAuthorityUsage - Failed for : {0}-{1}-{2}", ex, authorityType, relationID, relationType);
            }
            return null;
        }
    }
}
