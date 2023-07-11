(function ($) {
    var handlers = {
        onSave: function () {
            if (aware.validate(".entry-panel")) {
                aware.showLoading();
                return true;
            }
            return false;
        },
        onDelete: function () {
            var entryID=$("#ID").val();
            aware.confirm("Entry silinecek. Devam etmek istiyor musunuz?", function () {
                $.post("/Entry/Delete", { entryID: entryID }, function (result) {
                    if (result.success == 1) {
                        aware.showMessage(result.message, '', '', 'fa-check');
                        window.location.href = "/manage/entry-list";
                    }
                    else {
                        aware.showError(result.message, 'İşlem Başarısz', 'fa-minus-circle');
                    }
                });
            }, "Onaylayın?");
            return false;
        },
        setTinyMce: function (selector, height) {
            if (tinymce != undefined) {
                if (tinymce.editors.length > 0) {
                    while (tinymce.editors.length > 0) {
                        var editorFieldName = tinymce.editors[0].id;
                        try {
                            //tinymce.EditorManager.execCommand('mceRemoveEditor', true, editorFieldName);
                            tinymce.remove("#" + editorFieldName);
                        } catch (e) {
                            //console.log("TinyMce Remove Error : ",e);
                        }
                    }
                }

                tinymce.init({
                    theme_advanced_resizing: false,
                    theme: "modern",
                    menubar: false,
                    statusbar: false,
                    toolbar_items_size: 'small',
                    verify_html: false,
                    //language: "tr_TR",
                    selector: selector,
                    height: height || 300,
                    plugins: [
                      'advlist autolink lists link image charmap print preview anchor',
                      'searchreplace visualblocks code fullscreen',
                      'insertdatetime media table contextmenu paste code'
                    ],
                    toolbar: 'styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media code',
                    content_css: '//www.tinymce.com/css/codepen.min.css',

                    setup: function (editor) {
                        editor.on('change', function () {
                            tinymce.triggerSave();
                        });
                    }
                });
            }
        },
        setTinyContent: function (id, data) {
            var tinyEditor = tinyMCE.get(id);
            if (tinyEditor != null) {
                tinyEditor.setContent(data);
            }
        },
        ready: function () {
            $(".category-selector").selecto({allowSearch:true});

            handlers.setTinyMce(".entry-content");
        }
    };

    $(function () {
        $(document).on('ready', undefined, {}, handlers.ready);
        $(document).on('click', '.btn-save', {}, handlers.onSave);
        $(document).on('click', '.btn-delete', {}, handlers.onDelete);
    });
}(jQuery));