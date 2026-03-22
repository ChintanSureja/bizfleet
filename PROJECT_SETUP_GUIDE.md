# ============================================================
# BizFleet — B2B Bus Fleet Management Portal
# PROJECT SETUP GUIDE
# ============================================================

## REQUIREMENTS (Install these first)
- Visual Studio 2022 (Community Edition — Free)
  Download: https://visualstudio.microsoft.com/
  During install select: ASP.NET and web development workload

- SQL Server 2019/2022 (Developer Edition — Free)
  Download: https://www.microsoft.com/en-us/sql-server/sql-server-downloads

- SQL Server Management Studio (SSMS — Free)
  Download: https://aka.ms/ssmsfullsetup

- Git (for GitHub)
  Download: https://git-scm.com/

## ============================================================
## STEP 1 — CREATE PROJECT IN VISUAL STUDIO
## ============================================================

1. Open Visual Studio 2022
2. Click "Create a new project"
3. Search for "ASP.NET Web Application (.NET Framework)"
4. Click Next
5. Fill in:
   - Project Name  : BizFleet
   - Location      : C:\Projects\
   - Framework     : .NET Framework 4.8
6. Click Create
7. Select "MVC" template
8. Click Create

## ============================================================
## STEP 2 — FOLDER STRUCTURE
## ============================================================
## Visual Studio creates this automatically,
## but here is what your final structure will look like:

BizFleet/
│
├── App_Start/
│   ├── RouteConfig.cs          ← URL routing rules
│   └── FilterConfig.cs         ← Global filters
│
├── Controllers/                ← C# logic files
│   ├── HomeController.cs       ← Landing page
│   ├── AccountController.cs    ← Login / Register
│   ├── AdminController.cs      ← Admin dashboard
│   ├── FleetController.cs      ← Bus management
│   ├── BookingController.cs    ← Booking management
│   └── ReportController.cs     ← Reports & export
│
├── Models/                     ← C# data classes
│   ├── User.cs                 ← User model
│   ├── Bus.cs                  ← Bus model
│   ├── Booking.cs              ← Booking model
│   └── DashboardViewModel.cs   ← Dashboard data
│
├── Views/                      ← HTML pages (.cshtml)
│   ├── Home/
│   │   └── Index.cshtml        ← Landing page
│   ├── Account/
│   │   ├── Login.cshtml        ← Login page
│   │   └── Register.cshtml     ← Register page
│   ├── Admin/
│   │   └── Dashboard.cshtml    ← Admin dashboard
│   ├── Fleet/
│   │   ├── Index.cshtml        ← Bus list
│   │   ├── Create.cshtml       ← Add bus form
│   │   └── Edit.cshtml         ← Edit bus form
│   ├── Booking/
│   │   ├── Index.cshtml        ← Booking list
│   │   ├── Create.cshtml       ← New booking form
│   │   └── Manage.cshtml       ← Admin manage bookings
│   ├── Report/
│   │   └── Index.cshtml        ← Reports page
│   └── Shared/
│       ├── _Layout.cshtml      ← Master page (navbar etc.)
│       └── _LoginPartial.cshtml← Login status in navbar
│
├── Content/                    ← CSS files
│   └── bizfleet.css            ← Our custom styles
│
├── Scripts/                    ← JavaScript files
│   └── bizfleet.js             ← Our custom scripts
│
├── DAL/                        ← Database Access Layer
│   └── DatabaseHelper.cs       ← All SQL query methods
│
├── Web.config                  ← DB connection string goes here
└── Global.asax                 ← App startup file

## ============================================================
## STEP 3 — SET UP DATABASE CONNECTION
## ============================================================

## After running BizFleet_Database.sql in SSMS,
## open Web.config and find <connectionStrings>
## Replace it with this:

## <connectionStrings>
##   <add name="BizFleetDB"
##        connectionString="Data Source=.\SQLEXPRESS;
##                          Initial Catalog=BizFleetDB;
##                          Integrated Security=True"
##        providerName="System.Data.SqlClient" />
## </connectionStrings>

## NOTE: If your SQL Server instance name is different,
## replace .\SQLEXPRESS with your instance name
## You can find it in SSMS when you connect

## ============================================================
## STEP 4 — ADD REQUIRED PACKAGES (via NuGet)
## ============================================================

## In Visual Studio:
## Tools → NuGet Package Manager → Package Manager Console
## Run these commands one by one:

## Install-Package jQuery
## Install-Package Bootstrap
## Install-Package FontAwesome

## ============================================================
## STEP 5 — GITHUB SETUP
## ============================================================

## Open Git Bash or terminal in your project folder:

## git init
## git add .
## git commit -m "Initial commit — BizFleet project setup"
## git branch -M main
## git remote add origin https://github.com/YOURUSERNAME/bizfleet.git
## git push -u origin main

## ============================================================
## DEFAULT LOGIN CREDENTIALS (for testing)
## ============================================================

## Admin Login:
##   Email    : admin@bizfleet.com
##   Password : Admin@123

## Test User Login:
##   Email    : raj@abc.com
##   Password : Pass@123

## ============================================================
## NEXT FILES TO CREATE (in order):
## ============================================================
## 1. Web.config              ← connection string
## 2. DAL/DatabaseHelper.cs   ← all database methods
## 3. Models/*.cs             ← data classes
## 4. Controllers/*.cs        ← page logic
## 5. Views/**/*.cshtml       ← HTML pages
## 6. Content/bizfleet.css    ← styles
## 7. Scripts/bizfleet.js     ← scripts
