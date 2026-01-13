using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Application.Services;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.Application.Interfaces.Auth;
using ChatApp.Infrastructure.Auth;
using ChatApp.Application.Common;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure
{
    /// <summary>
    /// Handles dependency injection registrations for infrastructure and application services.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // -------------------- DATABASE --------------------
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // -------------------- REPOSITORIES --------------------
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageReceiptRepository, MessageReceiptRepository>();

            // -------------------- SERVICES --------------------
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatService, ChatService>();
            //----------------JWT-------------------
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


            services.AddScoped<IConversationParticipantRepository, ConversationParticipantRepository>();

            //----------email -----
           services.AddScoped<IEmailService, Services.EmailService>();
         

         //-------groups
         services.AddScoped<IGroupRepository, GroupRepository>();


              
            return services;
        }
    }
}
