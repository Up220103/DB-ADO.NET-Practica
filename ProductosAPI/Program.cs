using MYBACK.Endpoints;
using MYBACK.Repositories;
using MYBACK.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Productos con MySQL (ADO.NET). La cadena de conexion viene de user-secrets.
builder.Services.AddSingleton<ConexionDb>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapProductoEndpoints();

app.Run();
