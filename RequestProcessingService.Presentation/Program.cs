using FluentMigrator.Runner;
using RequestProcessingService.Access.Extensions;
using RequestProcessingService.BusinessLogic.Extensions;
using RequestProcessingService.DataAccess.Extensions;
using RequestProcessingService.Infrastructure.Extensions;
using RequestProcessingService.Presentation.Interceptors;
using RequestProcessingService.Presentation.Services;
using RequestProcessingService.Scheduler.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddBusinessLogicServices();
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddAccessServices();

builder.Services.AddScheduler();
builder.Services.AddReportRequestEventHandler(builder.Configuration);

builder.Services.AddLogging();

builder.Services.AddGrpc(options => { options.Interceptors.Add<ErrorHandlerInterceptor>(); });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcService<ReportRequestsGrpcService>();

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    runner.MigrateUp();
}

app.Run();