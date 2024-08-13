CREATE TABLE Products (
    Id VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE PricingRules (
    ProductId VARCHAR(255) NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    SpecialQuantity INT,
    SpecialPrice DECIMAL(18, 2),
    PRIMARY KEY (ProductId),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE CheckoutSessions (
    SessionId VARCHAR(36) PRIMARY KEY,
    Status VARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    EndedAt DATETIME NULL,
    TotalAmount DECIMAL(10, 2) NULL
);

CREATE TABLE CheckoutItems (
    SessionId VARCHAR(36) NOT NULL,
    ProductId VARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    PRIMARY KEY (SessionId, ProductId),
    FOREIGN KEY (SessionId) REFERENCES CheckoutSessions(SessionId),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);