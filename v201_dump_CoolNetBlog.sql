
CREATE DATABASE IF NOT EXISTS CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;
USE CoolNetBlog;

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for AdminUser
-- ----------------------------
DROP TABLE IF EXISTS `AdminUser`;
CREATE TABLE `AdminUser`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `AccountName` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'admin用户名 唯一',
  `Password` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Token` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '用户token 用于页面访问 ',
  `Email` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '邮箱',
  `EmailPassword` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '此admin的smtp邮箱授权密码',
  `SmtpHost` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '此admin的smtp邮箱服务器地址',
  `SmtpPort` int(0) NULL DEFAULT NULL COMMENT '此admin的smtp邮箱服务器端口',
  `SmtpIsUseSsl` bit(1) NULL DEFAULT b'1' COMMENT '此admin的smtp邮箱服务器是否使用ssl',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of AdminUser
-- ----------------------------
INSERT INTO `AdminUser` VALUES (1, 'bluesky', '25D55AD283AA400AF464C76D713C07AD', 'cd616bb840f14778b6cc628bb5d39d3b', NULL, NULL, NULL, NULL, b'1');

-- ----------------------------
-- Table structure for Article
-- ----------------------------
DROP TABLE IF EXISTS `Article`;
CREATE TABLE `Article`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '文章(帖子)自增主键',
  `MenuId` int(0) NOT NULL COMMENT '所属菜单Id',
  `Title` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '文章标题',
  `Content` varchar(10000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文章内容',
  `IsShowTitle` bit(1) NOT NULL COMMENT '是否展示此文章的标题',
  `Abstract` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文章自定义摘要描述',
  `CreatedTime` datetime(0) NOT NULL COMMENT '首次创建时间',
  `UpdateTime` datetime(0) NOT NULL COMMENT '最后更新时间',
  `IsLock` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否隐私文章，隐私文章需在页面解锁才显示主体内容',
  `LockPassword` varchar(6) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '隐私文章密码',
  `IsDraft` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否是草稿',
  `Labels` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '标签字符串：“xx,xx..”|\"xx，xx..\"|\"xx xx..\"',
  `CustUri` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '自定义文章uri，唯一，可通过此字段或Id找寻文章',
  `CommentType` int(0) NOT NULL DEFAULT 1 COMMENT '评论类型，1直接公开，2经过审核，3不允许评论',
  `IsSpecial` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否是特殊文章，通常\"关于\"\"友链\"等内容可以定义为特殊文章，特殊文章不能设为隐私且不会被列表显示和搜索到',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of Article
-- ----------------------------

-- ----------------------------
-- Table structure for ArticleThumbUp
-- ----------------------------
DROP TABLE IF EXISTS `ArticleThumbUp`;
CREATE TABLE `ArticleThumbUp`  (
  `ArticleId` int(0) NOT NULL COMMENT '文章Id',
  `ClientIp` varchar(26) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '客户端点赞文章Ip',
  `ClientDevice` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '客户端的设备名称',
  `ClientBrowser` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '客户端的浏览器名称',
  `UpTime` datetime(0) NULL DEFAULT NULL COMMENT '点赞时间',
  `Type` int(0) NULL DEFAULT NULL COMMENT '点赞类型：1觉得很赞2有被笑到3不敢苟同',
  PRIMARY KEY (`ArticleId`, `ClientIp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of ArticleThumbUp
-- ----------------------------

-- ----------------------------
-- Table structure for Comment
-- ----------------------------
DROP TABLE IF EXISTS `Comment`;
CREATE TABLE `Comment`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `SourceId` int(0) UNSIGNED NOT NULL COMMENT '此评论对应的对象表原Id，如：文章是文章表的Id',
  `SourceType` int(0) UNSIGNED NOT NULL COMMENT '此评论对应的对象表类型，1文章，2.。3.。。',
  `Name` varchar(12) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '评论者昵称',
  `SiteUrl` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评论者网址',
  `Email` varchar(25) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '评论者邮箱',
  `Content` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '评论内容',
  `IsPassed` bit(1) NULL DEFAULT NULL COMMENT '若文章的评论类型是2需经过审核，那么审核通过后才会显示此评论',
  `CommentTime` datetime(0) NOT NULL COMMENT '评论时间',
  `IsAdmin` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否来自管理员',
  `ClientIp` varchar(26) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '评论文章Ip',
  `ClientDevice` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评论文章设备名',
  `ClientBrowser` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '评论文章浏览器名称',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of Comment
-- ----------------------------

