/// <summary>
/// 角色维护界面：当用户在 “角色管理” 页面点击 "菜单设置" 按钮的时候出发该页面的事件
/// Role Management：When User click "MenuSetting" button on the "Role Management" page ,this page will be loading
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');

    /// <summary>
    /// 当用户点击"设置角色"的时候自动出发UserRoleUpdateInit 事件
    /// when use click "RoleSetting" button on the "Role Management" page ,it will trigger UserRoleUpdateInit function automatically 
    /// This function loading Tree-Grid  and Diaglog for dispalying Role-Menus-Setting
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.UserRoleUpdateInit = function (role_Guid) {
        var W = $(window);
        var h = W.height();
        var h1 = $("#con_tsearch").height();
        var h2 = $("#pagettl1").height();
       
        $('#dlgMenuSetting').dialog({
            title: 'Menus In Role Setting',
            width: 800,
            height: h - h2 - 50,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {
                    //var checkedRows = $('#menuSettingDG').treegrid('getChecked'); for being tested, use 'getChecked' will hit bug when  click "allCheck" chebox...
                    var checkedRows = $('#menuSettingDG').treegrid('getSelections');
                    var arrGuid = [];
                    $.each(checkedRows, function (index, rows) {
                        arrGuid.push(rows.MR_ID);
                    });
                    $.ajax({
                        url: "./UserRoleUpdate",
                        type: "POST",
                        dataType: "json",
                        data: {
                            Role_Guid:role_Guid,
                            menuGuids: arrGuid
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                //$('#roleManageDG').datagrid('reload');
                                $("#dlgMenuSetting").dialog("close");
                            } else {
                                $.messager.alert('UserManagement-AddRole', retData.Data, 'error');
                            }
                        },
                        error: function () {
                            $.messager.alert('UserManagement-dlgRoleManage', 'NetWork error when add new role', 'error');
                        }
                    });
                }
            }, {
                text: 'Close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#dlgMenuSetting").dialog("close");
                }
            }]
        });
    }
});