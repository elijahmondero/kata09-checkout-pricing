Feature: Checkout
  To calculate the total price of items in the shopping cart
  [Calculator](CheckoutPricing.Api.Tests/Features/Checkout.feature)
  **[Checkout Feature](http://codekata.com/kata/kata09-back-to-the-checkout/)**

  Scenario: Calculate total price without special offers
    Given the following pricing rules:
      | Item | UnitPrice |
      | A    | 50        |
      | B    | 30        |
      | C    | 20        |
      | D    | 15        |
    When I scan the following items:
      | Item |
      | A    |
      | B    |
      | C    |
    Then the total price should be 100

  Scenario: Calculate total price with special offers
    Given the following pricing rules:
      | Item | UnitPrice | SpecialQuantity | SpecialPrice |
      | A    | 50        | 3               | 130          |
      | B    | 30        | 2               | 45           |
      | C    | 20        |                 |              |
      | D    | 15        |                 |              |
    When I scan the following items:
      | Item |
      | A    |
      | A    |
      | A    |
      | B    |
      | B    |
    Then the total price should be 175

  Scenario: Calculate total price with a mix of special and regular prices
    Given the following pricing rules:
      | Item | UnitPrice | SpecialQuantity | SpecialPrice |
      | A    | 50        | 3               | 130          |
      | B    | 30        | 2               | 45           |
      | C    | 20        |                 |              |
      | D    | 15        |                 |              |
    When I scan the following items:
      | Item |
      | A    |
      | A    |
      | A    |
      | A    |
      | B    |
      | B    |
      | C    |
    Then the total price should be 245