/*  
    https://www.dailycoding.com/ 
    Topbar message plugin
*/
(function ($) {
    $.fn.showTopbarMessage = function (options) {

        var defaults = {
            background: "#FFFFFF",
            borderColor: "#000",
            border:"1px solid #000000",
            foreColor: "#000",
            height: "50px",
            fontSize: "20px",
            close: "click",
            winHeight: $(window).height() - 50
        };
        var options = $.extend(defaults, options);

        //var barStyle = " width: 100%;position: fixed;height: " + options.height + ";top: 0px;left: 0px;right: 0px;margin: 0px;display: none;";
        var barStyle = " width: 100%;position: fixed;height: " + options.height + ";top: " + options.winHeight + "px;left: 0px;right: 0px;margin: 0px;display: none;";
        var overlayStyle = "height: " + options.height + ";background-color: " + options.background + ";border-top: solid 3px " + options.borderColor + ";";
        var messageStyle = " width: 100%;position: absolute;height: " + options.height + ";top: 0px;left: 0px;right: 0px;margin: 0px;color: " + options.foreColor + ";font-weight: bold;font-size: " + options.fontSize + ";text-align: center;padding: 10px 0px";

        return this.each(function () {
            obj = $(this);

            if ($(".topbarBox").length > 0) {
                // Hide already existing bars
                $(".topbarBox").hide()
                $(".topbarBox").slideDown(200, function () {
                    $(".topbarBox").remove();
                });
            }


            var html = ""
                + "<div class='topbarBox' style='" + barStyle + "'>"
                + "  <div style='" + overlayStyle + "'>&nbsp;</div>"
                + "  <div style='" + messageStyle + "'>" + obj.html() + "</div>"
                + "</div>"

            if (options.close == "click") {
                $(html).click(function () {
                    $(this).slideUp(200, function () {
                        $(this).remove();
                    });
                }).appendTo($('body')).slideDown(200);
            }
            else {
                $(html).appendTo($('body')).slideDown(200).delay(options.close).slideDown(200, function () { $(this).remove(); });
            }

        });
    };
})(jQuery);