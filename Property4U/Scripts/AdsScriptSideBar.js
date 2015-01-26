$.ajax({
    url: location.protocol + "//" + location.host + '/api/Ads/GetActiveAdsWithDetailsSquare',
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
            adsCollectionClass.push((v.Order.Size === 2) ? "square-banner" : "fat-skyscraper-banner");
        });
        if (adsCollectionSrc.length != 0) {
            var randNum = getRandomInt(0, adsCollectionSrc.length - 1)
            var topAdBanner = $('#sidead > a > img');
            var newBannerImg = imgPrefix + adsCollectionSrc[randNum];
            var newBannerLink = adsCollectionHref[randNum]; // update new img src and link HREF value 
            $('#sidead').addClass(adsCollectionClass[randNum]);
            $(topAdBanner).attr('src', newBannerImg);
            $('#sidead > a').attr('href', newBannerLink);
        }
    }
});