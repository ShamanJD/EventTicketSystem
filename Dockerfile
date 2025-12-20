FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/EventTicket.Contracts/EventTicket.Contracts.csproj", "src/EventTicket.Contracts/"]
COPY ["src/EventTicket.Domain/EventTicket.Domain.csproj", "src/EventTicket.Domain/"]
COPY ["src/EventTicket.Application/EventTicket.Application.csproj", "src/EventTicket.Application/"]
COPY ["src/EventTicket.Infrastructure/EventTicket.Infrastructure.csproj", "src/EventTicket.Infrastructure/"]
COPY ["src/EventTicket.Api/EventTicket.Api.csproj", "src/EventTicket.Api/"]

RUN dotnet restore "src/EventTicket.Api/EventTicket.Api.csproj"

COPY . .
WORKDIR "/src/src/EventTicket.Api"
RUN dotnet build "EventTicket.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventTicket.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventTicket.Api.dll"]