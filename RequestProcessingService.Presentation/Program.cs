using FluentMigrator.Runner;
using RequestProcessingService.BusinessLogic.Extensions;
using RequestProcessingService.DataAccess.Extensions;
using RequestProcessingService.Infrastructure.Extensions;
using RequestProcessingService.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddBusinessLogicServices();
builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddReportRequestEventHandler(builder.Configuration);
builder.Services.AddLogging();

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