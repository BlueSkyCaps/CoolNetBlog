using CoolNetBlog.Base;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<WebEncoderOptions>(options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});

builder.Services.AddControllersWithViews().AddJsonOptions((options =>
{
    // ����System.Text.Json ȫ��Json��Ӣ���ַ�
    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);//�����ò����ַ��޷�ԭ����ʾ
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
}));


builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AdminEnterFilter());
});

builder.Services.Configure<RazorViewEngineOptions>(opt =>
{
    //��չAdminAccess�ļ�����ͼ����·��
    opt.ViewLocationFormats.Add("~/Views/AdminAccess/{1}/{0}" + RazorViewEngine.ViewExtension);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/WarningPage/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();


//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "defaultWithAdminAccess",
    pattern: "AdminAccess/{controller=Admin}/{action=Index}/{id?}");

app.Run();
