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

let scrollPos= 0;
let ticking = false;

// 固定顶部导航栏在顶部位置 当页面滚动时
function navFixedHandler(scrollPos) {
    if (scrollPos > 100) {
        $('#topNav').addClass('fixed-top');
    } else {
        $('#topNav').removeClass('fixed-top');
    }
}

// 当页面滚动时 触发事件
window.addEventListener('scroll', function (e) {
    scrollPos = window.scrollY;
    navFixedHandler(scrollPos);
});