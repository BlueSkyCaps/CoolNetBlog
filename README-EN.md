# CoolNetBlog

## A lightweight, concise, and fast blogging framework using asp.net core 6.

[![ASP.NET Core 6](https://shields.io/badge/-Asp_Net_Core_6-blue)](https://asp.net/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-v5.1-blue)](https://getbootstrap.com/)
[![Uikit](https://img.shields.io/badge/Uikit-v3.10.1-blue)](https://getuikit.com)
[![jQuery](https://img.shields.io/badge/jQuery-v3.5.1-blue)](https://jquery.com/)

### Basic features：
1. Support mobile and pc style. Menus can be set, keyword search, custom site tail and title can be customized.
2. Article management: There are standard articles, draft articles, privacy articles (password lock) and special articles.
3. admin can set the comment type of the article: 1 Public 2 Moderation 3 Do not allow comments
5. The background can review the message, can directly reply to the messager, and choose to send an email to the messager.
6. Upload images and files: Insert images into articles; extend custom file links and html pages.
7. Configure widgets: "Personality Picture", Custom Link List, "Small Text".
8. Turn on or off article likes.
9. Limit the number of messages a visitor can leave in a single day
10. Integrated Cravatar avatar service, allowing the avatar of the commenter to be displayed or turned off in the comment area
......


### Run|development project ：
#### IDE：.net core 6 sdk and asp.net core 6，visual studio 2022
By default, this project uses mysql8.0 for development, and sqlserver requires you to verify the development by yourself.

1. Select a Release version and download the code, such as v2.0.1
2. Create the file 'configs.json' in the 'CoolNetBlog' directory and copy the json content：
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
You can then use git to make configs.json an ignored file.

3. Add coolNetBlog database (version v2.0.1, similar to the rest)：

Use navicat and other database management tools to connect to the mysql server, run sql statements, and the sql file is the "v201_dump_CoolNetBlog.sql" under directory.

Another way is that the commands can be executed sequentially in the terminal：
```
mysql -u{name} -p
CREATE DATABASE if not exists CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;
exit;
mysql -u{name} -p{password} CoolNetBlog < v201_dump_CoolNetBlog.sql
```
This completes the addition of the CoolNetBlog database.

4. Visual studio starts the project. If the database address is a remote server, make sure that the address and password are entered correctly, that the server has port 3306 open, and that the mysql version of the remote connection method is configured correctly.

5. Background admin page is {site}/admin/login, default username is bluesky and password is 12345678

### Deploy to the server ：
#### Ubuntu & Nginx
#### CentOS & Apache
#### Windows & IIS

See the Microsoft MSDN documentation for details.
[Host and deploy ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-6.0"部署Linux|windows")

