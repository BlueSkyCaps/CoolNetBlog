-- MySQL dump 10.13  Distrib 8.0.28, for Linux (x86_64)

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `AdminUser`
--

CREATE DATABASE CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;

USE CoolNetBlog;

DROP TABLE IF EXISTS `AdminUser`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AdminUser` (
  `Id` int unsigned NOT NULL AUTO_INCREMENT,
  `AccountName` varchar(20) NOT NULL,
  `Password` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Token` varchar(50) NOT NULL COMMENT '用户token 用于页面访问 ',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AdminUser`
--

LOCK TABLES `AdminUser` WRITE;
/*!40000 ALTER TABLE `AdminUser` DISABLE KEYS */;
INSERT INTO `AdminUser` VALUES (1,'bluesky','25D55AD283AA400AF464C76D713C07AD','8cdcd1e51af24226b481ba66c990dfab');
/*!40000 ALTER TABLE `AdminUser` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Article`
--

DROP TABLE IF EXISTS `Article`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Article` (
  `Id` int unsigned NOT NULL AUTO_INCREMENT COMMENT '文章(帖子)自增主键',
  `MenuId` int NOT NULL COMMENT '所属菜单Id',
  `Title` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT '' COMMENT '文章标题',
  `Content` varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文章内容',
  `IsShowTitle` bit(1) NOT NULL COMMENT '是否展示此文章的标题',
  `Abstract` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文章自定义摘要描述',
  `CreatedTime` datetime NOT NULL COMMENT '首次创建时间',
  `UpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `IsLock` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否隐私文章，隐私文章需在页面解锁才显示主体内容',
  `LockPassword` varchar(6) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '隐私文章密码',
  `IsDraft` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否是草稿',
  `Labels` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '标签字符串：“xx,xx..”|"xx，xx.."|"xx xx.."',
  `CustUri` varchar(100) DEFAULT NULL COMMENT '自定义文章uri，唯一，可通过此字段或Id找寻文章',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Article`
--

LOCK TABLES `Article` WRITE;
/*!40000 ALTER TABLE `Article` DISABLE KEYS */;
/*!40000 ALTER TABLE `Article` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ArticleThumbUp`
--

DROP TABLE IF EXISTS `ArticleThumbUp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArticleThumbUp` (
  `ArticleId` int NOT NULL COMMENT '文章Id',
  `ClientIp` varchar(26) NOT NULL COMMENT '客户端点赞文章Ip',
  `ClientDevice` varchar(10) DEFAULT NULL COMMENT '客户端的设备名称',
  `ClientBrowser` varchar(10) DEFAULT NULL COMMENT '客户端的浏览器名称',
  `UpTime` datetime DEFAULT NULL COMMENT '点赞时间',
  `Type` int DEFAULT NULL COMMENT '点赞类型：1觉得很赞2有被笑到3不敢苟同',
  PRIMARY KEY (`ArticleId`,`ClientIp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ArticleThumbUp`
--

LOCK TABLES `ArticleThumbUp` WRITE;
/*!40000 ALTER TABLE `ArticleThumbUp` DISABLE KEYS */;
/*!40000 ALTER TABLE `ArticleThumbUp` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FilePath`
--

DROP TABLE IF EXISTS `FilePath`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FilePath` (
  `Id` int unsigned NOT NULL AUTO_INCREMENT,
  `FileRelPath` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '文件在wwwroot/articleImgs/下的文件路径，文件名是唯一的',
  `HelpName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '文件的助记名，这是唯一的',
  `UploadTime` datetime NOT NULL COMMENT '上传图片的时间',
  `Type` char(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '格式类型，图片=img 其他=other',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=58 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FilePath`
--

LOCK TABLES `FilePath` WRITE;
/*!40000 ALTER TABLE `FilePath` DISABLE KEYS */;
/*!40000 ALTER TABLE `FilePath` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LoveLook`
--

DROP TABLE IF EXISTS `LoveLook`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LoveLook` (
  `Id` int unsigned NOT NULL AUTO_INCREMENT,
  `AddedTime` datetime NOT NULL COMMENT '添加时间',
  `RelHref` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '链接相对路径，内部文章或上传的链接文件',
  `Type` int NOT NULL DEFAULT '1' COMMENT '链接类型，1内部文章链接；2其他上传的文件链接；3外部链接',
  `LinkName` varchar(20) NOT NULL COMMENT '自定义显示链接名 在侧边栏"看看这些"项中显示',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LoveLook`
--

LOCK TABLES `LoveLook` WRITE;
/*!40000 ALTER TABLE `LoveLook` DISABLE KEYS */;
/*!40000 ALTER TABLE `LoveLook` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Menu`
--

DROP TABLE IF EXISTS `Menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Menu` (
  `Id` int unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '菜单名',
  `PId` int NOT NULL DEFAULT '0' COMMENT '父菜单Id，顶级菜单值为0',
  `IsHome` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否是主页菜单，主页菜单必为\r\n顶级菜单且不能有下级菜单；只能有一个菜单是主页菜单',
  `OrderNumber` int NOT NULL DEFAULT '0' COMMENT '菜单排序顺序位',
  `IsShow` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否显示此菜单',
  `Tips` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT '' COMMENT '菜单提示文本',
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Menu`
--

LOCK TABLES `Menu` WRITE;
/*!40000 ALTER TABLE `Menu` DISABLE KEYS */;
/*!40000 ALTER TABLE `Menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SiteSetting`
--

DROP TABLE IF EXISTS `SiteSetting`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SiteSetting` (
  `SiteName` varchar(48) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '站点名称',
  `Host` varchar(25) DEFAULT NULL COMMENT '站点IP',
  `Domain` varchar(255) DEFAULT NULL COMMENT '站点域名',
  `FashionQuotes` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '个性签名',
  `IsShowSiteName` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否显示站点名',
  `IsShowEdgeSearch` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否侧边栏也显示搜索框组件',
  `IsShowLoveLook` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否显示侧边栏 "看看这些"组件',
  `IsShowQutoes` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否显示个性签名',
  `Cban` char(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '备案号',
  `TailContent` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '尾部文字内容',
  `OnePageCount` int NOT NULL DEFAULT '5' COMMENT '主页列表展示文章的条数',
  `LoginUriValue` varchar(25) DEFAULT NULL COMMENT '后台登录入口Value参数设置的值，若设置了值，必须验证正确的值显示登录界面',
  `LoveLookTitle` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '显示链接("侧边栏 看看这些")自定义标题文本',
  `WishPictureRelPath` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '侧边栏"心愿图片"展示路径(相对) 在[文件图片]板块中设置心愿图片',
  `IsShowWishPicture` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否展示"心愿图片"',
  `WishPictureName` varchar(60) DEFAULT NULL COMMENT '"心愿图片"标题祝福语',
  `IsOpenDetailThumb` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否开启文章点赞表态功能'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SiteSetting`
--

LOCK TABLES `SiteSetting` WRITE;
/*!40000 ALTER TABLE `SiteSetting` DISABLE KEYS */;
INSERT INTO `SiteSetting` VALUES ('大侠的个人博客站',NULL,NULL,'Github给个星星吧？！github.com/BlueSkyCaps/CoolNetBlog',_binary '',_binary '',_binary '\0',_binary '','xxx备案号xxx','欢迎浏览我的博客-博客版本v1.99',3,NULL,'推荐看看',NULL,_binary '\0','开心！哈哈哈！!',_binary '\0');
/*!40000 ALTER TABLE `SiteSetting` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-02-27 16:34:02
