Feature: ProductList
  To return products in the checkout system

  Scenario: Get a paged list of products
    Given I add the following product:
      | Id  | Name   | UnitPrice |
      | P04 | Banana | 0.50      |
      | P05 | Grape  | 1.20      |
      | P06 | Mango  | 1.50      |
    When I request page 1 with page size 2
    Then the response should contain the following products:
      | Id  | Name   | UnitPrice |
      | P04 | Banana | 0.50      |
      | P05 | Grape  | 1.20      |

  Scenario: Search products by name
    Given I add the following product:
      | Id  | Name   | UnitPrice |
      | P07 | Pineapple | 2.00   |
      | P08 | Peach     | 1.80   |
      | P09 | Pear      | 1.00   |
    When I search for products with name containing "Pea"
    Then the response should contain the following products:
      | Id  | Name   | UnitPrice |
      | P08 | Peach     | 1.80   |
      | P09 | Pear      | 1.00   |