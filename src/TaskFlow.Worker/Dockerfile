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

# Build and publish Worker
RUN dotnet publish src/TaskFlow.Worker/TaskFlow.Worker.csproj -c Release -o /app/publish

# Runtime stage with Chrome dependencies for Puppeteer
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install Chrome dependencies for Puppeteer
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
    ca-certificates \
    fonts-liberation \
    fonts-noto-color-emoji \
    fonts-noto-cjk \
    fonts-noto-core \
    libasound2 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libatspi2.0-0 \
    libcups2 \
    libdbus-1-3 \
    libdrm2 \
    libgbm1 \
    libgtk-3-0 \
    libnspr4 \
    libnss3 \
    libxcomposite1 \
    libxdamage1 \
    libxfixes3 \
    libxkbcommon0 \
    libxrandr2 \
    xdg-utils \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskFlow.Worker.dll"]
