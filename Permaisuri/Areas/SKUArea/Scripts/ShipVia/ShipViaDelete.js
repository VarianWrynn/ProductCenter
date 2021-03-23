define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var PostDataModel = require('./PostData');

    exports.ShipVia = {
        Delete: function (row) {
            $.ajax({
                url: "./DeleteShipVia",
                type: "POST",
                dataType: "json",
                data: {
                    SHIPVIAID: row.SHIPVIAID,
                },
                success: function (retData) {
                    if (retData.Status === 1) {
                        var queryParams = PostDataModel.PostData.queryPostData();
                        $("#SHipViaDG").datagrid('reload', queryParams);
                    } else {
                        $.messager.alert('ShipVia-Delete', retData.Data, 'error');
                    }
                },
                error: function () {
                    $.messager.alert('ShipVia-Delete', 'NetWork error when add delete ShipVia', 'error');
                }
            });
        }
    }
});