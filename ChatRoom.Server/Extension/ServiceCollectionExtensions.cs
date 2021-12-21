using ChatRoom.Server.Services;

namespace ChatRoom.Server.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IRoomService, RoomService>();

            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ICommandService, CommandService>();
            services.AddScoped<WebSocketHandler>();
        }
    }
}