-- ----------------------------
-- Table structure for CommonThumbUp
-- ----------------------------
DROP TABLE IF EXISTS `CommonThumbUp`;
CREATE TABLE `CommonThumbUp`  (
  `SourceId` int(0) NOT NULL COMMENT '源Id',
  `SourceType` int(0) NOT NULL COMMENT '源Id所属类型：1\"闲言碎语\"组件表，2.. (文章点赞不使用此表而是专门的文章点赞表ArticleThumbUp)',
  `ClientIp` varchar(26) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '点赞Ip',
  `ClientDevice` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '客户端设备名称',
  `ClientBrowser` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '客户端浏览器名称',
  `UpTime` datetime(0) NULL DEFAULT NULL COMMENT '点赞时间',
  PRIMARY KEY (`SourceId`, `SourceType`, `ClientIp`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of CommonThumbUp
-- ----------------------------

-- ----------------------------
-- Table structure for FilePath
-- ----------------------------
DROP TABLE IF EXISTS `FilePath`;
CREATE TABLE `FilePath`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `FileRelPath` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文件在wwwroot/img/{DayDir}下的文件路径，文件名是唯一的',
  `HelpName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '文件的助记名，这是唯一的',
  `UploadTime` datetime(0) NOT NULL COMMENT '上传图片的时间',
  `Type` char(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '格式类型，图片=img 其他=other',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of FilePath
-- ----------------------------

-- ----------------------------
-- Table structure for Gossip
-- ----------------------------
DROP TABLE IF EXISTS `Gossip`;
CREATE TABLE `Gossip`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '\'闲言碎语\'自增主键',
  `Type` int(0) NOT NULL DEFAULT 1 COMMENT '内容类型。1纯文字2带图片的文字',
  `Content` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '内容',
  `ImgUrl` varchar(120) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '内容类型2带图片的文字，图片url',
  `AddTime` datetime(0) NOT NULL COMMENT '发布时间',
  `StarNumber` int(0) NOT NULL DEFAULT 0 COMMENT '点赞总数',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 154 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of Gossip
-- ----------------------------

-- ----------------------------
-- Table structure for LoveLook
-- ----------------------------
DROP TABLE IF EXISTS `LoveLook`;
CREATE TABLE `LoveLook`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `AddedTime` datetime(0) NOT NULL COMMENT '添加时间',
  `RelHref` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '链接相对路径，内部文章或上传的链接文件',
  `Type` int(0) NOT NULL DEFAULT 1 COMMENT '链接类型，1内部文章链接；2其他上传的文件链接；3外部链接',
  `LinkName` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '自定义显示链接名 在侧边栏\"看看这些\"项中显示',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 19 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of LoveLook
-- ----------------------------

-- ----------------------------
-- Table structure for Menu
-- ----------------------------
DROP TABLE IF EXISTS `Menu`;
CREATE TABLE `Menu`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '菜单名',
  `PId` int(0) NOT NULL DEFAULT 0 COMMENT '父菜单Id，顶级菜单值为0',
  `IsHome` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否是主页菜单，主页菜单必为\r\n顶级菜单且不能有下级菜单；只能有一个菜单是主页菜单',
  `OrderNumber` int(0) NOT NULL DEFAULT 0 COMMENT '菜单排序顺序位',
  `IsShow` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否显示此菜单',
  `Tips` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '菜单提示文本',
  `UpdateTime` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 46 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of Menu
-- ----------------------------

-- ----------------------------
-- Table structure for Reply
-- ----------------------------
DROP TABLE IF EXISTS `Reply`;
CREATE TABLE `Reply`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `CommentId` int(0) NOT NULL COMMENT '对应的评论表Id',
  `Name` varchar(12) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '回复者昵称',
  `SiteUrl` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '回复者网址',
  `Email` varchar(25) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '回复者邮箱',
  `Content` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '回复内容',
  `IsPassed` bit(1) NULL DEFAULT NULL COMMENT '若文章的评论类型是2需经过审核，那么审核通过后才会显示此回复',
  `ReplyTime` datetime(0) NOT NULL COMMENT '回复时间',
  `IsAdmin` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否来自管理员',
  `ClientIp` varchar(26) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '回复文章Ip',
  `ClientDevice` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '回复文章设备名',
  `ClientBrowser` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '回复文章浏览器名称',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 124 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of Reply
-- ----------------------------

-- ----------------------------
-- Table structure for SiteSetting
-- ----------------------------
DROP TABLE IF EXISTS `SiteSetting`;
CREATE TABLE `SiteSetting`  (
  `SiteName` varchar(48) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '站点名称',
  `Host` varchar(25) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '站点IP',
  `Domain` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '站点域名',
  `FashionQuotes` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '个性签名',
  `IsShowSiteName` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否显示站点名',
  `IsShowEdgeSearch` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否侧边栏也显示搜索框组件',
  `IsShowLoveLook` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否显示侧边栏 \"看看这些\"组件',
  `IsShowQutoes` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否显示个性签名',
  `Cban` char(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '备案号',
  `TailContent` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '尾部文字内容',
  `OnePageCount` int(0) NOT NULL DEFAULT 5 COMMENT '主页列表展示文章的条数',
  `LoginUriValue` varchar(25) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '后台登录入口Value参数设置的值，若设置了值，必须验证正确的值显示登录界面',
  `LoveLookTitle` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '显示链接(\"侧边栏 看看这些\")自定义标题文本',
  `WishPictureRelPath` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '侧边栏\"心愿图片\"展示路径(相对) 在[文件图片]板块中设置心愿图片',
  `IsShowWishPicture` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否展示\"心愿图片\"',
  `WishPictureName` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '\"心愿图片\"标题祝福语',
  `IsOpenDetailThumb` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否开启文章点赞表态功能',
  `LeaveLimitCount` int(0) NULL DEFAULT 0 COMMENT 'ip一日内允许的留言次数(包括回复、评论)，0为无限制',
  `IsShowLeaveHeadImg` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否在评论区展示留言者的头像，采用Cravatar源：https://cravatar.cn',
  `IsShowGossip` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否显示侧边栏\"闲言碎语\"组件'
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of SiteSetting
-- ----------------------------
INSERT INTO `SiteSetting` VALUES ('大侠的个人博客站', NULL, NULL, 'Github给个星星吧？！github.com/BlueSkyCaps/CoolNetBlog', b'1', b'1', b'0', b'1', 'xxx备案号xxx', '欢迎浏览我的博客-博客版本v2.0.1', 5, NULL, '推荐看看', NULL, b'0', NULL, b'0', 0, b'0', b'0');

SET FOREIGN_KEY_CHECKS = 1;
