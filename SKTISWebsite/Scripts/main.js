var SKTIS = SKTIS || {};

/**
 * Configuration Section
 * Encapsulate all SKTIS javascript configuration values
 */
SKTIS.Config = (function () {
    var debug = 0;
    return {
        Get: function (config) {
            if (!(/[^\w\s]/gi).test(config))
                return eval(config);
        },
        Set: function (config, value) {
            eval(config + '=' + value);
        }
    }
})();

/**
 * Constant Section
 */
SKTIS.Constant = (function () {
    return {
        UnitCodeDefault: {
            PROD: 'PROD',
            MTNC: 'MTNC'
        },
        ProcessGroup: {
            ROLLING: 'rolling',
            CUTTING: 'cutting',
            PACKING: 'packing',
            STICKWRAPPING: 'stickwrapping'
        }
    }
})();

/**
 * Method Section
 */
SKTIS.Helper = (function () {
    /* Private Variable Here */
    var test = "test value";

    return {
        /*
         * SKTIS Notification
         * @message string
         * @type string (info,success,warning,error)
         */
        Notification: function (message, type) {
            type = type || 'warning';
            if (message.indexOf("Run submit data on background process.") > -1)
                type = 'success';
            noty({
                text: message,
                layout: 'top',
                timeout: 5000,
                maxVisible: 10,
                animation: {
                    open: 'animated flipInX',
                    close: 'animated zoomOut',
                    easing: 'swing',
                    speed: 500
                },
                theme: 'relax',
                type: type,
                template: '<div class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>',
            });
        },
        Log: function (message) {
            if (SKTIS.Config.Get('debug') === 1) {
                if (typeof console !== "undefined") {
                    console.log(message);
                }
            }
        },
        ResponseNotification: function (value, label, field) {
            var messagetype = value.ResponseType == 'Error' ? 'error' : 'success';
            SKTIS.Helper.Notification("<strong>" + label + "</strong> " + field + " : " +
                                      (value.ResponseType == 'Error' ? value.Message : 'Success'), messagetype);
        },
        OtherHelper: function () {
            alert('Not Implemented Yet!');
        }
    }
})();

SKTIS.Checker = (function () {
    function editedDataAvailable(data) {
        var editedData = 0;
        $.each(data, function (k, v) {
            editedData += v().length;
        });
        return (editedData > 0) ? true : false;
    }
    return {
        isFunction: function (func) {
            return typeof func == 'function';
        },
        modifiedDataExists: function (data) {
            window.onbeforeunload = function (e) {
                if (editedDataAvailable(data)) {
                    // Issue: Original browser message did not overided successfuly but the functionality working properly, please try another solutions!
                    var message = "Changes have not been saved, continue to move the page?",
                    e = e || window.event;
                    // For IE and Firefox
                    if (e) {
                        e.returnValue = message;
                    }
                    // For Safari
                    return message;
                }
            };
            if (editedDataAvailable(data))
                return true
            else
                return false
        },
        modifiedDataExistsForAjax: function (data) {
            if (editedDataAvailable(data)) {
                var result = confirm('Changes have not been saved, continue to move the page?');
                return !result;
            } else {
                return false;
            }
        },
        checkmodifiedDataExists: function (data) {
            return editedDataAvailable(data);
        }
    }
})();

/**
 * SKTIS jQuery Plugins
 * Put all custom jQuery plugin here!
 */
(function ($) {

    $.fn.getKOBindName = function () {
        var bindName = this.attr('data-bind-name');
        if (typeof bindName === typeof undefined) {
            var dataBind = this.attr('data-bind');
            var checkedBind = dataBind.match(/checked\s*:\s*(?:{.*,?\s*data\s*:\s*)?([^{},\s]+)/);
            var dataBindName = checkedBind[1].match(/\$(data|root\.newData)\.([a-zA-Z0-9_.]+)/);
            bindName = dataBindName[2];
        }
        return bindName;
    };

}(jQuery));