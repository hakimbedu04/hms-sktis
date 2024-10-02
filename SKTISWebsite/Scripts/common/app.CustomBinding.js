// DatePicker Binding
ko.bindingHandlers.dateTimePicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().dateTimePickerOptions || {};
        //console.log(allBindingsAccessor().evetapnt);
        var changeDate = allBindingsAccessor().dateChange;
        //$(element).change(function() {
        //    console.log('change date');
        //});
        options.format = options.format || 'DD/MM/YYYY';
        $(element).datetimepicker(options);

        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            //console.log(valueAccessor());
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                if (event.date != null && !(event.date instanceof Date)) {
                    value(event.date.format(options.format));
                }
            }
            if (typeof changeDate != "undefined") {
                changeDate(event);
                //    allBindingsAccessor().event(event);
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("DateTimePicker");
            if (picker) {
                picker.destroy();
            }
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //var options = allBindingsAccessor().dateTimePickerOptions || {};
        //var objDate = moment(new Date(ko.utils.unwrapObservable(valueAccessor())));
        var picker = $(element).data("DateTimePicker");
        //var value = valueAccessor();

        if (picker) {
            var koDate = ko.utils.unwrapObservable(valueAccessor());
            picker.date(ko.utils.unwrapObservable(valueAccessor()));
            //value(ko.utils.unwrapObservable(valueAccessor()));
        }
        
        // Force calendar picker layout on Edit Row
        //if ($(element).hasClass('insidetable')) {
        //    $(element).find('.input-group-addon').on('click', function () {
        //        var $menuItem = $(this),
        //        $submenuWrapper = $menuItem.prev('.dropdown-menu');

        //        // grab the menu item's position relative to its positioned parent
        //        var menuItemPos = $menuItem.parent().position();

        //        // place the submenu in the correct position relevant to the menu item
        //        $submenuWrapper.css({
        //            auto: menuItemPos.auto + Math.round($menuItem.height() * 1.50),
        //            left: menuItemPos.left,
        //        });
        //    });
        //}
    }
};

ko.bindingHandlers.dateTimePickerClick = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().dateTimePickerOptions || {};
        var changeDate = allBindingsAccessor().dateChange;
        options.format = options.format || 'DD/MM/YYYY';
        $(element).datetimepicker(options);
        var lastValue = ko.utils.unwrapObservable(valueAccessor());

        ko.utils.registerEventHandler(element, "dp.show", function (event) {
            var value = valueAccessor();
            //var lastValue = ko.utils.unwrapObservable(valueAccessor());
            if (ko.isObservable(value)) {
                if (lastValue != null && !(event.date instanceof Date)) {
                    //value(event.date.format(options.format));
                    value(lastValue);
                } else {
                    //value(allBindingsAccessor().defaultRequestDate());
                    value(options.defaultDate);
                }
            }
            if (typeof changeDate != "undefined") {
                changeDate(event);
                //    allBindingsAccessor().event(event);
            }
        });

        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            //console.log(valueAccessor());
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                if (event.date != null && !(event.date instanceof Date)) {
                    value(event.date.format(options.format));
                }
            }
            if (typeof changeDate != "undefined") {
                changeDate(event);
                //    allBindingsAccessor().event(event);
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("DateTimePicker");
            if (picker) {
                picker.destroy();
            }
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //var options = allBindingsAccessor().dateTimePickerOptions || {};
        //var objDate = moment(new Date(ko.utils.unwrapObservable(valueAccessor())));
        var picker = $(element).data("DateTimePicker");
        //var value = valueAccessor();

        if (picker) {
            var koDate = ko.utils.unwrapObservable(valueAccessor());
            picker.date(ko.utils.unwrapObservable(valueAccessor()));
            //value(ko.utils.unwrapObservable(valueAccessor()));
        }

        // Force calendar picker layout on Edit Row
        //if ($(element).hasClass('insidetable')) {
        //    $(element).find('.input-group-addon').on('click', function () {
        //        var $menuItem = $(this),
        //        $submenuWrapper = $menuItem.prev('.dropdown-menu');

        //        // grab the menu item's position relative to its positioned parent
        //        var menuItemPos = $menuItem.parent().position();

        //        // place the submenu in the correct position relevant to the menu item
        //        $submenuWrapper.css({
        //            auto: menuItemPos.auto + Math.round($menuItem.height() * 1.50),
        //            left: menuItemPos.left,
        //        });
        //    });
        //}
    }
};


