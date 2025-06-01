using Microsoft.Extensions.DependencyInjection;
using StickyNotes.Mapping;
using StickyNotes.Models;
using StickyNotes.Services;
using Microsoft.AspNetCore.Identity;

namespace StickyNotes.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            #region General Services
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            #endregion

            #region Mappers Services
            services.AddScoped<IUserMapper, UserMapper>();
            services.AddScoped<IStickyNoteMapper,  StickyNoteMapper>();
            #endregion

            #region Entity Services
            services.AddScoped<IStickyNoteService, StickyNoteService>();
            #endregion

            #region Token Service
            services.AddScoped<ITokenService, TokenService>();
            #endregion

            return services;
        }
    }
}