using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//appsettings.json �zerinden okuyabilmemiz i�in AddOptions'� eklememiz gerekiyor.
builder.Services.AddOptions();

//Cache servisi ekliyoruz.�rn dakikada 50 request yapabiliyor olaca��m�z zaman, 50 requesti ram'de tutaca��z.
builder.Services.AddMemoryCache();

// Ip adresi ile ilgili izinleri belirleyece�imiz appsettings i�erisine key verece�iz.
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IPRateLimit"));

//Policy belirtiyoruz.
builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IPRateLimitPolicies"));

//Uygulama �al��t���nda bir kere y�klenmesini,bir daha nesne �rne�i al�nmas�n� istemiyoruz.
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
// E�er docker kullan�yorsak DistributeCache kullanmam�z gerekiyor.(DistributedCacheIpPolicyStore).E�er kullanmaz isek request say�lar� o uygulaman�n aya�a kalkm�� oldu�u
//memory'de tutulaca��ndan dolay� tutars�z olacakt�r.

//Ka� request yap�ld��� verilerin tutulaca�� yer belirtiyoruz.
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

//Request yapan�n ip adresini, header bilgisini okuyabilmesi i�in IHttpContextAccessor'u implemente etmemiz gerekiyor.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
