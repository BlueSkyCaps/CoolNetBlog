function Gb_GetFmtDateStr(dateTime) {
    var date = new Date(dateTime);
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();
    var commnetTimeStr = year + "-" + (month > 9 ? month : "0" + month) + "-"
        + (day > 9 ? day : "0" + day) + " "
        + (hours > 9 ? hours : "0" + hours) + ":"
        + (minutes > 9 ? minutes : "0" + minutes);
        //+ ":"+ (seconds > 9 ? seconds : "0" + seconds);
    return commnetTimeStr;
}

String.prototype.format = function () {
    if (arguments.length == 0) return this;
    for (var s = this, i = 0; i < arguments.length; i++)
        s = s.replace(new RegExp("\\{" + i + "\\}", "g"), arguments[i]);
    return s;
};

String.prototype.isWhiteSpaceOrNull = function () {
    if (this === null || this === undefined || this === "undefined")
        return true;
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