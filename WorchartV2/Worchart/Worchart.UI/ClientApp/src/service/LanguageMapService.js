var languageList = {
    TR: 1,
    EN: 2,
    KR: 3
};

var resourceMap = [];
resourceMap["BTN.LOGIN"] = { tr: "Giriş Yap", en: "Login", kr: "Girmişka" };
resourceMap["BTN.SAVE"] = { tr: "Kaydet", en: "Save", kr: "Gaydetmışka" };
resourceMap["BTN.CANCEL"] = { tr: "Vazgeç", en: "Cancel", kr: "Haral lo!" };
resourceMap["BTN.SELECTFILE"] = { tr: "Seç", en: "Select", kr: "Seç" };

resourceMap["LBL.LOGINTITLE"] = { tr: "Üye Girişi", en: "Login", kr: "Girmişka" };
resourceMap["LBL.EMAIL"] = { tr: "E-posta", en: "Email", kr: "Meyıl" };
resourceMap["LBL.PASSWORD"] = { tr: "Şifre", en: "Password", kr: "Şifra wa" };
resourceMap["LBL.REMEMBERME"] = { tr: "Beni Hatırla", en: "Remember Me", kr: "Hotirlamişka" };
resourceMap["LBL.FORGOTPASSWORD"] = { tr: "Şifremi Unuttum", en: "Forgot Password", kr: "Şifrake Bıda" };
resourceMap["LBL.REGISTER"] = { tr: "Hemen Üye Ol", en: "Register Now", kr: "Üye olmişka" };
resourceMap["LBL.USERLOGIN"] = { tr: "Uzman Girişi", en: "Professionel", kr: "Dızane" };
resourceMap["LBL.COMPANYLOGIN"] = { tr: "Firma Girişi", en: "Firm", kr: "Şirkate" };
resourceMap["LBL.FIRMCODE"] = { tr: "Firma Kodu", en: "Firm Code", kr: "Şirkat Nomra" };
resourceMap["LBL.TEAMNAME"] = { tr: "Ekip Adı", en: "Team Name", kr: "Nove Ekibe" };
resourceMap["LBL.TEAMSHORTNAME"] = { tr: "Kısa Adı", en: "Short Name", kr: "Noveb cuk" };
resourceMap["LBL.DESCRIPTION"] = { tr: "Açıklama", en: "Description", kr: "Da bıbe" };
resourceMap["LBL.TEAMLOGO"] = { tr: "Logo Seçimi", en: "Logo", kr: "Resme Ekibe" };
resourceMap["LBL.SAVETEAM"] = { tr: "Ekip Tanımla", en: "Save Team", kr: "Ekip Tanimlamişka" };
resourceMap["LBL.SELECTFILE"] = { tr: "Dosya seçiniz", en: "Select files", kr: "Dosya seçmişka" };

resourceMap["MSG.LOADING"] = { tr: "Yükleniyor..", en: "Loading..", kr: "Co bıskına.." };
resourceMap["MSG.INPUTREQUIRED"] = { tr: "Bu alanı boş geçemezsiniz!", en: "This field must fulfilled!", kr: "Vo van noba gurbone!" };
resourceMap["MSG.REGISTER"] = { tr: "Henüz üyeliğiniz yok mu?", en: "Don't you have an account?", kr: "Çırtki lo?" };
resourceMap["MSG.COMMONERROR.TITLE"] = { tr: "İşlem Başarısız", en: "Operation Failed", kr: "Av Çıbu Lo" };
resourceMap["MSG.COMMONERROR"] = { tr: "İşleminiz gerçekleştirilirken bir hata meydana geldi. Lütfen daha sonra tekrar deneyiniz!", en: "An error occurred while performing your transaction. Please try again later!", kr: "Av Çıbu Lo" };
resourceMap["MSG.AUTHORIZATIONFAILED"] = { tr: "İşleminize devam etmek için giriş yapmalısınız!", en: "You must be logged-in to continue!", kr: "Av Çıbu Lo" };
resourceMap["MSG.TEAM.NOUSERADDED"] = { tr: "Henüz bu ekibe kullanıcı eklenmemiş!", en: "No user has been added to this team!", kr: "Lıve ekibe çolışane gasık tuneyna!" };

class LanguageMapService {
    currentLanguage = languageList.TR;

    get(key, defaultValue) {
        if (key) {
            var resource = resourceMap[key];
            if (resource) {
                switch (this.currentLanguage) {
                    case languageList.TR: return resource.tr;
                    case languageList.EN: return resource.en;
                    case languageList.KR: return resource.kr;
                    default:
                }
            }
        }
        return defaultValue || key;
    }

    changeLanguage(language) {
        if (language) {
            this.currentLanguage = language;
        }
    }
}

export default LanguageMapService = new LanguageMapService();