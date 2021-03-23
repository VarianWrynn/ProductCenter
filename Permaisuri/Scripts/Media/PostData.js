/// <summary>
/// HMNUM页面的相关操作,主要是编辑
/// Author:Lee, Date:2013-11-13
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.PostData = {
        Get: function () {
            var postData = {};
            postData.page = $("#aRow").text();
            postData.rows = $("#opPageSize").val();
            postData.SKUOrder = $("#SKUOrder").val();
            postData.HMNUM = $("#HMNUM").val();
            postData.Channel = $("#cbChannel").combobox('getValue');
            postData.Format = $("#cbFormat").combobox('getValue');
            postData.Status = $("#cbStatus").combobox('getValue');
            postData.Brand = $("#cbBrand").combobox('getValue');
            postData.CloudStatusId = $("#cbCloudStatus").combobox('getValue');
            return postData;
        }
    }
});