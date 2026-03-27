FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY eOpcina/eOpcina.csproj eOpcina/
RUN dotnet restore eOpcina/eOpcina.csproj

COPY . .
WORKDIR /src/eOpcina
RUN dotnet publish eOpcina.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eOpcina.dll"]
