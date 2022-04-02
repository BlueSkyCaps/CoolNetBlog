// 把日期对象转为通用字符串形式
function Gb_GetFmtDateStr(dateTime) {
    var date = new Date(dateTime);
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();
    var fmtTimeStr = year + "-" + (month > 9 ? month : "0" + month) + "-"
        + (day > 9 ? day : "0" + day) + " "
        + (hours > 9 ? hours : "0" + hours) + ":"
        + (minutes > 9 ? minutes : "0" + minutes) + ":"
        + (seconds > 9 ? seconds : "0" + seconds);
    return fmtTimeStr;
}

// 把日期对象转为"xx年|月|周|天|时|分|秒前"的字符串形式
function Gb_GetFlowTimeStr(dateTime) {
    // 格式化时间字符串
    var fmtTimeStr = Gb_GetFmtDateStr(dateTime);
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var week = day * 7;
    var month = day * 30;
    var year = month * 12;
    // 当前时间的时间戳 精确为毫秒 因此second以1000为基数
    var nowStamp = new Date().getTime();
    // 指定时间的时间戳
    var thefmtTimeStamp = Date.parse(fmtTimeStr);
    var deStamp = nowStamp - thefmtTimeStamp;

    var flowTimeStr = "";
    if (deStamp < 0) {
        flowTimeStr = "时间来自未来";
    } else if (deStamp / year >= 1) {
        flowTimeStr = parseInt(deStamp / year) + "年前";
    } else if (deStamp / month >= 1) {
        flowTimeStr = parseInt(deStamp / month) + "月前";
    } else if (deStamp / week >= 1) {
        flowTimeStr = parseInt(deStamp / week) + "周前";
    } else if (deStamp / day >= 1) {
        flowTimeStr = parseInt(deStamp / day) + "天前";
    } else if (deStamp / hour >= 1) {
        flowTimeStr = parseInt(deStamp / hour) + "小时前";
    } else if (deStamp / minute >= 1) {
        flowTimeStr = parseInt(deStamp / minute) + "分钟前";
    } else if (deStamp / second >= 1) {
        flowTimeStr = parseInt(deStamp / second) + "秒前";
    }
    return flowTimeStr;
}

function Gb_IsWhiteSpaceOrNull(v) {
    if (v === null || v === undefined || v === "undefined")
        return true;
    if (v.replace(/(^\s*)|(\s*$)/g, "") === "")
        return true;
    return false;
}

String.prototype.format = function () {
    if (arguments.length == 0) return this;
    for (var s = this, i = 0; i < arguments.length; i++)
        s = s.replace(new RegExp("\\{" + i + "\\}", "g"), arguments[i]);
    return s;
};

String.prototype.isWhiteSpace = function () {
    if (this.replace(/(^\s*)|(\s*$)/g, "")==="")
        return true;
    return false;
};

String.prototype.trimF = function (p) {
    if (p==="l"||p==="L")
        return this.replace(/(^\s*)/g, "");
    if (p === "r" || p === "R")
        return this.replace(/(\s*$)/g, "");
    return this.replace(/(^\s*)|(\s*$)/g, "");
};

/**
 * bootstrap.Popover组件 统一按钮定时弹出提示文本
 * @param {any} idStr 对应按钮元素id
 * @param {any} tipMsg 要弹出显示的提示文本
 * @param {any} dis 是否时间到禁用指定按钮
 * @param {any} lazyText 定时到时是否改变按钮的文本
 */
function Gb_PopoverShow(idStr, tipMsg, dis = false, lazyText = "") {
    $('#' + idStr).attr('data-bs-content', tipMsg);
    var popover = new bootstrap.Popover($('#' + idStr));
    popover.show();
    setTimeout(function () {
        popover.dispose();
        if (lazyText !== "") {
            $('#' + idStr).text(lazyText);
        }
        if (dis) {
            $('#' + idStr).attr("disabled", "disabled");
        }
    }, 3000);
}

/**
 * UIkit.notification组件 通知信息
 * @param {any} message 文本
 * @param {any} status 状态样式
 * @param {any} pos 显示位置
 */
function Gb_NotifShow(message = "", status = "success", pos = 'top-center') {
    UIkit.notification({
        message: message,
        status: status,
        pos: pos,
        timeout: 3000
    });
}

function Gb_isEmail(input) {
    return (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/.test(input));
}

function gb_debounceBase(func, immediate, wait) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
};

function Gb_Debounce(func, immediate, wait = 250) {
    (gb_debounceBase(func, immediate, wait))();
}