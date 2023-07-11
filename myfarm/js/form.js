var formHelper = {
    build: function (fields, cssClass) {
        cssClass = cssClass || "";
        var form = "<form class='" + cssClass + "'>";

        for (const field of fields) {
            var data = "name='" + field.name + "' id='" + field.name + "' data-validate='" + (field.validate ? "true" : "false") + "' class='" + (field.css ? field.css : "") + "'";
            if (field.type == "input") {
                form += formHelper.input(field, data);
            }
            else if (field.type == "select") {
                form += formHelper.select(field, data);
            }
            else if (field.type == "label") {
                form += formHelper.label(field);
            }
            else if (field.type == "button") {
                form += formHelper.button(field);
            }
        }

        form += "</form>";
        return form;
    },
    validate: function (selector) {
        var fields = $(selector + " .form-field [data-validate='true']");
        var isValid = true;

        for (const field of fields) {
            $(field).removeClass("invalid");
            if ($(field).prop("tagName") == "İNPUT" || field.type == "text") {
                var value = $(field).val();
                if (!value || value == "") {
                    isValid = false;
                    $(field).addClass("invalid");
                }
            }
        }

        if (!isValid) {
            $(selector + " .form-field .invalid:eq(0)").focus();
        }
        return isValid;
    },
    input: function (field, data) {
        field.maxLength = field.maxLength || 0;
        return "<div class='form-field'><input " + data + " type='text' placeholder='" + field.title + "' " + (field.maxLength > 0 ? "max-length='" + field.maxLength + "'" : "") + "/></div>";
    },
    select: function (field, data) {
        var options = field.list.map(m => "<option value='" + m.value + "'>" + m.name + "</option>")
        return "<div class='form-field'><select " + data + " placeholder='" + field.title + "'>" + options + "</select></div>";
    },
    label: function (field) {
        return "<div class='form-field'><label class='" + (field.css ? field.css : "") + "'>" + field.title + "</label></div>";
    },
    button: function (field) {
        return "<div class='form-field'><button class='mt20 mr10 " + (field.css ? field.css : "") + "'>" + field.title + "</button></div>";
    }
};