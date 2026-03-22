// ============================================================
// FILE    : Controllers/AccountController.cs
// PURPOSE : Handles Login and Register pages
// ============================================================

using System.Web.Mvc;
using System.Web;
using BizFleet.DAL;
using BizFleet.Models;

namespace BizFleet.Controllers
{
    public class AccountController : Controller
    {
        // Create one DatabaseHelper — used in all methods
        private DatabaseHelper db = new DatabaseHelper();

        // ── GET: /Account/Login ──
        // Shows the login page
        public ActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (Session["UserId"] != null)
                return RedirectToAction("Dashboard", "Admin");

            return View();
        }

        // ── POST: /Account/Login ──
        // Processes login form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // Security: prevents CSRF attacks
        public ActionResult Login(string email, string password)
        {
            // Check if email and password are provided
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter email and password.";
                return View();
            }

            // Check credentials in database
            User user = db.Login(email, password);

            if (user != null)
            {
                // ✅ Login successful — save user info in Session
                Session["UserId"]      = user.UserId;
                Session["UserName"]    = user.FullName;
                Session["CompanyName"] = user.CompanyName;
                Session["Role"]        = user.Role;

                // Redirect based on role
                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Booking");
            }
            else
            {
                // ❌ Login failed
                ViewBag.Error = "Invalid email or password. Please try again.";
                return View();
            }
        }

        // ── GET: /Account/Register ──
        public ActionResult Register()
        {
            return View();
        }

        // ── POST: /Account/Register ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            // Check if all required fields are valid
            if (ModelState.IsValid)
            {
                // Try to register the user
                bool success = db.RegisterUser(user);

                if (success)
                {
                    // ✅ Registered — show success message and go to login
                    TempData["Success"] = "Account created! Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Email already exists
                    ViewBag.Error = "This email is already registered. Please login.";
                }
            }

            return View(user);
        }

        // ── GET: /Account/Logout ──
        public ActionResult Logout()
        {
            // Clear all session data
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }
    }
}


// ============================================================
// FILE    : Controllers/AdminController.cs
// PURPOSE : Admin Dashboard — summary stats + recent activity
// ============================================================

using System.Web.Mvc;
using BizFleet.DAL;

namespace BizFleet.Controllers
{
    public class AdminController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // ── GET: /Admin/Dashboard ──
        public ActionResult Dashboard()
        {
            // Security: Only Admin can access this page
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            // Get all dashboard data from database
            var dashboardData = db.GetDashboardData();

            return View(dashboardData);
        }
    }
}


// ============================================================
// FILE    : Controllers/FleetController.cs
// PURPOSE : Bus Fleet Management — Add, Edit, Delete, View buses
// ============================================================

using System.Web.Mvc;
using BizFleet.DAL;
using BizFleet.Models;

namespace BizFleet.Controllers
{
    public class FleetController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // ── GET: /Fleet ──
        // Show list of all buses with optional filter
        public ActionResult Index(string status = null, string type = null)
        {
            // Both Admin and Users can view fleet
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // Get buses with optional filters
            var buses = db.GetBuses(status, type);

            // Pass filter values back to view for display
            ViewBag.StatusFilter = status;
            ViewBag.TypeFilter   = type;

            return View(buses);
        }

        // ── GET: /Fleet/Create ──
        // Show add bus form (Admin only)
        public ActionResult Create()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ── POST: /Fleet/Create ──
        // Save new bus to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bus bus)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                bool success = db.AddBus(bus);

                if (success)
                {
                    TempData["Success"] = "Bus added successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "Failed to add bus. Please try again.";
            }

            return View(bus);
        }

        // ── GET: /Fleet/Edit/5 ──
        // Show edit form for a specific bus
        public ActionResult Edit(int id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            // Get bus data from database
            Bus bus = db.GetBusById(id);

            if (bus == null)
            {
                TempData["Error"] = "Bus not found.";
                return RedirectToAction("Index");
            }

            return View(bus);
        }

        // ── POST: /Fleet/Edit ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Bus bus)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                bool success = db.UpdateBus(bus);

                if (success)
                {
                    TempData["Success"] = "Bus updated successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "Failed to update. Please try again.";
            }

            return View(bus);
        }

        // ── POST: /Fleet/Delete/5 ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            bool success = db.DeleteBus(id);

            if (success)
                TempData["Success"] = "Bus removed from fleet.";
            else
                TempData["Error"] = "Could not delete bus.";

            return RedirectToAction("Index");
        }
    }
}


