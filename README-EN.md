# CoolNetBlog

## A lightweight, concise, and fast blogging framework using asp.net core 6.

### Basic features：
Powerd By asp.net core 6 mvc，default use mysql8
Support mobile and pc style, you can set menus, keyword search, customize site tail and title.
Article management: Drafts, menu categorization, article locking, article text style, labeling and searching, article images (large, original, thumbnail).
Upload images and files: Insert images into articles; extend custom file links and html pages.
Configuration widgets: external links sidebar, wish-possible images, custom link lists.
Turn on or disable article likes.


### Run|development project ：
#### IDE：.net core 6 sdk and asp.net core 6，visual studio 2022
By default, this project uses mysql8.0 for development, and sqlserver requires you to verify the development by yourself.

1. Select a Release version(Lasted) of the repository and download the repository code, such as v2.0.1 or v1.9.9
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

Use navicat and other database management tools to connect to the mysql server, run sql statements, and the sql file is the "v199_dump_CoolNetBlog.sql" under directory.

Another way is that the commands can be executed sequentially in the terminal：
```
mysql -u{name} -p
CREATE DATABASE if not exists CoolNetBlog CHARACTER SET utf8 COLLATE utf8_general_ci;
exit;
mysql -u{name} -p{password} CoolNetBlog < v199_dump_CoolNetBlog.sql
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

