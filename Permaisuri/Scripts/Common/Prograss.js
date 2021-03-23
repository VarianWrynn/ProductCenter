define(function (require, exports, module) {
    var $ = require('jquery');
    var ajaxbg = $("#background,#progressBar");
    exports.show = function () {
        ajaxbg.show();
        $("#background").css({
            "height": function () { return $(document).height(); },
        });
        //$('#DivLocker').css({
        //    //"display": "block", 2013年10月4日9:53:47
        //    "display": "none",
        //    "position": "absolute",
        //    "margin-left": "1px",
        //    "margin-top": "1px",
        //    "background-color": "#000000",
        //    "height": function () { return $(document).height(); },
        //    "filter": "alpha(opacity=30)",
        //    "opacity": "0.3",
        //    "overflow": "hidden",
        //    "width": function () { return $(document).width()-10; },
        //    "z-index": "999"
        //});
    }
    exports.hide = function () {
        ajaxbg.hide();
        $('#DivLocker').css({ "display": "none" });
    }
});