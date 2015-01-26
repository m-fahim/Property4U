$(document).ready(function () {
    var wishlist = $.fn.cookieList("WishList"); // all items in the array.

    // Check Selected Items
    currentSelectionItem = (wishlist.items() != 0) ? wishlist.length() : 0;
    $("#totalWishlistItems").text(" (" + currentSelectionItem + ")");
    if (currentSelectionItem != 0) {
        $("#WishlistItems").val(wishlist.items());
    } else {

        $("#Wishlist a").removeAttr("onclick");
        $("#Wishlist input").remove();
        $("#WishlistSection input").remove();
    }

    $(".addToWishlist").click(function () {
        var selectionItem = parseInt($(this).attr("data-pid"));
        wishlist.add(selectionItem);
        window.location.hash += $(this).attr('href');
        location.reload();
    });

    $(".removeFromWishlist").click(function () {
        var selectionItem = parseInt($(this).attr("data-pid"));
        wishlist.remove(selectionItem);
        // Load Updated WishlistItem
        location.href = replaceUrlParam(window.location.href, "WishlistItems", wishlist.items());
    });

    $("#clearWishlist").click(function () {
        wishlist.clear();
        // Load Updated WishlistItem
        window.location.href = location.protocol + "//" + location.host + "/FrontEnd/Index";
    });
});