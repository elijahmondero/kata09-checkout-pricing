Feature: Product
  To manage products in the checkout system

  Scenario: Add a new product
    Given I add the following product:
      | Id  | Name   | UnitPrice |
      | P01 | Apple  | 0.60      |
    Then the product "P01" should exist

  Scenario: Update an existing product
    Given I add the following product:
      | Id  | Name   | UnitPrice |
      | P02 | Apple  | 0.60      |
    When I update the product with:
      | Id  | Name   | UnitPrice |
      | P02 | Orange | 0.70      |
    Then the product "P02" should have the name "Orange" and the unit price 0.70

  Scenario: Remove an existing product
    Given I add the following product:
      | Id  | Name   | UnitPrice |
      | P03 | Apple  | 0.60      |
    When I remove the product with id "P03"
    Then the product "P03" should not exist
