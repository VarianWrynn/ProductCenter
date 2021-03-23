define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.deleteUser = function (User_Guid) {
        $.ajax({
            url: "./DeleteUser",
            type: "POST",
            dataType: "json",
            data: {
                User_Guid: User_Guid,
            },
            success: function (retData) {
                if (retData.Status === 1) {
                    $('#userManageDG').datagrid('reload');
                    $("#userInfoDigForm").dialog("close");
                } else {
                    $.messager.alert('UserManagement-DeleteUser', retData.Data, 'error');
                }
            },
            error: function () {
                $.messager.alert('UserManagement-DeleteUser', 'NetWork error when add delete a user', 'error');
            }
        });
    }
});