define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var urLoadModel = require('./UserRoleLoading');
    exports.UpdateUserRole = function (User_Guid) {
        urLoadModel.initRoleList(User_Guid);
        $("#roleSetting").show();
        $("#roleSetting").dialog({
            title: 'Role Setting',
            width: 320,
            height: 350,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {
                    var rolesData = urLoadModel.rolesData;
                    //debugger;
                    //var rolesData = $("body").data("rolesData");
                    $.ajax({
                        url: "./UpdateRoleInUser",
                        type: "POST",
                        dataType: "json",
                        data: {
                            'User_Guid':User_Guid,
                            "ArrRoles": rolesData
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                //$('#userManageDG').datagrid('reload');
                                $("#roleSetting").dialog("close");
                            } else {
                                $.messager.alert('UserManagement-AddUser', retData.Data, 'error');
                            }
                        },
                        error: function () {
                            $.messager.alert('UserManagement-AddUser', 'NetWork error when add new user', 'error');
                        }
                    });
                }
            }, {
                text: 'Close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#roleSetting").dialog("close");
                }
            }]
        });
    }
});