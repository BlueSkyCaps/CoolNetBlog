// 加锁文章 输入密码
$("#toUnlockBtn").click(
    function () {
        var v = $('#inputPassword').val();
        if (v == null || v == "") {
            return;
        }

        $.ajax(
            {
                url: "/Detail/UnLock/ArticleUnLock",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    "ArticleId": $('#inputArticleId').val(),
                    "Password": $('#inputPassword').val()
                }),
                success: function (data, status) {
                    if (data['code'] == 1) {
                        // 密码正确隐藏表单div 显示返回后的主体内容
                        $('#lockFormDiv').hide();
                        $('#unLockContentP').html(data['content']);
                    } else {
                        // 错误显示提示文本
                        $('#pwErrorTipDiv').removeAttr('hidden');
                    }
                },
                error: function (err) {
                    alert(err)
                    console.log(err)
                    $('#pwErrorTipDiv').removeAttr('hidden');
                }
            }
        );
    }
);

var commentShowIndex = 0;
var isShowLeaveHeadImg = false;

// 发表评论
$("#toCommentBtn").click(
    function () {
        var email = $('#cEmailInput').val().trim();
        var name = $('#cNameInput').val().trim();
        var siteUrl = $('#cSiteInput').val().trim();
        var content = $('#cContentInput').val().trim();

        if (email == null || email == "" || name == null || name == "" || content == null || content == "") {
            Gb_PopoverShow("toCommentBtn", "请把内容填写完整哦!~");
            return;
        }
        if (content.length < 5) {
            Gb_PopoverShow("toCommentBtn", "评论内容太少啦!~");
            return;
        }
        if (!!!Gb_isEmail(email)) {
            Gb_PopoverShow("toCommentBtn", "不是有效的邮箱!~");
            return;
        }
        var articleIdInput = parseInt($('#inputArticleId').val());
        var articleId = !isNaN(articleIdInput) ? articleIdInput : 0;
        $.ajax(
            {
                url: "/Comment/Comment/",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    "Email": email,
                    "Name": name,
                    "SiteUrl": siteUrl,
                    "Content": content,
                    "SourceId": articleId,
                    "SourceType": 1
                }),
                success: function (data, status) {
                    if (data['code'] == 1) {
                        Gb_PopoverShow("toCommentBtn", data['tipMessage']);
                        $('#cEmailInput').val("");
                        $('#cSiteInput').val("");
                        $('#cNameInput').val("");
                        $('#cContentInput').val("");
                        // 评论成功且是公开的直接刷新 有可能当前获取的值不是最新的类型 不允许评论已被处理 无碍
                        if (parseInt($('#inputArticleLeavePublic').val()) == 1) {
                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        }
                    } else {
                        Gb_PopoverShow("toCommentBtn", data['tipMessage']);
                    }
                },
                error: function (err) {
                    Gb_PopoverShow("toCommentBtn", "评论失败,刷新再试试吧err");
                    console.log(err)
                }
            }
        );
    }
);


