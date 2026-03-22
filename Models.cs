// ============================================================
// FILE    : Models/User.cs
// PURPOSE : Represents a User (Admin or Business User)
//           Maps directly to the Users table in SQL Server
// ============================================================

using System;
using System.ComponentModel.DataAnnotations;

namespace BizFleet.Models
{
    /// <summary>
    /// User model — stores both Admin and Business User data
    /// </summary>
    public class User
    {
        // Primary key — auto set by database
        public int UserId { get; set; }

        // Company name (required)
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        // Full name of contact person
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        // Email used for login (must be unique)
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; }

        // Password (will be stored as plain text for simplicity)
        // In production: always hash passwords!
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Phone number (optional)
        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string Phone { get; set; }

        // Company address (optional)
        [StringLength(255)]
        public string Address { get; set; }

        // Role — 'Admin' or 'User'
        public string Role { get; set; } = "User";

        // Is account active? (Admin can disable accounts)
        public bool IsActive { get; set; } = true;

        // When was this account created
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}


// ============================================================
// FILE    : Models/Bus.cs
// PURPOSE : Represents a Bus in the fleet
//           Maps directly to the Buses table in SQL Server
// ============================================================

namespace BizFleet.Models
{
    /// <summary>
    /// Bus model — stores all bus/vehicle details
    /// </summary>
    public class Bus
    {
        // Primary key
        public int BusId { get; set; }

        // Unique bus plate number (e.g. MH-01-AB-1234)
        [Required(ErrorMessage = "Bus number is required")]
        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }

        // Type: Mini / Standard / Luxury
        [Required(ErrorMessage = "Bus type is required")]
        [Display(Name = "Bus Type")]
        public string BusType { get; set; }

        // How many passengers it can carry
        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        public int Capacity { get; set; }

        // Bus model name (e.g. Volvo 9400)
        [Display(Name = "Bus Model")]
        public string Model { get; set; }

        // Year of manufacture
        [Range(2000, 2030, ErrorMessage = "Enter a valid year")]
        public int? Year { get; set; }

        // Current status: Available / On Trip / Maintenance
        public string Status { get; set; } = "Available";

        // Optional image path for bus photo
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        // When was this bus added
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}


// ============================================================
// FILE    : Models/Booking.cs
// PURPOSE : Represents a Trip Booking Request
//           Maps directly to the Bookings table in SQL Server
// ============================================================

namespace BizFleet.Models
{
    /// <summary>
    /// Booking model — stores trip requests from businesses
    /// </summary>
    public class Booking
    {
        // Primary key
        public int BookingId { get; set; }

        // Which user/company made this booking
        public int UserId { get; set; }

        // Which bus was assigned (nullable — set after approval)
        public int? BusId { get; set; }

        // Pickup location
        [Required(ErrorMessage = "From location is required")]
        [Display(Name = "From Location")]
        public string FromLocation { get; set; }

        // Drop location
        [Required(ErrorMessage = "To location is required")]
        [Display(Name = "To Location")]
        public string ToLocation { get; set; }

        // Date of trip
        [Required(ErrorMessage = "Trip date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Trip Date")]
        public DateTime TripDate { get; set; }

        // Return date (optional for round trips)
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        // How many buses needed
        [Range(1, 20, ErrorMessage = "Must be between 1 and 20")]
        [Display(Name = "No. of Buses")]
        public int NoOfBuses { get; set; } = 1;

        // Total number of passengers
        [Display(Name = "No. of Passengers")]
        public int? NoOfPassengers { get; set; }

        // Reason for trip (e.g. Corporate Event, Team Outing)
        [StringLength(255)]
        public string Purpose { get; set; }

        // Booking status: Pending / Approved / Rejected
        public string Status { get; set; } = "Pending";

        // Admin notes/comments on this booking
        [Display(Name = "Admin Notes")]
        public string AdminNotes { get; set; }

        // When booking was created
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation (joined data — not in DB) ──
        // Company name from Users table (for display)
        public string CompanyName { get; set; }

        // Bus number from Buses table (for display)
        public string BusNumber { get; set; }
    }
}


// ============================================================
// FILE    : Models/DashboardViewModel.cs
// PURPOSE : Holds all data needed for Admin Dashboard page
//           Combines counts + lists into one object
// ============================================================

using System.Collections.Generic;

namespace BizFleet.Models
{
    /// <summary>
    /// ViewModel for Admin Dashboard — combines all summary data
    /// </summary>
    public class DashboardViewModel
    {
        // ── Summary Counts (shown as stat cards) ──

        // Total number of buses in fleet
        public int TotalBuses { get; set; }

        // Buses currently available for booking
        public int AvailableBuses { get; set; }

        // Buses currently on a trip
        public int BusesOnTrip { get; set; }

        // Buses under maintenance
        public int BusesInMaintenance { get; set; }

        // Total bookings ever made
        public int TotalBookings { get; set; }

        // Bookings waiting for approval
        public int PendingBookings { get; set; }

        // Approved bookings
        public int ApprovedBookings { get; set; }

        // Total registered business companies
        public int TotalCompanies { get; set; }

        // ── Recent Data (shown in tables) ──

        // Last 5 bookings (for recent activity)
        public List<Booking> RecentBookings { get; set; }

        // All pending bookings (for quick action)
        public List<Booking> PendingBookingsList { get; set; }
    }
}
