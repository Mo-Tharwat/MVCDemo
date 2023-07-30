using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Context;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.Helpers.InterFaces;
using Demo.PL.Helpers.Settings;
using Demo.PL.MappingProfile;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.Pl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(); //MVC Services

            //this line is to Allow Dependance injection For the DataBase 
            services.AddDbContext<MVCAppDemoDbcontext>(options =>
            {
                //this line is to get the Connection string form the AppSettings.JSON
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Allow the Dependance injection For the BLL Interfaces 
            //services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //AddScoped
            ///Gets the data when it gets requested and deletes it after the requset is deleted
            ///Best uses is for DbContext
            ///services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //AddSingleton
            ///Is a way to get the data one time for the app life time
            ///Best uses is for Logging Services and caching
            ///services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
            //AddTransient 
            ///Gets the data when it gets requested each time
            ///services.AddTransient<IDepartmentRepository, DepartmentRepository>();

            //For adding Mapper 
            services.AddAutoMapper(M => M.AddProfile(new EmployeeProfile()));
            services.AddAutoMapper(M => M.AddProfile(new DepartmentProfile()));


            // this Function sets the Defult values For security like (Cookies Expiration time) etc... 
			services.AddIdentity<ApplicationUser, IdentityRole>()
				//AddEntityFrameworkStores to Implement the Stores 
				.AddEntityFrameworkStores<MVCAppDemoDbcontext>()
                //Adding the Defult Tokens for resetPassword, two factor etc...
                .AddDefaultTokenProviders();

			//Applying Security 
			//Manually Wrong way
			///services.AddScoped<UserManager<ApplicationUser>>();
			///services.AddScoped<SignInManager<ApplicationUser>>();
			///services.AddScoped<RoleManager<IdentityRole>>();
			///
			///CookieAuthenticationDefaults.AuthenticationScheme is to set a scheme for a cookie
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie/*this is to control Cookie options*/(options=>
                {
                    options.LoginPath = "Account/Login"; // this is to make sure if the user is loged in or not if he/she isn`t the app will automaticly route him/her to the login page  
                    options.AccessDeniedPath = "Home/Error";
                });


            //this line is to map the MailSettings Section that we added in the APPSettings Folder to the class we made 
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IEmailSettings,EmailSettings>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            //use static files: requests the files attaced to the main linked resource like BootStrap and it`s Css|Js files 
            app.UseStaticFiles();

            app.UseRouting();

            //Activating Cookies and Auth options
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