$("#loadCommentBtn").on("click",
    function () {
        var articleIdInput = parseInt($('#inputArticleId').val());
        var articleId = !isNaN(articleIdInput) ? articleIdInput : 0;
        commentShowIndex++;
        var url = "/Comment/GetArticleComments?sourceId=" + articleId + "&index=" + commentShowIndex;
        $.ajax(
            {
                url: url,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data, status) {
                    if (data['code'] == 1) {
                        if (data['data'].length <= 0) {
                            commentShowIndex--;
                            // 没有更多评论数据 禁用按钮
                            if (commentShowIndex <= 0) {
                                $('#commentShowDiv').hide();//收起评论区域
                                Gb_PopoverShow("commentShowBtn", "暂无评论", true, "此内容暂无评论~");
                            } else {
                                Gb_PopoverShow("loadCommentBtn", "暂无更多评论啦~", true);
                            }
                            return;
                        }
                        // 追加当前获取的评论数据到评论区域
                        $.each(data['data'], function (i, cItem) {
                            // 迭代 拼接每个评论的所有回复
                            var rpNodeStr = "";
                            if (cItem['relatedReplies'].length > 0) {
                                $.each(cItem['relatedReplies'], function (i, rItem) {
                                    var idStr = "comment" + cItem.id + "reply" + rItem.id;
                                    var replyTimeStr = Gb_GetFlowTimeStr(rItem.replyTime);
                                    var adminBadgeStr = rItem.isAdmin ? '<span class="badge bg-info text-dark">博主</span> ' : '';
                                    var siteUrlStr = !(Gb_IsWhiteSpaceOrNull(rItem.siteUrl)) ? ' <a target="_blank" href="' + rItem.siteUrl + '" class="text-dark">🌐</a>' : '';
                                    var headSrcV = 'https://cravatar.cn/avatar/' + md5(rItem.email) + '?s=40&d=monsterid';
                                    var headPStr = isShowLeaveHeadImg ? '<img src="' + headSrcV +'" style="max-width:40px;max-height:40px;" class="border border-2 border-info rounded-circle float-start" alt="">':'';
                                    var replyContent = rItem.content;
                                    var replyInfoStr = '<small>' + replyTimeStr + siteUrlStr + ' <a href="javascript:;" onClick="toReplyDialog(this,1)" comment-id="{3}" to-name="{4}">回复TA</a></small>';
                                    var tmpRpNodeStr = '<div class="accordion" id="accordion{0}"><div class="accordion-item"><h2 class="accordion-header" id="heading{0}">' +
                                        '<button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse{0}" aria-expanded="false" aria-controls="collapse{0}">' +
                                        '<p>{5}<small class="text text-info">{4}</small><br/>{6}{1}</p>' +
                                        '</button></h2><div id="collapse{0}" class="accordion-collapse collapse" aria-labelledby="heading{0}" data-bs-parent="#accordion{0}"><div class="accordion-body text-end">' +
                                        '{2}' + '</div></div></div></div>';
                                    // 格式化占位符 第0个是元素的id 第1个是回复内容 第2个是回复时间|按钮组成的元素 
                                    // 第3个是关联的评论id 第4个是昵称 第5个是站主徽标元素 第6个是cravatar-api得到的头像元素
                                    rpNodeStr += tmpRpNodeStr.format(idStr, replyContent, replyInfoStr, cItem.id, rItem.name, adminBadgeStr, headPStr);
                                });
                            }
                            // 格式化评论时间
                            var commentTimeStr = Gb_GetFlowTimeStr(cItem.commentTime);
                            // 判断下一页回复按钮，当前评论有更多回复数据在下一页,HasReplyInNext会是true,显示'加载回复'按钮.
                            // 属性theNextIndex指向此评论的下一页回复的页索引 且根据后续ajax调用动态更新它的值
                            var rpInNextStr = cItem.hasReplyInNext ?
                                '<div class="text-end"><button commentId="' +
                                cItem.id +
                                '" id="comment' +
                                cItem.id +
                                'replyLoadBtn" theNextIndex="2" class="loadReplyBtn btn btn-outline-primary btn-sm" data-bs-placement="bottom" tabindex="0" ' +
                                'data-bs-toggle="popover" data-bs-trigger="focus" data-bs-content="">加载回复..</button></div>' : '';

                            var adminBadgeStr = cItem.isAdmin ? '<span class="badge bg-info text-dark">博主</span> ' : '';
                            var siteUrlStr = !(Gb_IsWhiteSpaceOrNull(cItem.siteUrl)) ? ' <a target="_blank" href="' + cItem.siteUrl + '" class="text-dark">🌐</a>' : '';
                            var headSrcV = 'https://cravatar.cn/avatar/' + md5(cItem.email) + '?s=40&d=monsterid';
                            var headPStr = isShowLeaveHeadImg ? '<img src="' + headSrcV + '" style="max-width:40px;max-height:40px;" class="border border-2 border-info rounded-circle float-start" alt="">':'';
                            var cmNodeStr = '<div class="card card-body commentItem"><p class="text-start card-title">' +
                                adminBadgeStr +
                                '<small class="text text-info">' +
                                cItem.name + '</small> <small>' +
                                commentTimeStr +
                                siteUrlStr +
                                ' <a href="javascript:;" onClick="toReplyDialog(this,0)" comment-id="' +
                                cItem.id + '" to-name="' +
                                cItem.name +
                                '">回复TA</a></small></p><p class="text-start card-text">' +
                                headPStr +
                                cItem.content +
                                '</p></div><div class="card-footer">' +
                                rpNodeStr + rpInNextStr
                            '</div></br>';
                            $('#loadCommentBtnDiv').before(cmNodeStr);
                            // 来自第一次点击"网友的评论" 此时再显示评论区 这样避免数据没有加载完毕"加载"按钮却出现
                            if (commentShowIndex == 1) {
                                $('#commentShowDiv').show();
                            }
                        });
                    } else {
                        commentShowIndex--;
                        if (commentShowIndex < 1) {
                            $('#commentShowDiv').hide();//收起评论区域
                            // 有问题 来自第一次点击"网友的评论" 在"网友的评论"按钮显示提示文本
                            Gb_PopoverShow("commentShowBtn", data['tipMessage']);
                        } else {
                            // 有问题 不是第一次点击"网友的评论" 而是后续点击加载评论 在"加载评论"按钮显示提示文本
                            Gb_PopoverShow("loadCommentBtn", data['tipMessage']);
                        }
                    }
                },
                error: function (err) {
                    commentShowIndex--;
                    if (commentShowIndex < 1) {
                        $('#commentShowDiv').hide();//收起评论区域
                        Gb_PopoverShow("commentShowBtn", "加载评论失败err,重试一下?!");
                    } else {
                        Gb_PopoverShow("loadCommentBtn", "加载评论失败err,重试一下?!");
                    }
                    console.log(err)
                },
                complete: function () {
                    // 无论如何执行此回调 还原"网友的评论"按钮最初的文本
                    $("#commentShowBtn").removeAttr("disabled");
                    $("#commentShowBtn").text("网友的评论▼");
                    //↑↑↑若是第一次加载评论且没有任何评论 Gb_PopoverShow()会在之后定时执行：改变按钮的文本为"此内容暂无评论"
                }
            }
        );
    }
);

