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


//// Ip adresi ile ilgili izinleri belirleyeceðimiz appsettings içerisine key vereceðiz.
//builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IPRateLimiting"));


// Ip adresi ile ilgili izinleri belirleyeceðimiz appsettings içerisine key vereceðiz.
builder.Services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));


////Policy belirtiyoruz.(Ip için)
//builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IPRateLimitPolicies"));


////Policy belirtiyoruz.(Client için)
builder.Services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

//Uygulama çalýþtýðýnda bir kere yüklenmesini,bir daha nesne örneði alýnmasýný istemiyoruz.(Ip rate limit için)
//builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
// Eðer docker kullanýyorsak DistributeCache kullanmamýz gerekiyor.(DistributedCacheIpPolicyStore).Eðer kullanmaz isek request sayýlarý o uygulamanýn ayaða kalkmýþ olduðu
//memory'de tutulacaðýndan dolayý tutarsýz olacaktýr.

//Uygulama çalýþtýðýnda bir kere yüklenmesini,bir daha nesne örneði alýnmasýný istemiyoruz.(Client rate limit için)
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();

//Kaç request yapýldýðý verilerin tutulacaðý yer belirtiyoruz.
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

//Request yapanýn ip adresini, header bilgisini okuyabilmesi için IHttpContextAccessor'u implemente etmemiz gerekiyor.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//RateLimit ana servisini ekliyoruz.
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


var app = builder.Build();

//GetService kullanýldýðýnda ilgili servis yoksa geriye null döner, GetRequiredService kullanýldýðýnda ilgili servis yoksa hata fýrlatýr.(Ip için)
//var ipPolicy = app.Services.GetRequiredService<IIpPolicyStore>();
//Bu method appsettings.json içerisindeki policy'leri uygulamayý saðlýyor.
//Wait metodu ilgili satýrdan sonuç gelinceye kadar bekliyor, diðer satýra geçmiyor.


//var ipPolicy = app.Services.GetRequiredService<IClientPolicyStore>();// (Client için)


//ipPolicy.SeedAsync().Wait();

////Aþaðýdaki taným, Service alanýnda tanýmlamýþ olduðumuz özellikleri kullanarak ip adresi üzerinden kýsýtlama ekleyecek. (Ip rate limit için)
//app.UseIpRateLimiting();

//Aþaðýdaki taným, Service alanýnda tanýmlamýþ olduðumuz özellikleri kullanarak client üzerinden kýsýtlama ekleyecek. (Client rate limit için)
app.UseClientRateLimiting();

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
