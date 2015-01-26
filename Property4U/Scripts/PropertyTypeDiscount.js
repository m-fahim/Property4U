// Discount Field hide Control
$("#Status").change(function () {
    var value = $(this).val();
    if (value == 4)
    {
        $("input#Discount").show();
    }else{
        $("input#Discount").hide();
    }
}).change();

// Fields hide Control
HideFields();
function HideFields() {
    $(".house-com, .house, .com, .bung, .shared, .developed").hide();
}
        
// Show fields related to Type
$("#OfTypeID").change(function () {
    $("#OfSubType").html('');
    var value = $(this).val();
    var valueTitle = $("#OfTypeID option:selected").text();
    if (value == "") {
        HideFields();
        $('#OfSubType').attr("disabled", true);
    }
    
    var subItems = "";

    var tokenKey = 'accessToken';
    var token = $.cookie(tokenKey);
    var headers = {};
    if (token) {
        headers.Authorization = 'Bearer ' + token;
    }

    if (value != "") {
        $.ajax({
            url: location.protocol + "//" + location.host + '/api/OfSubTypes/GetOfSubTypes/' + value,
            headers: headers,
            dataType: 'json',
            success: function (data) {
                $.each(data, function (index, item) {
                    subItems += "<option value='" + item.ID + "'>" + item.Title + "</option>";
                });


                $('#OfSubType').attr("disabled", false);
                $("#OfSubType").html(subItems)

                if (valueTitle == "House") {
                    HideFields();
                    $(".house-com, .house, .com, .bung, .shared, .developed").show();

                } else if (valueTitle == "Apartment") {
                    HideFields();
                    $(".house, .com, .shared, .developed").show();

                } else if (valueTitle == "Land") {
                    HideFields();
                    $(".house-com, .house, .com, .bung, .shared, .developed").hide();

                } else if (valueTitle == "Commercial") {
                    HideFields();
                    $(".house-com, .com, .shared, .developed").show();

                }
            }
        });
    }

}).change();

// Validate type fields in CreateProperty form 
$("#CreateProperty").submit(function(){
    var isFormValid = true;
    $(".house-com:visible input, .house:visible input, .com:visible input, .bung:visible input, .shared:visible input, .developed:visible input, input#Discount:visible, .house:visible select, .bung:visible select").each(function (index, value) {
        if ($.trim($(value).val()).length == 0) {
            var name = $(value).attr("name");
            // Eliminates replication of message and class
            if (!$(value).hasClass("input-validation-error")) {
                $(value).addClass("input-validation-error");
                $(value).after('<span class="field-validation-error text-danger" data-valmsg-replace="true">The ' + name + ' field is required.</span>');
            }
            isFormValid = false;
        } else {
            $(value).removeClass("input-validation-error");
        }
    });
    //if (!isFormValid) alert("Please fill in all the required fields (highlighted in red)");
    return isFormValid;
});