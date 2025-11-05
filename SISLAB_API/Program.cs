using Microsoft.AspNetCore.Http.Features;
using SISLAB_API.Areas.Maestros.Services;

using VideoReportApi.Repositories;
using VideoReportApi.Services;



var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 3L * 1024 * 1024 * 1024; // 3 GB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 3L * 1024 * 1024 * 1024; // 3 GB
});
// Obtener la ruta compartida de la configuración
var sharedPath = builder.Configuration["SharedPath"];

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORShttps://intranet.qf.com.pe
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://intranet.qf.com.pe")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

// Repositorios
builder.Services.AddScoped<EmpleadoRepository>();
builder.Services.AddScoped<LoginRepository>();
builder.Services.AddScoped<NoticiaRepository>();
builder.Services.AddScoped<DocumentRepository>();
builder.Services.AddScoped<InductionRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AgendaRepository>();
builder.Services.AddScoped<EmailRepository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<VideoReportRepository>();
builder.Services.AddScoped<BloqueAnuncioRepository>();


// Servicios
builder.Services.AddScoped<BloqueAnuncioService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<NoticiaService>();
builder.Services.AddScoped<InductionService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DataTransferService>();
builder.Services.AddScoped<AgendaService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<VideoReportService>();


// Registrar DocumentoService con la ruta compartida
builder.Services.AddScoped<DocumentoService>(provider =>
    new DocumentoService(
        provider.GetRequiredService<DocumentRepository>(),
         provider.GetRequiredService<UserService>(),  // Inyectar UserService
        provider.GetRequiredService<EmailService>(),


        sharedPath)); // Pasar la ruta compartida

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