define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.addBrand = function () {
        $("#brandName").val("");
        $("#shortName").val("");
        $("#Status").val(1);
        $("#brandInfoTable").show();
        $("#brandInfoDigForm").show();
        $('#brandInfoDigForm').dialog({
            title: 'Add New Brand',
            width: 600,
            height: 400,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {
                    // extend the 'confirm password' rule  
                    var fvalidate = $("#brandInfoDigForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }

                    $.ajax({
                        url: "./AddBrand",
                        type: "POST",
                        dataType: "json",
                        data: {
                            Brand_Name: $("#brandName").val(),
                            Short_Name: $("#shortName").val(),
                            Active: $("#Status").val()==1?true:false,
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                $('#brandManageDG').datagrid('reload');
                                $("#brandInfoDigForm").dialog("close");
                            } else {

                                $.messager.alert('BrandManagement-AddBrand', retData.Data, 'error');
                            }
                        },
                        error: function () {
                            $.messager.alert('BrandManagement-AddBrand', 'NetWork error when add new brand', 'error');
                        }
                    });
                }
            }, {
                text: 'Close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#brandInfoDigForm").dialog("close");
                }
            }]
        });
    }
})