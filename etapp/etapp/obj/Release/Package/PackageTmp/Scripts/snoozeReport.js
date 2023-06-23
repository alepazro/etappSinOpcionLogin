var rpt = false;
var usr = false;

function snoozeNow() {
    try {
        var period = $('#snoozeTimes').val();
        var data = 'rpt=' + rpt + '&usr=' + usr + '&period=' + period;
        var jsonResult = dbReadWrite('snoozeReport', data, true, false);
        alert('Snooze preference saved.  You can close this window now.');
    }
    catch (err) {
        alert('snoozeNow: ' + err.description);
    }
}