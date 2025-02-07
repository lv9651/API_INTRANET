
using SISLAB_API.Areas.Maestros.Repositories;
using SISLAB_API.Areas.Maestros.Services;
using YourNamespace.Services;

var builder = WebApplication.CreateBuilder(args);

// Obtener la ruta compartida de la configuración
var sharedPath = builder.Configuration["SharedPath"];

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3001")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

// Repositorios
builder.Services.AddScoped<EmpleadoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<MedicoRepository>();
builder.Services.AddScoped<VentaRepository>();


// Servicios
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IzipayService>();
builder.Services.AddScoped<VentaService>();
builder.Services.AddScoped<CorreoService>();


// Registrar DocumentoService con la ruta compartida

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar la política CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();