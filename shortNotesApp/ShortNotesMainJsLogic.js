document.addEventListener('DOMContentLoaded', function () {
    // Check if we already logged user data in this session
    if (!sessionStorage.getItem('userLogged')) {
        try {

            let name = localStorage.getItem('userName');
            let namePattern = /^[\u0D80-\u0DFF\u0B80-\u0BFFA-Za-z\s]+$/;

            let parts1 = "";
            // Split into parts
            if (name) {
                parts1 = name.split(/\s+/).filter(p => p.length > 0);
            }

            // check last update's usernames are correct
            let isValid = true;
            if (!name) {
                isValid = false;
            } else if (name == "Mahinda Mahattaya") {
                isValid = false;
                alert("Please be kind enough to enter a real full name 😊");
            }
            else if (name.length < 6) {
                isValid = false;
                alert("Please enter your correct full name (it makes me happy knowing someone real is using my web app 😊)");
            } else if (parts1.length < 2) {
                isValid = false
                alert("Please enter your correct full name (it makes me happy knowing someone real is using my web app 😊)");
            } else if (!namePattern.test(name)) {
                isValid = false
                alert("Please enter your correct full name (it makes me happy knowing someone real is using my web app 😊)");
            }

            let nameInput = "";
            while (isValid == false) {
                nameInput = prompt("Please enter your full name — it makes me happy knowing someone real is using my web app 😊:");

                // checking is prompt canceled
                if (nameInput == null) {
                    continue;
                }

                // otherwise not canceled


                nameInput = nameInput.trim();
                // Check cancel
                if (nameInput === null) {
                    alert("You must enter your name to continue.");
                    continue;
                }

                // Split into parts
                let parts = nameInput.split(/\s+/).filter(p => p.length > 0);

                if (nameInput.length < 6) {
                    alert("⚠️ Please enter your correct full name.");
                    continue;
                }

                if (parts.length < 2) {
                    alert("⚠️ Please enter your full name (at least two parts).");
                    continue;
                }

                if (!namePattern.test(nameInput)) {
                    alert("⚠️ Name can only contain Sinhala, Tamil, or English letters and spaces.");
                    continue;
                }

                // Capitalize each word
                nameInput = nameInput.replace(/\b\w/g, c => c.toUpperCase());

                alert("✅ Welcome, " + nameInput + "!");
                break; // Exit loop when valid
            }

            if (!name || isValid == false) {
                name = nameInput;
                if (name) localStorage.setItem('userName', name);
            }

            // Send to server only the first time
            if (name) {
                fetch('ShortNotesMain.aspx/LogUserName', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ userName: name })
                })
                    .then(response => response.json())
                    .then(data => {
                        // Mark as logged
                        sessionStorage.setItem('userLogged', 'true');
                    })
                    .catch(err => {
                        console.error('Error sending user name:', err);
                    });
            }
        } catch (err) {
            console(err);
        }

    }
});
/*
                document.getElementById('contactLink').onclick = function (e) {
                    e.preventDefault();
                    var message = prompt("Type your message for the site owner:");
                    var name = localStorage.getItem('userName');
                    if (name && name.trim() && message && message.trim()) {
                        $.ajax({
                            type: "POST",
                            url: "ShortNotesMain.aspx/SendContactMessage",
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify({ userName: name, userMessage: message }),
                            success: function () {
                                alert("Your message has been sent!");
                                $('#contactPopup').popup('close');
                                $('#contactMessage').val(""); // Clear
                                $('#contactName').val("");
                            },
                            error: function () { alert("Failed to send message."); }
                        });
                    } else {
                        alert("Please enter your name and message.");
                    }
                };*/


// this will send message to owner
function sendContactMessage() {
    var name = localStorage.getItem('userName');
    var message = prompt("Please enter your message.(If you need further help, also mention your whatsapp number or email)");
    if (name && name.trim() && message && message.trim()) {
        $.ajax({
            type: "POST",
            url: "ShortNotesMain.aspx/SendContactMessage",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ userName: name, userMessage: message }),
            success: function () {
                alert("Your message has been sent!");
            },
            error: function () { alert("Failed to send message."); }
        });
    } else {
        alert("Please enter your message.");
    }
}