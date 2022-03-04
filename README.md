# CoolNetBlog

## A lightweight, concise, and fast blogging framework using asp.net core 6.
使用 asp.net core 6 的轻量级、简洁、快速的博客框架。

### 使用介绍视频：
[点击：观看基本使用介绍视频](https://www.bilibili.com/video/BV19S4y1F7zA?share_source=copy_web "好玩：独自开发的极简博客框架~全手写部署Linux|windows")

### 截图展示：
![PC首页](https://s2.loli.net/2022/02/27/zhpayQPRT3OF6xw.png)
![PC文章页](https://s2.loli.net/2022/02/27/quPsDoGChXUyJWQ.png)
![移动端首页和文章页](https://s2.loli.net/2022/02/27/Jvqc1wPImfloSnu.png)
![后台面版](https://s2.loli.net/2022/02/27/sE6Otn5rgNUcbLJ.png)

### 基本功能(v1.9.9，当前版本)：
Powerd By asp.net core 6 mvc，default use mysql8
1. 支持移动端和pc端样式，可以设置菜单，关键词搜索，自定义站点尾巴和标题。
2. 文章管理：草稿、菜单归类、文章加锁、文章文字样式、设置标签且搜索、文章图片(大图、原始图、缩略图)。
3. 上传图片和文件：插入图片到文章；扩展自定义文件链接和html页面。
4. 配置小组件：外部链接侧边栏、心愿图片、自定义链接列表。
5. 开启或取消文章点赞功能。

### 后续功能(v2.0.1，下一版本)
- 扩充帖子评论留言功能，随时启用。
- 文章编辑优化，文本编辑可预览(不引用第三方富文本插件，暂无此想法)。

### 运行|开发项目：
#### 环境配置：.net core 6 sdk and asp.net core 6，visual studio 2022
本项目默认使用mysql进行开发完成，sqlserver等需自行验证开发
1. 选择仓库的某个Tag版本(最终版Lasted)，下载仓库代码，如v1.9.9 or v9.9.9
2. 在CoolNetBlog\目录下创建文件configs.json，并复制一下json内容：
```
{
  "DbConnStr": {
    //here your database connection string
    "Default": "server=你的主机ip;uid=你的有效用户名;pwd=mysql密码;database=CoolNetBlog",
    "MySql": "server=你的主机ip;uid=你的有效用户名;pwd=mysql密码;database=CoolNetBlog",
    "SqlServer": ""
  },
  "Redis": {}
}
```
然后可以使用git将configs.json设为忽略项

3. 添加CoolNetBlog数据库((版本v1.9.9，其余类似)：
使用navicat等数据库管理工具连接mysql服务器，运行sql语句，sql文件是本仓库下的"v199_dump_CoolNetBlog.sql"。
或者在终端依次执行命令：
```
mysql -u用户名 -p密码
CREATE DATABASE if not exists CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;
exit;
mysql -u用户名 -p密码 CoolNetBlog < v199_dump_CoolNetBlog.sql
```
这样便添加CoolNetBlog数据库完毕。

4. visual studio启动项目。若数据库地址是远程服务器，确保地址和密码输入正确且服务器开启3306端口、mysql8版本远程连接方式配置正确。
5. 后台页：/admin/login，默认账户bluesky和密码12345678

### 部署至服务器：
#### Ubuntu & NNginx
#### CentOS & Apache
#### Windows & IIS
参见微软msdn文档，有详细介绍。
[Host and deploy ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-6.0"部署Linux|windows")
