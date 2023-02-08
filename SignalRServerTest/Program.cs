using SignalRServerTest.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR(opt =>
{
    opt.MaximumReceiveMessageSize = 2 * 1024 * 1024;
    opt.MaximumParallelInvocationsPerClient = 10;
    opt.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    opt.HandshakeTimeout = TimeSpan.FromMinutes(1);
    opt.EnableDetailedErrors = true;
})
    .AddMessagePackProtocol();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<StreamHub>("/streamhub", opt =>
{
    opt.ApplicationMaxBufferSize = 2 * 1024 * 1024;
});
app.Run();
