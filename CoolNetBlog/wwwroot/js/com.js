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
        + (minutes > 9 ? minutes : "0" + minutes) + ":"
        + (seconds > 9 ? seconds : "0" + seconds);
    return commnetTimeStr;
}