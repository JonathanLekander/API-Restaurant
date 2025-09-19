using Application.Interfaces.Command;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using Application.UseCases;
using Infrastructure.Commands;
using Infrastructure.Persistence;
using Infrastructure.Querys;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Custom
builder.Services.AddDbContext<RestaurantDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSql")));

//CQRS
builder.Services.AddScoped<IDishQuery, DishQuery>();
builder.Services.AddScoped<IDishCommand, DishCommand>();
builder.Services.AddScoped<ICategoryQuery, CategoryQuery>();
builder.Services.AddScoped<IStatusQuery, StatusQuery>();
builder.Services.AddScoped<IDeliveryTypeQuery, DeliveryTypeQuery>();

//Services 
builder.Services.AddScoped<IServiceDish, ServiceDish>();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

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
