(function () {
    var debug = function () {
        var dispatcher = {
            enabled: !1
        },
                methods = ["log", "dir", "group", "groupCollapsed", "groupEnd", "fire", "addHandler", "getNodeName"];
        for (var i = 0; i < methods.length; i++) dispatcher[methods[i]] = function () { };
        return dispatcher
    } (),
        JSON = {},
        $ = {},
        jQuery = $,
        extractionIframe = null,
        baseURI, eventDistribute = function () { },
        inhibitSave = !1,
        thisScript, thisScriptId, clippingOn = !1,
        showAdvancedBookmarkletUI = !1,
        clipWidth, clipHeight, extractedClip = null,
        compactClip = null,
        compressedClip = null,
        clipText = null,
        boxShadow = "-3px 3px 10px rgba(0,0,0,0.4)",
        zindex = 9999999,
        boxBorder = 2,
        boxPadding = 2,
        controlScootHeight = 100,
        $control, $zoomRect, $canvasOverlay, $clippableElements, notify = null,
        promises = [],
        topMap, leftMap, widthMap, heightMap, clippableElementZIndexes, fixies = null,
        overflowData = {
            html: "",
            body: "",
            documentScrollTop: 0,
            documentScrollLeft: 0
        },
        rewriteClass = "node_314159265",
        doNotClipAttributeName = "CLIPBOARD_IGNORE",
        doNotClipAllAttributeName = "CLIPBOARD_IGNORE_ALL",
        mustClone = {
            embed: 1,
            object: 1,
            param: 1,
            table: 1,
            tbody: 1,
            thead: 1,
            th: 1,
            tr: 1,
            td: 1
        },
        idPrefix = "clipboard_widget",
        idSuffix = "_314159265",
        clipButtonID = idPrefix + "_clip_sprite" + idSuffix,
        homeButtonID = idPrefix + "_home_sprite" + idSuffix,
        closeButtonID = idPrefix + "_close_sprite" + idSuffix,
        widgetID = idPrefix + idSuffix,
        widgetLogoutID = idPrefix + "_logout" + idSuffix,
        messageID = idPrefix + "_message" + idSuffix,
        activeClassName = "clipboard_widget_active_314159265",
        cullNodeAttributeFlag = "clipboard314159265_cullnode",
        semanticBlockTagNames = {
            article: 1,
            aside: 1,
            details: 1,
            figcaption: 1,
            figure: 1,
            footer: 1,
            header: 1,
            hgroup: 1,
            menu: 1,
            nav: 1,
            section: 1
        },
        supportsRgba = !1,
        dpi = 96,
        splitString = function () {
            function cbSplit(str, separator, limit) {
                if (Object.prototype.toString.call(separator) !== "[object RegExp]") return cbSplit._nativeSplit.call(str, separator, limit);
                var output = [],
                    lastLastIndex = 0,
                    flags = (separator.ignoreCase ? "i" : "") + (separator.multiline ? "m" : "") + (separator.sticky ? "y" : ""),
                    separator2, match, lastIndex, lastLength;
                separator = new RegExp(separator.source, flags + "g"), str += "", cbSplit._compliantExecNpcg || (separator2 = new RegExp("^" + separator.source + "$(?!\\s)", flags));
                if (limit === undefined || +limit < 0) limit = Infinity;
                else {
                    limit = Math.floor(+limit);
                    if (!limit) return []
                }
                while (match = separator.exec(str)) {
                    lastIndex = match.index + match[0].length;
                    if (lastIndex > lastLastIndex) {
                        output.push(str.slice(lastLastIndex, match.index)), !cbSplit._compliantExecNpcg && match.length > 1 && match[0].replace(separator2, function () {
                            for (var i = 1; i < arguments.length - 2; i++) arguments[i] === undefined && (match[i] = undefined)
                        }), match.length > 1 && match.index < str.length && Array.prototype.push.apply(output, match.slice(1)), lastLength = match[0].length, lastLastIndex = lastIndex;
                        if (output.length >= limit) break
                    }
                    separator.lastIndex === match.index && separator.lastIndex++
                }
                return lastLastIndex === str.length ? (lastLength || !separator.test("")) && output.push("") : output.push(str.slice(lastLastIndex)), output.length > limit ? output.slice(0, limit) : output
            }
            return cbSplit._compliantExecNpcg = /()??/.exec("")[1] === undefined, cbSplit._nativeSplit = String.prototype.split, cbSplit
        } ();

    function flagAsUnclippable($element) {
        $element.attr(doNotClipAttributeName, "true")
    }
    function shouldClip($element) {
        return !$element.attr(doNotClipAttributeName) && !$element.attr(doNotClipAllAttributeName) && !isEasyXdmArtifact($element)
    }
    function isEasyXdmArtifact($element) {
        return $element.attr("id") && /^easyXDM_default\d*_provider$/.test($element.attr("id"))
    }
    function nodeText(node) {
        return node.textContent ? node.textContent : node.nodeValue ? node.nodeValue : ""
    }
    function load(url, callback) {
        var head = document.getElementsByTagName("head")[0],
            script = document.createElement("script"),
            done = !1;
        script.type = "text/javascript", script.src = url, head.appendChild(script), script.onload = script.onreadystatechange = function () {
            var isReady = !this.readyState || this.readyState === "loaded" || this.readyState === "complete";
            !done && isReady && (done = !0, callback(), script.onload = script.onreadystatechange = null, head.removeChild(script))
        }
    }
    function createIframe(parent) {
        var iframe = document.createElement("iframe");
        try {
            parent || (parent = document.body), parent.appendChild(iframe), iframe.doc = null, iframe.contentWindow ? iframe.doc = iframe.contentWindow.document : iframe.contentDocument ? iframe.doc = iframe.contentDocument : iframe.document && (iframe.doc = iframe.document)
        } catch (except) {
            iframe = $("<div>")
        }
        return iframe.doc && (iframe.doc.open(), iframe.doc.write("<!doctype html>"), iframe.doc.close(), iframe = $(iframe), iframe.attr("scrolling", "no"), iframe.attr("frameborder", "0")), iframe.css({
            left: "-10000px",
            visibility: "hidden",
            border: "none",
            position: "absolute",
            opacity: 0,
            overflow: "hidden",
            margin: 0,
            padding: 0
        }), iframe
    }
    function makeRecord(fields) {
        var rec = {
            source: window.location.href,
            title: $("title").html() || window.location.href,
            date: (new Date).toString(),
            version: "0.1"
        };
        for (var x in fields) rec[x] = fields[x];
        return rec
    }
    function shrinkEffect(source, callback) {
        var easing = "easeInOutQuad",
            time = 200 + source.width() * source.height() / 5e3,
            params = {
                left: [$control.offset().left - $(document).scrollLeft(), easing],
                top: [$control.offset().top - $(document).scrollTop(), easing],
                width: [$control.width(), easing],
                height: [$control.height(), easing],
                opacity: [0, easing]
            };
        source.animate(params, time, callback)
    }
    function removeFilenameFromUrl(url) {
        var parts, last, prefix;
        return url ? (prefix = "", url.charAt(0) === "/" && (prefix = "/", url = url.substring(1)), parts = url.split("/"), last = parts[parts.length - 1], last.indexOf(".") !== -1 && parts.pop(), prefix + parts.join("/")) : ""
    }
    function normalizeURI(href) {
        function prefixedBy(a, b) {
            return a.substring(0, b.length) == b
        }
        if (!href) return "";
        if (href.substring(0, 2) === "//") href = "https://" + href.substring(2);
        else if (href.charAt(0) === "/") href = document.location.protocol + "//" + document.location.host + href;
        else if (href.substring(0, 2) === "./") href = document.location.protocol + "//" + document.location.host + href.substring(1);
        else if (!(prefixedBy(href, "https://") || prefixedBy(href, "https://") || prefixedBy(href, "ftp://") || prefixedBy(href, "file://") || prefixedBy(href, "data:"))) {
            var path = document.location.pathname;
            path = removeFilenameFromUrl(path), href = href[0] == "/" ? href.substring(1) : href, href = document.location.protocol + "//" + document.location.host + "/" + path + "/" + href
        }
        return href
    }
    function quoteText(text) {
        return text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    }
    function _getCSS($element, attrs, rect) {
        var css = {},
            val;
        for (var i = 0; i < attrs.length; i++) {
            var attr = attrs[i];
            try {
                val = $element.css(attr)
            } catch (exception) {
                val = null
            }
            val && (css[attr] = val)
        }
        return css
    }
    function getBackgroundCSS($source, rect) {
        var css = _getCSS($source, ["background-clip", "background-color", "background-image", "background-origin", "background-repeat"]);
        return handleBackgroundImage(css, $source, rect), css
    }
    function convertUnitsToPixels(value, fontSize) {
        var match;
        if (match = /^(?:-?[\d\.]+(px|em|cm|pt|in)|0%?)$/.exec(value)) {
            if (match[1]) {
                var numericValue = parseFloat(value);
                switch (match[1]) {
                    case "px":
                        return numericValue;
                    case "em":
                        return numericValue * fontSize;
                    case "cm":
                        return numericValue * dpi / 2.54;
                    case "in":
                        return numericValue * dpi;
                    case "pt":
                        return numericValue * dpi / 72
                }
            }
            return 0
        }
        return !1
    }
    function handleBackgroundImage(cssMap, $source, rect) {
        function normalizeBackgroundPosition(value, image, orientation) {
            var convertedValue = convertUnitsToPixels(value, parseFloat($source.css("font-size")));
            if (convertedValue !== !1) return convertedValue;
            var originalValue = value;
            switch (value) {
                case "left":
                case "top":
                    return 0;
                case "right":
                case "bottom":
                    value = "100%";
                    break;
                case "center":
                    value = "50%"
            }
            if (!/^-?[\d\.]+%$/.test(value)) return 0;
            var percentageValue = parseFloat(value);
            image = /url\(['"]?(.+?)['"]?\)/i.exec(image)[1];
            if (!image) return 0;
            var $hiddenImg = $("<img/>").attr("src", image).appendTo($(extractionIframe).contents().find("body")),
                imageWidth = $hiddenImg.width(),
                imageHeight = $hiddenImg.height();
            $hiddenImg.remove(), $hiddenImg = null;
            var imageValue = orientation === "horizontal" || originalValue in {
                left: 1,
                right: 1
            } ? imageWidth : imageHeight,
                sourceValue = orientation === "horizontal" || originalValue in {
                    left: 1,
                    right: 1
                } ? $source.width() : $source.height();
            return percentageValue / 100 * sourceValue - imageValue * (percentageValue / 100)
        }
        var image = $source.css("background-image");
        if (image === "none") return;
        var position = $source.bgPosition().split(" ");
        typeof position[1] == "undefined" && (position[1] = "center");
        if (position.length >= 3) return;
        position[0] = normalizeBackgroundPosition(position[0], image, "horizontal"), position[1] = normalizeBackgroundPosition(position[1], image, "vertical");
        var sourceOffset = $source.offset();
        if (sourceOffset.top < rect.top || sourceOffset.left < rect.left) position[0] -= rect.left - sourceOffset.left, position[1] -= rect.top - sourceOffset.top;
        position[0] += "px", position[1] += "px", cssMap["background-image"] = image, cssMap["background-position"] = position.join(" "), cssMap["background-attachment"] = "scroll"
    }
    function handlePositionalOffsets($source, $element, rect) {
        var position = $source.css("position");
        if (position === "static" || position === "fixed") return;
        $element.css("position", position);
        if (position === "relative") return;
        var elementOffset = $source.offset();
        debug.groupCollapsed("handling positional offsets");
        var originalPosition = $source.position();
        if (position === "relative" && originalPosition.top === 0 && originalPosition.left === 0) return;
        debug.log($source), debug.groupCollapsed("original position"), debug.dir(originalPosition), debug.groupEnd();
        var newWidth = $source.width() + 1,
            newHeight = $source.height();
        newWidth = $source.width() - Math.max(0, elementOffset.left + $source.width() - rect.right), newWidth = Math.min(rect.width, newWidth), newHeight = $source.height() - Math.max(0, elementOffset.top + $source.height() - rect.bottom), newHeight = Math.min(rect.height, newHeight), $element.width(newWidth), $element.height(newHeight);
        var parentOffset = $source.offsetParent().offset();

        function setPositionalProperty(property, opposite, length) {
            elementOffset[property] < rect[property] && parentOffset[property] < rect[property] ? $element.css(property, 0) : parentOffset[property] >= rect[property] && parentOffset[property] + length <= rect[opposite] ? $element.css(property, originalPosition[property]) : (debug.log("computing difference for " + property), debug.dir(originalPosition), debug.dir(parentOffset), debug.dir(rect), $element.css(property, originalPosition[property] - (rect[property] - parentOffset[property])))
        }
        setPositionalProperty("left", "right", $source.width()), setPositionalProperty("top", "bottom", $source.height()), debug.groupCollapsed("new position"), debug.dir({
            left: $element.css("left"),
            top: $element.css("top")
        }), debug.groupEnd(), debug.groupEnd()
    }
    function getNonPositionalCSS($element) {
        var attrs = ["-moz-border-bottom-left-radius", "-moz-border-bottom-right-radius", "-moz-border-top-left-radius", "-moz-border-top-right-radius", "-moz-box-shadow", "-webkit-border-bottom-left-radius", "-webkit-border-bottom-right-radius", "-webkit-border-top-left-radius", "-webkit-border-top-right-radius", "-webkit-box-shadow", "background-clip", "background-color", "background-image", "background-origin", "background-position", "background-repeat", "border-bottom-color", "border-bottom-left-radius", "border-bottom-right-radius", "border-bottom-style", "border-bottom-width", "border-collapse", "border-left-color", "border-left-style", "border-left-width", "border-right-color", "border-right-style", "border-right-width", "border-spacing", "border-top-color", "border-top-left-radius", "border-top-right-radius", "border-top-style", "border-top-width", "box-shadow", "caption-side", "clear", "clip", "color", "content", "counter-increment", "counter-reset", "cursor", "direction", "display", "empty-cells", "float", "font-family", "font-size", "font-style", "font-variant", "font-weight", "letter-spacing", "line-height", "list-style-image", "list-style-position", "list-style-type", "margin-bottom", "margin-left", "margin-right", "margin-top", "marker-offset", "max-height", "max-width", "min-height", "min-width", "opacity", "outline-color", "outline-style", "outline-width", "overflow-x", "overflow-y", "padding-bottom", "padding-left", "padding-right", "padding-top", "page-break-after", "page-break-before", "page-break-inside", "quotes", "table-layout", "text-align", "text-decoration", "text-indent", "text-transform", "vertical-align", "visibility", "white-space", "word-spacing", "z-index"],
            css = {},
            val;
        for (var i = 0; i < attrs.length; i++) {
            var attr = attrs[i];
            if (attr === "line-height" && $element[0].currentStyle) val = $element[0].currentStyle.lineHeight;
            else if (attr === "margin-right" && document.defaultView && document.defaultView.getComputedStyle) val = document.defaultView.getComputedStyle($element[0], null).getPropertyValue("margin-right");
            else if (attr.indexOf("background-position") === 0) val = $element.bgPosition();
            else try {
                val = $element.css(attr)
            } catch (e) {
                val = null
            }
            val && (css[attr] = val)
        }
        return css.display == "inline" && css.clip && delete css.clip, css
    }
    function setCSS($element, cssProperties) {
        for (var cssProperty in cssProperties) {
            if (!cssProperties.hasOwnProperty(cssProperty)) continue;
            $element.css(cssProperty) != cssProperties[cssProperty] && $element.css(cssProperty, cssProperties[cssProperty])
        }
    }
    function doDomainSpecificHacks(clip) {
        function outerHTML(node) {
            return node.outerHTML || (new XMLSerializer).serializeToString(node)
        }
        function ytEmbedString(id, width, height) {
            return '<iframe src="https://www.youtube.com/embed/{id}?wmode=transparent" width="{w}" height="{h}" frameborder="0" allowfullscreen></iframe>'.replace("{id}", id).replace("{w}", width).replace("{h}", height)
        }
        var embed;
        switch (document.domain) {
            case "www.youtube.com":
                embed = $("embed", $(clip));
                if (embed) for (var i = 0; i < embed.length; ++i) {
                    var vids = outerHTML($("embed")[0]).match(/video_id=([^&\"]+)/g);
                    if (vids && vids.length >= 1) {
                        var last = vids[vids.length - 1],
                        videoId = /video_id=([^&\"]+)/.exec(last);
                        if (videoId && videoId.length >= 2) {
                            var ytString = ytEmbedString(videoId[1], embed[i].offsetWidth, embed[i].offsetHeight);
                            $(embed[i]).replaceWith(ytString)
                        }
                    }
                }
                break;
            case "www.ted.com":
                embed = $("object", $(clip));
                if (embed && embed.length == 1) {
                    var embedCode = $("#embedCode");
                    if (embedCode && embedCode.length == 1) {
                        var code = embedCode.val();
                        embed.replaceWith(code)
                    }
                }
        }
    }
    var useSearchCull = !0;

    function cullSearchSpace(rect) {
        if (!useSearchCull) return [];
        var culledElements = [];

        function isOutsideRect(i) {
            return leftMap[i] > rect.right && leftMap[i] + widthMap[i] < rect.left && topMap[i] > rect.bottom && topMap[i] + heightMap[i] < rect.top
        }
        for (var i = 0, max = $clippableElements.length; i < max; ++i) {
            if (isOutsideRect(i)) continue;
            var n = $clippableElements[i],
                tagName = n[0].tagName.toLowerCase();
            while (n && n.length && !n.attr(cullNodeAttributeFlag)) {
                n.attr(cullNodeAttributeFlag, !0), culledElements.push(n[0]), tagName = n[0].tagName.toLowerCase();
                if (tagName === "html") if (n.data("inIFrame" + idSuffix)) n = n.data("inIFrame" + idSuffix);
                else break;
                else n = n.parent()
            }
        }
        return culledElements
    }
    function extract() {
        debug.fire("extraction");
        var css = {
            left: -1e4,
            visibility: "hidden",
            border: "none",
            position: "absolute",
            opacity: 0,
            overflow: "hidden",
            margin: 0,
            padding: 0,
            background: "red"
        },
            origin = null,
            rect = {},
            widthAdjustment = +!!$.browser.mozilla || +!!$.browser.msie;
        if (downz && downz != lastz) rect.left = Math.min(leftMap[lastz], leftMap[downz]) - 1, rect.top = Math.min(topMap[lastz], topMap[downz]) - 1, rect.right = Math.max(leftMap[lastz] + widthMap[lastz], leftMap[downz] + widthMap[downz]) + 1, rect.bottom = Math.max(topMap[lastz] + heightMap[lastz], topMap[downz] + heightMap[downz]) + 1, rect.width = rect.right - rect.left + widthAdjustment, rect.height = rect.bottom - rect.top, eventDistribute("drag");
        else {
            origin = $clippableElements[lastz];
            if (!origin) return null;
            rect = origin.offset(), rect.width = origin.outerWidth(!0) + widthAdjustment, rect.height = origin.outerHeight(!0), rect.right = rect.left + rect.width - widthAdjustment, rect.bottom = rect.top + rect.height, origin = null, eventDistribute("click")
        }
        downz = 0, css.width = rect.width, css.height = rect.height;
        var culledElements = cullSearchSpace(rect);
        extractionIframe.css(css);
        var clip = $('<div class="clipping_314159265"></div>');
        extractionIframe[0].doc ? $(extractionIframe[0].doc.body).append(clip) : extractionIframe.append(clip), extractRect($("html"), clip, rect, origin), doDomainSpecificHacks(clip);
        if (extractionIframe[0].doc) {
            var $iframeBody = $(extractionIframe[0].doc.body),
                $root = $iframeBody.children(":first-child").children(":first-child").children(":first-child").children(":first-child");
            $root.width($iframeBody.outerWidth(!0)), $root.height($iframeBody.outerHeight(!0))
        }
        for (var i = 0, max = culledElements.length; i < max; ++i) culledElements[i].removeAttribute(cullNodeAttributeFlag);
        return {
            clip: clip,
            rect: rect
        }
    }
    function extractRect(source, dest, rect, origin) {
        if (source.css("visibility") === "hidden" || source.css("opacity") === 0) return debug.fire("notClipping", source), 0;
        debug.fire("rectangleExtraction", source, dest, rect, origin);
        if (origin && origin[0] === source[0]) return extractFull(source, dest, !0, !1, rect);
        var sourcePosition = source.offset();
        sourcePosition.width = source.width(), sourcePosition.height = source.height(), sourcePosition.right = sourcePosition.left + sourcePosition.width, sourcePosition.bottom = sourcePosition.top + sourcePosition.height, debug.groupCollapsed("source position and dimensions"), debug.dir(sourcePosition), debug.groupEnd();
        var overlap = {
            left: Math.max(rect.left, sourcePosition.left),
            right: Math.min(rect.right, sourcePosition.right),
            top: Math.max(rect.top, sourcePosition.top),
            bottom: Math.min(rect.bottom, sourcePosition.bottom)
        };
        overlap.width = Math.max(overlap.right - overlap.left, 0), overlap.height = Math.max(overlap.bottom - overlap.top, 0);
        var overlapArea = overlap.width * overlap.height,
            sourceArea = sourcePosition.width * sourcePosition.height,
            dx = rect.left - sourcePosition.left,
            dy = rect.top - sourcePosition.top,
            dist = Math.sqrt(dx * dx + dy * dy);
        return debug.groupCollapsed("overlap calculations"), debug.dir(overlap), debug.log("source area: %d", sourceArea), debug.log("overlap area: %d", overlapArea), debug.log("distance between x and y coordinates: %d", dist), debug.groupEnd(), !origin && sourceArea && overlapArea == sourceArea ? (debug.log("(no click) overlap area is equal to source area"), extractFull(source, dest, !0, !1, rect)) : !origin && sourceArea && overlapArea >= sourceArea * .95 ? (debug.log("overlap area is within 95% of source area"), extractFull(source, dest, !0, !1, rect)) : !origin && sourceArea === 0 && dist < 1e4 && sourcePosition.left >= rect.left && sourcePosition.left <= rect.right && sourcePosition.top >= rect.top && sourcePosition.top <= rect.bottom ? (debug.log("<br> detected"), extractFull(source, dest, !0, !1, rect)) : origin && sourceArea && sourceArea == overlapArea ? (debug.log("(click) overlap area is equal to source area"), extractPart(source, dest, rect, origin)) : extractPart(source, dest, rect, origin)
    }
    var applyClearfixIfNeeded = function () {
        function floatFilter() {
            return $(this).css("float") === "left" || $(this).css("float") === "right"
        }
        return function ($el) {
            $el.children().is(floatFilter) && $el.append('<div style="clear:both;"></div>')
        }
    } (),
        tagsToConvertToDiv = {
            body: 1,
            center: 1,
            iframe: 1,
            html: 1
        };

    function extractPart(source, dest, rect, origin) {
        debug.fire("partialExtraction", source, dest);
        var tag = source[0].tagName.toLowerCase();
        tag = tag in semanticBlockTagNames ? "div" : tag;
        var copy;
        tag in mustClone ? (copy = $(source[0].cloneNode(!1)), copy.removeAttr("class"), copy.removeAttr("id"), copy.removeAttr("width"), copy.removeAttr("height"), copy.removeAttr(cullNodeAttributeFlag), copy.text("")) : tag in tagsToConvertToDiv ? copy = $("<div>") : copy = $("<" + tag + ">"), dest.append(copy), tag === "html" ? (copy.width(rect.width).height(rect.height), copy.wrap('<div style="position:relative;z-index:0;overflow:hidden;"><div style="position:relative;z-index:-9999;background-color:#FFF;">'), copy.css({
            margin: 0
        })) : tag === "table" && (copy.attr("cellpadding", "0"), copy.attr("cellspacing", "0"), copy.attr("border", "0"), copy.css("border-collapse", "collapse")), setCSS(copy, {
            "margin-top": 0,
            "margin-bottom": 0,
            "margin-left": 0,
            "margin-right": 0,
            "padding-top": 0,
            "padding-bottom": 0,
            "padding-left": 0,
            "padding-right": 0
        }), setCSS(copy, getBackgroundCSS(source, rect)), source.css("position") !== "fixed" && handlePositionalOffsets(source, copy, rect), setCSS(copy, _getCSS(source, ["float", "clear", "font-size", "line-height", "font-family", "color"]));
        var want = 0;
        if (tag !== "iframe") {
            var kids = source.children();
            if (kids.length) {
                debug.groupCollapsed("extracting children (" + kids.length + ")");

                function shouldExtract($kid) {
                    return !useSearchCull || $kid.attr(cullNodeAttributeFlag) || $kid.is("br") || $kid.hasClass(rewriteClass) && $kid.find("br").length
                }
                for (var i = 0, $kid; i < kids.length; i++) $kid = $(kids[i]), shouldExtract($kid) && (want = Math.max(want, extractRect($kid, copy, rect, origin)));
                applyClearfixIfNeeded(copy), debug.groupEnd()
            }
        } else if (source[0].contentDocument) {
            var $iframeBody = $("body", source[0].contentDocument);
            want = extractRect($iframeBody, copy, rect, origin)
        }
        if (!want) copy.remove();
        else if (want === 1) {
            var flt = source.css("float"),
                clr = source.css("clear"),
                isclr = clr == "left" || clr == "right" || clr == "both",
                isflt = flt == "left" || flt == "right";
            origin && (isflt || isclr) ? (isflt && copy.css({
                width: source.width(),
                height: Math.min(source.height(), rect.height)
            }), copy.children().remove(), copy.css("background", "none")) : copy.remove()
        }
        return want
    }
    function extractFull(source, dest, init, inPre, rect) {
        if (!shouldClip(source)) return debug.fire("notClipping", source), 0;
        debug.fire("fullExtraction", source, dest);
        if (source[0].nodeType === 3) return dest.append($(document.createTextNode(source[0].nodeValue))), 2;
        if (source.hasClass(rewriteClass)) {
            var nodes = source[0].childNodes,
                extracted = 0;
            if (nodes.length) {
                debug.groupCollapsed("extracting children (" + nodes.length + ")");
                for (i = 0; i < nodes.length; i++) extracted = Math.max(extracted, extractFull($(nodes[i]), dest, init, inPre, rect));
                applyClearfixIfNeeded(dest), debug.groupEnd()
            }
            return extracted
        }
        var tag = source[0].tagName.toLowerCase();
        tag = tag in semanticBlockTagNames || tag === "body" ? "div" : tag, inPre = inPre || tag == "pre";
        if (tag === "style") return debug.fire("notClipping", source), 2;
        var copy;
        if (tag === "input") {
            if (source.attr("type") === "hidden") return 2;
            copy = $("<" + tag + ' type="' + source.attr("type") + '">')
        } else tag in mustClone ? (copy = $(source[0].cloneNode(!1)), copy.removeAttr("class"), copy.removeAttr("id"), copy.removeAttr("width"), copy.removeAttr("height"), copy.removeAttr(cullNodeAttributeFlag)) : copy = $("<" + tag + ">");
        var css = getNonPositionalCSS(source);
        if (source.css("position") == "fixed") return 0;
        init && (css.position = "static"), dest.append(copy), setCSS(copy, css);
        switch (tag) {
            case "input":
                copy.attr("value", source.attr("value"));
                break;
            case "button":
                copy.height(source.outerHeight()).width(source.outerWidth());
                break;
            case "img":
                var val;
                copy.attr("src", normalizeURI(source.attr("src"))), (val = source.attr("width")) && copy.attr("width", val), (val = source.attr("height")) && copy.attr("height", val);
                break;
            case "a":
                copy.attr("href", normalizeURI(source.attr("href")));
                break;
            case "iframe":
                copy.attr("src", normalizeURI(source.attr("src")));
                break;
            case "embed":
                copy.attr("src", normalizeURI(copy.attr("src")));
                var src = copy.attr("src"),
                match;
                (match = src.match(/(autoplay)=true/i)) && copy.attr("src", copy.attr("src") + "&" + match[1] + "=false");
                break;
            case "object":
                var val = copy.attr("data");
                val && copy.attr("data", normalizeURI(copy.attr("data")));
                break;
            case "param":
                switch (copy.attr("name").toLowerCase()) {
                    case "flashvars":
                        copy.attr("value", copy.attr("value") + "&autostart=false");
                        break;
                    case "play":
                        copy.attr("value", "false");
                        break;
                    case "autostart":
                        copy.attr("value", "false");
                        break;
                    case "movie":
                        copy.attr("value", normalizeURI(copy.attr("value")))
                }
        }
        var parent = source.parent();
        tag === "img" && parent[0].tagName.toLowerCase() === "td" && parent.contents().length === 1 && copy.css("display", "block");
        if (tag !== "iframe") {
            var kids = source[0].childNodes;
            if (kids.length) {
                debug.group("extracting children (" + kids.length + ")");
                for (var i = 0, n = kids.length; i < n; i++) if (kids[i].nodeType === 3) {
                    var text = quoteText(nodeText(kids[i]));
                    copy.append($(document.createTextNode(text)))
                } else kids[i].nodeType === 1 && extractFull($(kids[i]), copy, !1, inPre, rect);
                debug.groupEnd()
            }
        } else source[0].contentDocument && dest.append(copy);
        return css = _getCSS(source, ["width", "height"]), css.width.indexOf("%") !== -1 && (css.width = source.width() * parseFloat(css.width) / 100), setCSS(copy, css), handlePositionalOffsets(source, copy, rect), 2
    }
    function compactCSS(css) {
        function mergeIfAllPresent(attrs, merge) {
            var i;
            for (i = 0; i < attrs.length; i++) if (!(attrs[i] in css)) return;
            var val = [];
            for (i = 0; i < attrs.length; i++) val.push(css[attrs[i]]);
            css[merge] = val.join(" ");
            for (i = 0; i < attrs.length; i++) delete css[attrs[i]]
        }
        function mergeIfAllEqual(attrs, merge) {
            var i;
            for (i = 0; i < attrs.length; i++) if (!(attrs[i] in css)) return;
            var val = css[attrs[0]];
            for (i = 1; i < attrs.length; i++) if (css[attrs[i]] != val) return;
            css[merge] = val;
            for (i = 0; i < attrs.length; i++) delete css[attrs[i]]
        }
        function rewriteBackgroundRepeat() {
            if ("background-repeat" in css && css["background-repeat"].indexOf(" ") !== -1) {
                var repeats = css["background-repeat"].split(" ");
                repeats[0] === "repeat" ? repeats[1] === "repeat" ? css["background-repeat"] = "repeat" : css["background-repeat"] = "repeat-x" : repeats[1] === "repeat" ? css["background-repeat"] = "repeat-y" : css["background-repeat"] = "no-repeat"
            }
        }
        mergeIfAllEqual(["margin-top", "margin-right", "margin-bottom", "margin-left"], "margin"), mergeIfAllPresent(["margin-top", "margin-right", "margin-bottom", "margin-left"], "margin"), mergeIfAllEqual(["padding-top", "padding-right", "padding-bottom", "padding-left"], "padding"), mergeIfAllPresent(["padding-top", "padding-right", "padding-bottom", "padding-left"], "padding");
        var edges = ["top", "right", "bottom", "left"];
        for (var i = 0; i < edges.length; i++) {
            var e = edges[i];
            mergeIfAllPresent(["border-" + e + "-width", "border-" + e + "-style", "border-" + e + "-color"], "border-" + e)
        }
        var props = ["width", "style", "color"];
        for (var i = 0; i < props.length; i++) {
            var p = props[i];
            mergeIfAllEqual(["border-top-" + p, "border-right-" + p, "border-bottom-" + p, "border-left-" + p], "border-" + p)
        }
        mergeIfAllEqual(["border-top", "border-right", "border-bottom", "border-left"], "border");
        if ("background-position-x" in css || "background-position-y" in css) {
            var x = css["background-position-x"] || "0",
                y = css["background-position-y"] || "0";
            css["background-position"] = x + " " + y, delete css["background-position-x"], delete css["background-position-y"]
        }
        var bgProps = ["background-color", "background-image", "background-repeat", "background-attachment", "background-position"];
        rewriteBackgroundRepeat();
        for (var i = 0; i < bgProps.length; i++) css[bgProps[i]] == "initial" && delete css[bgProps[i]];
        mergeIfAllPresent(["background-color", "background-image", "background-repeat", "background-attachment", "background-position"], "background"), mergeIfAllPresent(["font-style", "font-weight", "font-size", "line-height", "font-family"], "font");
        if ("font" in css) {
            var x = css.font.split(" ");
            css.font = x.slice(0, 3).join(" ") + "/" + x.slice(3).join(" ")
        }
        mergeIfAllPresent(["list-style-type", "list-style-position", "list-style-image"], "list-style"), mergeIfAllPresent(["outline-width", "outline-style", "outline-color"], "outline");
        var prefix = ["", "-webkit-", "-moz-"];
        for (var i = 0; i < prefix.length; i++) {
            var p = prefix[i];
            mergeIfAllEqual([p + "border-top-right-radius", p + "border-top-left-radius", p + "border-bottom-right-radius", p + "border-bottom-left-radius"], p + "border-radius")
        }
        function RGBtoHex(R, G, B) {
            return "#" + toHex(R) + toHex(G) + toHex(B)
        }
        function toHex(decimal) {
            return decimal = parseInt(decimal), decimal === 0 ? "00" : (decimal = Math.max(0, decimal), decimal = Math.min(decimal, 255), decimal = Math.round(decimal), "0123456789ABCDEF".charAt((decimal - decimal % 16) / 16) + "0123456789ABCDEF".charAt(decimal % 16))
        }
        for (var property in css) {
            if (!css.hasOwnProperty(property)) continue;
            if (css[property] == "auto") delete css[property];
            else if (css[property].substring(0, 4) == "rgb(") {
                var rgb = css[property].substring(4, css[property].length - 1).split(",");
                rgb = RGBtoHex(rgb[0], rgb[1], rgb[2]), rgb[1] == rgb[2] && rgb[3] == rgb[4] && rgb[5] == rgb[6] && (rgb = "#" + rgb[2] + rgb[4] + rgb[6]), css[property] = rgb
            }
        }
    }
    function compactHTML(elem) {
        var $e = $(elem),
            styleRE = /(<[^<>]*)(style=\")([^\"]*)(\")/gim,
            html = $e.html(),
            match = styleRE.exec(html),
            start = 0,
            out = [];
        while (match) {
            out.push(html.substring(start, match.index)), out.push(match[1]), out.push(match[2]);
            var css = {},
                attrValRE = /\s*([^:]+)\s*:\s*([^;]+)\s*;?/gim,
                attvals = attrValRE.exec(match[3]);
            while (attvals) css[attvals[1].toLowerCase()] = attvals[2], attvals = attrValRE.exec(match[3]);
            compactCSS(css);
            for (var a in css) {
                if (!css.hasOwnProperty(a)) continue;
                out.push(a + ":" + css[a] + ";")
            }
            out.push(match[4]), start = styleRE.lastIndex, match = styleRE.exec(html)
        }
        return out.push(html.substring(start)), out = out.join(""), out
    }
    function clipCancel() {
        eventDistribute("clipCancel"), downz = 0, clippingOn = !1, $("#" + clipButtonID).removeClass(activeClassName), unbindEventHandlers(), removeAnimatedBox()
    }
    function unbindEventHandlers() {
        $canvasOverlay && $canvasOverlay.unbind("mousemove").unbind("mouseup").unbind("mousedown"), $(document).unbind("keyup.CLIPBOARD")
    }
    function removeAnimatedBox() {
        $zoomRect && $zoomRect.remove(), $canvasOverlay && $canvasOverlay.remove(), retireFixedPositioning($canvasOverlay), $zoomRect = $canvasOverlay = null
    }
    function dumpUIstyle(ui) {
        function dumpUIofElem(elem, doc) {
            var id = elem.attr("id");
            if (id) {
                doc.write("#", id + " {\n");
                var css = getNonPositionalCSS(elem);
                compactCSS(css);
                for (var a in css) {
                    if (!css.hasOwnProperty(a)) continue;
                    doc.write("  " + a + ": " + css[a] + ";\n")
                }
                doc.write("}\n\n");
                var child = elem.children();
                for (var i = 0, n = child.length; i < n; i++) dumpUIofElem($(child[i]), doc)
            }
        }
        var win = window.open("", "win");
        win.document.open("text/html", "replace"), win.document.write("<html><body><pre>"), dumpUIofElem(ui, win.document), win.document.write("</pre></body></html>"), win.document.close()
    }
    function stopPropagatingKeyEvents($element) {
        $element.bind("keypress keydown keyup", function (e) {
            if (e.keyCode === 27 || e.keyCode === 13) return;
            e.stopImmediatePropagation()
        })
    }
    function showReview(clip, clipData) {
        debug.fire("reviewDialogOpened", clip), clip = clip.replace(/&amp;/gm, "&"), $control.fadeOut(300);
        var $htmlDoc = $("html");
        overflowData.html = $htmlDoc.css("overflow"), overflowData.body = $(document.body).css("overflow"), overflowData.documentScrollTop = $(document).scrollTop(), overflowData.documentScrollLeft = $(document).scrollLeft();
        var $overlay = $("<div>").css({
            left: 0,
            top: 0,
            width: "150%",
            height: "100%",
            "z-index": zindex + 1,
            background: "#000",
            opacity: 0
        }).appendTo(document.body).animate({
            opacity: .8
        }, 500, "easeInOutQuad");
        assureFixedPositioning($overlay);
        var windowWidth = $(window).width(),
            windowHeight = $(window).height(),
            clipWidth = clipData.dimensions.width,
            clipHeight = clipData.dimensions.height + 20,
            chromeHeight = 160,
            desiredHeight = clipHeight + chromeHeight,
            width = Math.min(Math.max(600, clipWidth + 50), windowWidth - 100),
            height = Math.min(desiredHeight, windowHeight - 100),
            $reviewDialog = $('<div id="clip_review_314159">'),
            $reviewWrapper = $('<div id="clip_review_wrap_314159">'),
            $header = $("<div/>").attr("id", "clip_review_header").addClass("clipReviewChrome").css("height", "28px").append($("<h1/>").text("Annotate, tag or share your clip!")),
            $clipContents = $("<div/>").attr("id", "clip_review_contents").css("height", height - chromeHeight + "px");
        $("<div/>").attr("id", "clip_contents_314159").addClass("clipping_314159265").html(clip).appendTo($clipContents);
        var $controls = $("<div/>").attr("id", "clip_review_controls"),
            $annotationInput = $("<input/>").attr({
                id: "clip_review_annotate",
                placeholder: "Enter annotation, #tags, and @mentions",
                type: "text"
            }).appendTo($controls);
        stopPropagatingKeyEvents($annotationInput);
        var $publishWrapper = $("<span/>").attr("id", "clip_review_publish").append($("<span/>").attr("id", "clip_review_publish_control").addClass("clip_review_control")).append($("<span/>").attr("id", "clip_review_publish_text").text("Publish Clip")).click(function () {
            $(this).find(".clip_review_control").toggleClass("clip_review_control_active")
        });
        $controls.append($publishWrapper);
        var $footer = $('<div id="clip_review_footer">').addClass("clipReviewChrome");
        $("<span/>").attr("id", "clip_review_input_span").append($("<input/>").attr({
            type: "checkbox",
            id: "clip_review_show_adv_ui"
        })).append($("<label/>").attr({
            "for": "clip_review_show_adv_ui",
            id: "clip_review_label"
        }).text("Don't show this popup anymore")).appendTo($footer);
        var $buttonWrapper = $("<div/>").attr("id", "buttonWrap").appendTo($footer),
            $cancelButton = $("<button/>").attr("id", "clip_review_cancel").text("Cancel").appendTo($buttonWrapper),
            $saveButton = $("<button/>").attr("id", "clip_review_ok").text("Save").appendTo($buttonWrapper);

        function saveClip() {
            closeReview(!0, $htmlDoc, $overlay, $reviewDialog, clipData)
        }
        $saveButton.click(function () {
            saveClip()
        }), $cancelButton.click(function () {
            closeReview(!1, $htmlDoc, $overlay, $reviewDialog, clipData)
        }), $("#clip_review_show_adv_ui", $footer).change(function () {
            showEditUIChanged(!$(this).is(":checked"))
        }), $reviewWrapper.appendTo($reviewDialog), $header.appendTo($reviewWrapper), $clipContents.appendTo($reviewWrapper), $controls.appendTo($reviewWrapper), $footer.appendTo($reviewWrapper), $reviewDialog.appendTo("body"), assureFixedPositioning($reviewDialog), $reviewDialog.delay(200).animate({
            width: width,
            height: height,
            left: .5 * (windowWidth - width),
            top: .5 * (windowHeight - height),
            opacity: 1
        }, 500, "easeInOutQuad"), $htmlDoc.css("overflow", "hidden"), $(document.body).css("overflow", "hidden"), unbindEventHandlers(), $(document).bind("keydown.clipboard_review_dialog", function (e) {
            e.keyCode === 27 && (closeReview(!1, $htmlDoc, $overlay, $reviewDialog), $(document).unbind("keydown.clipboard_review_dialog"))
        }), $(document).bind("keyup.clipboard_review_dialog", function (e) {
            return e.keyCode === 13 && saveClip(), !1
        }), $(document).bind("keypress.clip_review_annotate", function (e) {
            return e.keyCode === 13 && saveClip(), !1
        })
    }

    function closeReview(shouldSendToServer, $htmlDoc, $overlay, $reviewDialog, clipData) {
        $htmlDoc.css("overflow", overflowData.html), $(document.body).css("overflow", overflowData.body), $(document).scrollTop(overflowData.documentScrollTop), $(document).scrollLeft(overflowData.documentScrollLeft), $overlay.fadeOut(200, function () {
            $overlay.remove()
        }), $reviewDialog.fadeOut(200, function () {
            $reviewDialog.remove()
        }), $control.fadeIn(200);
        if (shouldSendToServer) {
            var isPrivate = !$("#clip_review_publish_control", $reviewDialog).hasClass("clip_review_control_active");
            sendClip(isPrivate, clipData), closeHighlight(!1), eventDistribute("dialogConfirm")
        } else closeHighlight(!0), eventDistribute("dialogCancel");
        showAdvancedBookmarkletUI = !$("#clip_review_show_adv_ui", $reviewDialog).is(":checked"), $(document).unbind("keyup.clipboard_review_dialog"), $(document).unbind("keydown.clipboard_review_dialog"), $(document).unbind("keypress.clip_review_annotate")
    }
    function showEditUIChanged(yesOrNo) {
        var socket = new easyXDM.Socket({
            remote: baseURI + "utils/crossDomainPost.html",
            onReady: function () {
                var rec = {
                    showAdvancedBookmarkletUI: yesOrNo
                },
                    packet = {
                        APIpath: "/api/v1/users",
                        method: "PUT",
                        data: rec
                    },
                    data = JSON.stringify(packet);
                socket.postMessage(data)
            },
            onMessage: function (msg, origin) {
                socket.destroy()
            }
        })
    }
    function closeHighlight(restart) {
        shrinkEffect($zoomRect, function () {
            downz = 0, clippingOn = !1, $("#" + clipButtonID).removeClass(activeClassName), removeAnimatedBox(), restart && clipBegin()
        })
    }
    function sendClip(isPrivate, clipData) {
        eventDistribute("sendClip");
        if (inhibitSave) {
            showMessage("Saved");
            return
        }
        var annotation = $("#clip_review_annotate").val(),
            crossDomainLoadTimeoutId = setTimeout(function () {
                showMessage("Error :|")
            }, 5e3),
            socket = new easyXDM.Socket({
                remote: baseURI + "utils/crossDomainPost.html",
                onReady: function () {
                    clearTimeout(crossDomainLoadTimeoutId);
                    var rec = makeRecord({
                        type: "clip",
                        html: compressedClip,
                        text: clipText,
                        annotation: annotation,
                        width: clipData.dimensions.width,
                        height: clipData.dimensions.height,
                        top: clipData.rect.top,
                        left: clipData.rect.left,
                        "private": isPrivate
                    });
                    clipWidth = clipData.dimensions.width, clipHeight = clipData.dimensions.height;
                    var packet = {
                        APIpath: "/api/v1/clips",
                        method: "POST",
                        data: rec
                    },
                        data = JSON.stringify(packet);
                    socket.postMessage(data), extractedClip = null, compressedClip = null, clipText = null, compactClip = null
                },
                onMessage: function (msg, origin) {
                    try {
                        msg && msg.length > 0 ? (msg = JSON.parse(msg), msg.error || !msg.result ? finish(!0) : (addPromiseCheck({
                            key: msg.result.promiseKey,
                            tryCount: 0
                        }), finish(!1))) : finish(!0)
                    } catch (e) {
                        finish(!0)
                    }
                    function finish(error) {
                        error && showMessage("Error :/"), socket.destroy()
                    }
                }
            })
    }
    function clipEnd(e) {
        eventDistribute("clipEnd");
        if (e.which === 3) return;
        var clipData = extract();
        if (!clipData || typeof clipData.clip == "undefined") return;
        extractedClip = clipData.clip, compactClip = compactHTML(extractedClip[0]), extractionIframe[0].doc ? (clipData.dimensions = {
            width: $(extractionIframe).contents().find("body").outerWidth(!0),
            height: $(extractionIframe).contents().find("body").outerHeight(!0)
        }, $(extractionIframe[0].doc.body).children().remove()) : (clipData.dimensions = {
            width: $(extractionIframe).outerWidth(!0),
            height: $(extractionIframe).outerHeight(!0)
        }, extractionIframe.children().remove()), clipText = extractedClip.text().replace(/\s+/gm, " "), compressedClip = CLIPBOARD.data.compressText(compactClip), compressedClip.indexOf("data_clipboard3141592654") !== -1 && (compressedClip = compressedClip.replace("data_clipboard3141592654", "data"), compressedClip = compressedClip.replace(new RegExp("classid=['\"]?clsid:D27CDB6E-AE6D-11cf-96B8-444553540000['\"]?", "gi"), "")), showAdvancedBookmarkletUI ? showReview(compactClip, clipData) : (closeHighlight(!1), sendClip(!0, clipData))
    }
    function addPromiseCheck(promiseCheck) {
        promises.push(promiseCheck);
        if (promises.timeoutId) return;
        var maxRetryCount = 10,
            waitTimeInMS = 500;
        promises.timeoutId = setTimeout(function () {
            promises.timeoutId = null;
            if (promises.length === 0) return;
            var promise = promises.shift();
            $.ajax({
                dataType: "jsonp",
                url: baseURI + "api/v1/promises/" + encodeURIComponent(promise.key),
                async: !0,
                cache: !1,
                type: "GET",
                error: function () {
                    showMessage("Error :(")
                },
                success: function (msg) {
                    if (!msg) {
                        showMessage("Error :(");
                        return
                    }
                    switch (msg.status) {
                        case 0:
                            promise.tryCount > maxRetryCount ? showMessage("Error :|") : (promise.tryCount++, addPromiseCheck(promise));
                            break;
                        case 1:
                            showMessage("Saved");
                            break;
                        case -1:
                            showMessage("Error :/");
                            break;
                        case -2:
                            showMessage("Error :{")
                    }
                }
            })
        }, waitTimeInMS)
    }
    var downz = 0;

    function clipDown(e) {
        downz = lastz
    }
    var animID = 0,
        lastz = -1,
        zi = 0,
        maxZI = 0,
        zoomStartX = -1,
        zoomStartY = -1,
        curX, curY, dragThresh = 15,
        ignore = !1;

    function onMouseWheel(e, delta, deltaX, deltaY) {
        return ignore ? !1 : (zoomStartX = curX, zoomStartY = curY, zoomRect(deltaY < 0 ? 1 : -1), ignore = !0, setTimeout(function () {
            ignore = !1
        }, 200), !1)
    }
    function zoomRect(delta) {
        debug.log("zooming rectangle by " + delta), eventDistribute("zoom"), zi = Math.min(Math.max(zi + delta, 0), maxZI), animateRect(zoomStartX, zoomStartY)
    }
    function onMouseMove(e) {
        e.preventDefault(), curX = e.clientX + $(document).scrollLeft(), curY = e.clientY + $(document).scrollTop();
        if (zoomStartX < 0 || zoomStartY < 0 || (curX - zoomStartX) * (curX - zoomStartX) + (curY - zoomStartY) * (curY - zoomStartY) > dragThresh * dragThresh) zoomStartX = -1, zoomStartY = -1, zi = 0, animateRect(curX, curY)
    }
    function animateRect(x, y) {
        function elementInsideRectAt(i) {
            return x >= leftMap[i] && x <= leftMap[i] + widthMap[i] && y >= topMap[i] && y <= topMap[i] + heightMap[i]
        }
        var numClippableElements = $clippableElements.length,
            clippableElementIndices = [],
            elementRankings = [];
        maxZI = -1;
        for (var i = 0; i < numClippableElements; i++) elementRankings[i] = 1e11, elementInsideRectAt(i) && (elementRankings[i] = widthMap[i] * heightMap[i] - clippableElementZIndexes[i] * 1e6, maxZI++), clippableElementIndices[i] = i;
        clippableElementIndices.sort(function (a, b) {
            return elementRankings[a] - elementRankings[b]
        });
        var currentlyOutlineElementIndex = clippableElementIndices[zi];
        if (currentlyOutlineElementIndex === lastz || elementRankings[currentlyOutlineElementIndex] === 1e11) return;
        eventDistribute("move"), animID >= 0 && clearTimeout(animID), animID = setTimeout(function () {
            var left, top, width, height;
            if (downz && downz != currentlyOutlineElementIndex) {
                var right, bottom;
                left = Math.min(leftMap[currentlyOutlineElementIndex], leftMap[downz]), top = Math.min(topMap[currentlyOutlineElementIndex], topMap[downz]), right = Math.max(leftMap[currentlyOutlineElementIndex] + widthMap[currentlyOutlineElementIndex], leftMap[downz] + widthMap[downz]), bottom = Math.max(topMap[currentlyOutlineElementIndex] + heightMap[currentlyOutlineElementIndex], topMap[downz] + heightMap[downz]), width = right - left, height = bottom - top
            } else left = leftMap[currentlyOutlineElementIndex], top = topMap[currentlyOutlineElementIndex], width = widthMap[currentlyOutlineElementIndex], height = heightMap[currentlyOutlineElementIndex];
            $zoomRect && $zoomRect.stop(!0, !1).animate({
                opacity: supportsRgba ? 1 : .25,
                left: left - boxBorder - boxPadding,
                top: top - boxBorder - boxPadding,
                width: width,
                height: height
            }, 250, "easeOutQuad")
        }, 100), lastz = currentlyOutlineElementIndex
    }
    function clipButtonClick() {
        var button = $("#" + clipButtonID);
        button.hasClass(activeClassName) ? clipCancel() : (analyzePage(), clipBegin())
    }
    function clipBegin(force) {
        if (!$control && !force) return;
        eventDistribute("clipBegin");
        if (clippingOn) return;
        clippingOn = !0, $("#" + clipButtonID).addClass(activeClassName), $zoomRect = $("<div/>"), $zoomRect.css({
            position: "absolute",
            top: 0,
            left: 0,
            opacity: 0,
            "background-color": "rgba(64, 128, 256, 0.25)",
            padding: boxPadding,
            border: boxBorder + "px solid rgba(128,192,256,0.5)",
            "-moz-box-shadow": boxShadow,
            "-webkit-box-shadow": boxShadow,
            "box-shadow": boxShadow,
            zIndex: zindex
        }), supportsRgba || $zoomRect.css({
            "background-color": "rgb(64, 128, 256)",
            border: boxBorder + "px solid rgb(128, 192, 256)"
        }), $zoomRect.attr("id", "clipboard_zoom_314159265"), flagAsUnclippable($zoomRect), $canvasOverlay = $("<div/>"), flagAsUnclippable($canvasOverlay), $canvasOverlay.css({
            position: "fixed",
            top: 0,
            left: 0,
            opacity: 0,
            background: "#000",
            zIndex: zindex + 1,
            width: "100%",
            height: "100%"
        }).attr("id", "clipboard_canvas_314159265").mousedown(clipDown).mouseup(clipEnd).mousemove(onMouseMove), $("body").append($canvasOverlay).append($zoomRect), $canvasOverlay.mousewheel(onMouseWheel), assureFixedPositioning($canvasOverlay), $(document).bind("keyup.CLIPBOARD", function (e) {
            return e.keyCode == 27 && clipCancel(), !1
        })
    }
    function showMessage(msg) {
        if (!notify) return;
        var fade = !1;
        notify.timer && (clearTimeout(notify.timer), notify.timer = null, fade = !0), notify.html(""), notify.html(msg), notify.timer = setTimeout(function () {
            notify.slideUp(100, function () {
                notify.html("")
            })
        }, 3e3), notify.slideDown(100)
    }
    function buildControl() {
        var source = '<div id="' + widgetID + '"><ul>' + '<li id="' + clipButtonID + '" href="#"></li>' + '<li id="' + homeButtonID + '" href="#"></li>' + '<li id="' + closeButtonID + '" href="#"></li>' + '</ul><div style="color:#ddd;" id="' + messageID + '"></div></div>',
            widget = $(source);
        $("body").append(widget), flagAsUnclippable(widget), widget.data("isLoggedInControl", !0);
        var top = parseInt(widget.css("top"));
        return isNaN(top) && (top = 20), widget.css({
            top: top - controlScootHeight,
            position: "fixed",
            opacity: 0
        }), $("#" + clipButtonID, widget).click(clipButtonClick), $("#" + homeButtonID, widget).click(function (e) {
            window.open(baseURI, "clipboard_blank")
        }), $("#" + closeButtonID, widget).click(shutdown), notify = $("#" + messageID, widget), assureFixedPositioning(widget), widget.animate({
            top: top,
            opacity: .99
        }, 200), widget
    }
    function buildLoggedOutControl() {
        var source = '<div id="' + widgetLogoutID + '">Please login to Clipboard in order to clip:' + '<div style="margin-top: 8px;"><a href="' + baseURI + 'login" target="_blank">Login</a></div>' + '<div style="margin-top: 8px;"><a id="refreshWidget_3141" href="' + baseURI + 'login">Refresh</a></div>' + '<div id="' + messageID + '"></div></div>',
            widget = $(source);
        flagAsUnclippable(widget), widget.isLoggedOutControl = !0, $("body").append(widget), widget.css({
            position: "fixed",
            opacity: 0
        }), $("#" + closeButtonID, widget).click(shutdown), $("#refreshWidget_3141", widget).click(function () {
            return checkLoginStatus(), !1
        }), checkLoginStatus(!0), assureFixedPositioning(widget);
        var top = parseInt(widget.css("top"));
        return widget.css({
            top: top - controlScootHeight
        }), widget.animate({
            top: top,
            opacity: .99
        }, 200), widget
    }
    function assureFixedPositioning($elem) {
        $elem.css("position", "fixed");
        if ($elem.css("position") === "fixed") return;
        $elem.css("position", "absolute");
        var newFixie = {
            $elem: $elem,
            scrollLeft: 0,
            scrollTop: 0,
            scrollTo: function (left, top) {
                var diffX = left - this.scrollLeft,
                    diffY = top - this.scrollTop;
                this.scrollLeft = left, this.scrollTop = top;
                var css = {};
                css.top = parseInt(this.$elem.css("top")) + diffY;
                var newLeft = parseInt(this.$elem.css("left")) + diffX;
                isNaN(newLeft) || (css.left = newLeft);
                var newRight = parseInt(this.$elem.css("right")) - diffX;
                isNaN(newRight) || (css.right = newRight), this.$elem.css(css)
            }
        };
        newFixie.scrollTo($(window).scrollLeft(), $(window).scrollTop()), fixies ? fixies.push(newFixie) : (fixies = [newFixie], $(window).scroll(function () {
            var scrollLeft = $(window).scrollLeft(),
                scrollTop = $(window).scrollTop();
            $.each(fixies, function (index, fixie) {
                fixie.scrollTo(scrollLeft, scrollTop)
            })
        }))
    }
    function retireFixedPositioning($elem) {
        if (!fixies) return;
        $.each(fixies, function (index, fixie) {
            if (fixie.$elem == $elem) return fixies.splice(index, 1), !1
        })
    }
    function effectiveZindex($element) {
        var zi, p;
        try {
            zi = parseInt($element.css("z-index"))
        } catch (e) {
            return 0
        }
        if (!isNaN(zi)) return zi;
        p = $element.parents();
        for (var i = 0, n = p.length; i < n; i++) {
            $element = $(p[i]);
            try {
                zi = parseInt($element.css("z-index"))
            } catch (e) {
                return 0
            }
            if (!isNaN(zi)) return zi
        }
        return 0
    }
    function isHiddenByAncestry(e) {
        var fudge = 10,
            ew = e.width(),
            eh = e.height(),
            eoff = e.offset(),
            et = eoff.top,
            el = eoff.left,
            parents = e.parents();
        for (var i = 0, n = parents.length; i < n; i++) {
            var p = $(parents[i]);
            if (p.css("visibility") == "hidden" || p.css("opacity") === 0 || p.attr(doNotClipAllAttributeName) == "true") return !0;
            if (p.css("overflow") === "hidden") {
                var pw = p.width(),
                    ph = p.height(),
                    poff = p.offset(),
                    pt = poff.top,
                    pl = poff.left;
                if (el + ew + fudge < pl || el + fudge > pl + pw || et + eh + fudge < pt || et + fudge > pt + ph) return !0
            }
        }
        return !1
    }
    function shouldInclude(y, maxArea) {
        var nonWhiteSpaceRE = /\S/m,
            skipTags = {
                html: 1,
                head: 1,
                script: 1,
                style: 1,
                iframe: 1
            },
            $y = $(y);
        if (!shouldClip($y)) return !1;
        if ($y.css("position") === "fixed" || $y.css("visibility") === "hidden" || $y.css("opacity") === 0) return !1;
        if (y.tagName.toLowerCase() in skipTags) return !1;
        var area = $y.outerWidth() * $y.outerHeight();
        if (area < 1e3 || maxArea && area > maxArea) return !1;
        if (isHiddenByAncestry($y)) return !1;
        var text = $y.text();
        if (text.length === 0 || !nonWhiteSpaceRE.test(text)) {
            var tag = y.tagName.toLowerCase();
            return tag in {
                object: 1,
                embed: 1,
                img: 1
            } ? !0 : $y.find("img,embed,object").length ? !0 : area > 0
        }
        return !0
    }
    function analyzePage() {
        var $window = $(window),
            maxArea = $window.width() * $window.height() * .75,
            include = function (elem) {
                return shouldInclude(elem, maxArea)
            },
            offsets = [];
        $clippableElements = $.map($.grep($(":visible"), include), $);
        for (var i = 0, n = $clippableElements.length; i < n; i++) offsets.push(null);
        var iframes = $("iframe");
        for (var i = 0; i < iframes.length; ++i) try {
            if (!shouldClip($(iframes[i]))) continue;
            if (iframes[i] && iframes[i].contentDocument) {
                var ifVis = $(":visible", iframes[i].contentDocument);
                for (var k = 0; k < ifVis.length; ++k) {
                    var item = ifVis[k],
                        tagName = item.tagName.toLowerCase();
                    tagName === "html" && $(item).data("inIFrame" + idSuffix, $(iframes[i]))
                }
                var iframeItems = $.map($.grep(ifVis, include), $),
                    iframeOff = $(iframes[i]).offset();
                for (var kk = 0, n = iframeItems.length; kk < n; kk++) offsets.push(iframeOff);
                $.merge($clippableElements, iframeItems)
            }
        } catch (e) { }
        topMap = [], leftMap = [], widthMap = [], heightMap = [], clippableElementZIndexes = [];
        for (var i = 0, n = $clippableElements.length; i < n; i++) {
            var e = $clippableElements[i],
                topMargin = parseFloat(e.css("margin-top")),
                leftMargin = parseFloat(e.css("margin-left")),
                eoff = e.offset();
            topMap[i] = eoff.top - (topMargin >= 0 ? topMargin : 0), leftMap[i] = eoff.left - (leftMargin >= 0 ? leftMargin : 0), offsets[i] && (topMap[i] += offsets[i].top, leftMap[i] += offsets[i].left), widthMap[i] = e.outerWidth(!0), heightMap[i] = e.outerHeight(!0)
        }
        clippableElementZIndexes = $.map($clippableElements, effectiveZindex);
        if (debug) {
            debug.groupCollapsed("clippable elements");
            for (var i = 0; i < widthMap.length; i++) debug.groupCollapsed(debug.getNodeName($($clippableElements[i]))), debug.log($clippableElements[i]), debug.dir({
                width: widthMap[i],
                height: heightMap[i],
                left: leftMap[i],
                top: topMap[i],
                "z-index": clippableElementZIndexes[i]
            }), debug.groupEnd();
            debug.groupEnd()
        }
    }
    function shutdown() {
        if (!$control) return;
        eventDistribute("shutdown"), $zoomRect && $zoomRect.remove(), $canvasOverlay && ($canvasOverlay.remove(), retireFixedPositioning($canvasOverlay)), downz = 0, $zoomRect = $canvasOverlay = null, clippingOn = !1;
        var top = parseInt($control.css("top")),
            oldControl = $control;
        $control = null, oldControl.animate({
            top: top - controlScootHeight,
            opacity: 0
        }, function () {
            oldControl.remove(), retireFixedPositioning(oldControl)
        })
    }
    function rewritePageHTML() {
        var rewriteTag = "node";
        jQuery.browser.msie && jQuery.browser.version == "8.0" && (rewriteTag = "span");

        function rewriteNakedTextNodes(elem) {
            if (elem.hasClass(rewriteClass)) return;
            var skipTags = {
                th: 1,
                td: 1,
                head: 1,
                style: 1,
                link: 1,
                meta: 1,
                title: 1,
                base: 1,
                basefont: 1,
                isindex: 1,
                textarea: 1,
                button: 1
            },
                kids = elem[0].childNodes,
                tag = elem[0].tagName.toLowerCase(),
                hasTextNodes = !1,
                i;
            for (var i = 0; i < kids.length; i++) if (kids[i].nodeType == 3) {
                hasTextNodes = !0;
                break
            }
            if (hasTextNodes && !(tag in skipTags)) for (var i = 0; i < kids.length; i++) {
                var child = kids[i];
                if (child.nodeType == 3) {
                    var textValue = nodeText(child),
                        nonwhite = textValue.replace(/\s*/, "");
                    if (nonwhite.length > 3) {
                        var nextChild = kids[i + 1];
                        $(child).remove();
                        var pseudoParagraphs = splitString(textValue, /([\r\n]{2,})/);
                        for (var j = 0, $node; j < pseudoParagraphs.length; j++) $node = $("<" + rewriteTag + "/>").addClass(rewriteClass).text(pseudoParagraphs[j] + (pseudoParagraphs[++j] || "")), nextChild ? $node.insertBefore(nextChild) : $node.appendTo(elem)
                    }
                }
            }
            for (var i = 0; i < kids.length; i++) kids[i].nodeType == 1 && rewriteNakedTextNodes($(kids[i]))
        }
        var flashClsId = "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000",
            flashMime = "application/x-shockwave-flash",
            base = removeFilenameFromUrl(window.location.href);
        $("object").each(function () {
            var o, clsid, movie, movieUrl, objects, embeds, params, src, isFlash, type;
            o = $(this), clsid = o.attr("classid"), type = o.attr("type"), clsid === flashClsId && (o.attr("type") || o.attr("type", flashMime), movie = $("param[name=movie]", o), movie && (movieUrl = movie.attr("value"), o.attr("data") || o.attr("data_clipboard3141592654", movieUrl))), isFlash = type === flashClsId || type === flashMime, isFlash && $.support.htmlSerialize && (params = $('param[name="wmode"][value="transparent"]', o), params.length === 0 && o.prepend('<param name="wmode" value="transparent">'), params = $('param[name="base"]', o), params.each(function () {
                $(this).remove()
            }), o.prepend('<param name="base" value="' + base + '">'))
        }), embeds = $("embed"), embeds.each(function () {
            var wmode;
            $(this).attr("base", base), wmode = $(this).attr("wmode");
            if (!wmode || wmode !== "transparent") $(this).attr("wmode", "transparent"), $(this).replaceWith($(this).clone())
        }), $("br").wrap("<" + rewriteTag + ' class="' + rewriteClass + '">'), rewriteNakedTextNodes($("body"));
        var $iframes = $("iframe");
        for (var i = 0; i < $iframes.length; ++i) try {
            if ($iframes[i] && $iframes[i].contentDocument) {
                var body = $("body", $iframes[i].contentDocument);
                rewriteNakedTextNodes(body)
            }
        } catch (e) { }
    }
    var checkLoginStatusDelay = 2e3,
        checkLoginStatusCount = 0,
        checkLoginTimeoutId = null;

    function checkLoginStatus(repeat) {
        checkLoginTimeoutId && (clearTimeout(checkLoginTimeoutId), checkLoginTimeoutId = null);
        var socket = new easyXDM.Socket({
            remote: baseURI + "utils/crossDomainPost.html",
            onReady: function () {
                var packet = {
                    fetchUserData: !0
                },
                    data = JSON.stringify(packet);
                socket.postMessage(data)
            },
            onMessage: function (msg, origin) {
                if (msg && msg.length > 0) {
                    msg = JSON.parse(msg);
                    if (!msg.thirdPartyCookiesEnabled) {
                        showUnusableErrorMessage(), shutdown();
                        return
                    }
                    msg.loggedIn ? (showAdvancedBookmarkletUI = msg.showAdvancedBookmarkletUI, initLoggedIn(), clipBegin()) : (initLoggedOut(), repeat && (checkLoginTimeoutId = setTimeout(function () {
                        checkLoginStatus(!0)
                    }, checkLoginStatusDelay), ++checkLoginStatusCount, checkLoginStatusCount > 15 && (checkLoginStatusDelay *= 2)))
                }
                socket.destroy()
            }
        })
    }
    function initLoggedOut() {
        if ($control && $control.isLoggedOutControl) return;
        $control && ($control.fadeOut(), $control.remove(), $control = null), clipCancel(), $control = buildLoggedOutControl(), checkLoginStatus(!0)
    }
    function initLoggedIn() {
        if ($control && $control.data("isLoggedInControl") === !0) return;
        $control && $control.remove(), $control = buildControl()
    }
    function verifyClipperIsUsable() { }
    function main() {
        eventDistribute("main"), extractionIframe = createIframe(null);
        var style = document.createElement("link");
        style.type = "text/css", style.rel = "stylesheet", style.href = baseURI + "css/reset_0.7.43.css", extractionIframe[0].doc ? ($("head", extractionIframe[0].doc).append(style), $(extractionIframe[0].doc.body).css({
            "margin-top": 0,
            "margin-bottom": 0,
            "margin-left": 0,
            "margin-right": 0
        })) : $("head").append(style), extractionIframe.attr(doNotClipAttributeName, "true"), style = document.createElement("link"), style.type = "text/css", style.rel = "stylesheet", style.href = baseURI + "css/bookmarklet_0.7.43.css", $("head").append(style), initLoggedIn(), checkLoginStatus(!0), $(document).ready(function () {
            rewritePageHTML(), analyzePage(), clipBegin()
        })
    }
    function reload() {
        eventDistribute("reload"), clippingOn || (analyzePage(), initLoggedIn(), checkLoginStatus(!0), clipBegin())
    }
    function crc32(str, crc) {
        var table = "00000000 77073096 EE0E612C 990951BA 076DC419 706AF48F E963A535 9E6495A3 0EDB8832 79DCB8A4 E0D5E91E 97D2D988 09B64C2B 7EB17CBD E7B82D07 90BF1D91 1DB71064 6AB020F2 F3B97148 84BE41DE 1ADAD47D 6DDDE4EB F4D4B551 83D385C7 136C9856 646BA8C0 FD62F97A 8A65C9EC 14015C4F 63066CD9 FA0F3D63 8D080DF5 3B6E20C8 4C69105E D56041E4 A2677172 3C03E4D1 4B04D447 D20D85FD A50AB56B 35B5A8FA 42B2986C DBBBC9D6 ACBCF940 32D86CE3 45DF5C75 DCD60DCF ABD13D59 26D930AC 51DE003A C8D75180 BFD06116 21B4F4B5 56B3C423 CFBA9599 B8BDA50F 2802B89E 5F058808 C60CD9B2 B10BE924 2F6F7C87 58684C11 C1611DAB B6662D3D 76DC4190 01DB7106 98D220BC EFD5102A 71B18589 06B6B51F 9FBFE4A5 E8B8D433 7807C9A2 0F00F934 9609A88E E10E9818 7F6A0DBB 086D3D2D 91646C97 E6635C01 6B6B51F4 1C6C6162 856530D8 F262004E 6C0695ED 1B01A57B 8208F4C1 F50FC457 65B0D9C6 12B7E950 8BBEB8EA FCB9887C 62DD1DDF 15DA2D49 8CD37CF3 FBD44C65 4DB26158 3AB551CE A3BC0074 D4BB30E2 4ADFA541 3DD895D7 A4D1C46D D3D6F4FB 4369E96A 346ED9FC AD678846 DA60B8D0 44042D73 33031DE5 AA0A4C5F DD0D7CC9 5005713C 270241AA BE0B1010 C90C2086 5768B525 206F85B3 B966D409 CE61E49F 5EDEF90E 29D9C998 B0D09822 C7D7A8B4 59B33D17 2EB40D81 B7BD5C3B C0BA6CAD EDB88320 9ABFB3B6 03B6E20C 74B1D29A EAD54739 9DD277AF 04DB2615 73DC1683 E3630B12 94643B84 0D6D6A3E 7A6A5AA8 E40ECF0B 9309FF9D 0A00AE27 7D079EB1 F00F9344 8708A3D2 1E01F268 6906C2FE F762575D 806567CB 196C3671 6E6B06E7 FED41B76 89D32BE0 10DA7A5A 67DD4ACC F9B9DF6F 8EBEEFF9 17B7BE43 60B08ED5 D6D6A3E8 A1D1937E 38D8C2C4 4FDFF252 D1BB67F1 A6BC5767 3FB506DD 48B2364B D80D2BDA AF0A1B4C 36034AF6 41047A60 DF60EFC3 A867DF55 316E8EEF 4669BE79 CB61B38C BC66831A 256FD2A0 5268E236 CC0C7795 BB0B4703 220216B9 5505262F C5BA3BBE B2BD0B28 2BB45A92 5CB36A04 C2D7FFA7 B5D0CF31 2CD99E8B 5BDEAE1D 9B64C2B0 EC63F226 756AA39C 026D930A 9C0906A9 EB0E363F 72076785 05005713 95BF4A82 E2B87A14 7BB12BAE 0CB61B38 92D28E9B E5D5BE0D 7CDCEFB7 0BDBDF21 86D3D2D4 F1D4E242 68DDB3F8 1FDA836E 81BE16CD F6B9265B 6FB077E1 18B74777 88085AE6 FF0F6A70 66063BCA 11010B5C 8F659EFF F862AE69 616BFFD3 166CCF45 A00AE278 D70DD2EE 4E048354 3903B3C2 A7672661 D06016F7 4969474D 3E6E77DB AED16A4A D9D65ADC 40DF0B66 37D83BF0 A9BCAE53 DEBB9EC5 47B2CF7F 30B5FFE9 BDBDF21C CABAC28A 53B39330 24B4A3A6 BAD03605 CDD70693 54DE5729 23D967BF B3667A2E C4614AB8 5D681B02 2A6F2B94 B40BBE37 C30C8EA1 5A05DF1B 2D02EF8D";

        function decimalToHexString(number) {
            number < 0 && (number = 4294967295 + number + 1), number = number.toString(16).toLowerCase();
            for (var i = 0; i < 8 - number.length; i++) number = "0" + number;
            return number
        }
        var n = 0,
            x = 0;
        crc === undefined && (crc = 0), crc ^= -1;
        for (var i = 0, iTop = str.length; i < iTop; i++) n = (crc ^ str.charCodeAt(i)) & 255, x = "0x" + table.substr(n * 9, 8), crc = crc >>> 8 ^ x;
        return decimalToHexString(crc ^ -1)
    }
    function registerEvents(events) {
        var baseHost = baseURI.replace(/^https?:\/\/([^\/]+)\/.*/i, "$1");
        window.location.host == baseHost && (eventDistribute = events, window.location.pathname == "/tutorial" && (inhibitSave = !0))
    }
    function findSelf() {
        var scripts = document.getElementsByTagName("script");
        for (var i = 0, n = scripts.length; i < n; i++) {
            var src = scripts[i].src,
                id = scripts[i].id;
            if (src && id && id.length == 17) {
                var tok = id.substring(1, 9),
                    sig = id.substring(9),
                    sum = crc32(tok);
                if (sig == sum) {
                    thisScript = scripts[i], thisScriptId = id, baseURI = src.replace(/^((https?:)\/\/[^\/]+\/).*/i, "$1");
                    return
                }
            }
        }
    }
    findSelf();
    var stupidFuckingPrototype = !1,
        _getElementsByClassName = document.getElementsByClassName;
    if (window.Prototype && window.Prototype.Version) {
        var version = window.Prototype.Version.replace(/_.*$/g, "");
        debug.log("prototype version: %d", version);
        var ver = version.split("."),
            major = parseInt(ver[0]),
            minor = parseInt(ver[1]);
        major <= 1 && minor <= 5 && (stupidFuckingPrototype = !0, document.getElementsByClassName = function (x) {
            return $$("." + x)
        }, Array.prototype.shift = function () {
            var value = this[0];
            for (var i = 0; i < this.length - 1; i++) this[i] = this[i + 1];
            return this.length = Math.max(0, this.length - 1), value
        })
    }
    function showUnusableErrorMessage(messageType) {
        var $overlay = $("<div/>").css({
            position: "fixed",
            top: 0,
            left: 0,
            background: "#000000",
            zIndex: zindex,
            opacity: .5,
            width: "100%",
            height: "100%"
        }),
            fragment = "#faq-third-party-cookies",
            userAgent = navigator.userAgent;
        /MSIE/i.test(userAgent) ? fragment += "-ie" : /Firefox/i.test(userAgent) ? fragment += "-firefox" : /Chrome/i.test(userAgent) ? fragment += "-chrome" : /Safari/i.test(userAgent) && (fragment += "-safari");
        var message = "";
        switch (messageType) {
            case "browser":
                message = "<p>You appear to be using a browser that we do not support. We highly suggest you switch or upgrade to one of the following browsers:</p>";
                break;
            case "cookies":
            default:
                message = '<p style="">The clipper requires 3rd party cookies to be enabled. Head over to our <a target="_blank" href="' + baseURI + "help" + fragment + '" style="color: #469CB9">help page</a>' + " for instructions on how to enable them.</p>"
        }
        message += '<p style="position: absolute; bottom: 40px; margin: 0 auto; width: 90%">Click anywhere to dismiss this message.</p>';
        var width = 600,
            height = 440,
            radius = "3px",
            shadow = "-3px 3px 10px rgba(0,0,0,0.75)",
            positionalCss = {
                top: "50%",
                left: "50%",
                width: width,
                height: height,
                padding: "20px",
                "margin-left": -width / 2,
                "margin-top": -height / 2,
                position: "fixed"
            },
            $background = $("<div/>").css($.extend({}, positionalCss, {
                background: "#eee",
                "z-index": zindex + 3
            })).appendTo(document.body),
            $bgImage = $("<div/>").css($.extend({}, positionalCss, {
                background: "transparent url(" + baseURI + "images/logo1_0.7.43.png) no-repeat center center",
                opacity: .3,
                "z-index": zindex + 4
            })).appendTo(document.body),
            $messageBox = $("<div/>");
        $messageBox.css($.extend({}, positionalCss, {
            zIndex: zindex + 5,
            "-webkit-border-radius": radius,
            "-moz-border-radius": radius,
            "border-radius": radius,
            "-webkit-box-shadow": shadow,
            "box-shadow": shadow,
            background: "transparent",
            color: "rgb(57,39,27)",
            "text-align": "center",
            "font-family": '"lucida grande", tahoma, verdana, arial, sans-serif',
            "font-size": "22px"
        })).html(message);

        function hideMessage() {
            $bgImage.remove(), $overlay.remove(), $messageBox.remove(), $background.remove()
        }
        $overlay.appendTo(document.body).fadeIn(200).click(hideMessage), $bgImage.click(hideMessage), assureFixedPositioning($overlay), $messageBox.appendTo(document.body).fadeIn(200).click(hideMessage)
    }
    load(baseURI + "js/jquery_bookmarklet_0.7.43.js", function () {
        stupidFuckingPrototype && (document.getElementsByClassName = _getElementsByClassName), jQuery = $ = window.jQuery.noConflict(!1), supportsRgba = function () {
            var $temp = $("<div>").css({
                width: 0,
                height: 0,
                display: "none",
                "background-color": "rgba(64, 128, 256, 0.25)"
            }).appendTo(document.body),
                bgColor = $temp.css("background-color");
            return $temp.remove(), /^rgba/.test(bgColor)
        } (), debug.log("supports rgba: %s", supportsRgba), $.fn.bgPosition = function () {
            return document.defaultView && document.defaultView.getComputedStyle ? this.css("background-position") : this[0].currentStyle ? this[0].currentStyle.backgroundPositionX + " " + this[0].currentStyle.backgroundPositionY : "0 0"
        }, dpi = function () {
            var $div = $("<div/>").css({
                width: "1in",
                visibility: "hidden",
                position: "absolute",
                left: "-10000px",
                padding: 0
            }).appendTo(document.body),
                width = $div.width();
            return $div.remove(), $div = null, width
        } (), debug.log("dpi calculated to be %d", dpi), loadModules(), main()
    }), debug.log("script ID: %s", thisScriptId), window[thisScriptId] = {
        reload: reload,
        shutdown: shutdown,
        events: registerEvents,
        cancel: clipCancel
    }, window[thisScriptId + "_reload"] = reload, window.CLIPBOARD && window.CLIPBOARD.bookmarklet && (window.CLIPBOARD.bookmarklet.identity = thisScriptId), window.CLIPBOARD && window.CLIPBOARD.bookmarklet && window.CLIPBOARD.bookmarklet.events && registerEvents(window.CLIPBOARD.bookmarklet.events);

    function loadModules() {
        jQuery.easing.jswing = jQuery.easing.swing, jQuery.extend(jQuery.easing, {
            def: "easeOutQuad",
            swing: function (x, t, b, c, d) {
                return jQuery.easing[jQuery.easing.def](x, t, b, c, d)
            },
            easeInQuad: function (x, t, b, c, d) {
                return c * (t /= d) * t + b
            },
            easeOutQuad: function (x, t, b, c, d) {
                return -c * (t /= d) * (t - 2) + b
            },
            easeInOutQuad: function (x, t, b, c, d) {
                return (t /= d / 2) < 1 ? c / 2 * t * t + b : -c / 2 * (--t * (t - 2) - 1) + b
            },
            easeInCubic: function (x, t, b, c, d) {
                return c * (t /= d) * t * t + b
            },
            easeOutCubic: function (x, t, b, c, d) {
                return c * ((t = t / d - 1) * t * t + 1) + b
            },
            easeInOutCubic: function (x, t, b, c, d) {
                return (t /= d / 2) < 1 ? c / 2 * t * t * t + b : c / 2 * ((t -= 2) * t * t + 2) + b
            },
            easeInQuart: function (x, t, b, c, d) {
                return c * (t /= d) * t * t * t + b
            },
            easeOutQuart: function (x, t, b, c, d) {
                return -c * ((t = t / d - 1) * t * t * t - 1) + b
            },
            easeInOutQuart: function (x, t, b, c, d) {
                return (t /= d / 2) < 1 ? c / 2 * t * t * t * t + b : -c / 2 * ((t -= 2) * t * t * t - 2) + b
            },
            easeInQuint: function (x, t, b, c, d) {
                return c * (t /= d) * t * t * t * t + b
            },
            easeOutQuint: function (x, t, b, c, d) {
                return c * ((t = t / d - 1) * t * t * t * t + 1) + b
            },
            easeInOutQuint: function (x, t, b, c, d) {
                return (t /= d / 2) < 1 ? c / 2 * t * t * t * t * t + b : c / 2 * ((t -= 2) * t * t * t * t + 2) + b
            },
            easeInSine: function (x, t, b, c, d) {
                return -c * Math.cos(t / d * (Math.PI / 2)) + c + b
            },
            easeOutSine: function (x, t, b, c, d) {
                return c * Math.sin(t / d * (Math.PI / 2)) + b
            },
            easeInOutSine: function (x, t, b, c, d) {
                return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b
            },
            easeInExpo: function (x, t, b, c, d) {
                return t == 0 ? b : c * Math.pow(2, 10 * (t / d - 1)) + b
            },
            easeOutExpo: function (x, t, b, c, d) {
                return t == d ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b
            },
            easeInOutExpo: function (x, t, b, c, d) {
                return t == 0 ? b : t == d ? b + c : (t /= d / 2) < 1 ? c / 2 * Math.pow(2, 10 * (t - 1)) + b : c / 2 * (-Math.pow(2, -10 * --t) + 2) + b
            },
            easeInCirc: function (x, t, b, c, d) {
                return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b
            },
            easeOutCirc: function (x, t, b, c, d) {
                return c * Math.sqrt(1 - (t = t / d - 1) * t) + b
            },
            easeInOutCirc: function (x, t, b, c, d) {
                return (t /= d / 2) < 1 ? -c / 2 * (Math.sqrt(1 - t * t) - 1) + b : c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b
            },
            easeInElastic: function (x, t, b, c, d) {
                var s = 1.70158,
                    p = 0,
                    a = c;
                if (t == 0) return b;
                if ((t /= d) == 1) return b + c;
                p || (p = d * .3);
                if (a < Math.abs(c)) {
                    a = c;
                    var s = p / 4
                } else var s = p / (2 * Math.PI) * Math.asin(c / a);
                return -(a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * 2 * Math.PI / p)) + b
            },
            easeOutElastic: function (x, t, b, c, d) {
                var s = 1.70158,
                    p = 0,
                    a = c;
                if (t == 0) return b;
                if ((t /= d) == 1) return b + c;
                p || (p = d * .3);
                if (a < Math.abs(c)) {
                    a = c;
                    var s = p / 4
                } else var s = p / (2 * Math.PI) * Math.asin(c / a);
                return a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * 2 * Math.PI / p) + c + b
            },
            easeInOutElastic: function (x, t, b, c, d) {
                var s = 1.70158,
                    p = 0,
                    a = c;
                if (t == 0) return b;
                if ((t /= d / 2) == 2) return b + c;
                p || (p = d * .3 * 1.5);
                if (a < Math.abs(c)) {
                    a = c;
                    var s = p / 4
                } else var s = p / (2 * Math.PI) * Math.asin(c / a);
                return t < 1 ? -0.5 * a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * 2 * Math.PI / p) + b : a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * 2 * Math.PI / p) * .5 + c + b
            },
            easeInBack: function (x, t, b, c, d, s) {
                return s == undefined && (s = 1.70158), c * (t /= d) * t * ((s + 1) * t - s) + b
            },
            easeOutBack: function (x, t, b, c, d, s) {
                return s == undefined && (s = 1.70158), c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b
            },
            easeInOutBack: function (x, t, b, c, d, s) {
                return s == undefined && (s = 1.70158), (t /= d / 2) < 1 ? c / 2 * t * t * (((s *= 1.525) + 1) * t - s) + b : c / 2 * ((t -= 2) * t * (((s *= 1.525) + 1) * t + s) + 2) + b
            },
            easeInBounce: function (x, t, b, c, d) {
                return c - jQuery.easing.easeOutBounce(x, d - t, 0, c, d) + b
            },
            easeOutBounce: function (x, t, b, c, d) {
                return (t /= d) < 1 / 2.75 ? c * 7.5625 * t * t + b : t < 2 / 2.75 ? c * (7.5625 * (t -= 1.5 / 2.75) * t + .75) + b : t < 2.5 / 2.75 ? c * (7.5625 * (t -= 2.25 / 2.75) * t + .9375) + b : c * (7.5625 * (t -= 2.625 / 2.75) * t + .984375) + b
            },
            easeInOutBounce: function (x, t, b, c, d) {
                return t < d / 2 ? jQuery.easing.easeInBounce(x, t * 2, 0, c, d) * .5 + b : jQuery.easing.easeOutBounce(x, t * 2 - d, 0, c, d) * .5 + c * .5 + b
            }
        }), function (c) {
            var a = ["DOMMouseScroll", "mousewheel"];
            c.event.special.mousewheel = {
                setup: function () {
                    if (this.addEventListener) for (var d = a.length; d; ) this.addEventListener(a[--d], b, !1);
                    else this.onmousewheel = b
                },
                teardown: function () {
                    if (this.removeEventListener) for (var d = a.length; d; ) this.removeEventListener(a[--d], b, !1);
                    else this.onmousewheel = null
                }
            }, c.fn.extend({
                mousewheel: function (d) {
                    return d ? this.bind("mousewheel", d) : this.trigger("mousewheel")
                },
                unmousewheel: function (d) {
                    return this.unbind("mousewheel", d)
                }
            });

            function b(i) {
                var g = i || window.event,
                    f = [].slice.call(arguments, 1),
                    j = 0,
                    h = !0,
                    e = 0,
                    d = 0;
                return i = c.event.fix(g), i.type = "mousewheel", i.wheelDelta && (j = i.wheelDelta / 120), i.detail && (j = -i.detail / 3), d = j, g.axis !== undefined && g.axis === g.HORIZONTAL_AXIS && (d = 0, e = -1 * j), g.wheelDeltaY !== undefined && (d = g.wheelDeltaY / 120), g.wheelDeltaX !== undefined && (e = -1 * g.wheelDeltaX / 120), f.unshift(i, j, e, d), c.event.handle.apply(this, f)
            }
        } (jQuery), function (J, c, l, G, g, D) {
            var b = this,
                j = Math.floor(Math.random() * 1e4),
                m = Function.prototype,
                M = /^((http.?:)\/\/([^:\/\s]+)(:\d+)*)/,
                N = /[\-\w]+\/\.\.\//,
                B = /([^:])\/\//g,
                E = "",
                k = {},
                I = J.easyXDM,
                Q = "easyXDM_",
                A, u = !1;

            function y(U, W) {
                var V = typeof U[W];
                return V == "function" || V == "object" && !!U[W] || V == "unknown"
            }
            function q(U, V) {
                return typeof U[V] == "object" && !!U[V]
            }
            function n(U) {
                return Object.prototype.toString.call(U) === "[object Array]"
            }
            function S(V) {
                try {
                    var U = new ActiveXObject(V);
                    return U = null, !0
                } catch (W) {
                    return !1
                }
            }
            var r, t;
            if (y(J, "addEventListener")) r = function (W, U, V) {
                W.addEventListener(U, V, !1)
            }, t = function (W, U, V) {
                W.removeEventListener(U, V, !1)
            };
            else if (y(J, "attachEvent")) r = function (U, W, V) {
                U.attachEvent("on" + W, V)
            }, t = function (U, W, V) {
                U.detachEvent("on" + W, V)
            };
            else throw new Error("Browser not supported");
            var T = !1,
                F = [],
                H;
            "readyState" in c ? (H = c.readyState, T = H == "complete" || ~navigator.userAgent.indexOf("AppleWebKit/") && (H == "loaded" || H == "interactive")) : T = !!c.body;

            function o() {
                if (T) return;
                T = !0;
                for (var U = 0; U < F.length; U++) F[U]();
                F.length = 0
            }
            T || (y(J, "addEventListener") ? r(c, "DOMContentLoaded", o) : (r(c, "readystatechange", function () {
                c.readyState == "complete" && o()
            }), c.documentElement.doScroll && J === top &&
            function e() {
                if (T) return;
                try {
                    c.documentElement.doScroll("left")
                } catch (U) {
                    G(e, 1);
                    return
                }
                o()
            } ()), r(J, "load", o));

            function C(V, U) {
                if (T) {
                    V.call(U);
                    return
                }
                F.push(function () {
                    V.call(U)
                })
            }
            function i() {
                var W = parent;
                if (E !== "") for (var U = 0, V = E.split("."); U < V.length; U++) W = W[V[U]];
                return W.easyXDM
            }
            function d(U) {
                return J.easyXDM = I, E = U, E && (Q = "easyXDM_" + E.replace(".", "_") + "_"), k
            }
            function v(U) {
                return U.match(M)[3]
            }
            function f(W) {
                var U = W.match(M),
                    X = U[2],
                    Y = U[3],
                    V = U[4] || "";
                if (X == "http:" && V == ":80" || X == "https:" && V == ":443") V = "";
                return X + "//" + Y + V
            }
            function x(U) {
                U = U.replace(B, "$1/");
                if (!U.match(/^(http||https):\/\//)) {
                    var V = U.substring(0, 1) === "/" ? "" : l.pathname;
                    V.substring(V.length - 1) !== "/" && (V = V.substring(0, V.lastIndexOf("/") + 1)), U = l.protocol + "//" + l.host + V + U
                }
                while (N.test(U)) U = U.replace(N, "");
                return U
            }
            function L(U, X) {
                var Z = "",
                    W = U.indexOf("#");
                W !== -1 && (Z = U.substring(W), U = U.substring(0, W));
                var Y = [];
                for (var V in X) X.hasOwnProperty(V) && Y.push(V + "=" + D(X[V]));
                return U + (u ? "#" : U.indexOf("?") == -1 ? "?" : "&") + Y.join("&") + Z
            }
            var O = function (U) {
                U = U.substring(1).split("&");
                var W = {},
                        X, V = U.length;
                while (V--) X = U[V].split("="), W[X[0]] = g(X[1]);
                return W
            } (/xdm_e=/.test(l.search) ? l.search : l.hash);

            function p(U) {
                return typeof U == "undefined"
            }
            function K() {
                var V = {},
                    W = {
                        a: [1, 2, 3]
                    },
                    U = '{"a":[1,2,3]}';
                return typeof JSON != "undefined" && typeof JSON.stringify == "function" && JSON.stringify(W).replace(/\s/g, "") === U ? JSON : (Object.toJSON && Object.toJSON(W).replace(/\s/g, "") === U && (V.stringify = Object.toJSON), typeof String.prototype.evalJSON == "function" && (W = U.evalJSON(), W.a && W.a.length === 3 && W.a[2] === 3 && (V.parse = function (X) {
                    return X.evalJSON()
                })), V.stringify && V.parse ? (K = function () {
                    return V
                }, V) : null)
            }
            function P(U, V, W) {
                var Y;
                for (var X in V) V.hasOwnProperty(X) && (X in U ? (Y = V[X], typeof Y == "object" ? P(U[X], Y, W) : W || (U[X] = V[X])) : U[X] = V[X]);
                return U
            }
            function a() {
                var U = c.createElement("iframe");
                U.name = Q + "TEST", P(U.style, {
                    position: "absolute",
                    left: "-2000px",
                    top: "0px"
                }), c.body.appendChild(U), A = U.contentWindow !== J.frames[U.name], c.body.removeChild(U)
            }
            function w(U) {
                p(A) && a();
                var W;
                A ? W = c.createElement('<iframe name="' + U.props.name + '"/>') : (W = c.createElement("IFRAME"), W.name = U.props.name), W.id = W.name = U.props.name, delete U.props.name, U.onLoad && r(W, "load", U.onLoad), typeof U.container == "string" && (U.container = c.getElementById(U.container)), U.container || (W.style.position = "absolute", W.style.top = "-2000px", U.container = c.body);
                var V = U.props.src;
                return delete U.props.src, P(W, U.props), W.border = W.frameBorder = 0, U.container.appendChild(W), W.src = V, U.props.src = V, W
            }
            function R(X, W) {
                typeof X == "string" && (X = [X]);
                var V, U = X.length;
                while (U--) {
                    V = X[U], V = new RegExp(V.substr(0, 1) == "^" ? V : "^" + V.replace(/(\*)/g, ".$1").replace(/\?/g, ".") + "$");
                    if (V.test(W)) return !0
                }
                return !1
            }
            function h(W) {
                var ab = W.protocol,
                    V;
                W.isHost = W.isHost || p(O.xdm_p), u = W.hash || !1, W.props || (W.props = {});
                if (!W.isHost) {
                    W.channel = O.xdm_c, W.secret = O.xdm_s, W.remote = O.xdm_e, ab = O.xdm_p;
                    if (W.acl && !R(W.acl, W.remote)) throw new Error("Access denied for " + W.remote)
                } else W.remote = x(W.remote), W.channel = W.channel || "default" + j++, W.secret = Math.random().toString(16).substring(2), p(ab) && (f(l.href) == f(W.remote) ? ab = "4" : y(J, "postMessage") || y(c, "postMessage") ? ab = "1" : y(J, "ActiveXObject") && S("ShockwaveFlash.ShockwaveFlash") ? ab = "6" : navigator.product === "Gecko" && "frameElement" in J && navigator.userAgent.indexOf("WebKit") == -1 ? ab = "5" : W.remoteHelper ? (W.remoteHelper = x(W.remoteHelper), ab = "2") : ab = "0");
                switch (ab) {
                    case "0":
                        P(W, {
                            interval: 100,
                            delay: 2e3,
                            useResize: !0,
                            useParent: !1,
                            usePolling: !1
                        }, !0);
                        if (W.isHost) {
                            if (!W.local) {
                                var Z = l.protocol + "//" + l.host,
                                U = c.body.getElementsByTagName("img"),
                                aa, X = U.length;
                                while (X--) {
                                    aa = U[X];
                                    if (aa.src.substring(0, Z.length) === Z) {
                                        W.local = aa.src;
                                        break
                                    }
                                }
                                W.local || (W.local = J)
                            }
                            var Y = {
                                xdm_c: W.channel,
                                xdm_p: 0
                            };
                            W.local === J ? (W.usePolling = !0, W.useParent = !0, W.local = l.protocol + "//" + l.host + l.pathname + l.search, Y.xdm_e = W.local, Y.xdm_pa = 1) : Y.xdm_e = x(W.local), W.container && (W.useResize = !1, Y.xdm_po = 1), W.remote = L(W.remote, Y)
                        } else P(W, {
                            channel: O.xdm_c,
                            remote: O.xdm_e,
                            useParent: !p(O.xdm_pa),
                            usePolling: !p(O.xdm_po),
                            useResize: W.useParent ? !1 : W.useResize
                        });
                        V = [new k.stack.HashTransport(W), new k.stack.ReliableBehavior({}), new k.stack.QueueBehavior({
                            encode: !0,
                            maxLength: 4e3 - W.remote.length
                        }), new k.stack.VerifyBehavior({
                            initiate: W.isHost
                        })];
                        break;
                    case "1":
                        V = [new k.stack.PostMessageTransport(W)];
                        break;
                    case "2":
                        V = [new k.stack.NameTransport(W), new k.stack.QueueBehavior, new k.stack.VerifyBehavior({
                            initiate: W.isHost
                        })];
                        break;
                    case "3":
                        V = [new k.stack.NixTransport(W)];
                        break;
                    case "4":
                        V = [new k.stack.SameOriginTransport(W)];
                        break;
                    case "5":
                        V = [new k.stack.FrameElementTransport(W)];
                        break;
                    case "6":
                        W.swf || (W.swf = "../../tools/easyxdm.swf"), V = [new k.stack.FlashTransport(W)]
                }
                return V.push(new k.stack.QueueBehavior({
                    lazy: W.lazy,
                    remove: !0
                })), V
            }
            function z(X) {
                var Y, W = {
                    incoming: function (aa, Z) {
                        this.up.incoming(aa, Z)
                    },
                    outgoing: function (Z, aa) {
                        this.down.outgoing(Z, aa)
                    },
                    callback: function (Z) {
                        this.up.callback(Z)
                    },
                    init: function () {
                        this.down.init()
                    },
                    destroy: function () {
                        this.down.destroy()
                    }
                };
                for (var V = 0, U = X.length; V < U; V++) Y = X[V], P(Y, W, !0), V !== 0 && (Y.down = X[V - 1]), V !== U - 1 && (Y.up = X[V + 1]);
                return Y
            }
            function s(U) {
                U.up.down = U.down, U.down.up = U.up, U.up = U.down = null
            }
            P(k, {
                version: "2.4.13.112",
                query: O,
                stack: {},
                apply: P,
                getJSONObject: K,
                whenReady: C,
                noConflict: d
            }), k.DomHelper = {
                on: r,
                un: t,
                requiresJSON: function (U) {
                    q(J, "JSON") || c.write('<script type="text/javascript" src="' + U + '"></script>')
                }
            }, function () {
                var U = {};
                k.Fn = {
                    set: function (V, W) {
                        U[V] = W
                    },
                    get: function (W, V) {
                        var X = U[W];
                        return V && delete U[W], X
                    }
                }
            } (), k.Socket = function (V) {
                var U = z(h(V).concat([{
                    incoming: function (Y, X) {
                        V.onMessage(Y, X)
                    },
                    callback: function (X) {
                        V.onReady && V.onReady(X)
                    }
                }])),
                    W = f(V.remote);
                this.origin = f(V.remote), this.destroy = function () {
                    U.destroy()
                }, this.postMessage = function (X) {
                    U.outgoing(X, W)
                }, U.init()
            }, k.Rpc = function (W, V) {
                if (V.local) for (var Y in V.local) if (V.local.hasOwnProperty(Y)) {
                    var X = V.local[Y];
                    typeof X == "function" && (V.local[Y] = {
                        method: X
                    })
                }
                var U = z(h(W).concat([new k.stack.RpcBehavior(this, V),
                {
                    callback: function (Z) {
                        W.onReady && W.onReady(Z)
                    }
                }]));
                this.origin = f(W.remote), this.destroy = function () {
                    U.destroy()
                }, U.init()
            }, k.stack.SameOriginTransport = function (V) {
                var W, Y, X, U;
                return W = {
                    outgoing: function (aa, ab, Z) {
                        X(aa), Z && Z()
                    },
                    destroy: function () {
                        Y && (Y.parentNode.removeChild(Y), Y = null)
                    },
                    onDOMReady: function () {
                        U = f(V.remote), V.isHost ? (P(V.props, {
                            src: L(V.remote, {
                                xdm_e: l.protocol + "//" + l.host + l.pathname,
                                xdm_c: V.channel,
                                xdm_p: 4
                            }),
                            name: Q + V.channel + "_provider"
                        }), Y = w(V), k.Fn.set(V.channel, function (Z) {
                            return X = Z, G(function () {
                                W.up.callback(!0)
                            }, 0), function (aa) {
                                W.up.incoming(aa, U)
                            }
                        })) : (X = i().Fn.get(V.channel, !0)(function (Z) {
                            W.up.incoming(Z, U)
                        }), G(function () {
                            W.up.callback(!0)
                        }, 0))
                    },
                    init: function () {
                        C(W.onDOMReady, W)
                    }
                }
            }, k.stack.FlashTransport = function (X) {
                var Z, U, Y, aa, V, ab;

                function ac(ae, ad) {
                    G(function () {
                        Z.up.incoming(ae, aa)
                    }, 0)
                }
                function W(ag) {
                    var ad = X.swf + "?host=" + X.isHost,
                        af = "easyXDM_swf_" + Math.floor(Math.random() * 1e4);
                    k.Fn.set("flash_loaded", function () {
                        k.stack.FlashTransport.__swf = V = ab.firstChild, ag()
                    }), ab = c.createElement("div"), P(ab.style, {
                        height: "1px",
                        width: "1px",
                        position: "absolute",
                        left: 0,
                        top: 0
                    }), c.body.appendChild(ab);
                    var ae = "proto=" + b.location.protocol + "&domain=" + v(b.location.href) + "&ns=" + E;
                    ab.innerHTML = "<object height='1' width='1' type='application/x-shockwave-flash' id='" + af + "' data='" + ad + "'><param name='allowScriptAccess' value='always'></param><param name='wmode' value='transparent'><param name='movie' value='" + ad + "'></param><param name='flashvars' value='" + ae + "'></param><embed type='application/x-shockwave-flash' FlashVars='" + ae + "' allowScriptAccess='always' wmode='transparent' src='" + ad + "' height='1' width='1'></embed></object>"
                }
                return Z = {
                    outgoing: function (ae, af, ad) {
                        V.postMessage(X.channel, ae.toString()), ad && ad()
                    },
                    destroy: function () {
                        try {
                            V.destroyChannel(X.channel)
                        } catch (ad) { }
                        V = null, U && (U.parentNode.removeChild(U), U = null)
                    },
                    onDOMReady: function () {
                        aa = X.remote, V = k.stack.FlashTransport.__swf, k.Fn.set("flash_" + X.channel + "_init", function () {
                            G(function () {
                                Z.up.callback(!0)
                            })
                        }), k.Fn.set("flash_" + X.channel + "_onMessage", ac);
                        var ad = function () {
                            V.createChannel(X.channel, X.secret, f(X.remote), X.isHost), X.isHost && (P(X.props, {
                                src: L(X.remote, {
                                    xdm_e: f(l.href),
                                    xdm_c: X.channel,
                                    xdm_p: 6,
                                    xdm_s: X.secret
                                }),
                                name: Q + X.channel + "_provider"
                            }), U = w(X))
                        };
                        V ? ad() : W(ad)
                    },
                    init: function () {
                        C(Z.onDOMReady, Z)
                    }
                }
            }, k.stack.PostMessageTransport = function (X) {
                var Z, aa, V, W;

                function U(ab) {
                    if (ab.origin) return f(ab.origin);
                    if (ab.uri) return f(ab.uri);
                    if (ab.domain) return l.protocol + "//" + ab.domain;
                    throw "Unable to retrieve the origin of the event"
                }
                function Y(ac) {
                    var ab = U(ac);
                    ab == W && ac.data.substring(0, X.channel.length + 1) == X.channel + " " && Z.up.incoming(ac.data.substring(X.channel.length + 1), ab)
                }
                return Z = {
                    outgoing: function (ac, ad, ab) {
                        V.postMessage(X.channel + " " + ac, ad || W), ab && ab()
                    },
                    destroy: function () {
                        t(J, "message", Y), aa && (V = null, aa.parentNode.removeChild(aa), aa = null)
                    },
                    onDOMReady: function () {
                        W = f(X.remote), X.isHost ? (r(J, "message", function ab(ac) {
                            ac.data == X.channel + "-ready" && (V = "postMessage" in aa.contentWindow ? aa.contentWindow : aa.contentWindow.document, t(J, "message", ab), r(J, "message", Y), G(function () {
                                Z.up.callback(!0)
                            }, 0))
                        }), P(X.props, {
                            src: L(X.remote, {
                                xdm_e: f(l.href),
                                xdm_c: X.channel,
                                xdm_p: 1
                            }),
                            name: Q + X.channel + "_provider"
                        }), aa = w(X)) : (r(J, "message", Y), V = "postMessage" in J.parent ? J.parent : J.parent.document, V.postMessage(X.channel + "-ready", W), G(function () {
                            Z.up.callback(!0)
                        }, 0))
                    },
                    init: function () {
                        C(Z.onDOMReady, Z)
                    }
                }
            }, k.stack.FrameElementTransport = function (V) {
                var W, Y, X, U;
                return W = {
                    outgoing: function (aa, ab, Z) {
                        X.call(this, aa), Z && Z()
                    },
                    destroy: function () {
                        Y && (Y.parentNode.removeChild(Y), Y = null)
                    },
                    onDOMReady: function () {
                        U = f(V.remote), V.isHost ? (P(V.props, {
                            src: L(V.remote, {
                                xdm_e: f(l.href),
                                xdm_c: V.channel,
                                xdm_p: 5
                            }),
                            name: Q + V.channel + "_provider"
                        }), Y = w(V), Y.fn = function (Z) {
                            return delete Y.fn, X = Z, G(function () {
                                W.up.callback(!0)
                            }, 0), function (aa) {
                                W.up.incoming(aa, U)
                            }
                        }) : (c.referrer && f(c.referrer) != O.xdm_e && (J.top.location = O.xdm_e), X = J.frameElement.fn(function (Z) {
                            W.up.incoming(Z, U)
                        }), W.up.callback(!0))
                    },
                    init: function () {
                        C(W.onDOMReady, W)
                    }
                }
            }, k.stack.NameTransport = function (Y) {
                var Z, ab, af, X, ad, ae, V, U;

                function ac(ai) {
                    var ah = Y.remoteHelper + (ab ? "#_3" : "#_2") + Y.channel;
                    af.contentWindow.sendMessage(ai, ah)
                }
                function aa() {
                    ab ? (++ad === 2 || !ab) && Z.up.callback(!0) : (ac("ready"), Z.up.callback(!0))
                }
                function ag(ah) {
                    Z.up.incoming(ah, V)
                }
                function W() {
                    ae && G(function () {
                        ae(!0)
                    }, 0)
                }
                return Z = {
                    outgoing: function (ai, aj, ah) {
                        ae = ah, ac(ai)
                    },
                    destroy: function () {
                        af.parentNode.removeChild(af), af = null, ab && (X.parentNode.removeChild(X), X = null)
                    },
                    onDOMReady: function () {
                        ab = Y.isHost, ad = 0, V = f(Y.remote), Y.local = x(Y.local), ab ? (k.Fn.set(Y.channel, function (ai) {
                            ab && ai === "ready" && (k.Fn.set(Y.channel, ag), aa())
                        }), U = L(Y.remote, {
                            xdm_e: Y.local,
                            xdm_c: Y.channel,
                            xdm_p: 2
                        }), P(Y.props, {
                            src: U + "#" + Y.channel,
                            name: Q + Y.channel + "_provider"
                        }), X = w(Y)) : (Y.remoteHelper = Y.remote, k.Fn.set(Y.channel, ag)), af = w({
                            props: {
                                src: Y.local + "#_4" + Y.channel
                            },
                            onLoad: function ah() {
                                var ai = af || this;
                                t(ai, "load", ah), k.Fn.set(Y.channel + "_load", W), function aj() {
                                    typeof ai.contentWindow.sendMessage == "function" ? aa() : G(aj, 50)
                                } ()
                            }
                        })
                    },
                    init: function () {
                        C(Z.onDOMReady, Z)
                    }
                }
            }, k.stack.HashTransport = function (W) {
                var Z, ae = this,
                    ac, X, U, aa, aj, Y, ai, ad, V;

                function ah(al) {
                    if (!ai) return;
                    var ak = W.remote + "#" + aj++ + "_" + al;
                    (ac || !ad ? ai.contentWindow : ai).location = ak
                }
                function ab(ak) {
                    aa = ak, Z.up.incoming(aa.substring(aa.indexOf("_") + 1), V)
                }
                function ag() {
                    if (!Y) return;
                    var ak = Y.location.href,
                        am = "",
                        al = ak.indexOf("#");
                    al != -1 && (am = ak.substring(al)), am && am != aa && ab(am)
                }
                function af() {
                    X = setInterval(ag, U)
                }
                return Z = {
                    outgoing: function (ak, al) {
                        ah(ak)
                    },
                    destroy: function () {
                        J.clearInterval(X), (ac || !ad) && ai.parentNode.removeChild(ai), ai = null
                    },
                    onDOMReady: function () {
                        ac = W.isHost, U = W.interval, aa = "#" + W.channel, aj = 0, ad = W.useParent, V = f(W.remote);
                        if (ac) {
                            W.props = {
                                src: W.remote,
                                name: Q + W.channel + "_provider"
                            };
                            if (ad) W.onLoad = function () {
                                Y = J, af(), Z.up.callback(!0)
                            };
                            else {
                                var am = 0,
                                    ak = W.delay / 50;
                                (function al() {
                                    if (++am > ak) throw new Error("Unable to reference listenerwindow");
                                    try {
                                        Y = ai.contentWindow.frames[Q + W.channel + "_consumer"]
                                    } catch (an) { }
                                    Y ? (af(), Z.up.callback(!0)) : G(al, 50)
                                })()
                            }
                            ai = w(W)
                        } else Y = J, af(), ad ? (ai = parent, Z.up.callback(!0)) : (P(W, {
                            props: {
                                src: W.remote + "#" + W.channel + new Date,
                                name: Q + W.channel + "_consumer"
                            },
                            onLoad: function () {
                                Z.up.callback(!0)
                            }
                        }), ai = w(W))
                    },
                    init: function () {
                        C(Z.onDOMReady, Z)
                    }
                }
            }, k.stack.ReliableBehavior = function (V) {
                var X, Z, Y = 0,
                    U = 0,
                    W = "";
                return X = {
                    incoming: function (ac, aa) {
                        var ab = ac.indexOf("_"),
                            ad = ac.substring(0, ab).split(",");
                        ac = ac.substring(ab + 1), ad[0] == Y && (W = "", Z && Z(!0)), ac.length > 0 && (X.down.outgoing(ad[1] + "," + Y + "_" + W, aa), U != ad[1] && (U = ad[1], X.up.incoming(ac, aa)))
                    },
                    outgoing: function (ac, aa, ab) {
                        W = ac, Z = ab, X.down.outgoing(U + "," + ++Y + "_" + ac, aa)
                    }
                }
            }, k.stack.QueueBehavior = function (W) {
                var Z, aa = [],
                    ad = !0,
                    X = "",
                    ac, U = 0,
                    V = !1,
                    Y = !1;

                function ab() {
                    if (W.remove && aa.length === 0) {
                        s(Z);
                        return
                    }
                    if (ad || aa.length === 0 || ac) return;
                    ad = !0;
                    var ae = aa.shift();
                    Z.down.outgoing(ae.data, ae.origin, function (af) {
                        ad = !1, ae.callback && G(function () {
                            ae.callback(af)
                        }, 0), ab()
                    })
                }
                return Z = {
                    init: function () {
                        p(W) && (W = {}), W.maxLength && (U = W.maxLength, Y = !0), W.lazy ? V = !0 : Z.down.init()
                    },
                    callback: function (af) {
                        ad = !1;
                        var ae = Z.up;
                        ab(), ae.callback(af)
                    },
                    incoming: function (ah, af) {
                        if (Y) {
                            var ag = ah.indexOf("_"),
                                ae = parseInt(ah.substring(0, ag), 10);
                            X += ah.substring(ag + 1), ae === 0 && (W.encode && (X = g(X)), Z.up.incoming(X, af), X = "")
                        } else Z.up.incoming(ah, af)
                    },
                    outgoing: function (ai, af, ah) {
                        W.encode && (ai = D(ai));
                        var ae = [],
                            ag;
                        if (Y) {
                            while (ai.length !== 0) ag = ai.substring(0, U), ai = ai.substring(ag.length), ae.push(ag);
                            while (ag = ae.shift()) aa.push({
                                data: ae.length + "_" + ag,
                                origin: af,
                                callback: ae.length === 0 ? ah : null
                            })
                        } else aa.push({
                            data: ai,
                            origin: af,
                            callback: ah
                        });
                        V ? Z.down.init() : ab()
                    },
                    destroy: function () {
                        ac = !0, Z.down.destroy()
                    }
                }
            }, k.stack.VerifyBehavior = function (Y) {
                var Z, X, V, W = !1;

                function U() {
                    X = Math.random().toString(16).substring(2), Z.down.outgoing(X)
                }
                return Z = {
                    incoming: function (ac, aa) {
                        var ab = ac.indexOf("_");
                        ab === -1 ? ac === X ? Z.up.callback(!0) : V || (V = ac, Y.initiate || U(), Z.down.outgoing(ac)) : ac.substring(0, ab) === V && Z.up.incoming(ac.substring(ab + 1), aa)
                    },
                    outgoing: function (ac, aa, ab) {
                        Z.down.outgoing(X + "_" + ac, aa, ab)
                    },
                    callback: function (aa) {
                        Y.initiate && U()
                    }
                }
            }, k.stack.RpcBehavior = function (aa, V) {
                var X, ac = V.serializer || K(),
                    ab = 0,
                    Z = {};

                function U(ad) {
                    ad.jsonrpc = "2.0", X.down.outgoing(ac.stringify(ad))
                }
                function Y(ad, af) {
                    var ae = Array.prototype.slice;
                    return function () {
                        var ag = arguments.length,
                            ai, ah = {
                                method: af
                            };
                        ag > 0 && typeof arguments[ag - 1] == "function" ? (ag > 1 && typeof arguments[ag - 2] == "function" ? (ai = {
                            success: arguments[ag - 2],
                            error: arguments[ag - 1]
                        }, ah.params = ae.call(arguments, 0, ag - 2)) : (ai = {
                            success: arguments[ag - 1]
                        }, ah.params = ae.call(arguments, 0, ag - 1)), Z["" + ++ab] = ai, ah.id = ab) : ah.params = ae.call(arguments, 0), ad.namedParams && ah.params.length === 1 && (ah.params = ah.params[0]), U(ah)
                    }
                }
                function W(ak, aj, af, ai) {
                    if (!af) {
                        aj && U({
                            id: aj,
                            error: {
                                code: -32601,
                                message: "Procedure not found."
                            }
                        });
                        return
                    }
                    var ah, ae;
                    aj ? (ah = function (al) {
                        ah = m, U({
                            id: aj,
                            result: al
                        })
                    }, ae = function (al, am) {
                        ae = m;
                        var an = {
                            id: aj,
                            error: {
                                code: -32099,
                                message: al
                            }
                        };
                        am && (an.error.data = am), U(an)
                    }) : ah = ae = m, n(ai) || (ai = [ai]);
                    try {
                        var ad = af.method.apply(af.scope, ai.concat([ah, ae]));
                        p(ad) || ah(ad)
                    } catch (ag) {
                        ae(ag.message)
                    }
                }
                return X = {
                    incoming: function (ae, ad) {
                        var af = ac.parse(ae);
                        if (af.method) V.handle ? V.handle(af, U) : W(af.method, af.id, V.local[af.method], af.params);
                        else {
                            var ag = Z[af.id];
                            af.error ? ag.error && ag.error(af.error) : ag.success && ag.success(af.result), delete Z[af.id]
                        }
                    },
                    init: function () {
                        if (V.remote) for (var ad in V.remote) V.remote.hasOwnProperty(ad) && (aa[ad] = Y(V.remote[ad], ad));
                        X.down.init()
                    },
                    destroy: function () {
                        for (var ad in V.remote) V.remote.hasOwnProperty(ad) && aa.hasOwnProperty(ad) && delete aa[ad];
                        X.down.destroy()
                    }
                }
            }, b.easyXDM = k
        } (window, document, location, window.setTimeout, decodeURIComponent, encodeURIComponent), this.JSON || (this.JSON = {}), function () {
            function f(n) {
                return n < 10 ? "0" + n : n
            }
            typeof Date.prototype.toJSON != "function" && (Date.prototype.toJSON = function (key) {
                return isFinite(this.valueOf()) ? this.getUTCFullYear() + "-" + f(this.getUTCMonth() + 1) + "-" + f(this.getUTCDate()) + "T" + f(this.getUTCHours()) + ":" + f(this.getUTCMinutes()) + ":" + f(this.getUTCSeconds()) + "Z" : null
            }, String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function (key) {
                return this.valueOf()
            });
            var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
                escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
                gap, indent, meta = {
                    "\b": "\\b",
                    "\t": "\\t",
                    "\n": "\\n",
                    "\f": "\\f",
                    "\r": "\\r",
                    '"': '\\"',
                    "\\": "\\\\"
                },
                rep;

            function quote(string) {
                return escapable.lastIndex = 0, escapable.test(string) ? '"' + string.replace(escapable, function (a) {
                    var c = meta[a];
                    return typeof c == "string" ? c : "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4)
                }) + '"' : '"' + string + '"'
            }
            function str(key, holder) {
                var i, k, v, length, mind = gap,
                    partial, value = holder[key];
                value && typeof value == "object" && typeof value.toJSON == "function" && (value = value.toJSON(key)), typeof rep == "function" && (value = rep.call(holder, key, value));
                switch (typeof value) {
                    case "string":
                        return quote(value);
                    case "number":
                        return isFinite(value) ? String(value) : "null";
                    case "boolean":
                    case "null":
                        return String(value);
                    case "object":
                        if (!value) return "null";
                        gap += indent, partial = [];
                        if (Object.prototype.toString.apply(value) === "[object Array]") {
                            length = value.length;
                            for (i = 0; i < length; i += 1) partial[i] = str(i, value) || "null";
                            return v = partial.length === 0 ? "[]" : gap ? "[\n" + gap + partial.join(",\n" + gap) + "\n" + mind + "]" : "[" + partial.join(",") + "]", gap = mind, v
                        }
                        if (rep && typeof rep == "object") {
                            length = rep.length;
                            for (i = 0; i < length; i += 1) k = rep[i], typeof k == "string" && (v = str(k, value), v && partial.push(quote(k) + (gap ? ": " : ":") + v))
                        } else for (k in value) Object.hasOwnProperty.call(value, k) && (v = str(k, value), v && partial.push(quote(k) + (gap ? ": " : ":") + v));
                        return v = partial.length === 0 ? "{}" : gap ? "{\n" + gap + partial.join(",\n" + gap) + "\n" + mind + "}" : "{" + partial.join(",") + "}", gap = mind, v
                }
            }
            typeof JSON.stringify != "function" && (JSON.stringify = function (value, replacer, space) {
                var i;
                gap = "", indent = "";
                if (typeof space == "number") for (i = 0; i < space; i += 1) indent += " ";
                else typeof space == "string" && (indent = space);
                rep = replacer;
                if (!replacer || typeof replacer == "function" || typeof replacer == "object" && typeof replacer.length == "number") return str("", {
                    "": value
                });
                throw new Error("JSON.stringify")
            }), typeof JSON.parse != "function" && (JSON.parse = function (text, reviver) {
                var j;

                function walk(holder, key) {
                    var k, v, value = holder[key];
                    if (value && typeof value == "object") for (k in value) Object.hasOwnProperty.call(value, k) && (v = walk(value, k), v !== undefined ? value[k] = v : delete value[k]);
                    return reviver.call(holder, key, value)
                }
                text = String(text), cx.lastIndex = 0, cx.test(text) && (text = text.replace(cx, function (a) {
                    return "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4)
                }));
                if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]").replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) return j = eval("(" + text + ")"), typeof reviver == "function" ? walk({
                    "": j
                }, "") : j;
                throw new SyntaxError("JSON.parse")
            })
        } ();

        function utf8_encode(argString) {
            var string = argString + "",
                utftext = "",
                start, end, stringl = 0;
            start = end = 0, stringl = string.length;
            for (var n = 0; n < stringl; n++) {
                var c1 = string.charCodeAt(n),
                    enc = null;
                c1 < 128 ? end++ : c1 > 127 && c1 < 2048 ? enc = String.fromCharCode(c1 >> 6 | 192) + String.fromCharCode(c1 & 63 | 128) : enc = String.fromCharCode(c1 >> 12 | 224) + String.fromCharCode(c1 >> 6 & 63 | 128) + String.fromCharCode(c1 & 63 | 128), enc !== null && (end > start && (utftext += string.slice(start, end)), utftext += enc, start = end = n + 1)
            }
            return end > start && (utftext += string.slice(start, stringl)), utftext
        }
        function sha1(str) {
            var rotate_left = function (n, s) {
                var t4 = n << s | n >>> 32 - s;
                return t4
            },
                cvt_hex = function (val) {
                    var str = "",
                        i, v;
                    for (i = 7; i >= 0; i--) v = val >>> i * 4 & 15, str += v.toString(16);
                    return str
                },
                blockstart, i, j, W = new Array(80),
                H0 = 1732584193,
                H1 = 4023233417,
                H2 = 2562383102,
                H3 = 271733878,
                H4 = 3285377520,
                A, B, C, D, E, temp;
            str = this.utf8_encode(str);
            var str_len = str.length,
                word_array = [];
            for (i = 0; i < str_len - 3; i += 4) j = str.charCodeAt(i) << 24 | str.charCodeAt(i + 1) << 16 | str.charCodeAt(i + 2) << 8 | str.charCodeAt(i + 3), word_array.push(j);
            switch (str_len % 4) {
                case 0:
                    i = 2147483648;
                    break;
                case 1:
                    i = str.charCodeAt(str_len - 1) << 24 | 8388608;
                    break;
                case 2:
                    i = str.charCodeAt(str_len - 2) << 24 | str.charCodeAt(str_len - 1) << 16 | 32768;
                    break;
                case 3:
                    i = str.charCodeAt(str_len - 3) << 24 | str.charCodeAt(str_len - 2) << 16 | str.charCodeAt(str_len - 1) << 8 | 128
            }
            word_array.push(i);
            while (word_array.length % 16 != 14) word_array.push(0);
            word_array.push(str_len >>> 29), word_array.push(str_len << 3 & 4294967295);
            for (blockstart = 0; blockstart < word_array.length; blockstart += 16) {
                for (i = 0; i < 16; i++) W[i] = word_array[blockstart + i];
                for (i = 16; i <= 79; i++) W[i] = rotate_left(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);
                A = H0, B = H1, C = H2, D = H3, E = H4;
                for (i = 0; i <= 19; i++) temp = rotate_left(A, 5) + (B & C | ~B & D) + E + W[i] + 1518500249 & 4294967295, E = D, D = C, C = rotate_left(B, 30), B = A, A = temp;
                for (i = 20; i <= 39; i++) temp = rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 1859775393 & 4294967295, E = D, D = C, C = rotate_left(B, 30), B = A, A = temp;
                for (i = 40; i <= 59; i++) temp = rotate_left(A, 5) + (B & C | B & D | C & D) + E + W[i] + 2400959708 & 4294967295, E = D, D = C, C = rotate_left(B, 30), B = A, A = temp;
                for (i = 60; i <= 79; i++) temp = rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 3395469782 & 4294967295, E = D, D = C, C = rotate_left(B, 30), B = A, A = temp;
                H0 = H0 + A & 4294967295, H1 = H1 + B & 4294967295, H2 = H2 + C & 4294967295, H3 = H3 + D & 4294967295, H4 = H4 + E & 4294967295
            }
            return temp = cvt_hex(H0) + cvt_hex(H1) + cvt_hex(H2) + cvt_hex(H3) + cvt_hex(H4), temp.toLowerCase()
        } (function (exports) {
            var app;
            exports || (app = window.CLIPBOARD = window.CLIPBOARD || {}, app.blobsShared = app.blobsShared || {}, exports = app.blobsShared);
            var compressDict = null,
                compressRE = null,
                compressAttrs = ["-moz-border-bottom-left-radius", "-moz-border-bottom-right-radius", "-moz-border-top-left-radius", "-moz-border-top-right-radius", "-moz-border-radius", "-moz-box-shadow", "-webkit-border-bottom-left-radius", "-webkit-border-bottom-right-radius", "-webkit-border-top-left-radius", "-webkit-border-top-right-radius", "-webkit-border-radius", "-webkit-box-shadow", "background-attachment", "background-clip", "background-color", "background-image", "background-origin", "background-position", "background-repeat", "background", "border-bottom-color", "border-bottom-left-radius", "border-bottom-right-radius", "border-bottom-style", "border-bottom-width", "border-collapse", "border-left-color", "border-top", "border-right", "border-bottom", "border-left", "border-style", "border-color", "border-radius", "border-width", "border", "border-left-style", "border-left-width", "border-right-color", "border-right-style", "border-right-width", "border-spacing", "border-top-color", "border-top-left-radius", "border-top-right-radius", "border-top-style", "border-top-width", "bottom", "box-shadow", "caption-side", "clear", "clip", "color", "content", "counter-increment", "counter-reset", "cursor", "direction", "display", "empty-cells", "float", "font-family", "font-size", "font-style", "font-variant", "font-weight", "font", "height", "left", "letter-spacing", "line-height", "list-style-image", "list-style-position", "list-style-type", "list-style", "margin", "margin-bottom", "margin-left", "margin-right", "margin-top", "marker-offset", "max-height", "max-width", "min-height", "min-width", "opacity", "outline-color", "outline-style", "outline-width", "outline", "overflow-x", "overflow-y", "padding-bottom", "padding-left", "padding-right", "padding-top", "padding", "page-break-after", "page-break-before", "page-break-inside", "position", "quotes", "right", "table-layout", "text-align", "text-decoration", "text-indent", "text-transform", "top", "vertical-align", "visibility", "white-space", "width", "word-spacing", "z-index"],
                compressVals = ["normal", "no-repeat no-repeat", "none", "block", "pointer", "hidden", "relative", "table-cell", "left", "bold", "nowrap", "middle", "repeat no-repeat", "visible", "absolute", "Verdana", "inline-block", "right", "center", "text-bottom", "both", "inline", "verdana"],
                compressSymbols = [],
                compressInit = exports.compressInit = function () {
                    if (!compressDict) {
                        var c = 0;
                        compressDict = {};
                        for (var i = 0; i < compressAttrs.length; i++, c++) {
                            var s = compressAttrs[i].toLowerCase() + ":";
                            compressDict[s] = c, compressSymbols.push(s)
                        }
                        for (var i = 0; i < compressVals.length; i++, c++) {
                            var s = compressVals[i].toLowerCase() + ";";
                            compressDict[s] = c, compressSymbols.push(s)
                        }
                        var reStr = "(" + compressSymbols.join("|") + ")";
                        compressRE = new RegExp(reStr, "gim")
                    }
                };
            exports.compressText = function (text) {
                compressInit(), text = text.replace(/@/gim, "&at;");
                var start = 0,
                    out = [],
                    match = compressRE.exec(text);
                while (match) out.push(text.substring(start, match.index)), out.push("@" + compressDict[match[0].toLowerCase()] + ";"), start = compressRE.lastIndex, match = compressRE.exec(text);
                return out.push(text.substring(start)), out.join("")
            };
            var decompressText = exports.decompressText = function (text) {
                compressInit();
                var start = 0,
                        out = [],
                        match = text.indexOf("@", start);
                while (match != -1) {
                    out.push(text.substring(start, match));
                    var semicolon = text.indexOf(";", match),
                            numStr = text.substring(match + 1, semicolon);
                    out.push(compressSymbols[parseInt(numStr)]), start = semicolon + 1, match = text.indexOf("@", start)
                }
                return out.push(text.substring(start)), out.join("").replace(/&at;/gim, "@")
            }
        })(typeof process == "undefined" || !process.versions ? null : exports), function (win, doc, undefined) {
            var app = win.CLIPBOARD = win.CLIPBOARD || {},
                data = app.data = app.data || {},
                blobsShared = app.blobsShared = app.blobsShared || {},
                loc = window.location,
                baseURI = loc.protocol + "//" + loc.host;

            function AssertException(message) {
                this.message = message
            }
            AssertException.prototype.toString = function () {
                return "AssertException: " + this.message
            };

            function assert(exp, message) {
                if (!exp) throw new AssertException(message)
            }
            data.compressText = function (text) {
                return blobsShared.compressText(text)
            };
            var decompressText = data.decompressText = function (text) {
                return blobsShared.decompressText(text)
            },
                dataInitVal = !1;

            function dataInit() {
                dataInitVal || (USTORE.init(), blobsShared.compressInit(), dataInitVal = !0)
            }
            var timeDeltaVal = null;

            function timeDelta() {
                if (timeDeltaVal != null) return timeDeltaVal;
                var n = baseURI.length;
                if (window.location.href.substring(0, n) != baseURI) return 0;
                var serverTime = "";
                $.ajax({
                    type: "GET",
                    url: "/api/v1/time",
                    cache: !1,
                    async: !1,
                    success: function (res, status, xhr) {
                        serverTime = res
                    }
                });
                if (serverTime) {
                    var t1 = (new Date).getTime(),
                        t2 = parseInt(serverTime);
                    return timeDeltaVal = t2 - t1, timeDeltaVal
                }
                return null
            }
            function signMessage(url, _data, time, guid, secret) {
                var fields = [];
                _data != null && $.each(_data, function (k, v) {
                    var pair = encodeURIComponent(k) + "=" + encodeURIComponent(v);
                    fields.push(pair)
                }), fields.push("hmac_url=" + encodeURIComponent(url)), fields.push("hmac_time=" + encodeURIComponent(time)), fields.push("hmac_guid=" + encodeURIComponent(guid)), fields.push("hmac_nonce=" + sha1(url + time + secret));
                var str = fields.sort().join("&"),
                    sig = sha1(str + secret);
                return {
                    msg: str + "&hmac_sig=" + sig,
                    sig: sig
                }
            }
            function sendMessage(options, callback) {
                var sign = options.sign || !1,
                    verb = options.verb || null,
                    path = options.path || null,
                    cache = options.cache || !1,
                    raw = options.raw || null,
                    jsonp = options.jsonp || !1,
                    jsonpCallbackName = options.jsonpCallbackName || null;
                if (!verb || !path) return;
                if (jsonp && verb != "GET") return;
                var url = "";
                /^\/\//.test(path) ? url = path : url = baseURI + path;
                var time = (new Date).getTime() + timeDelta(),
                    successCallback = function (r, e, x) {
                        callback && callback(r.error, r.result, r.requestBody)
                    },
                    errorCallback = function (x, e, eo) {
                        if (!callback) return;
                        x.status >= 500 ? callback({
                            noConnection: !0,
                            statusCode: x.status
                        }, null) : callback({
                            unknown: !0,
                            statusCode: x.status
                        }, null)
                    },
                    isGet = verb === "GET";
                if (isGet) {
                    if (sign) {
                        var guid = data.guid(),
                            secret = data.secret(),
                            msgsig = signMessage(url, null, time, guid, secret);
                        url.indexOf("?") === -1 ? url += "?" + msgsig.msg : url += "&" + msgsig.msg
                    }
                    if (jsonp) {
                        var params = {
                            type: verb,
                            dataType: "jsonp",
                            url: url,
                            cache: cache,
                            async: !0,
                            success: successCallback,
                            error: errorCallback
                        };
                        jsonpCallbackName && (params.jsonp = !1, params.jsonpCallback = jsonpCallbackName), $.ajax(params)
                    } else $.ajax({
                        type: verb,
                        url: url,
                        cache: cache,
                        async: !0,
                        success: successCallback,
                        error: errorCallback
                    })
                } else {
                    var finalData = raw;
                    if (sign) {
                        var guid = data.guid(),
                            secret = data.secret(),
                            msgsig = signMessage(url, raw, time, guid, secret);
                        finalData = msgsig.msg
                    }
                    $.ajax({
                        type: verb,
                        url: url,
                        data: finalData,
                        cache: cache,
                        async: !0,
                        success: successCallback,
                        error: errorCallback
                    })
                }
            }
            var localSignPost = data.localSignPost = function (path, raw, callback) {
                sendMessage({
                    sign: !0,
                    verb: "POST",
                    path: path,
                    raw: raw
                }, callback)
            },
                localPost = data.localPost = function (path, raw, callback) {
                    sendMessage({
                        verb: "POST",
                        path: path,
                        raw: raw
                    }, callback)
                },
                localSignPut = data.localSignPut = function (path, raw, callback) {
                    sendMessage({
                        sign: !0,
                        verb: "PUT",
                        path: path,
                        raw: raw
                    }, callback)
                },
                localSignGet = data.localSignGet = function (path, callback) {
                    sendMessage({
                        sign: !0,
                        verb: "GET",
                        path: path
                    }, callback)
                },
                localGet = function (path, cache, callback) {
                    sendMessage({
                        verb: "GET",
                        path: path,
                        cache: cache
                    }, callback)
                },
                localSignDelete = data.localSignDelete = function (path, raw, callback) {
                    sendMessage({
                        sign: !0,
                        verb: "DELETE",
                        path: path,
                        raw: raw
                    }, callback)
                },
                localJsonP = function (path, cache, jsonpCallbackName, callback) {
                    sendMessage({
                        verb: "GET",
                        path: path,
                        cache: cache,
                        jsonp: !0,
                        jsonpCallbackName: jsonpCallbackName
                    }, callback)
                };

            function getData(name) {
                dataInit();
                var value = USTORE.getValue(name);
                return value == null || value == "" || value == "null" ? null : value
            }
            data.secret = function () {
                return getData("secret")
            }, data.login = function () {
                return getData("login")
            }, data.guid = function () {
                return getData("guid")
            }, data.searchPrivate = function (scope, query, beforeTime, callback) {
                var body = {
                    scope: scope
                };
                query && (body.query = query), beforeTime && (body.beforeTime = beforeTime), localSignPost("/api/v1/searchPrivate", body, callback)
            }, data.searchPublic = function (scope, query, beforeTime, callback) {
                var body = {
                    scope: scope
                };
                query && (body.query = query), beforeTime && (body.beforeTime = beforeTime), localPost("/api/v1/searchPublic", body, callback)
            }, data.getClip = function (clipId, callback) {
                localSignGet("/api/v1/clips/" + clipId, function (error, result) {
                    callback(error, result)
                })
            }, data.getBlob = function (blobId, callback) {
                var uri = "/api/v1/blobs/" + blobId;
                if (app.cdnBaseUrl) {
                    var jsonpCallbackName = "getBlobJsonPCallback_" + blobId;
                    uri = app.cdnBaseUrl + uri + "/" + jsonpCallbackName, localJsonP(uri, !0, jsonpCallbackName, cb)
                } else localGet(uri, !0, cb);

                function cb(error, result) {
                    error || result.html != null && (result.html = decompressText(result.html).replace(/&amp;/gm, "&")), callback(error, result)
                }
            }, data.deleteClip = function (key, callback) {
                localSignDelete("/api/v1/clips/" + key, {}, function (error, result) {
                    callback(error)
                })
            }, data.setClipAsPublic = function (clipID, isPublic, callback) {
                localSignPut("/api/v1/clips/" + clipID, {
                    isPrivate: !isPublic
                }, function (error, result) {
                    callback(error)
                })
            }, data.setClipAnnotation = function (clipID, annotation, callback) {
                localSignPut("/api/v1/clips/" + clipID, {
                    annote: annotation
                }, function (error, result) {
                    callback(error)
                })
            }, data.reClip = function (clipID, callback) {
                localSignPost("/api/v1/clips/" + clipID, {
                    reclip: !0
                }, function (error, result) {
                    callback(error)
                })
            }, data.getComments = function (clipIds, callback) {
                var idList = "";
                typeof clipIds == "string" ? idList = clipIds : clipIds.join && (idList = clipIds.join(",")), localSignGet("/api/v1/comments/" + idList, function (error, result) {
                    callback(error, result)
                })
            }, data.addComment = function (clipId, text, callback) {
                localSignPut("/api/v1/comments/" + clipId, {
                    text: text
                }, function (error, result) {
                    callback(error, result)
                })
            }, data.deleteComment = function (commentId, clipId, callback) {
                localSignDelete("/api/v1/comments/" + commentId, {
                    clipId: clipId
                }, function (error, result) {
                    callback(error, result)
                })
            }, data.whoAmI = function (callback) {
                localSignGet("/api/v1/secure/users/", function (error, result) {
                    callback(error, result)
                })
            }, data.updateUserProfile = function (updates, callback) {
                localSignPut("/api/v1/users/", updates, callback)
            }, data.emailClip = function (clipId, toEmail, callback) {
                localSignPost("/api/v1/share/emailClip", {
                    clipId: clipId,
                    toEmail: toEmail
                }, callback)
            }, data.getFbConfig = function (callback) {
                localGet("/api/v1/secure/fb/config", !1, callback)
            }, data.findFbFriends = function (callback) {
                localSignGet("/api/v1/secure/fb/friends/", callback)
            }, data.connectFbAccount = function (code, callback) {
                localSignPost("/api/v1/secure/fb/connect/", {
                    code: code
                }, callback)
            }, data.disconnectFbAccount = function (callback) {
                localSignDelete("/api/v1/secure/fb/connect/", {}, callback)
            }, data.fbUserInfo = function (callback) {
                localSignGet("/api/v1/secure/fb/me/", callback)
            }, data.getNotifications = function (callback) {
                localSignGet("/api/v1/notifications?markRead=1", callback)
            }, data.getUnreadNotificationCount = function (callback) {
                localSignGet("/api/v1/notifications/unreadCount", callback)
            }, data.sendInvite = function (email, callback) {
                localSignPost("/api/v1/invites/sendInvite", {
                    email: email
                }, callback)
            }, data.getInviteCount = function (callback) {
                localSignGet("/api/v1/invites/inviteCount", function (error, result) {
                    callback(error, result)
                })
            }, data.sendFeedback = function (message, callback) {
                localSignPost("/api/v1/feedback", {
                    message: message
                }, function (error, result) {
                    callback(error)
                })
            }, data.addFollow = function (favorite, callback) {
                localSignPost("/api/v1/follows", {
                    item: favorite
                }, function (error, result) {
                    callback(error)
                })
            }, data.deleteFollow = function (favorite, callback) {
                localSignDelete("/api/v1/follows/" + encodeURIComponent(encodeURIComponent(favorite)), {}, function (error, result) {
                    callback(error)
                })
            }, data.getFollows = function (callback) {
                localSignGet("/api/v1/follows", function (error, result) {
                    callback(error, result)
                })
            }, data.testFollow = function (key, callback) {
                localSignGet("/api/v1/follows/" + key, function (error, result) {
                    callback(error, result)
                })
            }, data.updateUserTag = function (user, tag, description, callback) {
                localSignPut("/api/v1/users/" + user + "/tags/" + encodeURIComponent(encodeURIComponent(tag)), {
                    description: description
                }, function (error, result) {
                    callback(error)
                })
            }, data.getUserTag = function (user, tag, callback) {
                localSignGet("/api/v1/users/" + user + "/tags/" + encodeURIComponent(encodeURIComponent(tag)), function (error, result) {
                    callback(error, result)
                })
            }, data.getLogin = function (login, callback) {
                localGet("/api/v1/secure/logins/" + login, !1, function (error, result) {
                    callback(error, result)
                })
            }, data.getSession = function (callback) {
                localSignGet("/api/v1/sessions", function (error, result) {
                    callback(error, result)
                })
            }, data.setAsFrozen = function (clipId, state, callback) {
                localSignPut("/api/v1/clips/" + clipId, {
                    frozen: state
                }, function (error, result) {
                    callback(error)
                })
            }, data.requestEmailVerification = function (callback, token) {
                var url = "/api/v1/validateemail";
                token && (url += "/" + token), localSignPost(url, {}, function (error, result) {
                    callback(error, result)
                })
            }, data.getEmailVerification = function (callback) {
                var url = "/api/v1/validateemail";
                localSignGet(url, function (error, result) {
                    callback(error, result)
                })
            }, function (data) {
                function updateBrokenState(clipId, isBroken, callback) {
                    localSignPut("/api/v1/clips/" + clipId, {
                        broken: isBroken ? !0 : !1
                    }, function (error, result) {
                        callback(error, result)
                    })
                }
                data.flagAsBroken = function (clipId, callback) {
                    updateBrokenState(clipId, !0, callback)
                }, data.flagAsNotBroken = function (clipId, callback) {
                    updateBrokenState(clipId, !1, callback)
                }
            } (data), data.flagAsInappropriate = function (clipId, reason, callback) {
                localSignPut("/api/v1/clips/" + clipId, {
                    _cb_action: "flagInappropriate",
                    reason: reason || "<none>"
                }, callback)
            }
        } (window, document)
    }
})()