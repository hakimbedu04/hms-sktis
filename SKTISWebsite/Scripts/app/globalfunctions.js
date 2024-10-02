function parseToFloat(valueToParse) {
    valueToParse = (typeof valueToParse !== 'undefined' && valueToParse != null) ? valueToParse.toString().replace(',', '.') : 0;
    var value = parseFloat(valueToParse);
    if (!isNaN(value)) {
        return value;
    }
    return 0;
}

function parseToInt(valueToParse) {
    var value = parseInt(valueToParse);
    if (!isNaN(value)) {
        return value;
    }
    return 0;
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires + " ; path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}
function locationDesc(options, item) {
    if (typeof (item) === 'undefined') return;

    $(options).attr('title', item.LocationCode);
    $(options).attr(
        'data-content',
        "<span class='text'><span style='width: 35px; display: inline-block;'>" + item.LocationCode + "</span> - " + item.LocationName + "</span>"
    );
}

function EmployeeDropDownAfterRender2(options, item) {
    if (typeof (item) === 'undefined') return;
    $(options).attr('title', item.EmployeeID);
    //console.log(item);
    var employeeNumber = item.EmployeeNumber;
    var lastTwoNumber = employeeNumber.slice(-2);
    $(options).attr(
        'data-content',
        "<span class='text'>"+ item.EmployeeID + " - " + lastTwoNumber + " - " + item.EmployeeName + "</span>"
    );
}

function EmployeeDropDownAfterRender(options, item) {
    if (typeof (item) === 'undefined') return;
    $(options).attr('title', item.EmployeeID);
    $(options).attr(
        'data-content',
        "<span class='text'>" + item.EmployeeID + " - " + item.EmployeeName + "</span>"
    );
}

function getMaxKeyOfArray(arr) {
    var max_index = -1;
    var max_value = Number.MIN_VALUE;
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] > max_value) {
            max_value = arr[i];
            max_index = i;
        }
    }
    return max_value;
}

function parseToFloatWithPrecision(value, precision) {

    precision = typeof precision == 'undefined' ? 0 : precision;

    return parseFloat(parseToFloat(value).toFixed(precision));
}