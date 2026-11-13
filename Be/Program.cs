using Be.Data;
using Be.Services;
using Be.Services.Interfaces;
using Be.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ĐĂNG KÝ CONTROLLERS VÀ SWAGGER
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==========================================
// 2. CẤU HÌNH KẾT NỐI DATABASE
// ==========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ==========================================
// 3. ĐĂNG KÝ SERVICES
// ==========================================
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<IOpenMeteoService, OpenMeteoService>();
builder.Services.AddScoped<IForecastService, ForecastService>();
// ==========================================
// 4. CẤU HÌNH OPEN-METEO
// ==========================================
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

// ==========================================
// 5. ĐĂNG KÝ HTTPCLIENT
// ==========================================
builder.Services.AddHttpClient("OpenMeteo", (serviceProvider, client) =>
{
    var options = serviceProvider
        .GetRequiredService<IOptions<OpenMeteoOptions>>()
        .Value;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IPythonMLService, PythonMLService>(
    (serviceProvider, client) =>
    {
    var  configuration = 
    serviceProvider.GetRequiredService<IConfiguration>();
    
    var baseUrl = configuration["PythonML:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException(
            "Chưa cấu hình PythonML:BaseUrl."
        );
    }
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromMinutes(2);
});


// ==========================================
// 6. KHỞI TẠO ỨNG DỤNG
// ==========================================
var app = builder.Build();

// ==========================================
// 7. CẤU HÌNH MIDDLEWARE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ==========================================
// 8. CHẠY ỨNG DỤNG
// ==========================================
app.Run();