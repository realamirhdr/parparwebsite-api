FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ParParWebsite.Api.csproj", "./"]
RUN dotnet restore "ParParWebsite.Api.csproj"
COPY . .
RUN dotnet publish "ParParWebsite.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ParParWebsite.Api.dll"]