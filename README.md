# CoolNetBlog

## 使用 asp.net core 6 的轻量级、简约、快速的博客框架。

[![ASP.NET Core 6](https://shields.io/badge/-Asp_Net_Core_6-blue)](https://asp.net/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-v5.1-blue)](https://getbootstrap.com/)
[![Uikit](https://img.shields.io/badge/Uikit-v3.10.1-blue)](https://getuikit.com)
[![jQuery](https://img.shields.io/badge/jQuery-v3.5.1-blue)](https://jquery.com/)


### 目前功能：
1. 支持移动端和pc端样式。可以设置菜单，关键词搜索，自定义站点尾巴和标题。
2. 文章管理：有标准文章、草稿文章、隐私文章(密码加锁)和特殊文章(如"友链"等)，文章文字样式(包含图片插入)、设置标签。
4. 可以设置文章的评论类型：1公开2审核3不允许评论
5. 可以审核留言，通过与不通过，也可以直接回复留言者，并且选择抄送邮件给留言者。
6. 上传图片和文件：插入图片到文章；扩展自定义文件链接和html页面。
7. 配置小组件：心愿图片、自定义链接列表、“闲言碎语”。
8. 开启或取消文章点赞功能。
9. 允许限制访客单日内的留言次数
10. 集成Cravatar头像服务，允许在评论区显示或关闭留言者的头像
......

### 运行|开发项目：
#### 环境配置：.net core 6 sdk and asp.net core 6，visual studio 2022
本项目默认使用mysql8.0进行开发完成，但采用Sqlsugar ORM框架，只需更改configs.json的连接字符串以使用sqlServer等，但非mysql数据库需要自行验证开发。
1. 选择某个Release版本，下载代码，如v2.0.1。
2. 在CoolNetBlog\目录下创建文件configs.json，并复制以下json内容：
```
{
  "DbConnStr": {
    //here your database connection string
    "Default": "server=your_ip;uid=your_username;pwd=your_mysql_password;database=CoolNetBlog",
    "MySql": "server=your_ip;uid=your_username;pwd=your_mysql_password;database=CoolNetBlog",
    "SqlServer": ""
  },
  "Redis": {}
}
```
然后可以使用git将configs.json设为忽略项

3. 添加CoolNetBlog数据库(版本v2.0.1，其余类似)：

使用navicat等数据库管理工具连接mysql服务器，运行sql语句，sql文件是本仓库下的"v201_dump_CoolNetBlog.sql"。

或者在终端依次执行命令：
```
mysql -u用户名 -p密码
CREATE DATABASE if not exists CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;
exit;
mysql -u用户名 -p密码 CoolNetBlog < v201_dump_CoolNetBlog.sql
```
这样便添加CoolNetBlog数据库完毕。

4. visual studio启动项目。若数据库地址是远程服务器，确保地址和密码输入正确且服务器开启3306端口、mysql版本远程连接方式配置正确。

5. 后台页是 站点/admin/login, 默认账户是bluesky默认密码12345678

### 部署至服务器：
#### Ubuntu & Nginx
#### CentOS & Apache
#### Windows & IIS
参见微软msdn文档，有详细介绍。
[Host and deploy ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-6.0"部署Linux|windows")
