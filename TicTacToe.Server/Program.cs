using TicTacToe.Server.Hubs;
using TicTacToe.Server.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});


builder.Services.AddSingleton<GameService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAll");
app.MapControllers();
app.MapHub<GameHub>("/game");

app.Run();