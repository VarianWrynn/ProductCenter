
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/prograss');
    var RoleAddModel = require('./RoleAdd');
    var RoleDeleteModel = require('./RoleDelete');
    var RoleEditModel = require('./RoleEdit');
    //var UserInlineEdit = require('./RoleInlineEdit');
    var RoleMenusLoadingModel = require('./RoleMenusLoading');
    var TagsModel = require('Common/OpenCMSTag');
    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();

        $(window).resize(function () {
            var W = $(window);
            var h = W.height();
            $('#roleManageDG').datagrid('resize', { width: W - 20, height: h - 20 });
        });
        $('#roleManageDG').datagrid({
            title: "Role Management",
            autoRowHeight: false,
            resizeHandle:"right",
            striped: true,
            sortName: 'Role_Name',
            sortOrder: 'desc',
            remoteSort: false,
            idField: 'Role_GUID',
            pagination: true,
            rownumbers: true,
            singleSelect:true,
            url: './GetRoleList',
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("Get Role List","Loading role data from server is wrong ","warning");
                    return false;
                }
            },
            toolbar: [{
                iconCls: 'icon-add',
                text: 'Add',
                handler: function () {
                    RoleAddModel.addRole();
                }
            }, '-', {
                iconCls: 'icon-edit',
                text: 'Edit',
                handler: function () {
                    var row = $('#roleManageDG').datagrid('getSelected');
                    if (row == null) {
                        $.messager.alert('RoleManagement-RoleEdit', "Please choose a row ", 'warning');
                        return false;
                    }
                    RoleEditModel.editRole(row.Role_GUID, row.Role_Name, row.Role_Desc);
                }
            }, '-', {
                iconCls: 'icon-no',
                text: 'delete',
                handler: function () {
                    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                        if (r) {

                            var row = $('#roleManageDG').datagrid('getSelected');
                            if (row == null) {
                                $.messager.alert('RoleManagement-RoleDeleted', "Please choose a row ", 'warning');
                                return false;
                           }
                            RoleDeleteModel.deleteRole(row.Role_GUID);
                        }
                    });
                }
            }, '-', {
                iconCls: 'icon-remove',
                text: 'MenuSetting',
                handler: function () {
                    var row = $('#roleManageDG').datagrid('getSelected');
                    if (row == null) {
                        $.messager.alert('RoleManagement-MenuSetting', "Please choose a row ", 'warning');
                        return false;
                    }
                    RoleMenusLoadingModel.RoleMenusLoading(row.Role_GUID);
                }
            }],
            onDblClickCell: function (rowIndex, field, value) {
                //UserInlineEdit.beginEditing(rowIndex, field, value);
            },
            columns: [[
                    { field: 'Role_GUID', title: 'GUID', width: 80, hidden: true },
                    {
                        field: 'Role_Name', title: 'Role Name', width: 200,
                        editor: {
                            type: 'validatebox',
                            options: { required: true }
                        }
                    },
                    { field: 'Role_Desc', title: 'Role Description', width: 150 },
                    { field: 'Operation', title: 'Operation', width: 200, align: 'right' }
            ]]
        });//end of datagrid

    });
});