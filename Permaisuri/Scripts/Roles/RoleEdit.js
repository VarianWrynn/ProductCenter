define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.editRole = function (Role_Guid, Role_Name, Role_Desc) {
        $("#actionSpan").text("Edit Role");
        $("#dlgRoleManage").show();
        $("#Role_Name").val(Role_Name);
        $("#Role_Desc").val(Role_Desc);

        $('#dlgRoleManage').dialog({
            title: 'Edit Role',
            width: 400,
            height: 280,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {
                    var fvalidate = $("#roleManageForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }

                    $.ajax({
                        url: './EditRole',
                        type: 'POST',
                        dataType: 'json',
                        data: {
                            Role_Guid: Role_Guid,
                            Role_Name: $("#Role_Name").val(),                    
                            Role_Desc: $("#Role_Desc").val()
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                $('#roleManageDG').datagrid('reload');
                                $("#dlgRoleManage").dialog("close");
                            }
                            else {                                
                                $.messager.alert('UserManagement-EditRole', retData.Data, 'error');
                            }
                        },
                        error: function () {
                            $.messager.alert('UserManagement-dlgRoleManage', 'NetWork error when edit role', 'error');
                        }
                    })
                }
            }, {
                text: 'Close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#dlgRoleManage").dialog("close");
                }
            }]
        });
    }
})