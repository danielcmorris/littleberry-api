# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY LittleberryApi/LittleberryApi.csproj LittleberryApi/
RUN dotnet restore LittleberryApi/LittleberryApi.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/LittleberryApi
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final stage - using Noble (Ubuntu 24.04) and configuring OpenSSL to support legacy TLS
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble AS final
WORKDIR /app

# Configure OpenSSL for maximum compatibility with older SQL Servers
# Setting SECLEVEL=0 allows SSLv3 and weak ciphers needed for some older SQL Servers
RUN sed -i 's/openssl_conf = openssl_init/# openssl_conf = openssl_init\nopenssl_conf = openssl_init_custom/' /etc/ssl/openssl.cnf && \
    echo "" >> /etc/ssl/openssl.cnf && \
    echo "[openssl_init_custom]" >> /etc/ssl/openssl.cnf && \
    echo "ssl_conf = ssl_sect" >> /etc/ssl/openssl.cnf && \
    echo "" >> /etc/ssl/openssl.cnf && \
    echo "[ssl_sect]" >> /etc/ssl/openssl.cnf && \
    echo "system_default = system_default_sect" >> /etc/ssl/openssl.cnf && \
    echo "" >> /etc/ssl/openssl.cnf && \
    echo "[system_default_sect]" >> /etc/ssl/openssl.cnf && \
    echo "MinProtocol = TLSv1" >> /etc/ssl/openssl.cnf && \
    echo "CipherString = DEFAULT:@SECLEVEL=0" >> /etc/ssl/openssl.cnf

# GCP Cloud Run expects the app to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published files
COPY --from=publish /app/publish .

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080

ENTRYPOINT ["dotnet", "LittleberryApi.dll"]
