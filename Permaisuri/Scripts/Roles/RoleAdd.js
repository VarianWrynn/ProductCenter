define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
  
    exports.addRole = function () {
        $("#dlgRoleManage").show();
        $("#Role_Name").val("");
        $("#Role_Desc").val("");
        var digWidth = $(window).width();
        var digHeight = $(window).height();

        $(window).resize(function () {
            var digWidth = $(window).width();
            var digHeight = $(window).height();
            $('#dlgRoleManage').dialog('resize', {
                width: digWidth - 650,
                height: digHeight - 300,
            });
        });


        $('#dlgRoleManage').dialog({
            title: 'Add New Role',
            width: digWidth - 650,
            height: digHeight - 300,
            //width: 300,
            //height:400,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {
                    var fvalidate = $("#roleManageForm").form('validate');
                    if (!fvalidate)
                    {
                        return;
                    }

                    $.ajax({
                        url: "./AddRole",
                        type: "POST",
                        dataType: "json",
                        data: {
                            Role_Name: $.trim($("#Role_Name").val()),
                            Role_Desc: $.trim($("#Role_Desc").val())
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                $('#roleManageDG').datagrid('reload');
                                $("#dlgRoleManage").dialog("close");
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
                    //消除校验，防止编辑的时候，明明有values缺提示不能为空。
                    //$("#Role_Name").validatebox('disableValidation');
                    $("#dlgRoleManage").dialog("close");
                }
            }]
        });
    }
});