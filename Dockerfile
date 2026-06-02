FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY KyTucXaManagement/KyTucXaManagement.csproj KyTucXaManagement/
RUN dotnet restore KyTucXaManagement/KyTucXaManagement.csproj
COPY . .
RUN dotnet publish KyTucXaManagement/KyTucXaManagement.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# SQLite db sẽ lưu trong /data khi chạy trên Railway
RUN mkdir -p /data
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/data/KyTucXa.db"

EXPOSE 8080
ENTRYPOINT ["dotnet", "KyTucXaManagement.dll"]
