var selectedElement;
var dragging = false;

$(document).ready(function () {
    if ($('.drop-elem-wrapper').length > 0) {
        $('#dragHelper').hide();
        $(initDragger);
    }
});

function initDragger() {
    $('.drag-elem').draggable({
        start: SetDragProperties,
        drag: DoDrag,
        cursor: 'normal',
        cursorAt: { left: 37, top: 37 },
        revert: true,
        helper: myHelper,
        containment: 'document',
        stop: RefreshElementProperties
    });

    $('.drop-elem').droppable({
        drop: handleDropEvent
    });
}

function SetDragProperties(e, ui) {
    selectedElement = $(this);
    dragging = true;
    $('.drop-elem-wrapper').fadeIn(1000);
    $(".drop-elem-wrapper").position({
        my: "right top",
        at: "left top",
        of: this // or $("#otherdiv)
        //collision: "fit"
    });
}

function DoDrag(event, ui) {
    $('#dragHelper').show();
    $(this).css('cursor', 'pointer');
}

function myHelper(event) {
    return '<div id="dragHelper">+1</div>';
}

function RefreshElementProperties() {
    selectedElement = undefined;
    dragging = false;
    $('.drop-elem-wrapper').fadeOut(1000);
}

function handleDropEvent(event, ui) {
    dragging = false;
    $('#dragHelper').hide();

    if ($(this).hasClass('list-elem')) {
        var listID = $(this).data('list-id');
        var listName = $(this).data('list-name');
        addToList(listID, listName);
    }
    if ($(this).hasClass('basket-elem')) { addToBasket(); }
    if ($(this).hasClass('new-list-elem')) {
        addToNewList();
    }

    //setTimeout(function() { ui.draggable.remove(); }, 1); // yes, even 1ms is enough
    $(initDragger);
}

function addToBasket() {
    var _itemID = $(selectedElement).data('item-id');
    var postData = { storeItemID: _itemID, quantityInfo: -1 };

    aware.showLoading();
    $.post("/Basket/AddToBasket", postData, function (data) {
        aware.hideDialog();
        if (data.success == 1) {
            window.toastr.info('Ürün başarıyla sepetinize eklendi.');
            $('.search-area .btn-basket-summary .item-count').html(data.itemsCount);
        }
        else {
            aware.showError(data.message);
        }
    });
}

function addToList(_listID, _listName) {
    var _itemID = $(selectedElement).data('item-id');
    $.post("/Basket/AddToList", { listID: _listID, storeItemID: _itemID }, function (data) {
        if (data.success == 1) {
            window.toastr.info('Ürün başarıyla ' + _listName + ' listesine eklendi.');
        }
        else {
            aware.showError(data.message);
        }
    });
}

function addToNewList() {
    var name = prompt("Yeni listeniz için bir isim belirleyin", "Liste");
    if (name != null && name.length > 0) {
        var _itemID = $(selectedElement).data('item-id');
        var postData = { listID: 0, storeItemID: _itemID, listName: name };

        $.post("/Basket/AddToList", postData, function (data) {
            if (data.success == 1) {
                var newList = '<div class="drop-elem list-elem" data-list-id="' + data.listID + '" data-list-name="' + name + '">';
                newList += name + ' Listesine Ekle</div>';
                $('.drop-elem-wrapper .new-list-elem').before(newList);

                window.toastr.info('Ürün başarıyla ' + name + ' listesine eklendi.');
                var listCount = $('.drop-elem-wrapper .list-elem').length;
                if (listCount >= 4) { $('.drop-elem-wrapper .new-list-elem').hide(); }
            }
            else {
                aware.showError(data.message);
            }
        });
    }
    else {
        window.toastr.error('<p>Yeni listeye ekleme işlemine devam edilemiyor...</p>', 'İşlem Başarısız');
    }
}