// Edit Row Binding
ko.bindingHandlers.inlineProccess = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        var _oldValues = ko.mapping.toJS(callback[1]);
        $(element).keyup(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode),
                context = ko.contextFor(this),
                target = event.target;
            if (keyCode === 13) {
                $('html,body').animate({
                    scrollTop: $(this).offset().top - ($(window).height() - $(this).outerHeight(true)) / 2
                }, 10);
                if (callback[0].applyValidationRules != null) {
                    callback[0].applyValidationRules(callback[1]);
                    callback[0].errors = ko.validation.group(callback[1]);
                }
                if (callback[0].errors().length != 0) {
                    $.each(callback[0].errors(), function (k, v) {
                        SKTIS.Helper.Notification(v);
                    });
                    // Rollback observable values
                    for (var i in callback[1]) {
                        if (ko.isObservable(callback[1][i]) && ['ResponseType', 'Message', 'errors'].indexOf(i) < 0)
                            if (!callback[1][i].isValid())
                                callback[1][i](_oldValues[i]);
                    }
                    return false;
                }
                callback[0].saveInline(callback[0].editingRowIndex(), callback[1]);
                return false;
            }
            if (keyCode === 27) {
                if (!callback[0].f2()) {
                    // Rollback observable values
                    for (var i in callback[1]) {
                        if (ko.isObservable(callback[1][i]) && ['ResponseType', 'Message', 'errors'].indexOf(i) < 0)
                            callback[1][i](_oldValues[i]);
                    }
                    callback[0].cancelInline();
                } else {
                    $(target).removeAttr('style');
                    callback[0].f2(false);
                }
                return false;
            }
            if (keyCode === 32) {
                if (target.type === 'checkbox') {
                    target.checked = !target.checked;
                    context.$data[$(target).getKOBindName()](target.checked);
                    return false;
                }
                return false;
            }
            return false;
        });

        var tr = $(element);
        $(tr).on('blur', 'input', function (event) {
            // If clicked element inside table
            if ($(clickedElement).closest('table').length) {
                // If clicked element is TD on change ROW, not input
                if ($(clickedElement).is('td')) {

                    callback[0].cancelInline();
                    //same as escape behavior
                    if (!callback[0].f2()) {
                        // Rollback observable values
                        for (var i in callback[1]) {
                            if (ko.isObservable(callback[1][i]) && ['ResponseType', 'Message', 'errors'].indexOf(i) < 0)
                                callback[1][i](_oldValues[i]);
                        }
                    } else {
                        $(target).removeAttr('style');
                        callback[0].f2(false);
                    }
                    
                    // not valid after new behavior CMIWW
                    //if (callback[0].applyValidationRules != null) {
                    //    callback[0].applyValidationRules(callback[1]);
                    //    callback[0].errors = ko.validation.group(callback[1]);
                    //}
                    //if (callback[0].errors().length != 0) {
                    //    $.each(callback[0].errors(), function (k, v) {
                    //        SKTIS.Helper.Notification(v);
                    //    });
                    //    // Rollback observable values
                    //    for (var i in callback[1]) {
                    //        if (ko.isObservable(callback[1][i]) && ['ResponseType', 'Message', 'errors'].indexOf(i) < 0)
                    //            if (!callback[1][i].isValid())
                    //                callback[1][i](_oldValues[i]);
                    //    }
                    //    callback[0].noError(false);
                    //    return false;
                    //}
                }
            }
        });

        if (typeof $(element).children().find('input').get(0) != 'undefined')
            $(element).children().find('input').get(0).focus();
        return false;
    }
};

// Insert New Row Binding
ko.bindingHandlers.inlineAddProccess = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keyup(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode),
                context = ko.contextFor(this),
                target = event.target;
            if (keyCode === 13) {
                SKTIS.Helper.Log(callback[1]);
                if (callback[0].addNewInline2(callback[1]))
                    if (SKTIS.Checker.isFunction(callback[0].resetDefault))
                        callback[0].resetDefault(callback[1]);
                return false;
            }
            if (keyCode === 27) {
                callback[0].cancelInline();
                if (SKTIS.Checker.isFunction(callback[0].resetDefault))
                    callback[0].resetDefault(callback[1]);
                return false;
            }
            if (keyCode === 32) {
                if (target.type === 'checkbox') {
                    target.checked = !target.checked;
                    context.$root.newData[$(target).getKOBindName()](target.checked);
                    return false;
                }
                return false;
            }
            return false;
        });
    }
};

