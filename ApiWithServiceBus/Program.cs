using ApiWithServiceBus.Options;
using ApiWithServiceBus.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ServiceBusOptions>(
    builder.Configuration.GetSection("ServiceBus"));

builder.Services.AddSingleton<ServiceBusClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
    return new ServiceBusClient(options.ConnectionString);
});

builder.Services.AddSingleton<IAzureServiceBus, AzureServiceBus>();

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

app.Run();
