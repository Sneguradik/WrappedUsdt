using System.Net;
using System.Net.Http.Headers;
using Quartz;

using WrappedUSDTCollector.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddHttpClient<IExmoService,ExmoService>(opt =>
{
    opt.BaseAddress = new Uri(ExmoService.BaseUrl);
    opt.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
    opt.DefaultRequestHeaders.TryAddWithoutValidation("Host", "chart.exmoney.com");
    
});

builder.Services.AddHttpClient<IYahooService, YahooService>(opt =>
{
    opt.BaseAddress = new Uri(YahooService.BaseUrl);
    opt.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
});

builder.Services.AddHttpClient<IIndexService, IndexService>(opt =>
{
    opt.BaseAddress = new Uri(IndexService.BaseUrl);
    opt.DefaultRequestHeaders.TryAddWithoutValidation("APIKEY", Environment.GetEnvironmentVariable("API_KEY"));
});

builder.Services.AddSingleton<ISyncService, SyncService>();

builder.Services.AddQuartz(options =>
{
    var jobKey = JobKey.Create(nameof(IndexJob));
    options.AddJob<IndexJob>(jobKey)
        .AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(x=>x.WithIntervalInMinutes(1).RepeatForever()));
});
builder.Services.AddQuartzHostedService();


var host = builder.Build();


host.Run();