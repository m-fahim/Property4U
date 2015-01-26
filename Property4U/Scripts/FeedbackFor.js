// Fields hide Control
HideFields();
function HideFields() {
    $(".process-feedback").hide();
}
        
// Show fields related to Type
$("#For").change(function () {
    var value = $(this).val();
    var valueTitle = $("#For option:selected").text();
    if (value == "") {
        HideFields();
    }

    if (valueTitle == "Process Feedback") {
        HideFields();
        $(".process-feedback").show();
    }
    else
    {
        $(".process-feedback").hide();
    }

}).change();

$("#CreateFeedback").submit(function () {
    var isFormValid = true;
    $(".process-feedback:visible select, .process-feedback:visible textarea").each(function (index, value) {
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