// 监听loadReplyBtn被点击 loadReplyBtn是个class属性 因为有多个评论可以点击"加载回复"
$(document).on("click", ".loadReplyBtn",
    function () {
        var targerRpLoadBtn = $(this);
        var cid = targerRpLoadBtn.attr("commentId");
        var theCommentReplyIndex = targerRpLoadBtn.attr("theNextIndex");
        var url = "/Comment/GetCommentReplys?commentId=" + cid + "&index=" + theCommentReplyIndex;
        $.ajax(
            {
                url: url,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data, status) {
                    if (data['code'] == 1) {
                        if (data['data']['replies'].length <= 0) {
                            Gb_PopoverShow(targerRpLoadBtn.attr("id"), "暂无更多回复啦~", true);
                            return;
                        }
                        // 追加当前获取的回复数据到当前聚焦的评论底部
                        $.each(data['data']['replies'], function (i, rItem) {
                            // 迭代 拼接每个回复
                            var rpNodeStr = "";
                            var idStr = "comment" + cid + "reply" + rItem.id;
                            var replyTimeStr = Gb_GetFlowTimeStr(rItem.replyTime);
                            var adminBadgeStr = rItem.isAdmin ? '<span class="badge bg-info text-dark">博主</span> ' : '';
                            var siteUrlStr = !(Gb_IsWhiteSpaceOrNull(rItem.siteUrl)) ? ' <a target="_blank" href="' + rItem.siteUrl + '" class="text-dark">🌐</a>' : '';
                            var headSrcV = 'https://cravatar.cn/avatar/' + md5(rItem.email) + '?s=40&d=monsterid';
                            var headPStr = isShowLeaveHeadImg ? '<img src="' + headSrcV + '" style="max-width:40px;max-height:40px;" class="border border-2 border-info rounded-circle float-start" alt="">':'';
                            var replyContent = rItem.content;
                            var replyInfoStr = '<small>' + replyTimeStr + siteUrlStr + ' <a href="javascript:;" onClick="toReplyDialog(this,1)" comment-id="{3}" to-name="{4}">回复TA</a></small>';
                            var tmpRpNodeStr = '<div class="accordion" id="accordion{0}"><div class="accordion-item"><h2 class="accordion-header" id="heading{0}">' +
                                '<button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapse{0}" aria-expanded="false" aria-controls="collapse{0}">' +
                                '<p>{5}<small class="text text-info">{4}</small><br/>{6}{1}</p>' +
                                '</button></h2><div id="collapse{0}" class="accordion-collapse collapse" aria-labelledby="heading{0}" data-bs-parent="#accordion{0}"><div class="accordion-body text-end">' +
                                '{2}' + '</div></div></div></div>';

                            // 格式化占位符 第0个是元素的id 第1个是回复内容 第2个是回复时间|按钮组成的元素 
                            // 第3个是关联的评论id 第4个是昵称 第5个是站主徽标元素 第6个是cravatar-api得到的头像元素
                            rpNodeStr += tmpRpNodeStr.format(idStr, replyContent, replyInfoStr, rItem.id, rItem.name, adminBadgeStr, headPStr);

                            // 追加到当前"加载回复"按钮的前面，此按钮被一层div包裹
                            targerRpLoadBtn.parent().before(rpNodeStr);
                        });
                        if (data['data']['hasReplyInNext'] !== true) {
                            // 当前评论没有下一页的回复数据
                            targerRpLoadBtn.parent().remove();
                        } else {
                            // 属性theNextIndex指向此评论的下一页回复的页索引 更新当前按钮属性值为下一页
                            var theNextIndex = parseInt(targerRpLoadBtn.attr("theNextIndex")) + 1;
                            targerRpLoadBtn.attr("theNextIndex", theNextIndex)
                        }
                    } else {
                        Gb_PopoverShow(targerRpLoadBtn.attr("id"), data['tipMessage']);
                    }
                },
                error: function (err) {
                    Gb_PopoverShow(targerRpLoadBtn.attr("id"), "加载评论失败err,重试一下?!");
                    console.log(err)
                }
            }
        );
    }
);

