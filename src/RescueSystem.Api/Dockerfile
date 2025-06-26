FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nuget.config .
COPY localpackages/ ./localpackages/

COPY src/RescueSystem.Api/RescueSystem.Api.csproj src/RescueSystem.Api/
COPY src/RescueSystem.Application/RescueSystem.Application.csproj src/RescueSystem.Application/
COPY src/RescueSystem.Domain/RescueSystem.Domain.csproj src/RescueSystem.Domain/
COPY src/RescueSystem.Infrastructure/RescueSystem.Infrastructure.csproj src/RescueSystem.Infrastructure/

RUN dotnet restore "src/RescueSystem.Api/RescueSystem.Api.csproj"

COPY . .

WORKDIR "/src/src/RescueSystem.Api"
RUN dotnet publish "RescueSystem.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "RescueSystem.Api.dll"]