// Insert New Row Binding (holiday)
ko.bindingHandlers.inlineAddProccessHoliday = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keyup(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode),
                context = ko.contextFor(this),
                target = event.target;
            if (keyCode === 13) {
                SKTIS.Helper.Log(callback[1]);
                if (callback[0].addNewInlineHoliday(callback[1]))
                    if (SKTIS.Checker.isFunction(callback[0].resetDefault))
                        callback[0].resetDefault(callback[1]);
                return false;
            }
            if (keyCode === 27) {
                callback[0].cancelInline();
                return false;
            }
            if (keyCode === 32) {
                if (target.type === 'checkbox') {
                    target.checked = !target.checked;
                    context.$root.newData[$(target).getKOBindName()](target.checked);
                    return false;
                }
                return false;
            }
            return false;
        });
    }
};

// Force focus first input control on last tab pressed
ko.bindingHandlers.tabToNext = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        var binding = allBindings();

        $(element).keydown(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode == '9') {
                if (event.shiftKey) return true;
                var ind = callback[0].editingRowIndex();
                callback[0].editInline3(ind + 1, callback[0].listDataItems()[ind + 1]);
                return false;
            }
            return true;
        });
    }
};

// Binding for Bootstrap Select
ko.bindingHandlers.selectPicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                } else {
                    // regular select and observable so call the default value binding
                    ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
                    //ko.bindingHandlers.options.init(element, valueAccessor, allBindingsAccessor);
                }
            }

            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            var options = {};

            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                options.container = (typeof selectPickerOptions.container !== undefined && selectPickerOptions.container != '') ? selectPickerOptions.container : false;
            }

            $(element).addClass('selectpicker').selectpicker(options);
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var valueUpdate = ko.unwrap(valueAccessor());
        if ($(element).is('select')) {
            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                var options = selectPickerOptions.optionsArray,
                    optionsText = selectPickerOptions.optionsText,
                    optionsValue = selectPickerOptions.optionsValue,
                    optionsCaption = selectPickerOptions.optionsCaption,
                    isDisabled = (allBindingsAccessor().isDisabled == 'undefined') ? selectPickerOptions.disabledCondition || false : allBindingsAccessor().isDisabled,
                    resetOnDisabled = selectPickerOptions.resetOnDisabled || false,
                    selectedText = selectPickerOptions.selectedText || false;
                if (typeof options !== 'undefined' && ko.utils.unwrapObservable(options).length > 0) {
                    // call the default Knockout options binding
                    ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                }
                if (isDisabled && resetOnDisabled) {
                    // the dropdown is disabled and we need to reset it to its first option
                    $(element).selectpicker('val', $(element).children('option:first').val());
                }
                if (selectedText) {
                    if (ko.isObservable(selectedText))
                        selectedText($(element).find(':selected').text());
                }
                $(element).prop('disabled', isDisabled);
            }
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                } else {
                    // call the default Knockout value binding
                    ko.bindingHandlers.value.update(element, valueAccessor);
                }
            }
            $(element).selectpicker('refresh');
        }
    }
};
// Selecpicker Cosutom
ko.bindingHandlers.selectPickerCustom = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                } else {
                    // regular select and observable so call the default value binding
                    ko.bindingHandlers.options.init(element, valueAccessor, allBindingsAccessor);
                }
            }
            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            var options = {};

            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                options.container = (typeof selectPickerOptions.container !== undefined && selectPickerOptions.container != '') ? selectPickerOptions.container : false;
            }

            $(element).addClass('selectpicker').selectpicker(options);
            //$(element).addClass('selectpicker').selectpicker();
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                var options = selectPickerOptions.optionsArray,
                    optionsText = selectPickerOptions.optionsText,
                    optionsValue = selectPickerOptions.optionsValue,
                    optionsCaption = selectPickerOptions.optionsCaption,
                    isDisabled = selectPickerOptions.disabledCondition || false,
                    resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                if (ko.utils.unwrapObservable(options).length > 0) {
                    // call the default Knockout options binding
                    ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                }
                if (isDisabled && resetOnDisabled) {
                    // the dropdown is disabled and we need to reset it to its first option
                    $(element).selectpicker('val', $(element).children('option:first').val());
                }
                $(element).prop('disabled', isDisabled);
            }
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                } else {
                    // call the default Knockout value binding
                    ko.bindingHandlers.value.update(element, valueAccessor);
                }
            }

            $(element).selectpicker('refresh');
        }
    }
};

