Feature: Reserve a book

  As a library member
  I want to reserve a book that is currently checked out
  So that I can borrow it when it becomes available

  Background:
    Given a user exists

  Scenario: User reserves a book with no available copies
    And a book exists with 0 available copies
    And the user has not already borrowed the book
    And the user has not already reserved the book
    When the user attempts to reserve the book
    Then the reservation should be successful
    And the user should be added to the reservation queue for the book

  Scenario Outline: User attempts to reserve a book
    And a book exists with <available_copies> available copies
    And the user has <loan_status> borrowed the book
    And the user has <reservation_status> reserved the book
    When the user attempts to reserve the book
    Then the result should be <expected_result>

    Examples:
      | available_copies | loan_status | reservation_status | expected_result |
      | 1                | not         | not                | BookAvailable   |
      | 0                | already     | not                | AlreadyBorrowed |
      | 0                | not         | already            | AlreadyReserved |