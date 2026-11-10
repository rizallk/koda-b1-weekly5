using ErrorHandling.Middlewares;
using Microsoft.Data.SqlClient;
using Weekly5.Repositories;
using Weekly5.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(
    option =>
    {
        option.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path.Value;

            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        };
    }
);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Konfigurasi koneksi db
builder.Services.AddScoped(_ => new SqlConnection(builder.Configuration.GetConnectionString("ewallet")));

// Repository
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IWalletRepository, SqlWalletRepository>();
builder.Services.AddScoped<ITransactionRepository, SqlTransactionRepository>();

// Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
     {
         options.SwaggerEndpoint("/openapi/v1.json", "E-Wallet API v1");
         options.RoutePrefix = "swagger";

         options.DocumentTitle = "E-Wallet | API Documentation";

         options.EnableFilter();
     });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllers();

app.Run();
