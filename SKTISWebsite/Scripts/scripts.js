$(document).ajaxError(function (event, request, settings) {
    if (request.status == 401) {
        location.reload();
    }
});

// a hack to prevent multiple backdrop stack and shown in front of the modal
// if you found a better way to do this, don't hesitate to change below code.
// otherwise, let this code alone!
$(document).on('shown.bs.modal', function () {
    if ($('.modal-backdrop').length > 1) {
        $('.modal-backdrop').remove();
    }
});

jQuery(function () {
    $( '#toggle-hide i.fa' ).click(function(){
        $('.navbar-brand').toggleClass('mini');
        $('#sidebar').toggleClass('hideSide');
        $('#main').toggleClass('miniLeft');
        $('#fixed-menu .dropdown-menu').hide();
    });
    $("#fixed-menu li a.trigger").on("click",function(e){
        if($('#sidebar').hasClass('hideSide')){

        }
        else{
            var current=$(this).next();
            var grandparent=$(this).parent().parent();
            grandparent.find(".sub-menu:visible").not(current).slideUp(200);
            current.slideToggle(200);
            e.stopPropagation();
        }
    });
    $("#fixed-menu li a:not(.trigger)").on("click",function(){
        if($('#sidebar').hasClass('hideSide')){

        }
        else{
            var root=$(this).closest('.dropdown');
            root.find('.sub-menu:visible').slideUp(200);
        }
    });
    $('.input-group.date.time').datetimepicker({
        widgetPositioning : {
            vertical: 'bottom'
        },
        format: 'DD/MM/YYYY HH:mm'
    });
    $('.input-group.date.notime').datetimepicker({
        widgetPositioning : {
            vertical: 'bottom'
        },
        format: 'DD/MM/YYYY'
    });

    var mainHeight = $('#main').outerHeight() - 33;
    $('.iframe-tableu').css('height', mainHeight);
});

