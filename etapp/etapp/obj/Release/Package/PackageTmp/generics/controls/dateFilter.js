$.fn.dateFilter = function (params) {

    //Destroy: $('#reportrange').data('daterangepicker').remove();

    var o = {};
    try {
        var that = this;
        
        /*----------------------------------------------------
        - Inicializa el control
        -----------------------------------------------------*/
        var defaultRanges = [];
        if (app.lan == 'es') {
            defaultRanges = {
                //Label: [ from, to, rangeCode, format ]
                'Hoy' : [moment().startOf('day'), moment({ hour: 23, minute: 59, seconds: 59 }), 'today', '@From'],
                'Ayer': [moment().startOf('day').subtract('days', 1), moment().subtract('days', 1).endOf('day'), 'yesterday', '@From'],
                'Hace 7 dias': [moment().startOf('day').subtract('days', 6), moment().endOf('day'), '-7', '@From - @To'],
                'Ult. 30 dias': [moment().startOf('day').subtract('days', 29), moment().endOf('day'), '-30', '@From - @To'],
                'Este Mes': [moment().startOf('day').startOf('month'), moment().endOf('month'), 'thismonth', '@From - @To'],
                'Mes Anterior': [moment().startOf('day').subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month'), 'lastmonth', '@From - @To']
            };
        }
        else{
            defaultRanges = {
                //Label: [ from, to, rangeCode, format ]
                'Today': [moment().startOf('day'), moment({ hour: 23, minute: 59, seconds: 59 }), 'today', '@From'],
                'Yesterday': [moment().startOf('day').subtract('days', 1), moment().subtract('days', 1).endOf('day'), 'yesterday', '@From'],
                'Last 7 Days': [moment().startOf('day').subtract('days', 6), moment().endOf('day'), '-7', '@From - @To'],
                'Last 30 Days': [moment().startOf('day').subtract('days', 29), moment().endOf('day'), '-30', '@From - @To'],
                'This Month': [moment().startOf('day').startOf('month'), moment().endOf('month'), 'thismonth', '@From - @To'],
                'Last Month': [moment().startOf('day').subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month'), 'lastmonth', '@From - @To']
            };
        }

        /*----------------------------------------------------
        - Opciones por default del control
        -----------------------------------------------------*/
        o.options = {
            //parentEl: this,
            startDate: moment().startOf('day'),//moment("1900-01-01", "YYYY-MM-DD"),//
            endDate: moment().endOf('day'),
            defaultRange: 'today',
            minDate: moment("1900-01-01", "YYYY-MM-DD"),
            maxDate: moment().endOf('day'),
         //   dateLimit: { days: 5 },
            showDropdowns: true,
            showWeekNumbers: true,
            timePicker: true,
            timePickerIncrement: 1,
            timePicker12Hour: true,
            ranges: defaultRanges,
            opens: 'right',
            buttonClasses: ['btn btn-default'],
            applyClass: 'btn-small btn-primary',
            cancelClass: 'btn-small',
            format: 'MM/DD/YYYY',
            separator: app.language.getLabel('DateFilter.Separator'),
            locale: {
                applyLabel: app.language.getLabel('DateFilter.Submit'),
                cancelLabel: app.language.getLabel('DateFilter.Clear'),
                fromLabel: app.language.getLabel('DateFilter.From'),
                toLabel: app.language.getLabel('DateFilter.To'),
                customRangeLabel: app.language.getLabel('DateFilter.Custom'),
                daysOfWeek: app.language.getLabel('DateFilter.DaysOfWeek'),
                monthNames: app.language.getLabel('DateFilter.MonthNames'),
                firstDay: 1
            },
            onChange: function () {
              
            },
            enableCustom: true,
            dateRangePickerID: 'daterangepicker',
            parent: that
        };
        $.extend(o.options, params);
       // console.log("parentID --->  +" + that.toSource());
        /*----------------------------------------------------
        - Funcion de Callback para cuando se selecciona un Item
        -----------------------------------------------------*/
        o.callback = function (start, end) {
            try {
                var li = $('.'+ o.options.dateRangePickerID +' > .ranges').find('.active')
                var txt = li.html();
                o.setRange(li.attr('data-code'), start, end);
            }
            catch (exc) {
                console.error('dateFilter.Callback: ' + exc.message);
            }
        };

        /*----------------------------------------------------
        - Funcion para setear un rango / fechas iniciales
        -----------------------------------------------------*/
        o.setRange = function (rangeCode, start, end) {
            try {
                var selected = null;
                for (var range in o.options.ranges) {
                    if (o.options.ranges[range][2] == rangeCode) {
                        selected = range;
                        break;
                    }
                }
                var display = '';
                if (selected != null) {
                    display = '<b>' + selected + '</b>: ' + o.options.ranges[selected][3];
                    start = (typeof (start) != 'undefined') ? start : o.options.ranges[selected][0];
                    end = (typeof (end) != 'undefined') ? end : o.options.ranges[selected][1];
                }
                else
                    display = ((rangeCode == 'custom') ? '<b>'+ app.language.getLabel('DateFilter.Custom') +'</b>: ' : '') + '@From - @To ';

               // display = display.replace('@From', start.format('ll'));
                // display = display.replace('@To', end.format('ll'));

                display = display.replace('@From', start.format('YYYY-MM-DD HH:mm'));
                display = display.replace('@To', end.format('YYYY-MM-DD HH:mm'));
                $(that).find('span').css("font-size","7px;");
                $(that).find('span').html(display);

                $(that).attr('data-from', start.format('YYYY-MM-DD HH:mm:ss'));
                $(that).attr('data-to', end.format('YYYY-MM-DD HH:mm:ss'));

                if (o.options.onChange != null)
                    o.options.onChange(rangeCode,start, end);
            }
            catch (exc) {
                console.error('fn.dateFilter.setRange: ' + exc.message);
            }
        };

        /*----------------------------------------------------
        - Destroy del control (Liberacion de recursos
        -----------------------------------------------------*/
        o.destroy = function () {
            try {
                el.data('daterangepicker').remove();
                el.remove();
            }
            catch (exc) {
                console.error('fn.dateFilter.destroy: ' + exc.message);
            }
        };

        /*----------------------------------------------------
        - Convierte el control en DateRangePicker
        -----------------------------------------------------*/
        o.options.dateRangePickerID = 'drp' + moment() + Math.floor((Math.random() * 1000) + 1);
        $(this).daterangepicker(o.options, o.callback);
        o.setRange(o.options.defaultRange);

        /*----------------------------------------------------
        - Store arbitrary data associated with the specified element and/or return the value that was set.
        -----------------------------------------------------*/
        var el = $(this);
        if (el.data('datefilter'))
            el.data('datefilter').remove();
        el.data('datefilter', o);
    }
    catch (exc) {
        console.error('fn.dateFilter error: ' + exc.message);
    }
    return o;
};