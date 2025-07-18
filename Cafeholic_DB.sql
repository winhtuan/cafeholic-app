create database Cafeholic
use Cafeholic
go

CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(255)
);

CREATE TABLE Role (
    Id INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(100)
);

CREATE TABLE Account (
    AccID INT PRIMARY KEY IDENTITY,
    PhoneNumber NVARCHAR(20),
    PasswordHash NVARCHAR(255),
    RegistDate DATETIME,
    VerificationToken NVARCHAR(255),
    IsVerified BIT,
    RoleId INT,
    UserId INT,
    FOREIGN KEY (RoleId) REFERENCES Role(Id),
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

CREATE TABLE PasswordResetToken (
    Id INT PRIMARY KEY IDENTITY,
    AccountId INT,
    Token NVARCHAR(255),
    ExpiryDate DATETIME,
    FOREIGN KEY (AccountId) REFERENCES Account(AccID)
);

CREATE TABLE Drink (
    DrinkId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Description NVARCHAR(500),
    Price DECIMAL(10, 2),
    IsAvailable BIT,
	img VARCHAR(255)
);

CREATE TABLE RoomType (
    TypeId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    MinCapacity INT,
    MaxCapacity INT,
    Description NVARCHAR(500)
);

CREATE TABLE StudyRoom (
    RoomId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    IsAvailable BIT,
    RoomTypeId INT,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomType(TypeId)
);

CREATE TABLE Reservation (
    ReservationId INT PRIMARY KEY IDENTITY,
    UserId INT,
    RoomId INT,
    StartTime DATETIME,
    EndTime DATETIME,
    Status NVARCHAR(50),
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (RoomId) REFERENCES StudyRoom(RoomId)
);

CREATE TABLE Voucher (
    VoucherId INT PRIMARY KEY IDENTITY,
    VoucherCode NVARCHAR(50),
    Description NVARCHAR(255),
    DiscountPercent DECIMAL(5, 2),
    ExpiryDate DATETIME,
    IsActive BIT
);

CREATE TABLE [Order] (
    OrderId INT PRIMARY KEY IDENTITY,
    UserId INT,
    OrderDate DATETIME,
    TotalAmount DECIMAL(10, 2),
    Status NVARCHAR(50),
    VoucherId INT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (VoucherId) REFERENCES Voucher(VoucherId)
);

CREATE TABLE OrderItem (
    OrderItemId INT PRIMARY KEY IDENTITY,
    OrderId INT,
    DrinkId INT,
    Quantity INT,
    Price DECIMAL(10, 2),
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
    FOREIGN KEY (DrinkId) REFERENCES Drink(DrinkId)
);

CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY IDENTITY,
    OrderId INT,
    TransactionDate DATETIME,
    AmountPaid DECIMAL(10, 2),
    PaymentMethod NVARCHAR(50),
    Status NVARCHAR(50),
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId)
);

CREATE TABLE Payment (
    PaymentId INT PRIMARY KEY IDENTITY,
    TransactionId INT,
    PaymentType NVARCHAR(50),
    PaymentDate DATETIME,
    Amount DECIMAL(10, 2),
    FOREIGN KEY (TransactionId) REFERENCES Transactions(TransactionId)
);

CREATE TABLE Menu (
    MenuId INT PRIMARY KEY IDENTITY,
    DrinkId INT,
    Name NVARCHAR(100),
    FOREIGN KEY (DrinkId) REFERENCES Drink(DrinkId)
);

CREATE TABLE Review (
    ReviewId INT PRIMARY KEY IDENTITY,
    UserId INT,
    DrinkId INT,
    Rating INT,
    Comment NVARCHAR(500),
    ReviewDate DATETIME,
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (DrinkId) REFERENCES Drink(DrinkId)
);

CREATE TABLE Suggestion (
    SuggestionId INT PRIMARY KEY IDENTITY,
    UserId INT,
    Content NVARCHAR(500),
    SuggestionDate DATETIME,
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

CREATE TABLE SuggestionItem (
    SuggestionId INT,
    DrinkId INT,
    PRIMARY KEY (SuggestionId, DrinkId),
    FOREIGN KEY (SuggestionId) REFERENCES Suggestion(SuggestionId),
    FOREIGN KEY (DrinkId) REFERENCES Drink(DrinkId)
);
