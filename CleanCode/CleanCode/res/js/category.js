(function ($) {
    var elem = {
        categoryItem: ".category-list .category-item",
        btnSaveCategory: ".btn-save-category",
        btnAddSubCategory: ".btn-add-sub-category",
        btnDeleteCategory: ".btn-delete-category",
        btnHierarchy: ".category-list .category i.direction",
        toggleIcon: "i.toggle-icon"
    };

    var handlers = {
        getCategory: function () {
            var el = $(this);
            var categoryID = $(this).data("id");
            $.post("/Category/GetCategory", { categoryID: categoryID }, function (response) {
                if (response.success == 1) {
                    $(".category-panel").html(response.html);
                    $(elem.categoryItem).removeClass("active");
                    $(el).addClass("active");
                }
                else {
                    aware.showToastr("Kategori bilgisi yüklenemedi!", "error");
                }
            });
            return false;
        },
        saveCategory: function () {
            if (aware.validate(elem.categoryDetail)) {
                var id = $(elem.categoryDetail + " #CategoryID").val();
                var parentID = $(elem.categoryDetail + " #ParentID").val();
                var name = $(elem.categoryDetail + " #Name").val();
                var status = $(elem.categoryDetail + " #Status").val();

                var postData = { id: id, parentID: parentID, name: name, status: status };
                $.post("/Category/Save", postData, function (data) {
                    if (data.success == 1) {
                        if (id > 0) {
                            $(elem.categoryItem + "[data-category-id='" + id + "'] .name").html(name);
                            aware.showMessage('Kategori başarıyla kaydedildi.');
                        } else {
                            var message = "'" + name + "' kategorisi başarıyla oluşturuldu. <br> <span style='font-size:16px;color:#fff;'>Sayfa Yenileniyor..</span>";
                            aware.showLoading(message);
                            aware.delayedRefresh(1000);
                        }
                    }
                    else {
                        aware.showError("İstek işlenirken bir hata ile karşılaşıldı. Lütfen daha sonra tekrar deneyin!");
                    }
                });
            }
        },
        deleteCategory: function () {
            aware.confirm('Bu kategoriyi silmek istediğinize emin misiniz?', function () {
                var id = $(".category-detail #CategoryID").val();
                var category = $(elem.categoryItem + "[data-category-id='" + id + "']").parent();

                $.post("/Category/Delete", { categoryID: id }, function (data) {
                    if (data.success == 1) {
                        aware.showMessage(undefined, data.message);

                        var parent = $(category).parents("li:eq(0)");
                        if (parent && parent.length > 0) {
                            $(parent).find("span.category:eq(0)").click();
                        } else {
                            $(elem.categoryItem + ":eq(0)").click();
                        }
                        $(category).remove();
                    }
                    else {
                        aware.showError(data.message);
                    }
                });
            });
            return false;
        },
        refreshHierarchy: function () {
            var parent = $(this).parent();
            var id = $(parent).data('category-id');
            var direction = $(this).data('direction');

            aware.showLoading();
            $.post("/Category/RefreshHierarchy", { categoryID: id, direction: direction }, function (data) {
                aware.hideDialog();
                if (data.success == 1) {
                    parent = $(parent).parent();
                    var replaceItem = direction == 1 ? $(parent).next() : $(parent).prev();
                    if (replaceItem) {
                        $(parent).remove();
                        direction == 1 ? $(parent).insertAfter(replaceItem) : $(parent).insertBefore(replaceItem);
                    }
                }
            });
            return false;
        },
        addSubCategory: function () {
            $(elem.btnAddSubCategory).hide();
            $(elem.btnDeleteCategory).hide();

            var parentID = $(elem.categoryDetail + ' #CategoryID').val();
            var hierarchy = $(elem.categoryDetail + ' #Hierarchy').html();

            $(elem.categoryDetail + ' #CategoryID').val(0);
            $(elem.categoryDetail + ' #ParentID').val(parentID);
            $(elem.categoryDetail + ' #Name').val('');
            $(elem.categoryDetail + ' #Hierarchy').html(hierarchy + ' > Yeni Kategori').removeClass("dn");
            $(elem.categoryDetail + ' .status-selector ul li[data-value="' + 1 + '"]').click();
            return false;
        },
        toggleHierarchy: function () {
            var parent = $(this).parents("li:eq(0)");
            var isOpen = $(parent).hasClass("open");

            if (isOpen) {
                $(parent).find("ul:eq(0)").slideUp();
                $(this).removeClass("fa-minus").addClass("fa-plus");
                $(parent).removeClass("open");
            }
            else {
                $(parent).find("ul:eq(0)").slideDown();
                $(this).removeClass("fa-plus").addClass("fa-minus");
                $(parent).addClass("open");
            }
            return false;
        },
        ready: function () {
            $(elem.categoryItem + ":eq(0)").click();
        }
    };

    $(function () {
        $(document).on("ready", undefined, {}, handlers.ready);
        $(document).on("click", elem.categoryItem, {}, handlers.getCategory);
    });
}(jQuery));