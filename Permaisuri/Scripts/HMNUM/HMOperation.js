/// <summary>
/// HMNUM页面的相关操作,主要是编辑
/// Author:Lee, Date:2013-11-13
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    var chosenCSS = require('PlugIn/Chosen/chosen.min.css');//dynamically loading css
    var chosen = require('PlugIn/Chosen/chosen.jquery.min');

    
    exports.HM = {
        Edit: function (User_Guid, callback) {
            $("#channelSettingTitle").text("Setting " + User_Account + " 's Channel");
            $("#channelSettingDiv").show();
            prograss.show();
            $.ajax({
                url: "./GetChannelByGuid",
                type: "POST",
                dataType: "json",
                cache: false,
                data: {
                    'User_Guid': User_Guid
                },
                success: function (retData) {
                    prograss.hide();
                    if (retData.Status === 1) {
                        $("#channelSettingDiv").dialog({
                            title: 'Channels Setting',
                            width: 450,
                            height: 550,
                            closed: false,
                            cache: false,
                            modal: true,
                            buttons: [{
                                text: 'Save',
                                iconCls: 'icon-save',
                                handler: function () {
                                    //alert($(".multipleSelect").val());
                                    //if (isChanged) {
                                    if (true) {
                                        $.ajax({
                                            url: "./UpdateUserChannel",
                                            type: "POST",
                                            dataType: "json",
                                            traditional: true,//prevent ajax deep copy oject 
                                            data: {
                                                'User_Guid': User_Guid,
                                                "ArrChannels": $(".multipleSelect").val()
                                            },
                                            success: function (retData) {
                                                if (retData.Status === 1) {
                                                    $.messager.alert('UserManagement-Channnels Setting', retData.Data, 'info');
                                                } else {
                                                    $.messager.alert('UserManagement-Channnels Setting', retData.Data, 'error');
                                                }
                                            },
                                            error: function () {
                                                $.messager.alert('UserManagement-Channnels Setting', 'NetWork error when add new user', 'error');
                                            }
                                        });
                                    } else {

                                    }
                                }
                            }, {
                                text: 'Close',
                                iconCls: 'icon-cancel',
                                handler: function () {
                                    //$(".multipleSelect").chosen('destroy');
                                    $("#channelSettingDiv").dialog("close");
                                }
                            }]
                        }); // end of  dialog

                    } else {
                        $.messager.alert('UserManagement-Channnels Setting', retData.Data, 'error');
                    }
                },
                error: function () {
                    prograss.hide();
                    $.messager.alert('UserManagement-Channnels Setting', 'Error when try to open Channel Setting Pannel', 'error');
                }
            });
        },//end of Func Edit();
        SynchHMNewData: function () {//add by Lee 2013年12月11日15:31:24
            prograss.show();
            $.ajax({
                url: "./SynchHMNewData",
                type: "POST",
                dataType: "json"
            }).done(function (retData) {
                prograss.hide();
                if (retData.Status === 1) {
                    $('#HMNUMManageDG').datagrid('reload');
                } else {
                    $.messager.alert('HMManagement-Synchronizing HM Data', retData.Data, 'error');
                }
            }).fail(function () {
                prograss.hide();
                $.messager.alert('HMManagement-Synchronizing HM Data', 'NetWork Error when Synchronizing HM Data', 'error');
            });
        }
    }
});