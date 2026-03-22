// ============================================================
// FILE    : DAL/DatabaseHelper.cs
// PURPOSE : Database Access Layer — all SQL queries live here
//           Controllers call these methods to get/save data
//           Uses plain ADO.NET (no Entity Framework)
// ============================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using BizFleet.Models;

namespace BizFleet.DAL
{
    /// <summary>
    /// DatabaseHelper — central place for all database operations
    /// Every method opens a connection, runs a query, closes connection
    /// </summary>
    public class DatabaseHelper
    {
        // ── Get connection string from Web.config ──
        // Name must match what you set in Web.config
        private static string _connectionString =
            ConfigurationManager.ConnectionStrings["BizFleetDB"].ConnectionString;

        // ────────────────────────────────────────────────────
        // HELPER: Get a new SQL connection (ready to open)
        // ────────────────────────────────────────────────────
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }


        // ============================================================
        // USER METHODS
        // ============================================================

        /// <summary>
        /// Check login — returns User if email+password match, else null
        /// </summary>
        public User Login(string email, string password)
        {
            User user = null;

            // SQL query to find matching user
            string query = @"
                SELECT UserId, CompanyName, FullName, Email, Role, IsActive
                FROM   Users
                WHERE  Email    = @Email
                  AND  Password = @Password
                  AND  IsActive = 1";  // Only active accounts can login

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Use parameters to prevent SQL injection!
                cmd.Parameters.AddWithValue("@Email",    email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Found a matching user — build the object
                    user = new User
                    {
                        UserId      = Convert.ToInt32(reader["UserId"]),
                        CompanyName = reader["CompanyName"].ToString(),
                        FullName    = reader["FullName"].ToString(),
                        Email       = reader["Email"].ToString(),
                        Role        = reader["Role"].ToString(),
                        IsActive    = Convert.ToBoolean(reader["IsActive"])
                    };
                }
            }

