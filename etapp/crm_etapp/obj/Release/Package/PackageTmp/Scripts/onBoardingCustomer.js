function nextDiv(ele) {
    try {
        $(ele).parent().next().show();
    }
    catch (err) {
        alert(err);
    }
}

function nextSectionDiv(ele) {
    try {
        var ele = $('#section2').children()[0];
        $(ele).show();
    }
    catch (err) {
        alert(err);
    }
}