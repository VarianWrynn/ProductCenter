define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.editBrand = function (Brand_Id, Brand_Name, Short_Name, Active) {
        $("#brandName").val(Brand_Name);
        $("#shortName").val(Short_Name);
        Active ? $("#Status").val(1) : $("#Status").val(0);
        $("#brandInfoTable").show();
        $("#brandInfoDigForm").show();
        $('#brandInfoDigForm').dialog({
            title: 'Edit Brand Information',
            width: 600,
            height: 400,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {

                    var fvalidate = $("#brandInfoDigForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }
                    $.ajax({
                        url: "./EditBrand",
                        type: "post",
                        dataType: "json",
                        data: {
                            Brand_Id: Brand_Id,
                            Brand_Name: $("#brandName").val(),
                            Short_Name:  $("#shortName").val(),
                            Active: $("#Status").val()==1?true:false
                        },
                        success: function (retData) {
                            if (retData.Status == 1) {
                                $('#brandManageDG').datagrid('reload');
                                $("#brandInfoDigForm").dialog("close");
                            }
                            else { $.messager.alert('BrandManagement-EditBrand', retData.Data, 'error'); }
                        },
                        error: function () {
                            $.messager.alert('BrandManagement-EditBrand', 'NetWork error when edit new brand', 'error');
                        }
                    });
                }
            }, {
                text: 'close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#brandInfoDigForm").dialog("close");
                }
            }]
        });
    }
})