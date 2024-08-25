Feature: Checkout
  To calculate the total price of items in the shopping cart

  Scenario: Calculate total price without special offers
    Given the following products exist:
      | Id  | Name  | UnitPrice |
      | A1  | Apple | 50        |
      | B1  | Banana | 30       |
      | C1  | Carrot | 20       |
    And the following pricing rules:
      | Item | UnitPrice |
      | A1   | 50        |
      | B1   | 30        |
      | C1   | 20        |
    When I start a new checkout session
    And I scan the following items:
      | Item |
      | A1   |
      | B1   |
      | C1   |
    Then the total price should be 100
    And I end the checkout session with payment details:
      | PaymentMethod | CardNumber | CardExpiry | CardCvc |
      | CreditCard    | 1234567890 | 12/23      | 123     |

  Scenario: Calculate total price with special offers
    Given the following products exist:
      | Id  | Name  | UnitPrice |
      | A2  | Apple | 50        |
      | B2  | Banana | 30       |
      | C2  | Carrot | 20       |
    And the following pricing rules:
      | Item | UnitPrice | SpecialQuantity | SpecialPrice |
      | A2   | 0        | 3               | 130          |
      | B2   | 0        | 2               | 45           |
      | C2   | 0        |                 |              |
    When I start a new checkout session
    And I scan the following items:
      | Item |
      | A2   |
      | A2   |
      | A2   |
      | A2   |
      | B2   |
      | B2   |
    Then the total price should be 225
    And I end the checkout session with payment details:
      | PaymentMethod | CardNumber | CardExpiry | CardCvc |
      | CreditCard    | 1234567890 | 12/23      | 123     |

  Scenario: Calculate total price with a mix of special and regular prices
    Given the following products exist:
      | Id  | Name  | UnitPrice |
      | A3  | Apple | 50        |
      | B3  | Banana | 30       |
      | C3  | Carrot | 20       |
    And the following pricing rules:
      | Item | UnitPrice | SpecialQuantity | SpecialPrice |
      | A3   | 50        | 3               | 130          |
      | B3   | 30        | 2               | 45           |
      | C3   | 20        |                 |              |
    When I start a new checkout session
    And I scan the following items:
      | Item |
      | A3   |
      | A3   |
      | A3   |
      | A3   |
      | B3   |
      | B3   |
      | C3   |
    Then the total price should be 245
    And I end the checkout session with payment details:
      | PaymentMethod | CardNumber | CardExpiry | CardCvc |
      | CreditCard    | 1234567890 | 12/23      | 123     |

  Scenario: Incremental scanning of items
    Given the following products exist:
      | Id | Name  | UnitPrice |
      | A  | Apple | 50        |
      | B  | Banana | 30       |
      | C  | Carrot | 20       |
    And the following pricing rules:
      | Item | UnitPrice | SpecialQuantity | SpecialPrice |
      | A    | 50        | 3               | 130          |
      | B    | 30        | 2               | 45           |
      | C    | 20        |                 |              |
    When I start a new checkout session
    And I scan the item "A"
    Then the total price should be 50
    When I scan the item "B"
    Then the total price should be 80
    When I scan the item "A"
    Then the total price should be 130
    When I scan the item "A"
    Then the total price should be 160
    When I scan the item "B"
    Then the total price should be 175
    When I scan the item "C"
    Then the total price should be 195
    And I end the checkout session with payment details:
      | PaymentMethod | CardNumber | CardExpiry | CardCvc |
      | CreditCard    | 1234567890 | 12/23      | 123     |