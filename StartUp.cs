using System.Text.Encodings.Web;
using System.Text.Unicode;
using AspNetCoreHero.ToastNotification;
using Du_An.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Du_An
{
    public class StartUp
    {
        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration{get;}
        // Cấu hình các dịch vụ, đăng kí dịch vụ mới vào ứng dụng
        public void ConfigureServices(IServiceCollection services){
            services.AddDbContext<dbMarketssContext>(options => {
                string connectString = Configuration.GetConnectionString("AppMvcConnectionString");
                options.UseSqlServer(connectString);
            });
            // Đăng kí dịch vụ để có thể hoạt động theo mô hình mvc
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSession();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(p=> 
                    {
                        p.LoginPath = "/dang-nhap.html";
                        p.AccessDeniedPath="/";
                    });
            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] {UnicodeRanges.All}));
            services.AddNotyf(config => {config.DurationInSeconds =10; config.IsDismissable=true;config.Position=NotyfPosition.TopRight;});
            // services.AddSingleton(typeof(ProductService), typeof(ProductService));
        }
        // Tạo ra pipeline 1 chuỗi các middle Ware mà HttpContext phải đi qua
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
            if(env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
            }
            else{
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            // Xác định danh tính
            app.UseAuthentication();
            // Xác thực quyền truy cập
            app.UseAuthorization();
            
            // Tạo ra các điểm enpoint ánh xạ Url đến thành phần của ứng dụng 
            app.UseEndpoints(endpoints => {
                // Tạo ra các ánh xạ Url CA
                // /{controller}/{action}/{id}
                // Abc/Xyz => Controller = Abc, gọi method Xyz
                endpoints.MapControllerRoute(
                    name: "Admin",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();
            });
        }
    }
}

// tao area: dotnet aspnet-codegenerator area Admin
// tao controller: dotnet aspnet-codegenerator controller -name AdminRoleController -namespace Du_An.Areas.Admin.Controllers -m Du_An.Models.Role -udl -dc Du_An.Models.dbMarketssContext -outDir Areas/Admin/Controllers  