using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Repositories.BookKeeping;
using XChange.Data.Repositories.CompanyExchangeFunds;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Data.Repositories.ExchangeInfo;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<XChangeContext>();

builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookKeepingRepository, BookKeepingRepository>();
builder.Services.AddScoped<ICompanyExchangeFundsRepository, CompanyExchangeFundsRepository>();
builder.Services.AddScoped<IExchangeInfoRepository, ExchangeInfoRepository>();
builder.Services.AddScoped<IUserFundsRepository, UserFundsRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddScoped<ICurrencyRateUpdaterService, CurrencyRateUpdaterService>();
builder.Services.AddHostedService<CurrencyRateUpdaterBackgroundService>();

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