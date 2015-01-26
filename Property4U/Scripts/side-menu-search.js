/*====================================
 search-menu-items - Sidebar
======================================*/
//$("#search-menu-items").change(function () {
//    $('#nested-menu-item').click()
//});

(function ($, undefined) {
    $.expr[":"].containsNoCase = function (el, i, m) {
        var search = m[3];
        if (!search) return false;
        return new RegExp(search, "i").test($(el).text());
    };

    $.fn.searchFilter = function (options) {

        var opt = $.extend({
            // target selector
            targetSelector: "",
            // number of characters before search is applied
            charCount: 0
        }, options);

        return this.each(function () {

            var $el = $(this);
            $el.keyup(function () {
                var search = $(this).val();

                var $target = $(opt.targetSelector);
                $target.show();

                if (search && search.length >= opt.charCount)
                    $target.not(":containsNoCase(" + search + ")").hide("fast", function () {
                        // Use arguments.callee so we don't need a named function
                        $(this).prev().hide("fast", arguments.callee);
                    });

            });
        });
    };
})(jQuery);

$("#search-menu-items").searchFilter({ targetSelector: "#menu-item a", charCount: 1 })

/* Set an #page-wrapper Height Equal to the Window’s Height for sidebar length control */
//$(document).ready(function() {
//    function setHeight() {
//        windowHeight = $(window).innerHeight();
//        $('#page-wrapper').css('min-height', windowHeight);
//        /* Set an .body-content Height Equal to the Window’s Height for footer */
//        $('.body-content').css('min-height', windowHeight-63);
//    };
//    setHeight();
  
//    $(window).resize(function() {
//        setHeight();
//    });
//});