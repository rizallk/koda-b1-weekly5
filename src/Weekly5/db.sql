-- CREATE DATABASE ewallet;

DROP TABLE dbo.Transactions;
DROP TABLE dbo.Wallets;
DROP TABLE dbo.Users;

CREATE TABLE dbo.Users (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(100) NOT NULL,
  Email NVARCHAR(80) NOT NULL UNIQUE,
  Phone NVARCHAR(20)
);

CREATE TABLE dbo.Wallets (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  UserId INT NOT NULL,
  WalletNumber NVARCHAR(20) NOT NULL UNIQUE,
  Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
  
  CONSTRAINT CK_Wallets_Balance CHECK(Balance >= 0),
  CONSTRAINT FK_Wallets_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
);

CREATE TABLE dbo.Transactions (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  SenderWalletId INT DEFAULT NULL,
  ReceiverWalletId INT NOT NULL,
  Type NVARCHAR(30) NOT NULL,
  Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
  Description NVARCHAR(100),
  CreatedAt DATETIME2 DEFAULT GETDATE(),

  CONSTRAINT CK_Transactions_Amount CHECK(Amount >= 0),
  CONSTRAINT FK_Transactions_Wallets_Sender FOREIGN KEY (SenderWalletId) REFERENCES dbo.Wallets(Id),
  CONSTRAINT FK_Transactions_Wallets_Receiver FOREIGN KEY (ReceiverWalletId) REFERENCES dbo.Wallets(Id)
);
GO

-- Insert data dummy
INSERT INTO dbo.Users (Name, Email, Phone) VALUES 
(N'Budi Santoso', N'budi.santoso@example.com', N'081234567890'),
(N'Siti Rahma', N'siti.rahma@example.com', N'082345678901'),
(N'Andi Wijaya', N'andi.wijaya@example.com', N'083456789012');

INSERT INTO dbo.Wallets (UserId, WalletNumber, Balance) VALUES 
(1, N'W001', 1500000.00),
(2, N'W002', 2500000.50),
(3, N'W003', 500000.00);

INSERT INTO dbo.Transactions (SenderWalletId, ReceiverWalletId, Type, Amount, Description, CreatedAt) 
VALUES 
(NULL, 1, N'topup',   1500000.00, N'Top Up via Mobile Banking', '2026-03-01 07:30:00'),
(NULL, 1, N'income',   500000.00, N'Gaji Freelance',            '2026-03-01 08:00:00'),
(NULL, 2, N'topup',   1800000.50, N'Top Up via Virtual Account','2026-03-01 08:30:00'),
(NULL, 3, N'topup',    600000.00, N'Top Up via Minimarket',     '2026-03-01 09:00:00'),
(1,    2, N'expense',  500000.00, N'Transfer ke Siti Rahma',    '2026-03-01 10:15:00'),
(3,    2, N'expense',  200000.00, N'Pembayaran Jasa ke Siti',   '2026-03-01 11:30:00'),
(NULL, 3, N'income',   100000.00, N'Bonus Cashback Promo',      '2026-03-01 12:00:00');
GO


SELECT * FROM dbo.Users;
SELECT * FROM dbo.Wallets;
SELECT * FROM dbo.Transactions;

-- DELETE FROM dbo.Transactions;
-- DELETE FROM dbo.Wallets;
-- DELETE FROM dbo.Users;