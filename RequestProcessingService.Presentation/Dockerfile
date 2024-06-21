FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RequestProcessingService.Controllers/RequestProcessingService.Controllers.csproj", "RequestProcessingService.Controllers/"]
RUN dotnet restore "RequestProcessingService.Controllers/RequestProcessingService.Controllers.csproj"
COPY . .
WORKDIR "/src/RequestProcessingService.Controllers"
RUN dotnet build "RequestProcessingService.Controllers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RequestProcessingService.Controllers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RequestProcessingService.Controllers.dll"]