// ============================================================
// FILE    : Controllers/BookingController.cs
// PURPOSE : Booking management for both Users and Admin
// ============================================================

using System;
using System.Web.Mvc;
using BizFleet.DAL;
using BizFleet.Models;

namespace BizFleet.Controllers
{
    public class BookingController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // ── GET: /Booking ──
        // Users see their own bookings; Admin sees all
        public ActionResult Index(string status = null)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int    userId = Convert.ToInt32(Session["UserId"]);
            string role   = Session["Role"].ToString();

            // Admin sees all bookings; User sees only their own
            var bookings = role == "Admin"
                ? db.GetBookings(null,   status)
                : db.GetBookings(userId, status);

            ViewBag.StatusFilter = status;
            ViewBag.IsAdmin      = (role == "Admin");

            return View(bookings);
        }

        // ── GET: /Booking/Create ──
        // Show new booking request form (Users only)
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ── POST: /Booking/Create ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Booking booking)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Set the UserId from session (current logged-in user)
                booking.UserId = Convert.ToInt32(Session["UserId"]);

                bool success = db.CreateBooking(booking);

                if (success)
                {
                    TempData["Success"] = "Booking submitted! Waiting for approval.";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "Booking failed. Please try again.";
            }

            return View(booking);
        }

        // ── POST: /Booking/UpdateStatus ──
        // Admin approves or rejects a booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int bookingId, string status, int? busId, string adminNotes)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            bool success = db.UpdateBookingStatus(bookingId, status, busId, adminNotes);

            if (success)
                TempData["Success"] = $"Booking {status} successfully!";
            else
                TempData["Error"] = "Update failed. Please try again.";

            return RedirectToAction("Index");
        }
    }
}


// ============================================================
// FILE    : Controllers/ReportController.cs
// PURPOSE : Monthly reports + CSV export
// ============================================================

using System;
using System.Data;
using System.Text;
using System.Web.Mvc;
using BizFleet.DAL;

namespace BizFleet.Controllers
{
    public class ReportController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();

        // ── GET: /Report ──
        public ActionResult Index(int year = 0)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            // Default to current year
            if (year == 0) year = DateTime.Now.Year;

            DataTable reportData = db.GetMonthlyReport(year);

            ViewBag.Year       = year;
            ViewBag.ReportData = reportData;

            return View();
        }

        // ── GET: /Report/ExportCSV ──
        // Download monthly report as CSV file
        public ActionResult ExportCSV(int year = 0)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (year == 0) year = DateTime.Now.Year;

            DataTable data = db.GetMonthlyReport(year);

            // Build CSV content
            StringBuilder csv = new StringBuilder();

            // CSV Header row
            csv.AppendLine("Month,Year,Total Bookings,Total Buses Used,Approved,Rejected,Pending");

            // Month names array for readable output
            string[] months = { "", "January", "February", "March", "April",
                                 "May", "June", "July", "August", "September",
                                 "October", "November", "December" };

            // Add each row of data
            foreach (DataRow row in data.Rows)
            {
                int monthNum = Convert.ToInt32(row["Month"]);
                csv.AppendLine($"{months[monthNum]}," +
                               $"{row["Year"]},"      +
                               $"{row["TotalBookings"]},"  +
                               $"{row["TotalBusesUsed"]}," +
                               $"{row["Approved"]},"       +
                               $"{row["Rejected"]},"       +
                               $"{row["Pending"]}");
            }

            // Return as downloadable file
            byte[] bytes    = Encoding.UTF8.GetBytes(csv.ToString());
            string fileName = $"BizFleet_Report_{year}.csv";

            return File(bytes, "text/csv", fileName);
        }
    }
}
