//auth

var welcomeTitle = "", userFullName = ""; function setTokenCookie(a) { try { var b = new Date; b.setDate(b.getDate() + 1); document.cookie = a.tokenCookie + "=" + escape(a.token) + "; expires=" + b.toUTCString() } catch (c) { alert("setTokenCookie: " + c.Description) } }
function getTokenCookie(a) { try { var b = ""; if (document.cookie.length > 0 && (c_start = document.cookie.indexOf(a + "="), c_start != -1)) { c_start = c_start + a.length + 1; c_end = document.cookie.indexOf(";", c_start); if (c_end == -1) c_end = document.cookie.length; b = unescape(document.cookie.substring(c_start, c_end)) } return b } catch (c) { alert("getTokenCookie: " + c.Description) } } function deleteTokenCookie(a) { try { if (document.cookie.length > 0) document.cookie = a + "=" } catch (b) { alert("deleteTokenCookie: " + b.Description) } }
function changePanel(a) { try { a == 1 ? location.href = "Tracking.html" : a == 2 ? location.href = "Reports.html" : a == 3 ? location.href = "Settings.html" : a == 4 ? location.href = "Multi-Tracking.html" : alert("changePanel: Undefined panel id") } catch (b) { alert("changePanel: " + b.description) } }
function validateCredentialsResponse(a, b) {
    try {
        if (b == "success") { if ($("string", a).text() == "failure") return alert("Login failed.  Please try again."), false; var c = $("string", a).text(); if (c == "") return alert("Invalid Credentials.  Please try again."), false; var e = eval("(" + c + ")"); setTokenCookie(e); welcomeTitle = e.welcomeTitle; fullName = e.fullName; document.getElementById("txtLogin").value = ""; document.getElementById("txtPassword").value = ""; location.href = "tracking.html"; return true } else return alert("Sign in status: " +
b), false
    } catch (f) { return alert("validateCredentialsResponse: " + f.Description), false } 
} function validateCredentials() { try { var a = document.getElementById("txtLogin").value, b = document.getElementById("txtPassword").value, c = "Login=" + escape(a) + "&pw=" + escape(b); $.post("ETWS.asmx/ValidateCredentials", c, function (a, b) { validateCredentialsResponse(a, b) }) } catch (e) { alert("validateCredentials: " + e.Description) } }
function logout() { try { deleteTokenCookie("ETTK"), location.href = "login.html" } catch (a) { alert("Logout: " + a.Description) } }
function validateToken() {
    try {
        var a = getTokenCookie("ETTK"), b = false; if (a == null || a == "") { if (location.pathname.toLowerCase().indexOf("login.html") == -1) location.href = "login.html"; return false } else jQuery.ajax({ url: "ETWS.asmx/validateToken", data: "t=" + escape(a), dataType: "xml", type: "POST", success: function (a, c) {
            if (c == "success") {
                var d = $("string", a).text(); if (d == "") return alert("Invalid Credentials.  Please try again."), false; d = eval("(" + d + ")"); if (d.isValid == true) {
                    welcomeTitle = d.welcomeTitle; fullName = d.fullName;
                    if (location.pathname.toLowerCase().indexOf("tracking.html") == -1 && location.pathname.toLowerCase().indexOf("reports.html") == -1 && location.pathname.toLowerCase().indexOf("settings.html") == -1) location.href = "tracking.html"; b = true
                } else { if (location.pathname.toLowerCase().indexOf("login.html") == -1) location.href = "login.html"; b = false } 
            } else alert("Error validating token"), b = false
        }, error: function () { alert("Error validating token"); b = false }, async: false
        }); return b
    } catch (c) {
        if (location.pathname.toLowerCase().indexOf("login.html") ==
-1) return location.href = "login.html", false
    } 
} function validateDemoToken() { try { var a = getParameterByName("t"); if (a.length > 0) { data = "t=" + escape(a); var b = dbReadWrite("validateDemoToken", data, true, false); setTokenCookie(b); welcomeTitle = b.welcomeTitle; fullName = b.fullName; location.href = "tracking.html"; return true } else return alert("Invalid access token"), false } catch (c) { alert("validateDemoToken: " + c.description) } };