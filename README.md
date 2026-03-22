# 🚌 BizFleet — B2B Bus Fleet Management Portal

![ASP.NET MVC](https://img.shields.io/badge/ASP.NET-MVC-blue?style=flat-square)
![C#](https://img.shields.io/badge/C%23-.NET%20Framework-purple?style=flat-square)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red?style=flat-square)
![jQuery](https://img.shields.io/badge/Frontend-jQuery-yellow?style=flat-square)
![Status](https://img.shields.io/badge/Status-Active-green?style=flat-square)

A full-stack **B2B web portal** for managing bus fleets, enabling businesses to register, submit booking inquiries, and track fleet status — all managed through a powerful Admin dashboard.

---

## ✨ Features

### 👤 Business Users Can:
- Register company account and login securely
- Submit bus booking/trip requests via web forms
- Track booking status (Pending / Approved / Rejected)
- View available fleet

### 🔐 Admin Can:
- View real-time dashboard with fleet & booking stats
- Add / Edit / Delete buses from fleet
- Approve or reject booking requests with notes
- Search & filter buses by type and status
- Generate monthly reports and export to CSV

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | C#, ASP.NET MVC (.NET Framework 4.8) |
| Frontend | HTML5, CSS3, JavaScript, jQuery |
| Database | SQL Server (plain ADO.NET — no ORM) |
| Auth | Session-based (ASP.NET Session) |
| UI | Custom CSS — Glassmorphism + 3D effects |
| Tools | Visual Studio 2022, SSMS, Git |

---

## 📁 Project Structure

```
BizFleet/
├── Controllers/
│   ├── AccountController.cs    ← Login / Register
│   ├── AdminController.cs      ← Admin Dashboard
│   ├── FleetController.cs      ← Bus CRUD
│   ├── BookingController.cs    ← Booking Management
│   └── ReportController.cs     ← Reports + CSV Export
├── Models/
│   ├── User.cs
│   ├── Bus.cs
│   ├── Booking.cs
│   └── DashboardViewModel.cs
├── DAL/
│   └── DatabaseHelper.cs       ← All SQL queries
├── Views/
│   ├── Account/Login + Register
│   ├── Admin/Dashboard
│   ├── Fleet/Index + Create + Edit
│   ├── Booking/Index + Create
│   └── Report/Index
├── Content/bizfleet.css        ← Modern 3D UI styles
├── Scripts/bizfleet.js         ← jQuery interactions
└── SQL/BizFleet_Database.sql   ← Full DB setup script
```

---

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 (with ASP.NET workload)
- SQL Server (Developer or Express edition)
- SQL Server Management Studio (SSMS)

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/yourusername/bizfleet.git
cd bizfleet
```

**2. Set up the database**
- Open SSMS
- Open `SQL/BizFleet_Database.sql`
- Run the entire script (F5)
- This creates the database, tables, and sample data

**3. Configure connection string**

Open `Web.config` and update:
```xml
<connectionStrings>
  <add name="BizFleetDB"
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=BizFleetDB;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**4. Run the project**
- Open `BizFleet.sln` in Visual Studio
- Press `Ctrl + F5` to run without debugging
- Browser opens automatically at `localhost:PORT`

---

## 🔐 Default Login Credentials

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@bizfleet.com | Admin@123 |
| **Business User** | raj@abc.com | Pass@123 |
| **Business User** | priya@xyz.com | Pass@123 |

---

## 📸 Screenshots

> _Add screenshots of your app here after running it!_
> Tools → Snipping Tool → capture each page

| Page | Description |
|------|-------------|
| Login | Modern glassmorphism login screen |
| Dashboard | Real-time stats with animated counters |
| Fleet | Bus cards with 3D hover effect |
| Bookings | Status management table |
| Reports | Monthly data + CSV export |

---

## 🗄️ Database Schema

```
Users     → UserId, CompanyName, FullName, Email, Password, Role
Buses     → BusId, BusNumber, BusType, Capacity, Model, Year, Status
Bookings  → BookingId, UserId, BusId, FromLocation, ToLocation, TripDate, Status
Reports   → ReportId, Month, Year, CompanyId, TotalTrips
```

---

## 🎯 Key Technical Highlights

- **Role-based access control** — Admin vs Business User pages
- **SQL injection prevention** — all queries use parameterized inputs
- **jQuery client-side validation** — real-time form feedback
- **Dynamic SQL filtering** — search/filter without hardcoded queries
- **CSV export** — pure C# byte stream, no third-party libraries
- **Glassmorphism UI** — modern frosted glass design with 3D card tilt
- **Responsive layout** — works on desktop and mobile

---

## 👨‍💻 Author

**[ Your Name ]**
- GitHub: [@yourusername](https://github.com/yourusername)
- LinkedIn: [linkedin.com/in/yourname](https://linkedin.com/in/yourname)
- Email: your.email@gmail.com

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

⭐ **If you found this project helpful, please give it a star!**
