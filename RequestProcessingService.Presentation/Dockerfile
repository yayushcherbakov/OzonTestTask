﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RequestProcessingService.Presentation/RequestProcessingService.Presentation.csproj", "RequestProcessingService.Presentation/"]
COPY ["RequestProcessingService.BusinessLogic/RequestProcessingService.BusinessLogic.csproj", "RequestProcessingService.BusinessLogic/"]
COPY ["RequestProcessingService.DataAccess/RequestProcessingService.DataAccess.csproj", "RequestProcessingService.DataAccess/"]
COPY ["RequestProcessingService.Access/RequestProcessingService.Access.csproj", "RequestProcessingService.Access/"]
COPY ["RequestProcessingService.Infrastructure/RequestProcessingService.Infrastructure.csproj", "RequestProcessingService.Infrastructure/"]
RUN dotnet restore "RequestProcessingService.Presentation/RequestProcessingService.Presentation.csproj"
COPY . .
WORKDIR "/src/RequestProcessingService.Presentation"
RUN dotnet build "RequestProcessingService.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RequestProcessingService.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RequestProcessingService.Presentation.dll"]