// Binding for Enable Filupload when Primary key already inputed
ko.bindingHandlers.enableWhenPrimaryFieldFilled = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
        var check = true;
        $.each(value, function (k, v) {
            check = check && (v() != '');
        });
        if (check) {
            $(element).removeAttr('disabled');
        } else {
            $(element).attr('disabled','disabled');
        }
    }
};

// Binding for Enable Filupload when Primary key already inputed
ko.bindingHandlers.hideWhenPrimaryFieldFilled = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
        var check = true;
        $.each(value, function (k, v) {
            check = check && (v() != '');
        });
        if (check) {
            $(element).removeAttr('style');
        } else {
            $(element).attr('style', 'display:none');
        }
    }
};

//Binding get selected text
ko.bindingHandlers.selectedText = {
    init: function (element, valueAccessor) {
        if ($(element).is('select')) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value($(element).find(':selected').text());

                $(element).change(function() {
                    value($(element).find(':selected').text());
                });
            }
        }
    },
    update: function (element, valueAccessor) {
        if ($(element).is('select')) {
            if (ko.isObservable(value)) {
                var value = valueAccessor();
                value($(element).find(':selected').text());
            }
        }
    }
};

ko.bindingHandlers.isSelectPicker = {
    init: function (element) {
        if ($(element).is('select')) {
            $(element).addClass('selectpicker').selectpicker();
        }
    }
};

ko.myToJSON = function (obj) {
    return JSON.stringify(ko.toJS(obj), function (key, val) {
        return key === '__ko_mapping__' ? undefined : val;
    });
}

//ko.bindingHandlers.dropdown = {
//    init: function (element, valueAccessor, allBindingsAccessor) {
//        if ($(element).is('select')) {
//            if (ko.isObservable(valueAccessor())) {
//                ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
//            }
//            $(element).selectpicker();
//        }
//    },
//    update: function (element, valueAccessor, allBindingsAccessor) {
//        if ($(element).is('select')) {
//            var selectPickerOptions = allBindingsAccessor().options;
//            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
//                //var options = selectPickerOptions.options;
//                var options = allBindingsAccessor().options,
//                        optionsText = allBindingsAccessor().optionsText,
//                        optionsValue = allBindingsAccessor().optionsValue,
//                        optionsCaption = allBindingsAccessor().optionsCaption;
//                if (ko.utils.unwrapObservable(options).length > 0) {
//                    ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
//                }
//            }
//            if (ko.isObservable(valueAccessor())) {
//                ko.bindingHandlers.value.update(element, valueAccessor);
//            }
//            $(element).selectpicker('refresh');
//        }
//    }
//};

// DatePicker Binding
ko.bindingHandlers.timePicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();

        var options = allBindingsAccessor().timePickerOptions || {};
        $(element).timepicker(options);
        $(element).timepicker('setTime', ko.utils.unwrapObservable(value));

        ko.utils.registerEventHandler(element, "changeTime.timepicker", function (event) {
            if (ko.isObservable(value)) {
                value(event.time.value);
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).timepicker('hideWidget');
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var valueUpdate = ko.unwrap(valueAccessor());
        var koTime = ko.utils.unwrapObservable(valueAccessor());
        // buggy when user edit with keyboard
        //var picker = $(element).timepicker('setTime', koTime);
    }
};

// Insert New Row Binding Worker Asignment
ko.bindingHandlers.inlineAddProccessWorkerAssignment = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keyup(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode),
                context = ko.contextFor(this),
                target = event.target;
            if (keyCode === 13) {
                SKTIS.Helper.Log(callback[1]);
                if (callback[0].addNewInlineWorkerAssignment(callback[1]))
                    if (SKTIS.Checker.isFunction(callback[0].resetDefault))
                        callback[0].resetDefault(callback[1]);
                return false;
            }
            if (keyCode === 27) {
                callback[0].cancelInline();
                if (SKTIS.Checker.isFunction(callback[0].resetDefault))
                    callback[0].resetDefault(callback[1]);
                return false;
            }
            if (keyCode === 32) {
                if (target.type === 'checkbox') {
                    target.checked = !target.checked;
                    context.$root.newData[$(target).getKOBindName()](target.checked);
                    return false;
                }
                return false;
            }
            return false;
        });
    }
};

function round(value, decimals) {
    return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
}

var toMoney = function (num, precision) {
    num = (num !== null && !isNaN(num) && num !== '') ? num : 0;

    precision = typeof (precision) === 'undefined' ? 2 : precision;
    numeral.language('en-gb');
    //return parseFloat(num).toFixed(precision).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return numeral(parseFloat(num).toFixed(precision)).format('0,0.[0000]');
};

