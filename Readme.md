# 🌟 GRHs Human Resources Management Solution

Welcome to GRHs, a comprehensive Human Resources Management solution designed to streamline and automate HR processes. This system helps manage employee information, holidays, work certificates, and more.

## 📚 Table of Contents
- [Features](#features)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## ✨ Features

- 📋 **Employee Management**: Easily manage employee records and personal information.
- 📆 **Holiday Management**: Track and approve employee holidays and leave requests.
- 📄 **Work Certificates**: Generate and manage employee work certificates.
- 🔐 **User Authentication**: Secure login and session management using JWT.
- 🗄️ **Data Management**: Efficiently handle employee data with SQL Server.

## 🚀 Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

Ensure you have the following software installed:

- [.NET Core](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio](https://visualstudio.microsoft.com/) (recommended)

### Installation

1. **Clone the repository**

    ```bash
    git clone https://github.com/mabchoor/GRHs.git
    cd GRHs
    ```

2. **Set up the database**

    - Update the connection string in `appsettings.json` with your SQL Server details.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=DESKTOP-ASRVTQG;Database=EmployeeManagementDB;Trusted_Connection=True;"
    }
    ```

    - Run the database migrations to set up the database schema.

    ```bash
    dotnet ef database update
    ```

3. **Run the application**

    ```bash
    dotnet run
    ```

## 💻 Usage

### Employee Management

- Add, update, and delete employee records.
- View detailed employee information.

### Holiday Management

- Manage holidays.
- Track employee leave days.

### Work Certificates

- Generate work certificates for employees.
- Manage certificate issuance records.

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📜 License

Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Contact

Mabchour Abderrahmane - [m.abderrahmane.dev@gmail.com](mailto:m.abderrahmane.dev@gmail.com)

Project Link: [https://github.com/your-username/GRHs](https://github.com/mabchoor/GRHs)
