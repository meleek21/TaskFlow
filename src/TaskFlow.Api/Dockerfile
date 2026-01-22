# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TaskFlow.sln ./
COPY src/TaskFlow.Domain/*.csproj ./src/TaskFlow.Domain/
COPY src/TaskFlow.Application/*.csproj ./src/TaskFlow.Application/
COPY src/TaskFlow.Infrastructure/*.csproj ./src/TaskFlow.Infrastructure/
COPY src/TaskFlow.Api/*.csproj ./src/TaskFlow.Api/
COPY src/TaskFlow.Worker/*.csproj ./src/TaskFlow.Worker/

# Restore packages
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish API
RUN dotnet publish src/TaskFlow.Api/TaskFlow.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TaskFlow.Api.dll"]
