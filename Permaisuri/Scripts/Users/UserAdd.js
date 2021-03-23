define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
  
    exports.addUser = function () {
        $("#userAccount").val('');
        $('#userAccount').removeAttr("readonly");  
        $("#displayName").val('');
        $("#emaile").val('');
        $("#userStatus").val('1');
        $("#rpwd").val('');
        $("#pwd").val('');
        $("#userInfoTable").show();
        $("#userInfoDigForm").show();
        $('#userInfoDigForm').dialog({
            title: 'Add New User',
            width: 600,
            height: 400,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {

                    // extend the 'confirm password' rule  
                    $.extend($.fn.validatebox.defaults.rules, {
                        equals: {
                            validator: function (value, param) {
                                return value == $(param[0]).val();
                            },
                            message: 'Password field do not match.'
                        }
                    });

                    //for remote user check extend.
                    /*$.extend($.fn.validatebox.defaults.rules, {
                        IsExistUser: {
                            validator: function (value, param) {
                                return false;
                            },
                            message: 'aaa!'
                        }
                    });*/

                    var fvalidate = $("#userInfoDigForm").form('validate');
                    if (!fvalidate)
                    {
                        return;
                    }

                    $.ajax({
                        url: "./AddUser",
                        type: "POST",
                        dataType: "json",
                        data: {
                            User_Account: $("#userAccount").val(),
                            Display_Name: $("#displayName").val(),
                            User_Pwd: $("#pwd").val(),
                            Primary_Email: $("#emaile").val(),
                            Status:  $("#userStatus").val()
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                $('#userManageDG').datagrid('reload');
                                $("#userInfoDigForm").dialog("close");
                            } else {
                               // $("#retMsg").show();
                                //$("#retMsg").html(retData.Data);
                                $.messager.alert('UserManagement-AddUser', retData.Data, 'error');
                            }
                        },
                        error: function () {
                           // $("#retMsg").show();
                            //$("#retMsg").html("NetWork error when add new user");
                            $.messager.alert('UserManagement-AddUser', 'NetWork error when add new user', 'error');
                        }
                    });
                }
            }, {
                text: 'Close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#userInfoDigForm").dialog("close");
                }
            }]
        });
    }
});