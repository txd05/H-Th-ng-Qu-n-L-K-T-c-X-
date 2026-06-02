FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY KyTucXaManagement/KyTucXaManagement.csproj KyTucXaManagement/
RUN dotnet restore KyTucXaManagement/KyTucXaManagement.csproj

COPY . .
RUN dotnet publish KyTucXaManagement/KyTucXaManagement.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# SQLite db lưu ở /data (mount volume trên Railway)
RUN mkdir -p /data

# Railway inject $PORT động — phải dùng ASPNETCORE_URLS với ${PORT}
ENV ASPNETCORE_URLS="http://+:${PORT}"
ENV ConnectionStrings__DefaultConnection="Data Source=/data/KyTucXa.db"

ENTRYPOINT ["dotnet", "KyTucXaManagement.dll"]
