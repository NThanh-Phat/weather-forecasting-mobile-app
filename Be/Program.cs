using Be.Data;
using Be.Services;
using Be.Services.Interfaces;
using Be.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controllers
builder.Services.AddControllers();

// Đăng ký kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Đăng ký Province Service
builder.Services.AddScoped<IProvinceService, ProvinceService>();

// Đăng ký cấu hình Open-Meteo
builder.Services
    .AddOptions<OpenMeteoOptions>()
    .Bind(builder.Configuration.GetSection(OpenMeteoOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(
            options.BaseUrl,
            UriKind.Absolute,
            out var uri
        ) && (uri.Scheme == Uri.UriSchemeHttp ||
              uri.Scheme == Uri.UriSchemeHttps),
        "Open-Meteo BaseUrl không hợp lệ."
    )
    .ValidateOnStart();

// Đăng ký HttpClient cho Open-Meteo
builder.Services.AddHttpClient("OpenMeteo", (serviceProvider, client) =>
{
    var options = serviceProvider
        .GetRequiredService<IOptions<OpenMeteoOptions>>()
        .Value;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Đăng ký Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();