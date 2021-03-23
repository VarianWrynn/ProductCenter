define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    exports.addChannel = function () {
        $("#channelName").val(''); shortName
        $("#shortName").val('');
        $("#API").val(1);
        $("#Export2CSV").val(1);
        $("#channelInfoDigForm").show();
        $("#channelInfoTable").show();
        $('#channelInfoDigForm').dialog({
            title: 'Add New Channel',
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
                    var fvalidate = $("#channelInfoDigForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }

                    $.ajax({
                        url: "./AddChannel",
                        type: "POST",
                        dataType: "json",
                        data: {
                            ChannelName: $("#channelName").val(),
                            ShortName: $("#shortName").val(),
                            API: $("#API").val() == 1 ? true : false,
                            Export2CSV: $("#Export2CSV").val() == 1 ? true : false
                        },
                        success: function (retData) {
                            if (retData.Status === 1) {
                                $('#channelManageDG').datagrid('reload');
                                $("#channelInfoDigForm").dialog("close");
                            } else {

                                $.messager.alert('ChannelManagement-AddChannel', retData.Data, 'error');
                            }
                        },
                        error: function () {
                            $.messager.alert('ChannelManagement-AddChannel', 'NetWork error when add new channel', 'error');
                        }
                    });
                }
            }, {
                text: 'Close',
                iconCls: 'icon-cancel',
                handler: function () {
                    $("#channelInfoDigForm").dialog("close");
                }
            }]
        });
    }
});