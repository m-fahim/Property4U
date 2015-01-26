$.ajax({
    url: location.protocol + "//" + location.host + '/api/Ads/GetActiveAdsWithDetailsWide',
    async: true,
    dataType: 'json',
    success: function (data) {
        var adsCollectionSrc = new Array();
        var adsCollectionHref = new Array();
        var adsCollectionClass = new Array();
        var imgPrefix = "/Content/Uploads/Ads/";
        $.each(data, function (k, v) {
            adsCollectionSrc.push(v.Path);
            adsCollectionHref.push(v.WebsiteURL);
            adsCollectionClass.push((v.Order.Size === 0) ? "full-banner" : "small-banner");
        });
        if (adsCollectionSrc.length != 0) {
            var randNum = getRandomInt(0, adsCollectionSrc.length - 1)
            var topAdBanner = $('#topad > a > img');
            var newBannerImg = imgPrefix + adsCollectionSrc[randNum];
            var newBannerLink = adsCollectionHref[randNum]; // update new img src and link HREF value 
            $('#topad').addClass(adsCollectionClass[randNum]);
            $(topAdBanner).attr('src', newBannerImg);
            $('#topad > a').attr('href', newBannerLink);
        }
    }
});