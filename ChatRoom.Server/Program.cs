using ChatRoom.Server.Extension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseWebSockets();

app.UseWebSocketMiddleware();

app.Run();