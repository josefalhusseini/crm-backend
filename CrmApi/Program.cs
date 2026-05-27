using CrmApi.Data;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CRM API", Version = "v1" });
});

builder.Services.AddSingleton<CosmosClient>(_ =>
    new CosmosClient(builder.Configuration["CosmosDb:ConnectionString"]));

builder.Services.AddScoped<CustomerRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
