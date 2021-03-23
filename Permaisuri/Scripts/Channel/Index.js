define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var addChannel = require('./ChannelAdd');
    var editChannel= require('./ChannelEdit');
    var deleteChannel = require('./ChannelDelete');
    var TagsModel = require('Common/OpenCMSTag');
    var progress = require('Common/prograss');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        var H = $('#userManageDG').height();
        $(window).resize(function () {
            var W = $(window).width();
            $('#channelManageDG').datagrid('resize', { width: W - 20, height: H });
        });


        $("#btnSearching").on("click", function () {
            $('#channelManageDG').datagrid('load', {
                ChannelName: $.trim($('#shChannelName').val()),
                ShortName: $.trim($('#shShortName').val()),
                queryAPI: $("#shAPI").combobox('getValue'),
                queryExport2CSV: $("#shExport2CSV").combobox('getValue'),
            });
        });

        $("#btnReset").on("click", function () {
            $('#shChannelName').val("");
            $('#shShortName').val("");
            $('#shAPI').combobox('select', 2);
            $('#shExport2CSV').combobox('select', 2);
        });

        $("#channelManageDG").datagrid({
            title: "Channel Management",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            page: 1,
            rows: 10,
            remoteSort: false,
            idField: 'ChannelID',
            pagination: true,
            rownumbers: true,
            singleSelect: true,
            queryParams: {//add by lee  2013年11月21日16:08:09
                queryAPI: 2,
                queryExport2CSV: 2
            },
            url: './GetChannelList',
            onLoadSuccess: function () {
            },
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("Get Brand Data", retData.Data, "warning");
                    return false;
                }
            },
            toolbar: [{
                iconCls: 'icon-add',
                text: 'Add',
                handler: function () {
                    //$.messager.alert("I gonan add a new user", "done");
                    addChannel.addChannel();
                }
            }, '-', {
                iconCls: 'icon-edit',
                text: 'Edit',
                handler: function () {
                    var row = $('#channelManageDG').datagrid('getSelected');
                    if (row == null) {
                        $.messager.alert('ChannelManagement-ChannelEdit', "Please choose a row ", 'warning');
                        return false;
                    }
                    editChannel.editChannel(row.ChannelID, row.ChannelName, row.ShortName, row.API, row.Export2CSV);
                }
            }, '-', {
                iconCls: 'icon-no',
                text: 'delete',
                handler: function () {
                    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                        if (r) {
                            var row = $('#channelManageDG').datagrid('getSelected');
                            if (row == null) {
                                $.messager.alert('channelManagement-channelDeleted', "Please choose a row ", 'warning');
                                return false;
                            }
                            deleteChannel.deleteChannel(row.ChannelID);
                        }
                    });
                }
            }],
            columns: [[
                    { field: 'ChannelID', title: 'ID', width: 80, hidden: true },
                    { field: 'ChannelName', title: 'Channel Name', width: 320 },
                    {
                        field: 'ShortName', title: 'Short Name', width: 200,
                        editor: {
                            type: 'validatebox',
                            options: { required: true }
                        }
                    },
                    {
                        field: 'API', title: 'API', width: 100,
                        formatter: function (value, row, index) {
                            if (value == 1) {
                                return "Yes";
                            }
                            else {
                                return "No";
                            }
                        }
                    },
                    {
                        field: 'Export2CSV', title: 'Export2CSV', width: 100,
                        formatter: function (value, row, index) {
                            if (value == 1) {
                                return "Yes";
                            }
                            else {
                                return "No";
                            }
                        }
                    },
                    
                    { field: 'Modifier', title: 'Modifier', width: 150 }
                    ,
                    {
                        field: 'strModify_Date', title: 'Modifier Date', align: 'right',
                    }
            ]]
        }) //end of datagrid

    });
})