# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia el csproj y restaura (nombre correcto)
COPY ["MantiaServiceAPI.csproj", "./"]
RUN dotnet restore "MantiaServiceAPI.csproj"

# Copia todo el código y publica
COPY . .
RUN dotnet publish "MantiaServiceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render inyecta la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "MantiaServiceAPI.dll"]
