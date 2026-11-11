# Weekly 5 - E-Wallet RESTful API (.NET)

Implementasi RESTful API sistem E-Wallet berbasis .NET dengan arsitektur MVC (Model-View-Controller)

---

## Struktur Project

```text
Weekly5/
├── Controllers/
│   ├── UserController.cs
│   ├── WalletController.cs
│   ├── TransferController.cs
|   ...
├── DTOs/
│   ├── UserRequest.cs
│   ├── WalletRequest.cs
│   ├── TransactionResponse.cs
|   ...
├── Models/
│   ├── Users.cs
│   ├── Wallets.cs
│   └── Transactions.cs
├── Repositories/
├── Services/
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── Program.cs
└── Weekly5.csproj
```

---

## Alur API dan Arsitektur MVC

Aplikasi menggunakan pola desain **MVC / Layered Architecture** dengan alur pemrosesan data sebagai berikut:

```text
[Client / Postman] 
       │
       ▼ (HTTP Request)
┌──────────────┐
│  Controller  │ ──── Menerima request HTTP, memvalidasi DTO, dan mengembalikan HTTP Response.
└──────┬───────┘
       │
       ▼
┌──────────────┐
│   Service    │ ──── Mengeksekusi aturan bisnis (misal: cek kecukupan saldo transfer).
└──────┬───────┘
       │
       ▼
┌──────────────┐
│  Repository  │ ──── Melakukan query & manipulasi data ke Database melalui Entity Framework/ORM.
└──────┬───────┘
       │
       ▼
┌──────────────┐
│    Models    │ ──── Memetakan entitas domain (User, Wallet, Transaction) beserta enkapsulasinya.
└──────────────┘
```

---

## Entity Relationship Diagram (ERD)

```mermaid
erDiagram
    USERS ||--o{ WALLETS : "memiliki"
    WALLETS ||--o{ TRANSACTIONS : "pengirim (sender)"
    WALLETS ||--o{ TRANSACTIONS : "penerima (receiver)"

    USERS {
        int Id PK
        string Name
        string Email
        string Phone
    }

    WALLETS {
        int Id PK
        int UserId FK
        string WalletNumber
        decimal Balance
    }

    TRANSACTIONS {
        int Id PK
        int SenderWalletId FK "nullable"
        int ReceiverWalletId FK
        string Type
        decimal Amount
        string Description
        datetime CreatedAt
    }
```

---

## Struktur Kelas Utama (Class Diagram)

Berikut diagram kelas berbasis model C# yang ada pada folder `Models/`:

```mermaid
classDiagram
  class User {
    +int Id
    +string Name
    +string Email
    +string Phone
    +User(id, name, email, phone)
    +User(name, email, phone)
  }

  class Wallet {
    +int Id
    +int UserId
    +string WalletNumber
    +decimal Balance
    +Wallet(id, userId, walletNumber, balance)
    +Wallet(userId, walletNumber, balance)
    +IncreaseBalance(amount: decimal) void
    +DecreaseBalance(amount: decimal) void
  }

  class Transaction {
    +int Id
    +int SenderWalletId
    +int ReceiverWalletId
    +string Type
    +decimal Amount
    +string Description
    +DateTime CreatedAt
    +Transaction(id, senderWalletId, receiverWalletId, type, amount, description, createdAt)
    +Transaction(senderWalletId, receiverWalletId, type, amount, description)
  }

  User "1" -- "0..*" Wallet : owns
  Wallet "1" -- "0..*" Transaction : sends/receives
```
---

## Cara Menjalankan Project

### Prasyarat
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- Database (SQL Server / PostgreSQL / SQLite sesuai konfigurasi `appsettings.json`)

### Langkah-langkah Running
4. Jalankan aplikasi:
   ```bash
   dotnet run
   ```

5. Akses Swagger UI di browser untuk pengujian interaktif:
   ```text
   https://localhost:5219/swagger
   ```

---

## Tampilan 
<p align="center">
  <img src="image1.png" alt="Testing History Transaksi" width="600"/>
</p>
<p align="center">
  <img src="image2.png" alt="Testing History Transaksi" width="600"/>
</p>
<p align="center">
  <img src="image3.png" alt="Testing History Transaksi" width="600"/>
</p>
<p align="center">
  <img src="image4.png" alt="Testing History Transaksi" width="600"/>
</p>
<p align="center">
  <img src="image5.png" alt="Testing History Transaksi" width="600"/>
</p>s