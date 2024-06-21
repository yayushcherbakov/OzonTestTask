using RequestProcessingService.BusinessLogic.Extensions;
using RequestProcessingService.DataAccess.Extensions;
using RequestProcessingService.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddBusinessLogicServices();
builder.Services.AddDataAccess();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcService<GreeterService>();

app.Run();