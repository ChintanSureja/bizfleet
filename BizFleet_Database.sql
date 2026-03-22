-- ============================================================
-- PROJECT   : BizFleet — B2B Bus Fleet Management Portal
-- FILE      : BizFleet_Database.sql
-- PURPOSE   : Creates the full database with all tables,
--             sample data, and stored procedures
-- AUTHOR    : [ Your Name ]
-- DATE      : 2026
-- ============================================================

-- ── STEP 1: Create the Database ──────────────────────────────
CREATE DATABASE BizFleetDB;
GO

-- Switch to our new database
USE BizFleetDB;
GO

-- ============================================================
-- TABLE 1: Users
-- Stores both Admin and Business User accounts
-- ============================================================
CREATE TABLE Users (
    UserId       INT PRIMARY KEY IDENTITY(1,1),  -- Auto-increment ID
    CompanyName  VARCHAR(100) NOT NULL,           -- Business company name
    FullName     VARCHAR(100) NOT NULL,           -- Contact person name
    Email        VARCHAR(100) NOT NULL UNIQUE,    -- Login email (must be unique)
    Password     VARCHAR(255) NOT NULL,           -- Hashed password
    Phone        VARCHAR(20),                     -- Contact number
    Address      VARCHAR(255),                    -- Company address
    Role         VARCHAR(20) NOT NULL             -- 'Admin' or 'User'
                 DEFAULT 'User',
    IsActive     BIT NOT NULL DEFAULT 1,          -- 1=Active, 0=Disabled
    CreatedAt    DATETIME NOT NULL                -- When account was created
                 DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE 2: Buses
-- Stores all bus details in the fleet
-- ============================================================
CREATE TABLE Buses (
    BusId        INT PRIMARY KEY IDENTITY(1,1),  -- Auto-increment ID
    BusNumber    VARCHAR(20) NOT NULL UNIQUE,     -- Unique bus plate number
    BusType      VARCHAR(50) NOT NULL,            -- e.g. Mini, Standard, Luxury
    Capacity     INT NOT NULL,                    -- Number of seats
    Model        VARCHAR(100),                    -- Bus model name
    Year         INT,                             -- Manufacturing year
    Status       VARCHAR(30) NOT NULL             -- Available/On Trip/Maintenance
                 DEFAULT 'Available',
    ImageUrl     VARCHAR(255),                    -- Optional bus image path
    CreatedAt    DATETIME NOT NULL
                 DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE 3: Bookings
-- Stores all trip booking requests from businesses
-- ============================================================
CREATE TABLE Bookings (
    BookingId      INT PRIMARY KEY IDENTITY(1,1), -- Auto-increment ID
    UserId         INT NOT NULL,                   -- Which company made booking
    BusId          INT,                            -- Assigned bus (nullable until approved)
    FromLocation   VARCHAR(100) NOT NULL,          -- Pickup location
    ToLocation     VARCHAR(100) NOT NULL,          -- Drop location
    TripDate       DATE NOT NULL,                  -- Date of trip
    ReturnDate     DATE,                           -- Return date (optional)
    NoOfBuses      INT NOT NULL DEFAULT 1,         -- How many buses needed
    NoOfPassengers INT,                            -- Total passengers
    Purpose        VARCHAR(255),                   -- Reason for trip
    Status         VARCHAR(20) NOT NULL            -- Pending/Approved/Rejected
                   DEFAULT 'Pending',
    AdminNotes     VARCHAR(500),                   -- Admin comments on booking
    CreatedAt      DATETIME NOT NULL
                   DEFAULT GETDATE(),

    -- Link to Users table
    CONSTRAINT FK_Bookings_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId),

    -- Link to Buses table
    CONSTRAINT FK_Bookings_Buses
        FOREIGN KEY (BusId) REFERENCES Buses(BusId)
);
GO

-- ============================================================
-- TABLE 4: Reports (Monthly Summary Cache)
-- Stores generated monthly reports for quick access
-- ============================================================
CREATE TABLE Reports (
    ReportId     INT PRIMARY KEY IDENTITY(1,1),
    ReportMonth  INT NOT NULL,                    -- Month number (1-12)
    ReportYear   INT NOT NULL,                    -- Year
    CompanyId    INT,                             -- NULL = all companies
    TotalTrips   INT DEFAULT 0,                   -- Total bookings that month
    TotalBuses   INT DEFAULT 0,                   -- Total buses used
    ApprovedTrips INT DEFAULT 0,                  -- Approved bookings count
    RejectedTrips INT DEFAULT 0,                  -- Rejected bookings count
    GeneratedAt  DATETIME DEFAULT GETDATE(),      -- When report was created

    CONSTRAINT FK_Reports_Users
        FOREIGN KEY (CompanyId) REFERENCES Users(UserId)
);
GO

-- ============================================================
-- INDEXES
-- Speed up common search queries
-- ============================================================

-- Speed up login lookup by email
CREATE INDEX IX_Users_Email
    ON Users(Email);

-- Speed up booking searches by status
CREATE INDEX IX_Bookings_Status
    ON Bookings(Status);

-- Speed up booking searches by date
CREATE INDEX IX_Bookings_TripDate
    ON Bookings(TripDate);

-- Speed up fleet filter by bus status
CREATE INDEX IX_Buses_Status
    ON Buses(Status);
GO

-- ============================================================
-- SAMPLE DATA — Admin Account
-- Default admin login:
--   Email    : admin@bizfleet.com
--   Password : Admin@123 (store hashed in real app)
-- ============================================================
INSERT INTO Users (CompanyName, FullName, Email, Password, Phone, Role)
VALUES (
    'BizFleet Admin',
    'System Administrator',
    'admin@bizfleet.com',
    'Admin@123',        -- ⚠️ In real app, hash this password!
    '9999999999',
    'Admin'
);
GO

-- ============================================================
-- SAMPLE DATA — Business Users
-- ============================================================
INSERT INTO Users (CompanyName, FullName, Email, Password, Phone, Address, Role)
VALUES
    ('ABC Corporates',   'Raj Sharma',   'raj@abc.com',   'Pass@123', '9876543210', 'Mumbai, India',   'User'),
    ('XYZ Industries',   'Priya Patel',  'priya@xyz.com', 'Pass@123', '9876543211', 'Delhi, India',    'User'),
    ('Global Traders',   'Amit Singh',   'amit@gt.com',   'Pass@123', '9876543212', 'Bangalore, India','User');
GO

-- ============================================================
-- SAMPLE DATA — Bus Fleet
-- ============================================================
INSERT INTO Buses (BusNumber, BusType, Capacity, Model, Year, Status)
VALUES
    ('MH-01-AB-1234', 'Standard',  40, 'Tata Starbus',   2022, 'Available'),
    ('MH-01-AB-1235', 'Luxury',    30, 'Volvo 9400',     2023, 'Available'),
    ('MH-01-AB-1236', 'Mini',      20, 'Force Traveller', 2021, 'On Trip'),
    ('MH-01-AB-1237', 'Standard',  40, 'Ashok Leyland',  2022, 'Available'),
    ('MH-01-AB-1238', 'Luxury',    35, 'Volvo 9600',     2024, 'Maintenance'),
    ('DL-02-CD-5678', 'Standard',  45, 'Tata Starbus',   2023, 'Available'),
    ('DL-02-CD-5679', 'Mini',      15, 'Mahindra Supro', 2022, 'Available');
GO

-- ============================================================
-- SAMPLE DATA — Bookings
-- ============================================================
INSERT INTO Bookings (UserId, FromLocation, ToLocation, TripDate, NoOfBuses, NoOfPassengers, Purpose, Status)
VALUES
    (2, 'Mumbai',    'Pune',      '2026-04-10', 2, 80,  'Corporate Offsite',    'Approved'),
    (3, 'Delhi',     'Agra',      '2026-04-15', 1, 30,  'Client Visit',         'Pending'),
    (4, 'Bangalore', 'Mysore',    '2026-04-20', 3, 120, 'Team Building Event',  'Pending'),
    (2, 'Mumbai',    'Nashik',    '2026-03-05', 1, 40,  'Annual Picnic',        'Approved'),
    (3, 'Delhi',     'Jaipur',    '2026-03-12', 2, 60,  'Trade Fair',           'Rejected');
GO

-- ============================================================
-- USEFUL QUERIES — Use these in your C# DAL (Data Access Layer)
-- ============================================================

-- 1. Get all available buses
-- SELECT * FROM Buses WHERE Status = 'Available'

-- 2. Get all pending bookings with company name
-- SELECT b.BookingId, u.CompanyName, b.FromLocation, b.ToLocation,
--        b.TripDate, b.NoOfBuses, b.Status
-- FROM Bookings b
-- INNER JOIN Users u ON b.UserId = u.UserId
-- WHERE b.Status = 'Pending'
-- ORDER BY b.CreatedAt DESC

-- 3. Get dashboard summary counts
-- SELECT
--   (SELECT COUNT(*) FROM Buses)                          AS TotalBuses,
--   (SELECT COUNT(*) FROM Buses WHERE Status='Available') AS AvailableBuses,
--   (SELECT COUNT(*) FROM Bookings WHERE Status='Pending') AS PendingBookings,
--   (SELECT COUNT(*) FROM Bookings WHERE Status='Approved') AS ApprovedBookings,
--   (SELECT COUNT(*) FROM Users WHERE Role='User')        AS TotalCompanies

-- 4. Search buses by type or status
-- SELECT * FROM Buses
-- WHERE BusType LIKE '%Standard%' AND Status = 'Available'

-- 5. Monthly report query
-- SELECT
--   MONTH(TripDate) AS Month,
--   YEAR(TripDate)  AS Year,
--   COUNT(*)        AS TotalBookings,
--   SUM(NoOfBuses)  AS TotalBusesUsed
-- FROM Bookings
-- WHERE Status = 'Approved'
-- GROUP BY YEAR(TripDate), MONTH(TripDate)
-- ORDER BY Year DESC, Month DESC

-- ============================================================
-- END OF SCRIPT
-- ============================================================
