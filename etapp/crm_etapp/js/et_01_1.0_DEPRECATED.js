//db
var jsonDevices = false;
function getDevices() {
    try {
        var b = getTokenCookie("ETTK"), a = false; if (b == null || b == "") { if (location.pathname.toLowerCase().indexOf("index.html") == -1) location.href = "index.html"; a = false } else jQuery.ajax({ url: "ETWS.asmx/getDevices", data: "t=" + escape(b), dataType: "xml", type: "POST", success: function (b, c) {
            c == "success" ? ($("string", b).text() == "failure" && (alert("Error loading devices"), a = false), jsonDevices = eval("(" + $("string", b).text() + ")"), jsonDevices.result == "failure" ? (alert(jsonDevices.error), a = false) : a = true) : (alert("Error loading devices"),
a = false)
        }, error: function () { alert("Error loading devices"); a = false }, async: false
        }); return a
    } catch (d) { return alert("Error loading devices"), false } 
}
function dbReadWrite(b, a, d, e) { var c = false; e == void 0 && (e = true); d == void 0 && (d = true); try { if (b == void 0 || b == null) alert("Invalid method name"); else { if (a == void 0 || a == null) a = ""; jQuery.ajax({ url: "ETWS.asmx/" + b, data: a, dataType: "xml", type: "POST", success: function (a, b) { b == "success" && $("string", a).text() != "failure" && (c = eval("(" + $("string", a).text() + ")"), c.result == "failure" && d == true && alert(c.error)) }, error: function () { }, async: e }) } return c } catch (f) { return alert("dbReadWrite: Error loading " + b), false } };