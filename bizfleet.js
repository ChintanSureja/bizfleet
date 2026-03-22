// ============================================================
// FILE    : Scripts/bizfleet.js
// PURPOSE : All custom JavaScript and jQuery for BizFleet
// ============================================================

// Wait for page to fully load before running any code
$(document).ready(function () {

    // ────────────────────────────────────────────────────────
    // 1. LIVE CLOCK — shows current time in dashboard
    // ────────────────────────────────────────────────────────
    function updateClock() {
        var now = new Date();
        // Format: Sunday, March 22, 2026 — 10:30:45 AM
        var options = {
            weekday: 'long', year: 'numeric',
            month: 'long', day: 'numeric',
            hour: '2-digit', minute: '2-digit', second: '2-digit'
        };
        var timeString = now.toLocaleDateString('en-US', options);
        $('#currentTime').text(timeString);
    }

    // Update clock every second
    if ($('#currentTime').length) {
        updateClock();
        setInterval(updateClock, 1000);
    }

    // ────────────────────────────────────────────────────────
    // 2. AUTO DISMISS ALERTS after 5 seconds
    // ────────────────────────────────────────────────────────
    setTimeout(function () {
        $('.alert-glass').fadeOut('slow');
    }, 5000);

    // ────────────────────────────────────────────────────────
    // 3. CONFIRM DELETE — extra safety on delete buttons
    // ────────────────────────────────────────────────────────
    // This catches any delete form that doesn't already have onclick
    $('form[action*="Delete"]').on('submit', function (e) {
        if (!confirm('Are you sure you want to delete this? This cannot be undone.')) {
            e.preventDefault(); // Cancel submission
        }
    });

    // ────────────────────────────────────────────────────────
    // 4. STAT CARD COUNTER ANIMATION
    // Animates numbers counting up when page loads
    // ────────────────────────────────────────────────────────
    $('.stat-number').each(function () {
        var $el     = $(this);
        var target  = parseInt($el.text()); // Final number to count to
        var current = 0;
        var step    = Math.ceil(target / 30); // How much to add each frame

        // Only animate if number is valid
        if (isNaN(target)) return;

        $el.text('0');

        var timer = setInterval(function () {
            current += step;
            if (current >= target) {
                $el.text(target); // Snap to final value
                clearInterval(timer);
            } else {
                $el.text(current);
            }
        }, 40); // Run every 40ms = ~25 frames/second
    });

    // ────────────────────────────────────────────────────────
    // 5. FORM VALIDATION — highlight empty required fields
    // ────────────────────────────────────────────────────────
    $('form').on('submit', function () {
        var valid = true;

        // Check all required inputs
        $(this).find('input[required], select[required]').each(function () {
            if (!$(this).val().trim()) {
                $(this).css('border-color', '#EF4444'); // Red border
                valid = false;
            } else {
                $(this).css('border-color', 'var(--glass-border)'); // Reset
            }
        });

        return valid; // If false, form won't submit
    });

    // Remove red border when user starts typing
    $('input, select').on('input change', function () {
        $(this).css('border-color', 'var(--glass-border)');
    });

    // ────────────────────────────────────────────────────────
    // 6. SEARCH TABLE — filter table rows live as user types
    // ────────────────────────────────────────────────────────
    $('#tableSearch').on('keyup', function () {
        var searchText = $(this).val().toLowerCase(); // What user typed

        // Loop through each table row
        $('table tbody tr').each(function () {
            var rowText = $(this).text().toLowerCase(); // Row content
            // Show row if it contains the search text, hide otherwise
            $(this).toggle(rowText.includes(searchText));
        });
    });

    // ────────────────────────────────────────────────────────
    // 7. CARD HOVER 3D TILT EFFECT
    // Makes cards tilt slightly when hovering (3D feel)
    // ────────────────────────────────────────────────────────
    $('.card-glass').on('mousemove', function (e) {
        var card    = $(this);
        var offset  = card.offset();
        var width   = card.outerWidth();
        var height  = card.outerHeight();

        // Calculate mouse position relative to card center
        var centerX = offset.left + width  / 2;
        var centerY = offset.top  + height / 2;
        var deltaX  = (e.pageX - centerX) / (width  / 2);
        var deltaY  = (e.pageY - centerY) / (height / 2);

        // Apply tilt transform (max 5 degrees)
        card.css('transform',
            'translateY(-4px) rotateX(' + (-deltaY * 3) + 'deg) rotateY(' + (deltaX * 3) + 'deg)'
        );
    });

    // Reset card transform when mouse leaves
    $('.card-glass').on('mouseleave', function () {
        $(this).css('transform', '');
    });

    // ────────────────────────────────────────────────────────
    // 8. BOOKING STATUS BADGE COLORS
    // Dynamically apply correct color class to status text
    // ────────────────────────────────────────────────────────
    $('td').each(function () {
        var text = $(this).text().trim().toLowerCase();

        // Auto-badge status cells
        if (text === 'pending') {
            $(this).html('<span class="badge-status badge-pending">Pending</span>');
        } else if (text === 'approved') {
            $(this).html('<span class="badge-status badge-approved">Approved</span>');
        } else if (text === 'rejected') {
            $(this).html('<span class="badge-status badge-rejected">Rejected</span>');
        } else if (text === 'available') {
            $(this).html('<span class="badge-status badge-available">Available</span>');
        } else if (text === 'on trip') {
            $(this).html('<span class="badge-status badge-on-trip">On Trip</span>');
        } else if (text === 'maintenance') {
            $(this).html('<span class="badge-status badge-maintenance">Maintenance</span>');
        }
    });

    // ────────────────────────────────────────────────────────
    // 9. SMOOTH PAGE ENTRANCE ANIMATION
    // Pages fade in smoothly when loaded
    // ────────────────────────────────────────────────────────
    $('body').css('opacity', '0').animate({ opacity: 1 }, 400);

}); // End document.ready
