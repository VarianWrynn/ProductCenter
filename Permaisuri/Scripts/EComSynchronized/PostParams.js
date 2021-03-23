
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.Get = {
        PostParams: function () {
            return {
                Keywords: $.trim($("#keywordbg").val()),
                SKUOrder: $.trim($(".searchSKU").val()),
                UpdateBy: $.trim($("#UpdateBy").val()),//2014年2月14日16:10:01
                BrandID: $("#cbBrand").combobox('getValue'),
                ChannelID: $("#cbChannel").combobox('getValue'),
                multiplePartType: $("#cbMultiplePart").combobox('getValue'),
                Status: $('#cbStatus').combobox('getValue'),
                CategoryID: $("#cbCategory").combobox('getValue'),
                InventoryType: $('#cbInventory').combobox('getValue'),
                CMS_Ecom_Sync: { test: "1" }//发{}空对象会被jQuery忽略掉，不传送该对象到后台，导致后台这个对象无法初始化。2014年3月28日 CMS_Ecom_Sync[test]:1
            }
        }
    }
});