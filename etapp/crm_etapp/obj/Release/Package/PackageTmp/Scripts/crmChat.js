var client = new fm.websync.client('/websync.ashx');

client.connect({
    onSuccess: function (args) {
        // initialize the chat area
        chat.clientId = args.clientId;
        chat.util.clearLog();
        chat.util.logSuccess('Connected to WebSync.');
        chat.util.show(chat.dom.prechat.container);
        chat.util.hide(chat.dom.chat.container);
    },
    onFailure: function (args) {
        chat.util.logSuccess('Could not connect to WebSync.');
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
