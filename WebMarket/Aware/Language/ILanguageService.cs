using System.Collections.Generic;
using Aware.Language.Model;
using Aware.Util.Model;

namespace Aware.Language
{
    public interface ILanguageService
    {
        List<Model.Language> GetLanguages(bool onlyActive=false);
        List<Model.Language> GetCachedLanguages();
        List<LanguageValue> GetLanguageValues(int relationType, List<int> relationIDs,int languageID=0);
        string GetFieldContent(List<LanguageValue> valueList, int relationID, string fieldName, string defaultValue);
        LanguageValueDisplayModel GetDisplayModel(int relationID, int relationType);
        Result SaveValues(int relationID, int relationType, List<LanguageValue> languageValue);
        Result Save(Model.Language model);
        Result SaveValue(LanguageValue model);
        Result SetAsDefault(int languageID);
        Result DeleteLanguage(int languageID);
    }
}
