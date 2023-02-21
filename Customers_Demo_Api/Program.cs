using Customers_Demo_Api.HostedService;
using Customers_Demo_Api.Middleware;
using Customers_Demo_Service.Service;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// register custom service
builder.Services.AddSingleton<ICustomerService, CustomerService>();

// register hosted service
builder.Services.AddHostedService<CustomerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// use exception interceptor
app.UseMiddleware<ExceptionMiddleware>(app);

app.UseAuthorization();

app.MapControllers();

app.Run();
