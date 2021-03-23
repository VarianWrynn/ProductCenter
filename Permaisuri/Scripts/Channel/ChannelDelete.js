define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.deleteChannel = function (channelID) {
        $.ajax({
            url: "./DeleteChannel",
            type: "post",
            dataType: "json",
            data: {
                ChannelID: channelID,
            },
            success: function (retData) {
                if (retData.Status == 1) {
                    $('#channelManageDG').datagrid('reload');
                    $("#channelInfoDigForm").dialog("close");
                }
                else { $.messager.alert('ChannelManagement-EditChannel', retData.Data, 'error'); }
            },
            error: function () {
                $.messager.alert('ChannelManagement-DeleteChannel', 'NetWork error when delete channel', 'error');
            }
        });
    }
})