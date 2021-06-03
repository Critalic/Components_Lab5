FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-nanoserver-1709 AS dotnet_builder
WORKDIR /app/

# Copy csproj and restore as distinct layers
COPY ./Producer/*.csproj ./Producer/
COPY ./Lab.Shared/*.csproj ./Lab.Shared/


RUN dotnet restore ./Producer/Producer.csproj

# Copy everything else and build
COPY  ./Producer ./Producer/
COPY ./Lab.Shared ./Lab.Shared/


RUN dotnet publish ./Producer/Producer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0


WORKDIR /app

RUN mkdir ./out/
COPY . ./out


ENTRYPOINT ["dotnet", "Producer.dll"]
