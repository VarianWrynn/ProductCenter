define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.deleteBrand = function (Brand_Id) {
        $.ajax({
            url: "./DeleteBrand",
            type: "post",
            dataType: "json",
            data: {
                Brand_Id: Brand_Id,
            },
            success: function (retData) {
                if (retData.Status == 1) {
                    $('#brandManageDG').datagrid('reload');
                    $("#brandInfoDigForm").dialog("close");
                }
                else { $.messager.alert('BrandManagement-EditBrand', retData.Data, 'error'); }
            },
            error: function () {
                $.messager.alert('BrandManagement-DeleteBrand', 'NetWork error when delete brand', 'error');
            }
        });
    }
})