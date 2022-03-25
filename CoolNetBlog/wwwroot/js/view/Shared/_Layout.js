$('.toSearchBtn').click(function () {
    // 若搜索框输入值不是空白 get跳转搜索
    var kwValue = $('#' + event.target.id).parent('form').children(':input[name="kw"]').val();
    if (kwValue == undefined || kwValue == 'undefined' || $.trim(kwValue) == '') {
        return;
    }
    var getSearchUri = '/Home/Index/?kw=' + $.trim(kwValue) + '&from=keyword';
    $(location).attr('href', getSearchUri);
});

function searchInputEnterDown(target) {
    if (event.key == 'Enter') {
        var kwValue = $(target).val();
        if (kwValue == undefined || kwValue == 'undefined' || $.trim(kwValue) == '') {
            return;
        }
        var getSearchUri = '/Home/Index/?kw=' + $.trim(kwValue) + '&from=keyword';
        $(location).attr('href', getSearchUri);
    }
}