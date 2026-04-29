using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EmhsConnection");

builder.Services.AddDbContext<EmhsDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<EmhsService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
