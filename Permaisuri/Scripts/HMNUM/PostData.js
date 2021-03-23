/// <summary>
/// HMNUM页面的相关操作,主要是编辑
/// Author:Lee, Date:2013-11-13
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.PostData = {
        Get: function () {
            return {
                HMNUM: $.trim($('#HMNUM').val()),
                ProductName: $.trim($('#ProductName').val()),
                StockKey: $.trim($('#StockKey').val()),
                queryIsGroup: $("#queryIsGroup").combobox("getValue"),
                ISOrphan: $("#ISOrphan").prop("checked"),
                IsExcludedSubHMNUM: $("#IsExcludedSubHMNUM").prop("checked"),
                StatusID: $("#Status").combobox("getValue"),
                RequestType: $.getUrlVar('RequestType')
            }
        }
    }
});