FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ParParWebsite.Api.csproj", "./"]
RUN dotnet restore "ParParWebsite.Api.csproj"

COPY . .
RUN dotnet publish "ParParWebsite.Api.csproj" -c Release -o /app/publish

# -------------------------
# FINAL IMAGE
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Install EF Core CLI in container
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Add entrypoint script
COPY src/entrypoint.sh .
RUN chmod +x /app/entrypoint.sh

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["/app/entrypoint.sh"]
