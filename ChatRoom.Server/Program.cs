using ChatRoom.Server;
using ChatRoom.Server.Extension;
using ChatRoom.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IRoomService, RoomService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseWebSockets();

app.UseWebSocketMiddleware();

app.Run();