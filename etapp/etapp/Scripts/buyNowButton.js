
function buyNow() {
    try {
        if (userIsAdmin == true) {
            window.open('https://easitrack.net/buy-now.html?s=1&t=' + getTokenCookie('ETTK'), target = "_blank");
        }
        else {
            alert('This option is only available for Administrator users');
        }
    }
    catch (err) {
        alert('accountSettings: ' + err.description);
    }
}
