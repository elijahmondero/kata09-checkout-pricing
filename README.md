# 🛒 Checkout Pricing Kata

Welcome to the **Checkout Pricing Kata**! This project is designed to solve a common problem in e-commerce platforms: calculating the total price of items in a shopping cart, including handling special offers and discounts.

## 📖 The Kata

The task is to develop a system that can calculate the total price for a list of scanned items, considering various pricing rules, including unit prices and special offers like "3 for the price of 2" or "Buy one get one free".

## 🛠️ The Solution

This project is structured with two main components:

1. **CheckoutPricing.Api** (`CheckoutPricing.Api.csproj`)  
   A .NET 8 Web API that manages the checkout process.

2. **CheckoutPricing.Api.Tests** (`CheckoutPricing.Api.Tests.csproj`)  
   A .NET 8 SpecFlow test project using xUnit. It leverages `TestServer` to perform integration testing against the API. The tests are written in Gherkin syntax and ensure the correctness of the checkout logic.

3. **CheckoutPricing.Web**  
   A React application that serves as the UI for the checkout system. It is built using Node.js and served using Nginx. The Dockerfile for this component is structured in two stages:
   - **Stage 1:** Builds the React app using Node.js.
   - **Stage 2:** Serves the built React app using Nginx.

## 🎯 Main Features
- Simple Pricing Rules: Set unit prices for each item.
- Special Offers: Handle special offers like bulk discounts.
- Integration Testing: SpecFlow tests that validate the API's behavior through realistic scenarios.

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio or your preferred C# IDE

### Running the API

1. Navigate to the `CheckoutPricing.Api` project.
2. Run the API using your IDE or the command line:

```bash
dotnet run --project CheckoutPricing.Api.csproj
```

### Running the Tests
Navigate to the CheckoutPricing.Api.Tests project.
Execute the tests using the following command:

```bash
Copy code
dotnet test
```

This will run the SpecFlow tests, which will interact with the API through TestServer to validate the checkout functionality. The tests will also use Testcontainers to run a mysql:8.0 image as part of the tests.

### Running the API with Docker

1. Build the Docker image:

```bash
docker build -t kata09-checkout-pricing .
```

2. Run the Docker container:

```bash
docker run -d -p 5062:8080 --name checkout-pricing kata09-checkout-pricing
```

This will build and run the API in a Docker container, exposing it on port 80.

### Running the API with Docker Compose
1. Build and run the Docker Compose services:

```bash
docker-compose up --build -d
```
This will build and run the API, UI, and database services defined in the docker-compose.yml file. The API will be exposed on port 5062, the UI on port 3000, and the database on port 3306.



## 📚 API Endpoints

| **Controller**       | **Method** | **URL**                                 | **Description**                                | **Request Body**         |
|----------------------|------------|-----------------------------------------|------------------------------------------------|--------------------------|
| **ProductController**| `POST`     | `/product`                              | Adds a new product.                            | `Product` object         |
|                      | `PUT`      | `/product/{id}`                         | Updates an existing product.                   | `Product` object         |
|                      | `DELETE`   | `/product/{id}`                         | Removes a product.                             |                          |
|                      | `GET`      | `/product/{id}`                         | Retrieves a product by ID.                     |                          |
|                      | `GET`      | `/product`                              | Retrieves a paged list of products with optional search. |                          |
|                      |            |                                         |                                                |                          |
| **CheckoutController**| `POST`    | `/checkout/session/start`               | Starts a new checkout session.                 |                          |
|                      | `POST`     | `/checkout/session/end/{sessionId}`     | Ends an active checkout session.               | `PaymentDetails` object  |
|                      | `POST`     | `/checkout/session/cancel/{sessionId}`  | Cancels an active checkout session.            |                          |
|                      | `POST`     | `/checkout/scan/{sessionId}/{productId}`| Scans a product and adds it to the checkout session. |                          |
|                      | `GET`      | `/checkout/total/{sessionId}`           | Gets the total amount for the checkout session.|                          |
|                      | `POST`     | `/checkout/pricing/rules`               | Sets the pricing rules for products.           | List of `PricingRule` objects |
