define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.editUser = function (User_Guid,UserAccount,DisplayName, Email,Status,password) {
        $("#userInfoTable").show();
        $("#userAccount").val(UserAccount);
        $("#userAccount").attr("readonly", "true");
        $("#displayName").val(DisplayName);
        $("#emaile").val(Email);
        $("#userStatus").val(Status);
        $("#rpwd").val(password);
        $("#pwd").val(password);

        $("#userInfoDigForm").show();
        $('#userInfoDigForm').dialog({
            title: 'Edit User Information',
            width: 600,
            height: 400,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {                    

                    $.extend($.fn.validatebox.defaults.rules, {
                        equals: {
                            validator: function (value, param) {
                                return value == $(param[0]).val();
                            },
                            message: 'Password field do not match.'
                        }
                    });

                    var fvalidate = $("#userInfoDigForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }

                    $.ajax({
                        url: "./EditUser",
                        type: "post",
                        dataType: "json",
                        data: {
                            User_Guid:User_Guid,
                            Display_Name: $("#displayName").val(),
                            User_Pwd: $("#pwd").val(),
                            Primary_Email: $("#emaile").val(),
                            Status: $("#userStatus").val()
                        },
                        success: function (retData) {
                            if (retData.Status == 1) {
                                $('#userManageDG').datagrid('reload');
                                $("#userInfoDigForm").dialog("close");
                            }
                            else { $.messager.alert('UserManagement-EditUser', retData.Data, 'error'); }
                        },
                        error: function () {
                            $.messager.alert('UserManagement-EditUser', 'NetWork error when edit new user', 'error');
                        }
                    });
                }
            }, {
                text: 'close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#userInfoDigForm").dialog("close");
                }
            }]
        });
    }
})