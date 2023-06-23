var thisScript = document.getElementsByTagName("script");
thisScript = thisScript[thisScript.length - 1];
//Global variables
var webSyncHandleUrl = (thisScript.hasAttribute('data-handler')) ? thisScript.getAttribute('data-handler') : null;

fm.util.addOnLoad(function () {

    var dealerId = getCookie('ETCRMDID');
    var firstName = getCookie('ETCRMFN');
    if (firstName == '') {
        firstName = 'Unknown';
    }

    var chat = {
        alias: firstName,
        clientId: 0,
        channels: {
            main: '/etChat/' + dealerId
        },
        dom: {
            prechat: {
                container: document.getElementById('prechat'),
                alias: document.getElementById('alias'),
                start: document.getElementById('start')
            },
            chat: {
                container: document.getElementById('chat'),
                text: document.getElementById('text'),
                log: document.getElementById('log'),
                send: document.getElementById('send')
            }
        },
        util: {
            start: function () {
                if (chat.util.isEmpty(chat.dom.prechat.alias)) {
                    chat.util.setInvalid(chat.dom.prechat.alias);
                } else {
                    chat.alias = chat.dom.prechat.alias.value;
                    chat.util.hide(chat.dom.prechat.container);
                    chat.util.show(chat.dom.chat.container);
                    chat.util.scroll();
                    chat.dom.chat.text.focus();
                }
            },
            stopEvent: function (event) {
                if (event.preventDefault) {
                    event.preventDefault();
                } else {
                    event.returnValue = false;
                }
                if (event.stopPropagation) {
                    event.stopPropagation();
                } else {
                    event.cancelBubble = true;
                }
            },
            send: function () {
                if (chat.util.isEmpty(chat.dom.chat.text)) {
                    chat.util.setInvalid(chat.dom.chat.text);
                    alert('Invalid content - please write something');
                } else {
                    client.publish({
                        channel: chat.channels.main,
                        data: {
                            alias: chat.alias,
                            text: chat.dom.chat.text.value
                        },
                        onSuccess: function (args) {
                            chat.util.clear(chat.dom.chat.text);
                        },
                        onFailure: function (args) {
                            chat.util.start();
                        }
                    });
                }
            },
            show: function (el) {
                el.style.display = '';
            },
            hide: function (el) {
                el.style.display = 'none';
            },
            clear: function (el) {
                el.value = '';
            },
            observe: fm.util.observe,
            isEnter: function (e) {
                return (e.keyCode == 13);
            },
            isEmpty: function (el) {
                return (el.value == '');
            },
            setInvalid: function (el) {
                el.className = 'invalid';
            },
            clearLog: function () {
                chat.dom.chat.log.innerHTML = '';
            },
            logMessage: function (alias, text, me) {
                var html = '<span';
                if (me) {
                    html += ' class="me"';
                }
                html += '>' + alias + ': ' + text + '</span>';
                chat.util.log(html);
            },
            logSuccess: function (text) {
                chat.util.log('<span class="success">' + text + '</span>');
            },
            logFailure: function (text) {
                chat.util.log('<span class="failure">' + text + '</span>');
            },
            log: function (html) {
                var div = document.createElement('div');
                div.innerHTML = html;
                chat.dom.chat.log.appendChild(div);
                chat.util.scroll();
            },
            scroll: function () {
                chat.dom.chat.log.scrollTop = chat.dom.chat.log.scrollHeight;
            }
        }
    };

    chat.util.observe(chat.dom.prechat.start, 'click', function (e) {
        chat.util.start();
    });

    chat.util.observe(chat.dom.prechat.alias, 'keydown', function (e) {
        if (chat.util.isEnter(e)) {
            chat.util.start();
            chat.util.stopEvent(e);
        }
    });

    chat.util.observe(chat.dom.chat.send, 'click', function (e) {
        chat.util.send();
    });

    chat.util.observe(chat.dom.chat.text, 'keydown', function (e) {
        if (chat.util.isEnter(e)) {
            chat.util.send();
            chat.util.stopEvent(e);
        }
    });

    var client = new fm.websync.client(webSyncHandleUrl);

    client.connect({
        onSuccess: function (args) {
            chat.clientId = args.clientId;
            chat.util.clearLog();
            chat.util.logSuccess('Connected to eTrack Chat.');
            chat.util.show(chat.dom.prechat.container);
            chat.util.hide(chat.dom.chat.container);
        },
        onFailure: function (args) {
            chat.util.logSuccess('Could not connect to eTrack Chat.');
        }
    });

    client.subscribe({
        channel: chat.channels.main,
        onSuccess: function (args) {
            chat.util.logSuccess('Subscribed to chat.');
            var logs = args.getExtensionValue('logs');
            for (var i = 0; i < logs.length; i++) {
                chat.util.logMessage(logs[i].alias, logs[i].text, false);
            }
        },
        onFailure: function (args) {
            chat.util.logSuccess('Could not subscribe to chat.');
        },
        onReceive: function (args) {
            chat.util.logMessage(args.getData().alias, args.getData().text, args.getWasSentByMe());
        }
    });

});

function getCookie(name) {
    try {
        var value = '';
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(name + "=");
            if (c_start != -1) {
                c_start = c_start + name.length + 1;
                c_end = document.cookie.indexOf(";", c_start);
                if (c_end == -1) c_end = document.cookie.length;
                value = unescape(document.cookie.substring(c_start, c_end));
            }
        }
        return value;
    }
    catch (err) {

    }
}