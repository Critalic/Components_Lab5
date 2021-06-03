FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-nanoserver-1709 AS dotnet_builder
WORKDIR /app/

# Copy csproj and restore as distinct layers
COPY ./Consumer/*.csproj ./Consumer/
COPY ./Lab.Shared/*.csproj ./Lab.Shared/

RUN dotnet restore ./Consumer/Consumer.csproj

# Copy everything else and build
COPY  ./Consumer ./Consumer/
COPY ./Lab.Shared ./Lab.Shared/

RUN dotnet publish ./Consumer/Consumer.csproj -c Release -o /publish/

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine3.13

COPY --from=dotnet_builder /publish/ /app/

WORKDIR /app/

ENTRYPOINT ["dotnet", "Consumer.dll"]
