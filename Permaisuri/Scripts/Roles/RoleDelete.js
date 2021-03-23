define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.deleteRole = function (role_Guid) {
        $.ajax({
            url: "./DeleteRole",
            type: "POST",
            dataType: "json",
            data: {
                Role_Guid: role_Guid,
            },
            success: function (retData) {
                if (retData.Status === 1) {
                    $('#roleManageDG').datagrid('reload');
                    $("#dlgRoleManage").dialog("close");
                } else {
                    $.messager.alert('UserManagement-DeleteRole', retData.Data, 'error');
                }
            },
            error: function () {
                $.messager.alert('UserManagement-DeleteRole', 'NetWork error when add delete a role', 'error');
            }
        });
    }
});