
function tsvToJson(input, hasHeaders) {
    try {
        var info = input.replace(/['"]/g, '');
        var lines = info.split('\n');
        var firstLine = lines.shift().split('\t');
        var json = [];

        // Helper function to remove quotes
        // and parse numeric values
        var removeQuotes = function (string) {
            string = string.replace(/(['"])/g, "\\$1");
            if (!isNaN(string)) {
                string = parseFloat(string);
            }
            return string;
        };

        var c = 'A';
        if (hasHeaders == false) {
            $.each(firstLine, function (index, item) {
                firstLine[index] = c;
                c = String.fromCharCode(c.charCodeAt() + 1) // Returns the next letter
            });
        }

        $.each(lines, function (index, item) {
            var lineItem = item.split('\t');
            var jsonLineEntry = {};

            $.each(lineItem, function (index, item) {
                jsonLineEntry[firstLine[index]] = removeQuotes(item);
            });
            json.push(jsonLineEntry);

        });

        return json;
    }
    catch (err) {

    }
}

function json2Table(jsonArray, tableName) {
    try {
        var i = 0;
        var table = $(document.createElement('table'));
        table.attr('style', 'border-collapse: collapse; border:1px solid #000;padding:5px;spacing:5px;width:90%;')
        $.each(jsonArray, function (key, item) {
            var table_row = $('<tr>');
            table_row.attr('id', 'uploadedFileRow' + i);
            i += 1;
            table_row.attr('style', 'border:1px solid #000;text-align:left;')
            $.each(item, function (itemKey, itemValue) {
                if (key == 0) {
                    var th = $(document.createElement('th'));
                    th.attr('style', 'border:1px solid #000; text-align:left;font-weight:600;padding:5px;spacing:5px;');
                    th.html(itemKey);
                    table.append(th);
                }
                var td = $(document.createElement('td'));
                td.attr('style', 'border:1px solid #000; text-align:left;padding:5px;spacing:5px;');
                td.html(itemValue);
                table_row.append(td);
            });
            var td = $(document.createElement('td'));
            td.attr('style', 'border:1px solid #000; text-align:left;padding:5px;spacing:5px;');
            td.attr('class', 'uploadedFileRowComment')
            td.html('');
            table_row.append(td);

            table.append(table_row);
        });
        $('#' + tableName).append(table);
    }
    catch (err) {

    }
}
