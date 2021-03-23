/// <reference path="../jquery-1.7.1-vsdoc.js" />
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');
    var TagsModel = require('Common/OpenCMSTag');

    $(document).ready(function () {

        TagsModel.CMSTags.BackSpace();

        /*在网络环境恶劣的情况下，会导致用户在很多脚本都未加载完毕就输入用户名、密码点击提交，然后没有任何反应，
        解决办法就是在必要的脚本全部加载完之前禁用这些文本和按钮
        Lee  2013年10月16日9:39:17
        */
        $("#UserID").css({
            "display":"block"
        });
        $("#UserPWD").css({
            "display": "block"
        });
        $("#submit").attr({
            "disabled": false
        });

        $("#submit").bind("click", function () {

            var fvalidate = $("#CMSLoginForm").form('validate');
            if (!fvalidate)
            {
                return;
            }
            $("#submit").prop("disabled", "disabled");
            var userAcc = $.trim($("#UserID").val());
            var UserPwd = $.trim($("#UserPWD").val());

            //Get papreamters those will be post to server. 获取传递前台的参数 
            var postData = {
                userAccount: userAcc,
                userPwd: UserPwd
            };
            prograss.show();
            $.ajax({
                type: "POST",
                url: './CheckUserLogin',
               // url: './CheckUserLogin',
                data: postData,
                dataType: 'json',
                success: function (ret) {
                    prograss.hide();
                    $("#submit").prop("disabled", "");
                    //if (ret.Status === 1) { illegal character...... Cause there is a SPACE here!!!
                    if (ret.Status === 1) {
                         window.location.href = "../Main/Index";
                     }
                    else {
                        //$("#UserID").val("");
                        $("#UserPWD").val("")
                        $.messager.alert("UserLogin", ret.Data, "warning");
                        //setTimeout(function () {
                        //    window.location.href = "./Index";
                        //}, 3000);
                        // window.location.href = "/Login/Index/";--加了这个反斜杠，会对全局filter造成大麻烦...2013年8月5日9:47:12
                     }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    prograss.hide();
                    $("#submit").prop("disabled", "");
                    //$("#UserID").val("");
                    $("#UserPWD").val("")
                    $.messager.alert("UserLogin", "Cannot login CMS system,please cntact administrator ", "error");
                    //setTimeout(function () {
                    //    window.location.href = "./Index";
                    //}, 3000);
                }
            }); /*end of ajax*/

            return false;// prevent html5 auto submit

        });// end of bind submit funtion 

    }).keyup(function (event) {

        //获取当前按键的键值   
        //jQuery的event对象上有一个which的属性可以获得键盘按键的键值   
        var keycode = event.which;
        switch (keycode) {

            case 13:// Enter
                var isDisabled = $("#submit").prop("disabled");
                //先判断下当前按钮是否被禁用了，如果被禁用了，说明之前已经点过一次button了，不要再点，否则前面那一次的
                //提交会报错... 或者把前面的提交逻辑搬迁这里也可以2014年4月17日10:50:09
                if (!isDisabled) {
                    $("#submit").click();
                }
                return false;
                break;

            default:
                break;
        }
    });
});