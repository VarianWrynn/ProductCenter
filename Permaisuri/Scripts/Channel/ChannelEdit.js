define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.editChannel = function (ChannelID, ChannelName, ShortName, API, Export2CSV) {
        $("#channelName").val(ChannelName);
        $("#shortName").val(ShortName);

        API ? $("#API").val(1) : $("#API").val(0);
        Export2CSV ? $("#Export2CSV").val(1) : $("#Export2CSV").val(0);
        $("#channelInfoTable").show();
        $("#channelInfoDigForm").show();
        $('#channelInfoDigForm').dialog({
            title: 'Edit Channel Information',
            width: 600,
            height: 400,
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                text: 'Save',
                iconCls: 'icon-save',
                handler: function () {

                    var fvalidate = $("#channelInfoDigForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }
                    $.ajax({
                        url: "./EditChannel",
                        type: "post",
                        dataType: "json",
                        data: {
                            ChannelID:ChannelID,
                            ChannelName: $("#channelName").val(),
                            ShortName: $("#shortName").val(),
                            API: $("#API").val() == 1 ? true : false,
                            Export2CSV: $("#Export2CSV").val() == 1 ? true : false,
                        },
                        success: function (retData) {
                            if (retData.Status == 1) {
                                $('#channelManageDG').datagrid('reload');
                                $("#channelInfoDigForm").dialog("close");
                            }
                            else { $.messager.alert('ChannelManagement-EditChannel', retData.Data, 'error'); }
                        },
                        error: function () {
                            $.messager.alert('ChannelManagement-EditChannel', 'NetWork error when edit new channel', 'error');
                        }
                    });
                }
            }, {
                text: 'close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#channelInfoDigForm").dialog("close");
                }
            }]
        });
    }
})