$(document).ready(function(){
    // whenever we hover over a menu item that has a submenu
    //$(document).on("rendered.bs.select", '.selectpicker', function () {
    //    $('table .bootstrap-select button').on('click', function() {
    //        var $menuItem = $(this),
    //        $submenuWrapper = $menuItem.next('.dropdown-menu');
        
    //        // grab the menu item's position relative to its positioned parent
    //        var menuItemPos = $menuItem.position();
            
    //        var isAuto = $menuItem.parent().hasClass("sp-auto-width");
            
    //        // place the submenu in the correct position relevant to the menu item
    //        $submenuWrapper.css({
    //            auto: menuItemPos.auto + Math.round($menuItem.height() * 1.25),
    //            left: menuItemPos.left,
    //            width: isAuto ? "auto" : "200px"
    //        });
    //    });  
    //});
    
    //$(document).on("click", '.date.insidetable .input-group-addon', function () {
    //    var $menuItem = $(this),
    //    $submenuWrapper = $menuItem.prev('.dropdown-menu');
    
    //    // grab the menu item's position relative to its positioned parent
    //    var menuItemPos = $menuItem.parent().position();
        
    //    // fix out of bond date picker
    //    var oob = $menuItem.parent().hasClass("out-of-bond");
        
    //    // place the submenu in the correct position relevant to the menu item
    //    $submenuWrapper.css({
    //        auto: menuItemPos.auto + Math.round($menuItem.height() * 1.50),
    //        // Prevent dtp to out of bond when positioned on the most right of the table
    //        left: oob ? menuItemPos.left - Math.round($menuItem.parent().width()) - (Math.round($menuItem.parent().width() / 2)) : menuItemPos.left,
    //    });
    //});
    
    $(document).on('keypress', 'input[type=number], input[type=text]', function (event) {
        var input = $(this);
        var validationType = input.data('sktis-validation');
        if ((validationType === undefined || validationType === null) || (validationType.trim() == ""))
            return true;
        // Backspace, tab, enter, end, home, left, right
        // We don't support the del key in Opera because del == . == 46.
        var controlKeys = [8, 9, 13, 35, 36, 37, 39];
        // IE doesn't support indexOf
        var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
        // Some browsers just don't raise events for control keys. Easy.
        // e.g. Safari backspace.
        switch (validationType) {
            case "digit":
                if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                    (48 <= event.which && event.which <= 57) || // Always 1 through 9
                    isControlKey) { // Opera assigns values for control keys.
                    return;
                }
                break;
            case "number":
                if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                    (49 <= event.which && event.which <= 57) || // Always 1 through 9
                    (48 == event.which && $(this).val()) || // No 0 first digit
                    isControlKey) { // Opera assigns values for control keys.
                    return;
                }
                break;
            case "decimal":
                if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                    (48 <= event.which && event.which <= 57) || // Always 1 through 9
                    (44 == event.which) || // dot
                    (46 == event.which) || // comma
                    isControlKey) { // Opera assigns values for control keys.
                    return;
                }
                break;
            case "decimal-dot":
                if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                    (48 <= event.which && event.which <= 57) || // Always 1 through 9
                    (46 == event.which) || // dot
                    isControlKey) { // Opera assigns values for control keys.
                    return;
                }
                break;
            case "integer":
                if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                    (48 <= event.which && event.which <= 57) || // Always 1 through 9
                    (45 == event.which) || // minus
                    isControlKey) { // Opera assigns values for control keys.
                    return;
                }
                break;
        }
        event.preventDefault();
    });

    $("tr > td > input").focus(function(e){
        $(this).parent().parent().addClass('highlight');
    }).blur(function(e){
        $(this).parent().parent().removeClass('highlight');
    });


    /* carousel tab */
    $('#carousel-tab').each(function(){
        $(this).carousel({
            interval: false
        });
    });
    
    var navigation = true;
    var tabWidth = 0;
    $('#carousel-tab li.item').each(function(){
         tabWidth += $(this).outerWidth();
    }); 
    if($('#carousel-tab').outerWidth() > tabWidth)
        navigation = false;

    $("#carousel-tab .left.carousel-control").hide();
    
    if(!navigation)
        $("#carousel-tab .right.carousel-control").hide();

    $('#carousel-tab a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        e.target // newly activated tab
        e.relatedTarget // previous active tab
        if(navigation){
            $(".left.carousel-control").show();
            $(".right.carousel-control").show();

            if($('#carousel-tab li.item.active').is(':first-child')){
                $(".left.carousel-control").hide();
                $('#carousel-tab .tab-long').css('left', '0');
            }
            else if($('#carousel-tab li.item.active').is(':last-child')){
                $(".right.carousel-control").hide();

            }

            var decrease = 0;
            $('#carousel-tab li.item').each(function(){
                if($(this).hasClass('active')){
                    $('#carousel-tab .tab-long').css('left', -decrease)
                }else{
                 decrease += $(this).outerWidth();
                }
            }); 
        }
    });

    $('#carousel-tab').on('slid.bs.carousel', function () {  
        var data = $(this).find('.item.active a').attr('href');
        $(this).parent().find('.tab-pane.over').each(function(){
            $(this).removeClass('in');
            $(this).removeClass('active');
        });
        $(data).addClass('active in');

        if(navigation){
            $(".left.carousel-control").show();
            $(".right.carousel-control").show();
        }

        if($(this).find('li.item.active').is(':first-child')){
            $(".left.carousel-control").hide();
            $('#carousel-tab .tab-long').css('left', '0');
        }
        else if($(this).find('li.item.active').is(':last-child')){
            $(".right.carousel-control").hide();
        }

    });

    $('#carousel-tab .carousel-control.left').click(function() {
        var target = $(this).parent().parent().find('li.item.active a');
        var datali = target.outerWidth();
        var left = $(this).parent().parent().find('.tab-long').css('left');
        $(this).parent().parent().find('.tab-long').css('left', parseInt(left) + datali);
    });

    $('#carousel-tab .carousel-control.right').click(function() {
        var datali = $(this).parent().parent().find('li.item.active a').outerWidth();
        var left = $(this).parent().parent().find('.tab-long').css('left');
        $(this).parent().parent().find('.tab-long').css('left', parseInt(left) - datali);
    });
    
    // Ajax Loader
    //$body = $("body");
    //$(document).on({
    //    ajaxStart: function () { $body.addClass("loading"); },
    //    ajaxStop: function () { $body.removeClass("loading"); }
    //});
});

var clickedElement;
$(document).mousedown(function (e) {
    clickedElement = $(e.target);
    });
 
$(document).mouseup(function (e) {
    	 
       clickedElement = null;
    
    });