//文章点赞按钮点击 target表态按钮对象；id文章id;type表态类型
function thumbUpBtnClick(target, id, type) {
    $('.thumbUpBtn').attr("disabled", "true");
    $.ajax(
        {
            url: "/ThumbsUp/ThumbsUp",
            type: "POST",
            dataType: "json",
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            data: {
                "ArticleId": id,
                "Type": type,
            },
            success: function (data, status) {
                if (data['code'] == 1) {
                    // 点赞成功 将点赞数重显 
                    $('#thumbUpTextStart').text(data['data']['thumbUpStart']);
                    $('#thumbUpTextFun').text(data['data']['thumbUpFun']);
                    $('#thumbUpTextSilence').text(data['data']['thumbUpSilence']);
                }
                // bootstrap显示按钮弹出层 作为提示
                var thumbUpBtnId = target.getAttribute('id');
                Gb_PopoverShow(thumbUpBtnId, data['tipMessage']);

            },
            error: function (err) {
                var thumbUpBtnId = target.getAttribute('id');
                Gb_PopoverShow(thumbUpBtnId, "表态失败，好意心领啦err");
            }
        }
    );

}

// 网友评论按钮被点击
function commentToggleBtnClick(status) {
    if (status == 1) {
        // commentShowIndex <1，说明还没有显示过评论，触发加载评论事件去加载；否则不必触发，而是直接显示评论区域
        if (commentShowIndex < 1) {
            // 触发loadCommentBtn的点击事件
            $("#commentShowBtn").text("加载评论中~~~");
            $("#commentShowBtn").attr("disabled", "disabled");
            $("#loadCommentBtn").click();
        } else {
            $('#commentShowDiv').show();
        }
    } else {
        // status==0 隐藏评论区域
        $('#commentShowDiv').hide();
    }
}

