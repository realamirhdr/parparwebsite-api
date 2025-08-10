# Use official .NET image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ParParWebsite.Api.csproj", "./"]
RUN dotnet restore "ParParWebsite.Api.csproj"
COPY . .
RUN dotnet publish "ParParWebsite.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ParParWebsite.Api.dll"]