var toMoneyJkn = function (num) {
    num = (num !== null && !isNaN(num) && num !== '') ? num : 0;
    var num2Precision = round(num, 2);
    return num2Precision;
};

var toDecimal = function (num, precision) {
    if (isNaN(num) || num === '' || num === null) return null;

    precision = typeof (precision) === 'undefined' ? 5 : precision;
    //console.log("DECIMAAAAAL TAPLAK");
    numeral.language('en-gb');
    //console.log(numeral(1000).format('0,0'));
    //console.log(numeral(parseFloat(num).toFixed(precision)).format('0,0'));

    //return parseFloat(num).toFixed(precision).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    if (precision == 3)
        return numeral(parseFloat(num).toFixed(precision)).format('0,0.[000]');
    else if (precision == 2)
        return numeral(parseFloat(num).toFixed(precision)).format('0,0.[00]');
    else if (precision == 0)
        return numeral(Math.floor(parseFloat(num))).format('0,0.[00]');
    else
        return numeral(parseFloat(num).toFixed(precision)).format('0,0.[00]');
};

var toDigit = function (num) {
    if (isNaN(num) || num === '' || num === null) return null;

    numeral.language('en-gb');
    return numeral(parseFloat(num)).format('0,0');
};

var handler = function (element, valueAccessor, allBindings) {
    var $el = $(element);
    var method;

    // Gives us the real value if it is a computed observable or not
    var valueUnwrapped = ko.unwrap(valueAccessor());

    if ($el.is(':input')) {
        method = 'val';
    } else {
        method = 'text';
    }
    return $el[method](toMoney(valueUnwrapped));
};

var handlerMoneyJkn = function (element, valueAccessor, allBindings) {
    var $el = $(element);
    var method;

    // Gives us the real value if it is a computed observable or not
    var valueUnwrapped = ko.unwrap(valueAccessor());

    if ($el.is(':input')) {
        method = 'val';
    } else {
        method = 'text';
    }
    return $el[method](toMoneyJkn(valueUnwrapped));
};

ko.bindingHandlers.money = {
    update: handler
};

ko.bindingHandlers.moneyJkn = {
    update: handlerMoneyJkn
};

ko.bindingHandlers.roundedMoney = {
    update: function(element, valueAccessor, allBindings) {
        var $el = $(element);
        var method;

        // Gives us the real value if it is a computed observable or not
        var valueUnwrapped = ko.unwrap(valueAccessor());

        if ($el.is(':input')) {
            method = 'val';
        } else {
            method = 'text';
        }
        return $el[method](toMoney(valueUnwrapped, 2).replace(/(\.(.*))/g, ''));
    }
};

ko.bindingHandlers.decimal = {
    update: function (element, valueAccessor, allBindings) {
        var $el = $(element);
        var method;
        var prec;

        if (typeof allBindings().precision == 'undefined') {
            prec = 2;
        }
        else
            prec = allBindings().precision;

        // Gives us the real value if it is a computed observable or not
        var valueUnwrapped = ko.unwrap(valueAccessor());

        if ($el.is(':input')) {
            method = 'val';
        } else {
            method = 'text';
        }
        return $el[method](toDecimal(valueUnwrapped, prec));
    }
}

ko.bindingHandlers.digit = {
    update: function (element, valueAccessor, allBindings) {
        var $el = $(element);
        var method;

        // Gives us the real value if it is a computed observable or not
        var valueUnwrapped = ko.unwrap(valueAccessor());

        if ($el.is(':input')) {
            method = 'val';
        } else {
            method = 'text';
        }
        return $el[method](toDigit(valueUnwrapped));
    }
}

ko.bindingHandlers.isDisabled = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var val = ko.unwrap(valueAccessor()),
            $el = $(element);
        
        if (typeof allBindingsAccessor().dateTimePicker != 'undefined') {
            $el.find('input').attr('disabled', val);
        } else {
            $el.attr('disabled', val);
        }

        if ($el.is('select')) {
            $el.selectpicker('refresh');
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var val = ko.unwrap(valueAccessor()),
            $el = $(element);

        if (typeof allBindingsAccessor().dateTimePicker != 'undefined') {
            $el.find('input').attr('disabled', val);
        } else {
            $el.attr('disabled', val);
        }

        if ($el.is('select')) {
            $el.selectpicker('refresh');
        }
    }
}