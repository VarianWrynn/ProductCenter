
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    /// <summary>
    /// Click Logout button
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.Logout = function () {
        $.messager.confirm('Confirm', 'Are you sure to LOGOUT CMS system?', function (r) {
            if (r) {
                $(".personInfo").addClass('auto-loading');
                //$("#aLogout").hide();
                $.ajax({
                    url: "./UserLogout",
                    type: "POST",
                    dataType: "json"
                }).done(function (retData) {
                    $(".personInfo").removeClass('auto-loading');
                   // $("#aLogout").show();
                    if (retData.Status === 1) {
                        window.location.href = "../Login/Index";
                    } else {
                        $.messager.alert('UserLogout', retData.Data, 'error');
                    }
                }).fail(function () {
                    $(".personInfo").removeClass('auto-loading');
                   // $("#aLogout").show();
                    $.messager.alert('UserLogout', 'NetWork Error,Please contact administrator。', 'error');
                });
            }
        }); // end of messaer.confirm()
    }
});