// 分享链接按钮 复制url到剪贴板
function copyCommand() {
    $('#urlHidInput').val(window.location.href);
    $('#urlHidInput').removeAttr('hidden');
    $('#urlHidInput').focus();
    $('#urlHidInput').select();
    if (document.execCommand('copy')) {
        document.execCommand('copy');
        $('#urlHidInput').attr('hidden', 'hidden');
        $('#urlShareBtn').text("√链接已复制");
        $('#urlShareBtn').attr('disabled', 'disabled');
    }
}

// 确认回复
function toReply() {
    var email = $('#rEmailInput').val().trim();
    var name = $('#rNameInput').val().trim();
    var siteUrl = $('#rSiteInput').val().trim();
    var content = $('#rContentInput').val().trim();

    if (email == null || email == "" || name == null || name == "" || content == null || content == "") {
        Gb_PopoverShow("ReplyModalOk", "请把内容填写完整哦!~");
        return;
    }
    if (content.length < 5) {
        Gb_PopoverShow("ReplyModalOk", "回复内容太少啦!~");
        return;
    }
    if (!!!Gb_isEmail(email)) {
        Gb_PopoverShow("ReplyModalOk", "不是有效的邮箱!~");
        return;
    }
    var cid = $('#rCommentIdInput').val().trim();
    var toNmae = $('#rToNameInput').val().trim();
    var layer = $('#rLayerInput').val();
    if (Gb_IsWhiteSpaceOrNull(cid) || Gb_IsWhiteSpaceOrNull(toNmae) || Gb_IsWhiteSpaceOrNull(layer)) {
        Gb_PopoverShow("ReplyModalOk", "不允许的操作，请重试!~");
        return;
    }
    // 1回复某个回复，加上艾特符号；0回复评论，不用加
    if (layer == "1")
        content = toNmae + "：" + content;
    // ajax调用回复
    $.ajax(
        {
            url: "/Comment/Reply/",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                "Email": email,
                "Name": name,
                "SiteUrl": siteUrl,
                "Content": content,
                "CommentId": cid,
            }),
            success: function (data, status) {
                if (data['code'] == 1) {
                    $('#ReplyModalOk').text(data['tipMessage']);
                    $('#ReplyModalOk').attr("disabled", "disabled");
                    setTimeout(function () {
                        $('#ReplyModal').modal('hide');
                        $('#ReplyModalOk').removeAttr("disabled");
                        $('#ReplyModalOk').text("回复");
                        // 回复成功且是公开的直接刷新 有可能当前获取的值不是最新的类型 不允许评论已被处理 无碍
                        if (parseInt($('#inputArticleLeavePublic').val()) == 1) {
                            location.reload();
                        }
                    }, 2000);

                } else {
                    Gb_PopoverShow("ReplyModalOk", data['tipMessage']);
                }
            },
            error: function (err) {
                Gb_PopoverShow("ReplyModalOk", "回复失败,重新再试试吧err");
                console.log(err)
            }
        }
    );

}

// 点击了某个回复按钮 显示回复弹出层
function toReplyDialog(target, layer) {
    CloseReplyModal();
    var cid = target.getAttribute('comment-id');
    var toName = target.getAttribute('to-name');
    if (Gb_IsWhiteSpaceOrNull(cid) || Gb_IsWhiteSpaceOrNull(toName))
        return;
    // 设置回复弹出层隐藏的属性值
    $('#rCommentIdInput').val(cid);
    $('#ReplyModalTip').text("@" + toName);
    $('#rToNameInput').val("@" + toName);
    // 1回复某个回复，0回复评论
    $('#rLayerInput').val(layer);
    $('#ReplyModal').modal("show");
}

function CloseReplyModal() {
    // 关闭清空回复弹出层
    $('#ReplyModal input').val("");
    $('#ReplyModal textarea').val("");
    $('#ReplyModal').modal('hide');
}

// 当DOM加载完毕时，自动触发"网友评论"按钮
$('#commentShowDiv').ready(function () {
    // 获取隐藏的checkbox值 是否需要显示评论区留言者头像
    isShowLeaveHeadImg = $('#inputIsShowLeaveHeadImg').prop("checked");
    commentToggleBtnClick(1);
});