            return user; // Returns null if not found
        }

        /// <summary>
        /// Register a new business user account
        /// Returns true if successful, false if email already exists
        /// </summary>
        public bool RegisterUser(User user)
        {
            // First check if email already exists
            if (EmailExists(user.Email))
                return false; // Email taken

            string query = @"
                INSERT INTO Users
                    (CompanyName, FullName, Email, Password, Phone, Address, Role)
                VALUES
                    (@CompanyName, @FullName, @Email, @Password, @Phone, @Address, 'User')";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CompanyName", user.CompanyName);
                cmd.Parameters.AddWithValue("@FullName",    user.FullName);
                cmd.Parameters.AddWithValue("@Email",       user.Email);
                cmd.Parameters.AddWithValue("@Password",    user.Password);
                cmd.Parameters.AddWithValue("@Phone",       (object)user.Phone   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address",     (object)user.Address ?? DBNull.Value);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0; // True if inserted
            }
        }

        /// <summary>
        /// Check if an email address is already registered
        /// </summary>
        public bool EmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Get all business users (for Admin user management page)
        /// </summary>
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            string query = @"
                SELECT UserId, CompanyName, FullName, Email,
                       Phone, Address, Role, IsActive, CreatedAt
                FROM   Users
                WHERE  Role = 'User'
                ORDER  BY CreatedAt DESC";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand    cmd    = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserId      = Convert.ToInt32(reader["UserId"]),
                        CompanyName = reader["CompanyName"].ToString(),
                        FullName    = reader["FullName"].ToString(),
                        Email       = reader["Email"].ToString(),
                        Phone       = reader["Phone"].ToString(),
                        Role        = reader["Role"].ToString(),
                        IsActive    = Convert.ToBoolean(reader["IsActive"]),
                        CreatedAt   = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }

            return users;
        }


        // ============================================================
        // BUS / FLEET METHODS
        // ============================================================

        /// <summary>
        /// Get all buses — with optional filter by status or type
        /// Pass null/empty to get all buses
        /// </summary>
        public List<Bus> GetBuses(string statusFilter = null, string typeFilter = null)
        {
            List<Bus> buses = new List<Bus>();

            // Build dynamic query based on filters
            string query = @"
                SELECT BusId, BusNumber, BusType, Capacity,
                       Model, Year, Status, CreatedAt
                FROM   Buses
                WHERE  1=1";  // 1=1 trick lets us append AND conditions easily

            // Add filters only if provided
            if (!string.IsNullOrEmpty(statusFilter))
                query += " AND Status = @Status";

            if (!string.IsNullOrEmpty(typeFilter))
                query += " AND BusType = @BusType";

            query += " ORDER BY CreatedAt DESC";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Only add parameters if filters are set
                if (!string.IsNullOrEmpty(statusFilter))
                    cmd.Parameters.AddWithValue("@Status",  statusFilter);

                if (!string.IsNullOrEmpty(typeFilter))
                    cmd.Parameters.AddWithValue("@BusType", typeFilter);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    buses.Add(new Bus
                    {
                        BusId     = Convert.ToInt32(reader["BusId"]),
                        BusNumber = reader["BusNumber"].ToString(),
                        BusType   = reader["BusType"].ToString(),
                        Capacity  = Convert.ToInt32(reader["Capacity"]),
                        Model     = reader["Model"].ToString(),
                        Year      = reader["Year"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Year"]),
                        Status    = reader["Status"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }

            return buses;
        }

        /// <summary>
        /// Get a single bus by ID (for Edit page)
        /// </summary>
        public Bus GetBusById(int busId)
        {
            Bus bus = null;

            string query = @"
                SELECT BusId, BusNumber, BusType, Capacity,
                       Model, Year, Status
                FROM   Buses
                WHERE  BusId = @BusId";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BusId", busId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    bus = new Bus
                    {
                        BusId     = Convert.ToInt32(reader["BusId"]),
                        BusNumber = reader["BusNumber"].ToString(),
                        BusType   = reader["BusType"].ToString(),
                        Capacity  = Convert.ToInt32(reader["Capacity"]),
                        Model     = reader["Model"].ToString(),
                        Year      = reader["Year"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Year"]),
                        Status    = reader["Status"].ToString()
                    };
                }
            }

            return bus;
        }

        /// <summary>
        /// Add a new bus to the fleet
        /// </summary>
        public bool AddBus(Bus bus)
        {
            string query = @"
                INSERT INTO Buses
                    (BusNumber, BusType, Capacity, Model, Year, Status)
                VALUES
                    (@BusNumber, @BusType, @Capacity, @Model, @Year, @Status)";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BusNumber", bus.BusNumber);
                cmd.Parameters.AddWithValue("@BusType",   bus.BusType);
                cmd.Parameters.AddWithValue("@Capacity",  bus.Capacity);
                cmd.Parameters.AddWithValue("@Model",     (object)bus.Model ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Year",      (object)bus.Year  ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status",    bus.Status);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Update an existing bus record
        /// </summary>
        public bool UpdateBus(Bus bus)
        {
            string query = @"
                UPDATE Buses
                SET    BusNumber = @BusNumber,
                       BusType   = @BusType,
                       Capacity  = @Capacity,
                       Model     = @Model,
                       Year      = @Year,
                       Status    = @Status
                WHERE  BusId = @BusId";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BusId",     bus.BusId);
                cmd.Parameters.AddWithValue("@BusNumber", bus.BusNumber);
                cmd.Parameters.AddWithValue("@BusType",   bus.BusType);
                cmd.Parameters.AddWithValue("@Capacity",  bus.Capacity);
                cmd.Parameters.AddWithValue("@Model",     (object)bus.Model ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Year",      (object)bus.Year  ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status",    bus.Status);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Delete a bus from fleet (only if not in active booking)
        /// </summary>
        public bool DeleteBus(int busId)
        {
            string query = "DELETE FROM Buses WHERE BusId = @BusId";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BusId", busId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // ============================================================
        // BOOKING METHODS
        // ============================================================

        /// <summary>
        /// Get bookings — Admin gets all, Users get only their own
        /// Optional filter by status (Pending/Approved/Rejected)
        /// </summary>
        public List<Booking> GetBookings(int? userId = null, string statusFilter = null)
        {
            List<Booking> bookings = new List<Booking>();

            // Join with Users to get company name
            string query = @"
                SELECT b.BookingId, b.UserId, b.BusId,
                       b.FromLocation, b.ToLocation,
                       b.TripDate, b.ReturnDate,
                       b.NoOfBuses, b.NoOfPassengers,
                       b.Purpose, b.Status, b.AdminNotes,
                       b.CreatedAt,
                       u.CompanyName,
                       bs.BusNumber
                FROM   Bookings b
                INNER  JOIN Users u ON b.UserId = u.UserId
                LEFT   JOIN Buses bs ON b.BusId = bs.BusId
                WHERE  1=1";

            // If userId given, show only that user's bookings
            if (userId.HasValue)
                query += " AND b.UserId = @UserId";

            // Filter by status if given
            if (!string.IsNullOrEmpty(statusFilter))
                query += " AND b.Status = @Status";

            query += " ORDER BY b.CreatedAt DESC";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                if (userId.HasValue)
                    cmd.Parameters.AddWithValue("@UserId", userId.Value);

                if (!string.IsNullOrEmpty(statusFilter))
                    cmd.Parameters.AddWithValue("@Status", statusFilter);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bookings.Add(new Booking
                    {
                        BookingId      = Convert.ToInt32(reader["BookingId"]),
                        UserId         = Convert.ToInt32(reader["UserId"]),
                        BusId          = reader["BusId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["BusId"]),
                        FromLocation   = reader["FromLocation"].ToString(),
                        ToLocation     = reader["ToLocation"].ToString(),
                        TripDate       = Convert.ToDateTime(reader["TripDate"]),
                        ReturnDate     = reader["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ReturnDate"]),
                        NoOfBuses      = Convert.ToInt32(reader["NoOfBuses"]),
                        NoOfPassengers = reader["NoOfPassengers"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["NoOfPassengers"]),
                        Purpose        = reader["Purpose"].ToString(),
                        Status         = reader["Status"].ToString(),
                        AdminNotes     = reader["AdminNotes"].ToString(),
                        CreatedAt      = Convert.ToDateTime(reader["CreatedAt"]),
                        CompanyName    = reader["CompanyName"].ToString(),
                        BusNumber      = reader["BusNumber"].ToString()
                    });
                }
            }

            return bookings;
        }

        /// <summary>
        /// Create a new booking request
        /// </summary>
        public bool CreateBooking(Booking booking)
        {
            string query = @"
                INSERT INTO Bookings
                    (UserId, FromLocation, ToLocation, TripDate,
                     ReturnDate, NoOfBuses, NoOfPassengers, Purpose)
                VALUES
                    (@UserId, @FromLocation, @ToLocation, @TripDate,
                     @ReturnDate, @NoOfBuses, @NoOfPassengers, @Purpose)";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId",         booking.UserId);
                cmd.Parameters.AddWithValue("@FromLocation",   booking.FromLocation);
                cmd.Parameters.AddWithValue("@ToLocation",     booking.ToLocation);
                cmd.Parameters.AddWithValue("@TripDate",       booking.TripDate);
                cmd.Parameters.AddWithValue("@ReturnDate",     (object)booking.ReturnDate     ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NoOfBuses",      booking.NoOfBuses);
                cmd.Parameters.AddWithValue("@NoOfPassengers", (object)booking.NoOfPassengers ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Purpose",        (object)booking.Purpose        ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Admin approves or rejects a booking
        /// Also assigns a bus and adds notes
        /// </summary>
        public bool UpdateBookingStatus(int bookingId, string status, int? busId, string adminNotes)
        {
            string query = @"
                UPDATE Bookings
                SET    Status     = @Status,
                       BusId      = @BusId,
                       AdminNotes = @AdminNotes
                WHERE  BookingId  = @BookingId";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookingId",  bookingId);
                cmd.Parameters.AddWithValue("@Status",     status);
                cmd.Parameters.AddWithValue("@BusId",      (object)busId      ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdminNotes", (object)adminNotes ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // ============================================================
        // DASHBOARD METHODS
        // ============================================================

        /// <summary>
        /// Get all summary counts for Admin Dashboard
        /// Returns counts as a DataTable with one row
        /// </summary>
        public DashboardViewModel GetDashboardData()
        {
            DashboardViewModel dashboard = new DashboardViewModel();

            // One query to get all counts at once
            string query = @"
                SELECT
                    (SELECT COUNT(*) FROM Buses)                               AS TotalBuses,
                    (SELECT COUNT(*) FROM Buses WHERE Status = 'Available')    AS AvailableBuses,
                    (SELECT COUNT(*) FROM Buses WHERE Status = 'On Trip')      AS BusesOnTrip,
                    (SELECT COUNT(*) FROM Buses WHERE Status = 'Maintenance')  AS BusesInMaintenance,
                    (SELECT COUNT(*) FROM Bookings)                            AS TotalBookings,
                    (SELECT COUNT(*) FROM Bookings WHERE Status = 'Pending')   AS PendingBookings,
                    (SELECT COUNT(*) FROM Bookings WHERE Status = 'Approved')  AS ApprovedBookings,
                    (SELECT COUNT(*) FROM Users    WHERE Role  = 'User')       AS TotalCompanies";

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand    cmd    = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dashboard.TotalBuses          = Convert.ToInt32(reader["TotalBuses"]);
                    dashboard.AvailableBuses       = Convert.ToInt32(reader["AvailableBuses"]);
                    dashboard.BusesOnTrip          = Convert.ToInt32(reader["BusesOnTrip"]);
                    dashboard.BusesInMaintenance   = Convert.ToInt32(reader["BusesInMaintenance"]);
                    dashboard.TotalBookings        = Convert.ToInt32(reader["TotalBookings"]);
                    dashboard.PendingBookings      = Convert.ToInt32(reader["PendingBookings"]);
                    dashboard.ApprovedBookings     = Convert.ToInt32(reader["ApprovedBookings"]);
                    dashboard.TotalCompanies       = Convert.ToInt32(reader["TotalCompanies"]);
                }
            }

            // Get recent 5 bookings for dashboard table
            dashboard.RecentBookings      = GetBookings().GetRange(0, Math.Min(5, GetBookings().Count));
            dashboard.PendingBookingsList = GetBookings(null, "Pending");

            return dashboard;
        }


        // ============================================================
        // REPORT METHODS
        // ============================================================

        /// <summary>
        /// Get monthly booking report data
        /// Returns list of anonymous data — month, year, counts
        /// </summary>
        public DataTable GetMonthlyReport(int year)
        {
            string query = @"
                SELECT
                    MONTH(TripDate)   AS Month,
                    YEAR(TripDate)    AS Year,
                    COUNT(*)          AS TotalBookings,
                    SUM(NoOfBuses)    AS TotalBusesUsed,
                    SUM(CASE WHEN Status = 'Approved'  THEN 1 ELSE 0 END) AS Approved,
                    SUM(CASE WHEN Status = 'Rejected'  THEN 1 ELSE 0 END) AS Rejected,
                    SUM(CASE WHEN Status = 'Pending'   THEN 1 ELSE 0 END) AS Pending
                FROM  Bookings
                WHERE YEAR(TripDate) = @Year
                GROUP BY YEAR(TripDate), MONTH(TripDate)
                ORDER BY Month ASC";

            DataTable dt = new DataTable();

            using (SqlConnection conn = GetConnection())
            {
                SqlCommand    cmd    = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Year", year);

                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt); // Fill DataTable directly
            }

            return dt;
        }
    }
}
