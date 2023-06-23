fm.util.addOnLoad(
    function () {
        var cookies = {
            get: function (name) {
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
            },
            set: function (name, value) {
                var exdate = new Date();
                var expiredays = 10;
                exdate.setDate(exdate.getDate() + expiredays);
                document.cookie = name + "=" + value + "; expires=" + exdate.toUTCString();
            }
        };

        var isNotification = false;
        var host = 'https://saasCommander.com/';

        //alert('Host is set to local');
        //host = '';

        var webSyncHandleUrl = host + 'websync.ashx';
        var client = new fm.websync.client(webSyncHandleUrl);
        var __sc = {
            license: cookies.get('__scLicense'),
            venue: cookies.get('__scVenue'),
            userId: cookies.get('__scUserId'),
            userName: cookies.get('__scUserName')
        };

        var chat = {
            userName: __sc.userName,
            clientId: 0,
            channels: {
                main: '/' + __sc.license + '/' + __sc.venue
            },
            dom: {
                chat: {
                    text: document.getElementById('text'),
                    log: document.getElementById('log'),
                    send: document.getElementById('send')
                }
            },
            util: {
                connect: function () {
                    client.connect({
                        onSuccess: function (args) {
                            chat.clientId = args.clientId;
                            chat.util.clearLog();
                            chat.util.logSuccess('Connected...');
                        },
                        onFailure: function (args) {
                            chat.util.logSuccess('Could not connect to Chat.');
                            chat.util.logSuccess('Error.connect: ' + args.getException().message);
                        }
                    });

                    client.subscribe({
                        channel: chat.channels.main,
                        onSuccess: function (args) {
                            chat.util.post('chat/subscribe', __sc);
                            chat.util.logSuccess('Subscribed as ' + chat.userName);
                            var logs = args.getExtensionValue('logs');
                            if (!_.isNull(logs)) {
                                var itsMe = false;
                                for (var i = 0; i < logs.length; i++) {
                                    if (chat.userName == logs[i].userName) {
                                        itsMe = true;
                                    }
                                    else {
                                        itsMe = false;
                                    }
                                    chat.util.logMessage(logs[i].userName, logs[i].text, itsMe);
                                }
                            }
                        },
                        onFailure: function (args) {
                            chat.util.logSuccess('Could not subscribe to chat.');
                            chat.util.logSuccess('Error.subscribe: ' + args.getException().message);
                        },
                        onReceive: function (args) {
                            chat.util.logMessage(args.getData().userName, args.getData().text, args.getWasSentByMe());
                            if (!args.getWasSentByMe()) {
                                var notif = args.getData().userName + ': ' + args.getData().text;
                                if (isNotification == true) {
                                    notifyMe(notif);
                                }
                                else {
                                    toastr.info(notif);
                                }
                            }
                        }
                    });
                },
                start: function () {
                    if (__sc.userName != '') {
                        chat.userName = __sc.userName;
                        $('#chatLogArea').show();
                        chat.util.scroll();
                        chat.dom.chat.text.focus();
                        chat.util.connect();
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
                    } else {
                        client.publish({
                            channel: chat.channels.main,
                            data: {
                                license: __sc.license,
                                venue: __sc.venue,
                                userId: __sc.userId,
                                userName: chat.userName,
                                text: chat.dom.chat.text.value
                            },
                            onSuccess: function (args) {
                                chat.util.clear(chat.dom.chat.text);
                            },
                            onFailure: function (args) {
                                alert(('Error.publish: ' + args.getException().message));
                            }
                        });
                    }
                },
                post: function (ws, data) {
                    var url = host + 'chat.svc/' + ws;
                    data = JSON.stringify(data);
                    $.ajax({
                        type: "POST",
                        url: url,
                        contentType: "application/json",
                        data: data,
                        dataType: "json",
                        processdata: false,
                        success: function (data) {
                            var a = 1;
                        },
                        error: function (a, b, c) {
                            var x = 1;
                        }
                    });
                },
                notifyMe: function (text) {
                    // Let's check if the browser supports notifications
                    if (!("Notification" in window)) {
                        //alert("This browser does not support desktop notification");
                        isNotification = false;
                        return;
                    }

                        // Let's check whether notification permissions have already been granted
                    else if (Notification.permission === "granted") {
                        // If it's okay let's create a notification
                        isNotification = true;
                        var notification = new Notification(text);
                    }

                        // Otherwise, we need to ask the user for permission
                    else if (Notification.permission !== 'denied') {
                        Notification.requestPermission(function (permission) {
                            // If the user accepts, let's create a notification
                            if (permission === "granted") {
                                isNotification = true;
                                var notification = new Notification(text);
                            }
                        });
                    }

                    // At last, if the user has denied notifications, and you 
                    // want to be respectful there is no need to bother them any more.

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
                clear: function (el) {
                    el.value = '';
                },
                clearLog: function () {
                    chat.dom.chat.log.innerHTML = '';
                },
                logMessage: function (alias, text, me) {
                    var html = '<div style="border-radius: 5px; border:1px solid #d3d3d3; padding:5px;display:inline-block;"><span data-channel="' + chat.channels.main + '" data-alias="' + chat.alias + '"';
                    if (me) {
                        html += ' class="me"';
                        html += '>Me: ' + text + '</span></div>';
                    }
                    else {
                        html += '>' + alias + ': ' + text + '</span></div>';
                    }
                    html = '<div style="padding:5px;">' + html + '</div>';
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
                    $(div).attr('data-channel', chat.channels.main);
                    $(div).attr('data-userName', chat.userName);
                    div.innerHTML = html;
                    chat.dom.chat.log.appendChild(div);
                    chat.util.scroll();
                },
                scroll: function () {
                    chat.dom.chat.log.scrollTop = chat.dom.chat.log.scrollHeight;
                }
            }
        };

        chat.util.observe(chat.dom.chat.send, 'click', function (e) {
            chat.util.send();
        });

        chat.util.observe(chat.dom.chat.text, 'keydown', function (e) {
            if (chat.util.isEnter(e)) {
                chat.util.send();
                chat.util.stopEvent(e);
            }
        });

        var init = function () {
            var h = $(window).height();
            $('#log').css('height', (h - 200).toString() + 'px');
            chat.util.notifyMe('Desktop notifications are active now.');
            chat.util.start();
        }();

    });

