using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//appsettings.json üzerinden okuyabilmemiz için AddOptions'ý eklememiz gerekiyor.
builder.Services.AddOptions();

//Cache servisi ekliyoruz.Örn dakikada 50 request yapabiliyor olacaðýmýz zaman, 50 requesti ram'de tutacaðýz.
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
// Ip adresi ile ilgili izinleri belirleyeceðimiz appsettings içerisine key vereceðiz.
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IPRateLimiting"));

//Policy belirtiyoruz.
builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IPRateLimitPolicies"));

//Uygulama çalýþtýðýnda bir kere yüklenmesini,bir daha nesne örneði alýnmasýný istemiyoruz.
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
// Eðer docker kullanýyorsak DistributeCache kullanmamýz gerekiyor.(DistributedCacheIpPolicyStore).Eðer kullanmaz isek request sayýlarý o uygulamanýn ayaða kalkmýþ olduðu
//memory'de tutulacaðýndan dolayý tutarsýz olacaktýr.

//Kaç request yapýldýðý verilerin tutulacaðý yer belirtiyoruz.
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

//Request yapanýn ip adresini, header bilgisini okuyabilmesi için IHttpContextAccessor'u implemente etmemiz gerekiyor.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//RateLimit ana servisini ekliyoruz.
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


var app = builder.Build();

//Aþaðýdaki taným, Service alanýnda tanýmlamýþ olduðumuz özellikleri kullanarak ip adresi üzerinden kýsýtlama ekleyecek.
app.UseIpRateLimiting();


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
