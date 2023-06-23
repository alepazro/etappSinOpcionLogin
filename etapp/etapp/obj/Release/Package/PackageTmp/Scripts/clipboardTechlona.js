startScan = function () {

    var target = null;
    var enabled = true;
    var then = +new Date();
    var box = $("<div></div>").appendTo("body");

    $("body").bind("mousemove", handler);

    $("*").not("body,html").not("embed,object").click(function () {
        if (enabled) {
            alert(get_XPath(target[0]));
        }
        //Disable further scannings
        //enabled = false;
    });

    function handler(e) {
        //Disable scan upon click
        if (enabled == false) {
            return;
        }
        var offset, el = e.target;
        var now = +new Date();
        if (now - then < 25) {
            return;
        }
        then = now;

        if (el === document.body || el.className === "block-overlay") {
            box.hide();
            return;
        } else if (el === box[0]) {
            box.hide();
            el = document.elementFromPoint(e.clientX, e.clientY);
        }
        target = $(el);
        offset = target.offset();
        box.css({
            zIndex: (parseInt(target.css("z-index")) || 1) + " !important",
            height: target.outerHeight(),
            width: target.outerWidth(),
            backgroundColor: "rgba(130, 180, 230, 0.5)",
            outline: "solid 1px #0F4D9A",
            boxSizing: "border-box",
            position: "absolute",
            left: offset.left,
            top: offset.top
        });
        box.show();
    }
}

function get_XPath(elt) {
    var path = '';
    for (; elt && elt.nodeType == 1; elt = elt.parentNode) {
        var idx = $(elt.parentNode).children(elt.tagName).index(elt) + 1;
        idx > 1 ? (idx = '[' + idx + ']') : (idx = '');
        path = '/' + elt.tagName.toLowerCase() + idx + path;
    }
    return path;
}


//https://www.squidoo.com/load-jQuery-dynamically
load = function () {
    if (typeof $ == "undefined") {
        load.getScript("https://ajax.googleapis.com/ajax/libs/jquery/1.6.4/jquery.min.js");
        // do stuff when jQuery finishes loading.
        load.tryReady(0);
    }
}
load.getScript = function (filename) {
    var fileref = document.createElement('script')
    fileref.setAttribute("type", "text/javascript")
    fileref.setAttribute("src", filename)
    if (typeof fileref != "undefined") document.getElementsByTagName("head")[0].appendChild(fileref)
}
load.tryReady = function (time_elapsed) { 
    if (typeof $ == "undefined") {
        if (time_elapsed <= 5000) {
            setTimeout("load.tryReady(" + (time_elapsed + 200) + ")", 200);
        } else {
            alert("Timed out while loading jQuery.")
        }
    } else {
        startScan();
    }
}

//Start Loading jQuery
