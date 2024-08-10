# 🛒 Checkout Pricing Kata

Welcome to the **Checkout Pricing Kata**! This project is designed to solve a common problem in e-commerce platforms: calculating the total price of items in a shopping cart, including handling special offers and discounts.

## 📖 The Kata

The task is to develop a system that can calculate the total price for a list of scanned items, considering various pricing rules, including unit prices and special offers like "3 for the price of 2" or "Buy one get one free".

## 🛠️ The Solution

This project is structured with two main components:

1. **CheckoutPricing.Api** (`CheckoutPricing.Api.csproj`)  
   A .NET 8 Web API that manages the checkout process. It handles the following endpoints:
   - `POST /checkout/pricingrules` to set the pricing rules.
   - `POST /checkout/scan/{item}` to scan an item.
   - `GET /checkout/total` to calculate and return the total price based on the scanned items.

2. **CheckoutPricing.Api.Tests** (`CheckoutPricing.Api.Tests.csproj`)  
   A .NET 8 SpecFlow test project using xUnit. It leverages `TestServer` to perform integration testing against the API. The tests are written in Gherkin syntax and ensure the correctness of the checkout logic.

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio or your preferred C# IDE

### Running the API

1. Navigate to the `CheckoutPricing.Api` project.
2. Run the API using your IDE or the command line:
   ```bash
   dotnet run --project CheckoutPricing.Api.csproj
