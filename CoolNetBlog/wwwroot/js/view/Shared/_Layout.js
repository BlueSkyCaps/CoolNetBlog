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
    if (scrollPos > 1) {
        $('#topNav').addClass('fixed-top');
    } else {
        $('#topNav').removeClass('fixed-top');
    }
}

// 当页面滚动时 触发事件
window.addEventListener('scroll', function () {
    scrollPos = window.scrollY;
    navFixedHandler(scrollPos);
});


// 当此DOM加载完毕 执行此回调代码 处理"闲言碎语"组件的数据加载与显示
$('#gossipDiv').ready(function () {
    var isShow = $('#gossipShowInput').prop("checked")
    if (!isShow) {
        return;
    }
    var domStr = gossipDataCall();
    if (domStr=="") {
        return;
    }

    $('#gossipDiv').removeAttr("hidden");
});

let currentGossipIndex = 1;

// '闲言碎语'带有图片的dom元素字符串
var gosspRowImgTextDomStr = '<div class="row">< div class="col-4 d-flex align-items-center">' +
    '<img class="gossipImg img-fluid" src="{2}"></div><div class="col-8 d-flex align-items-center ">' +
    '<div class="card-body"><p class="card-text text-secondary">{0}<br><small class="text-muted">{1}</small></p>' +
    '</div></div></div>';

// '闲言碎语'不带有图片的dom元素字符串
var gosspRowTextDomStr = '<div class="row">< div class="col-12 d-flex align-items-center">' +
    '<div class="card-body"><p class="card-text text-secondary">{0}<br><small class="text-muted">{1}</small></p>' +
    '</div></div></div>';

// 获取'闲言碎语'数据
function gossipDataCall() {
    currentGossipIndex++;
    var url = "/Gossip/GetGossips?index=" + currentGossipIndex;
    var domStr = "";
    $.ajax(
        {
            url: url,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            success: function (data, status) {
                if (data['code'] == 1) {
                    if (data['data'].length <= 0) {
                        // 没有更多数据 滚动条底部显示文字
                        currentGossipIndex--;
                        return;
                    }
                    // 追加当前获取的数据到组件滚动区域底部
                    $.each(data['data'], function (i, gItem) {
                        // 格式化时间
                        var addTimeStr = Gb_GetFmtDateStr(gItem['addTime']);
                        // type==1 文字，2带图片。格式化元素字符串，第一个是内容 第二个是时间 第三个若有是图片的地址
                        if (gItem['type']==1) {
                            domStr += gosspRowTextDomStr.format(gItem['content'], addTimeStr);
                        } else {
                            domStr += gosspRowTextDomStr.format(gItem['content'], addTimeStr, gItem['ImgUrl']);
                        }
                        return domStr;
                    });
                } else {
                    currentGossipIndex--;
                    Gb_PopoverShow("commentShowBtn", data['tipMessage']);
                }
            },
            error: function (err) {
                currentGossipIndex--;
                Gb_PopoverShow("commentShowBtn", "加载失败err,重试一下?!");
                console.log(err)
            },
            complete: function () {
            }
        }
    );
}