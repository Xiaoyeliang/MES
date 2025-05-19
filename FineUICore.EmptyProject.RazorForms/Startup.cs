using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Schema;

namespace FineUICore.EmptyProject.RazorForms
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession();

            // 配置请求参数限制
            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = 1024;   // 请求参数的个数限制（默认值：1024）
                x.ValueLengthLimit = 4194304;   // 单个请求参数值的长度限制（默认值：4194304 = 1024 * 1024 * 4）
            });
            
            // FineUI 服务
            services.AddFineUI(Configuration);

           var mvcBuilder= services.AddRazorPages().AddMvcOptions(options =>
            {
                // 自定义JSON模型绑定（添加到最开始的位置）
                options.ModelBinderProviders.Insert(0, new FineUICore.JsonModelBinderProvider());

                // 自定义RazorForms过滤器（仅在启用EnableRazorForms时有效）
                options.Filters.Insert(0, new FineUICore.RazorFormsFilter());

            }).AddNewtonsoftJson().AddRazorRuntimeCompilation();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // 配置请求语言提供程序
            var supportedCultures = new List<CultureInfo>()
            {
                new CultureInfo("zh-CN"),
                new CultureInfo("zh-TW"),
                new CultureInfo("en-US")
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0].Name, uiCulture: supportedCultures[0].Name);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // 自定义请求语言提供器（默认的Cookie格式：.AspNetCore.Culture=c=zh-CN|uic=zh-CN，为了复用之前已经定义过的Cookie：Language=zh_CN）
                options.AddInitialRequestCultureProvider(new MyCustomRequestCultureProvider());
            });

            // 配置视图和数据注解的多语言支持
            mvcBuilder.AddViewLocalization().AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedAnnotationResources));
            });
            // ##结束配置##多语言服务##########################
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
 // 使用请求语言服务
            app.UseRequestLocalization();
            app.UseStaticFiles();   
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            // FineUI 中间件（确保 UseFineUI 位于 UseEndpoints 的前面）
            app.UseFineUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
