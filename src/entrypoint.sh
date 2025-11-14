#!/bin/sh

echo "Showing the current dir..."
pwd

echo "Showing the content of the dir..."
ls -l

echo "Running EF Core migrations..."
dotnet ef database update

echo "Starting application..."
exec dotnet ParParWebsite.Api.dll
