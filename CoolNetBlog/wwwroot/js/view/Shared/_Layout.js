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

let _scrollPos= 0;

// 固定顶部导航栏在顶部位置 当页面滚动时
function navFixedHandler(_scrollPos) {
    if (_scrollPos > 1) {
        $('#topNav').addClass('fixed-top');
    } else {
        $('#topNav').removeClass('fixed-top');
    }
}

// 当页面滚动时 触发事件
window.addEventListener('scroll', function () {
    _scrollPos = window.scrollY;
    navFixedHandler(_scrollPos);
});

// 根据设备设置"闲言碎语"滚动区域的高度
function setGossipScrollStyle() {
    var wid = document.body.clientWidth;
    var needH = "200px";
    if (wid <= 768) {
        //small or Medium devices
        needH = "200px";
    } else if (wid <= 1200) {
        //Large devices
        needH = "300px";
    } else {
        // desktops
        needH = "400px";
    }
    $("#gossipScroll").css({
        "width": "100%",
        "height": needH,
        "overflow-y": "auto"
    });
}


// 事件：当"闲言碎语"滚动区域滚动时 判断是否滚动到了底部
document.getElementById("gossipScroll").onscroll = () => {
    var allContentHeight = $('#gossipScroll').scrollHeight;
    var scrollTopPos = $('#gossipScroll').scrollTop;
    var gossipScrollHeight = $('#gossipScroll').clientHeight;
    console.log(allContentHeight)
    console.log(scrollTopPos)
    console.log(gossipScrollHeight)
    console.log()

    if ((gossipScrollHeight + scrollTopPos) >= allContentHeight) {
        alert("已经到达底部");
        gossipDataCall();
    }
    if (scrollTopPos == 0) {
        alert("已经到达顶部");
    }
}

var _gossipCallRes;

// 当此DOM加载完毕 执行此回调代码 处理"闲言碎语"组件的数据加载与显示
$('#gossipDiv').ready(function () {
    var isShow = $('#gossipShowInput').prop("checked")
    if (!isShow) {
        return;
    }
    setGossipScrollStyle();

    // 首次，直接调用接口获取首次的数据
    gossipDataCall();
});

let _currentGossipIndex = 0;

// '闲言碎语'带有图片的dom元素字符串
var gosspRowImgTextDomStr = '<div class="row"><div class="col-4 d-flex align-items-center">' +
    '<img class="gossipImg img-fluid" src="{2}"></div><div class="col-8 d-flex align-items-center ">' +
    '<div class="card-body"><p class="card-text text-secondary">{0}<br><small class="text-muted">{1}</small></p>' +
    '</div></div></div>';

// '闲言碎语'不带有图片的dom元素字符串
var gosspRowTextDomStr = '<div class="row"><div class="col-12 d-flex align-items-center">' +
    '<div class="card-body"><p class="card-text text-secondary">{0}<br><small class="text-muted">{1}</small></p>' +
    '</div></div></div>';

/**
 * 获取'闲言碎语'数据
 * return status: bool, robj: string
 * */
function gossipDataCall() {
    _currentGossipIndex++;
    var url = "/Gossip/GetGossips?index=" + _currentGossipIndex;
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
                        _currentGossipIndex--;
                        _gossipCallRes = { "status": false, "robj": "(已被你翻得底朝天啦)" };
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
                            domStr += gosspRowImgTextDomStr.format(gItem['content'], addTimeStr, gItem['imgUrl']);
                        }
                    });
                    _gossipCallRes =  { "status": true, "robj": domStr};
                } else {
                    _currentGossipIndex--;
                    _gossipCallRes = { "status": false, "robj": data['tipMessage'] };
                }
            },
            error: function (err) {
                _currentGossipIndex--;
                _gossipCallRes = { "status": false, "robj": "加载失败err,重试一下?!"};
                console.log(err)
            },
            complete: function () {
                // 处理当前gossipDataCall ajax最终处理完的结果
                $('#gossipDiv').removeAttr("hidden");
                if (!_gossipCallRes.status) {
                    // 没有数据字符串获取到，统一在组件底部显示文字
                    $('#gossipScroll').append('<p class="text-secondary text-center"><small>' + _gossipCallRes.robj + '</small></p>');
                    return;
                }
                // 追加重绘到组件区域 
                $('#gossipScroll').append(_gossipCallRes.robj);
            }
        }
    );
}