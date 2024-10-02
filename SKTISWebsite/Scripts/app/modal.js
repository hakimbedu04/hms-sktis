$(function () {
    $.ajaxSetup({ cache: false });
    initSKTModal();
});

function initSKTModal() {
    $("a[data-modal]").on("click", function (e) {
        var modalTrigger = this;
        $('#sktModalContent').load(this.href, function () {
            $('#sktModal').modal({
                keyboard: true
            }, 'show');

            bindForm(this, $(modalTrigger).data("oncomplete"));
        });
        return false;
    });

    $("tr[data-modal]").on("click", function (e) {
        var modalTrigger = this;
        $('#sktModalContent').load($(this).data("url"), function () {
            $('#sktModal').modal({
                keyboard: true
            }, 'show');

            bindForm(this, $(modalTrigger).data("oncomplete"));
        });
        return false;
    });
}

function bindForm(dialog, callback) {
    $('form', dialog).submit(function () {
        console.log(callback);
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#sktModal').modal('hide');
                    $('#progress').hide();
                    if (callback) eval(callback)();
                    //location.reload();
                } else {
                    $('#progress').hide();
                    $('#sktModalContent').html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}