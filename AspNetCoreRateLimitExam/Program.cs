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

builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();


//// Ip adresi ile ilgili izinleri belirleyece�imiz appsettings i�erisine key verece�iz.
//builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IPRateLimiting"));


// Ip adresi ile ilgili izinleri belirleyece�imiz appsettings i�erisine key verece�iz.
builder.Services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));


////Policy belirtiyoruz.(Ip i�in)
//builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IPRateLimitPolicies"));


////Policy belirtiyoruz.(Client i�in)
builder.Services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

//Uygulama �al��t���nda bir kere y�klenmesini,bir daha nesne �rne�i al�nmas�n� istemiyoruz.(Ip rate limit i�in)
//builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
// E�er docker kullan�yorsak DistributeCache kullanmam�z gerekiyor.(DistributedCacheIpPolicyStore).E�er kullanmaz isek request say�lar� o uygulaman�n aya�a kalkm�� oldu�u
//memory'de tutulaca��ndan dolay� tutars�z olacakt�r.

//Uygulama �al��t���nda bir kere y�klenmesini,bir daha nesne �rne�i al�nmas�n� istemiyoruz.(Client rate limit i�in)
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();

//Ka� request yap�ld��� verilerin tutulaca�� yer belirtiyoruz.
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

//Request yapan�n ip adresini, header bilgisini okuyabilmesi i�in IHttpContextAccessor'u implemente etmemiz gerekiyor.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//RateLimit ana servisini ekliyoruz.
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


var app = builder.Build();

//GetService kullan�ld���nda ilgili servis yoksa geriye null d�ner, GetRequiredService kullan�ld���nda ilgili servis yoksa hata f�rlat�r.(Ip i�in)
//var ipPolicy = app.Services.GetRequiredService<IIpPolicyStore>();
//Bu method appsettings.json i�erisindeki policy'leri uygulamay� sa�l�yor.
//Wait metodu ilgili sat�rdan sonu� gelinceye kadar bekliyor, di�er sat�ra ge�miyor.


//var ipPolicy = app.Services.GetRequiredService<IClientPolicyStore>();// (Client i�in)


//ipPolicy.SeedAsync().Wait();

////A�a��daki tan�m, Service alan�nda tan�mlam�� oldu�umuz �zellikleri kullanarak ip adresi �zerinden k�s�tlama ekleyecek. (Ip rate limit i�in)
//app.UseIpRateLimiting();

//A�a��daki tan�m, Service alan�nda tan�mlam�� oldu�umuz �zellikleri kullanarak client �zerinden k�s�tlama ekleyecek. (Client rate limit i�in)
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
