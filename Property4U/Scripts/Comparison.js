$(document).ready(function () {
    var Comparelist = $.fn.cookieList("Comparelist"); // all items in the array.

    // Check Selected Items
    currentSelectionItemC = (Comparelist.items() != 0) ? Comparelist.length() : 0;
    $("#totalComparelistItems").text(" (" + currentSelectionItemC + ")");
    if (currentSelectionItemC != 0) {
        $("#ComparelistItems").val(Comparelist.items());
    } else {

        $("#Comparelist a").removeAttr("onclick");
        $("#Comparelist input").remove();
    }

    $(".addToComparelist").click(function () {
        var selectionItem = parseInt($(this).attr("data-pid"));
        Comparelist.add(selectionItem);
        window.location.hash += $(this).attr('href');
        location.reload();
    });

    $(".removeFromComparelist").click(function () {
        var selectionItem = parseInt($(this).attr("data-pid"));
        Comparelist.remove(selectionItem);
        // Load Updated ComparelistItem
        location.href = replaceUrlParam(window.location.href, "ComparelistItems", Comparelist.items());
    });

    $("#clearComparelist").click(function () {
        Comparelist.clear();
        // Load Updated ComparelistItem
        window.location.href = location.protocol + "//" + location.host + "/FrontEnd/Index";
    });

});