/// <summary>
/// 角色维护界面：当用户在 “角色管理” 页面点击 "菜单设置" 按钮的时候触该页面的事件
/// Role Management：When User click "MenuSetting" button on the "Role Management" page ,this page will be loading
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var RoleMenuUpdateModel = require('./RoleMenuUpdate');

    /// <summary>
    /// 当用户点击"设置角色"的时候自动出发UserRoleUpdateInit 事件
    /// when use click "RoleSetting" button will trigger UserRoleUpdateInit function automatically 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.RoleMenusLoading = function (Role_GUID) {
        rowData = [];
        var W = $(window);
        var h = W.height();
        var h1 = $("#con_tsearch").height();
        var h2 = $("#pagettl1").height();
        $("#dlgMenuSetting").show();
        RoleMenuUpdateModel.UserRoleUpdateInit(Role_GUID);
        //loading Menu List 
        $("#menuSettingDG").treegrid({
            width:'auto',
            height:h-h2-50,
            resizeHandle: "right",
            striped: true,
            sortName: 'SortNo',
            sortOrder: 'desc',
            idField: 'MenuID',
            treeField: 'name',
            rownumbers: true,
            singleSelect: false,
            loadMsg: 'Ladoing，pls waiting！',
            url: './RoleMenusLoading',
            queryParams: {
                Role_Guid: Role_GUID
            },
            onLoadSuccess: function (row,data) {
                $.each(data.rows, function (index, item) {
                    if (item.Role_Checked) {
                        $('#menuSettingDG').treegrid('select', item.MenuID);
                    }
                });
             
            },
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("Get Role List", "Loading role data from server is wrong ", "warning");
                    return false;
                }
            },
            columns: [[
                    { field: 'ck', title: 'checkbox', checkbox: true},
                    { field: 'name', title: 'MenuName', width: 280 },
                    { field: 'Icon', title: 'Icon', width: 80 },
                    { field: 'MenuID', title: 'ID', width: 80 },
                    { field: 'Memo', title: 'Memo', width: 240 },
                    { field: 'MR_ID', title: 'GUID', width: 0, hidden: true },

            ]]
        });
    }
});