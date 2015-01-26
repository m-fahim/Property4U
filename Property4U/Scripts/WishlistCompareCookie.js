(function ($) {
    $.fn.extend({
        cookieList: function (cookieName) {
            var cookie = $.cookie(cookieName);

            var items = cookie ? eval("([" + cookie + "])") : [];

            return {
                add: function (val) {
                    var index = items.indexOf(val);

                    // Note: Add only unique values.
                    if (index == -1 && items.length < 200) {
                        items.push(val);
                        $.cookie(cookieName, items.join(','), { expires: 365, path: '/' });
                    }
                },
                remove: function (val) {
                    var index = items.indexOf(val);

                    if (index != -1) {
                        items.splice(index, 1);
                        $.cookie(cookieName, items.join(','), { expires: 365, path: '/' });
                    }
                },
                indexOf: function (val) {
                    return items.indexOf(val);
                },
                clear: function () {
                    items = [];
                    $.cookie(cookieName, items, { path: '/' });
                },
                items: function () {
                    return items;
                },
                length: function () {
                    return items.length;
                },
                join: function (separator) {
                    return items.join(separator);
                }
            };
        }
    });
})(jQuery);