using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.EmailAggr;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Infrastructure;
using Sem5Pi2425.Infrastructure.Shared;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SurgeryRoomAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.AppointmentInfra;
using Sem5Pi2425.Infrastructure.SystemUser;
using Sem5Pi2425.Infrastructure.OperationRequestInfra;
using Sem5Pi2425.Infrastructure.OperationTypeInfra;
using Sem5Pi2425.Infrastructure.PatientInfra;
using Sem5Pi2425.Infrastructure.StaffInfra;
using Sem5Pi2425.Infrastructure.SurgeryRoomInfra;

namespace Sem5Pi2425 {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<Sem5Pi2425DbContext>(opt =>
                opt.UseInMemoryDatabase("Sem5Pi2425DB")
                .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());
            
            ConfigureMyServices(services);
            

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void ConfigureMyServices(IServiceCollection services) {
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IEmailService, EmailService>();
            
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<UserService>();

            services.AddTransient<IAppointmentRepository, AppointmentRepository>();

            services.AddTransient<IOperationRequestRepository, OperationRequestRepository>();

            services.AddTransient<IOperationTypeRepository, OperationTypeRepository>();

            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<PatientService>();

            services.AddTransient<IStaffRepository, StaffRepository>();

            services.AddTransient<ISurgeryRoomRepository, SurgeryRoomRepository>();
        }
    }
}