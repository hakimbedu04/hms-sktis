// Knockout Date Validator Hotfix
ko.validation.rules['cannotGreaterThan'] = {
    validator: function (val, otherVal) {
        var val = moment(val, "DD/MM/YYYY");
        var otherVal = moment(otherVal, "DD/MM/YYYY");        
        if (otherVal.isBefore(val))
            return true;
        else if (otherVal.isSame(val))
            return true;
        else
            return false;
    },
    message: 'The date must be greater than <strong>{0}</strong>'
};

ko.validation.rules['cannotLessThan'] = {
    validator: function (val, otherVal) {
        var val = moment(val, "DD/MM/YYYY");
        var otherVal = moment(otherVal, "DD/MM/YYYY");
        if (otherVal.isBefore(val))
            return true;
        else if (otherVal.isSame(val))
            return true;
        else
            return false;
    },
    message: 'The date must be greater than <strong>{0}</strong>'
};

ko.validation.rules['isAfter'] = {
    validator: function (val, otherVal) {
        var val = moment(val, "DD/MM/YYYY");
        var otherVal = moment(otherVal, "DD/MM/YYYY");
        if (otherVal.isAfter(val))
            return true;
        else if (otherVal.isSame(val))
            return true;
        else
            return false;
    },
    message: 'The date must be less than <strong>{0}</strong>'
};

ko.validation.rules['maxOneWeekFrom'] = {
    validator: function (val, otherVal) {
        val = moment(val, "DD/MM/YYYY");
        var otherVal2 = moment(otherVal, "DD/MM/YYYY").add(6, 'days');
        if (otherVal2.isAfter(val))
            return true;
        else if (otherVal2.isSame(val))
            return true;
        else
            return false;
    },
    message: 'Expired date cannot be greater than one week from Effective date!'
};

ko.validation.rules['isCurrentWeek'] = {
    validator: function (val, otherVal) {
        val = moment(val, "DD/MM/YYYY");
        var startDate = moment().startOf('isoweek');
        var endDate = moment().startOf('isoweek').add(6, 'days');
        if (startDate <= val && val <= endDate) {
            var r = confirm("You'll change current week data. Current week TPO Fee will re-calculated. Proceed?");
            if (r == true) {
                return true;
            } else {
                return false;
            }
        } else {
            return true;
        }
    },
    message: 'Please confirm!'
};

ko.validation.registerExtenders();