#!/bin/sh

echo "Running EF Core migrations..."
dotnet ef database update

echo "Starting application..."
exec dotnet ParParWebsite.Api.dll
