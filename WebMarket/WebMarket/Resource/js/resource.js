$.ResourceManager = function () {
    var EN_Resources = new Array();
    var TR_Resources = new Array();
    this.lang = 0; //0:TR, 1:EN
    this.Value = function (key) {
        if (this.lang == 1) {
            return EN_Resources[key];
        }
        return TR_Resources[key];
    };

    this.init = function () {
        this.lang = $('#SiteLanguage').val();
        loadTRResources();
        loadENResources();
    };

    function loadTRResources() {
        TR_Resources['Search_MinQueryLength'] = 'Arama yapabilmek için en az üç karakter girmelisiniz!';
        TR_Resources['General_ErrorMessage'] = 'İşlem gerçekleştirilirken bir hata ile karşılaştık. Lütfen daha sonra tekrar deneyiniz!';
        TR_Resources['Basket_Empty'] = 'Sepetiniz Boş';
        TR_Resources['Basket_EmptyMessage'] = 'Sepetinizde henüz ürün yok.';
    }

    function loadENResources() {
        EN_Resources['Search_MinQueryLength'] = 'To start search at least 3 characters neeeded!';
        EN_Resources['General_ErrorMessage'] = 'An error occurred while processing your request. Please try again later!';
        EN_Resources['Basket_Empty'] = 'Empty Basket';
        EN_Resources['Basket_EmptyMessage'] = 'Your basket is empty.';

    }
};

var resource = new $.ResourceManager();
$(document).ready(function() {
    resource.init();
});