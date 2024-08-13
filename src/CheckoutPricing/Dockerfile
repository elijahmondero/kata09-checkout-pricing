# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the project file and restore dependencies
COPY CheckoutPricing.Api/CheckoutPricing.Api.csproj CheckoutPricing.Api/
RUN dotnet restore CheckoutPricing.Api/CheckoutPricing.Api.csproj

# Copy the rest of the source code
COPY CheckoutPricing.Api/. CheckoutPricing.Api/

# Build the app
WORKDIR /source/CheckoutPricing.Api
RUN dotnet publish -c Release -o /app

# Use the official ASP.NET Core runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Expose the port the app runs on
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "CheckoutPricing.Api.dll"]
