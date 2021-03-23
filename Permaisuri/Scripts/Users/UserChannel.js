/// <summary>
/// 点击“Channels Setting”设置当前用户和渠道关联
/// Author:Lee, Date:2013-11-1
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    var chosenCSS = require('PlugIn/Chosen/chosen.min.css');//dynamically loading css
    var chosen = require('PlugIn/Chosen/chosen.jquery.min');
    var USOModel = require('./UserStatusOperation');
    
    exports.UserChannel = {
        /// <summary>
        /// 点击Channels Setting，打开选择渠道的面板设置
        /// Modified:将多选改成单选..因为报表的存储过程暂不支持！.2013年11月4日11:14:11
        /// </summary>
        /// <param name="User_Guid">用于动态查询当前用户的渠道信息的参数</param>
        /// <param name="User_Account">用于Dialog面板展示使用</param>
        OpenDialog: function (User_Guid, User_Account) {
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
                        //先关闭再打开，否者会报错第二次打开会报错！文档没说明，全凭瞎猜测,或者增加一个参数用来判断是否第一次打开
                        //$(".multipleSelect").chosen('close');

                        //必须等到channelSettingDiv show了之后才能做chosen，否则就会影响chosen的计算，导致chosen计算出来的长度为0 lee
                        $(".multipleSelect").chosen({
                            //placeholder_text_multiple: "Click here to select Channels. 'Alt + Right click' to select more than one.",
                            //inherit_select_classes: true,/*这个选项非常坑爹，加上去会导致打开各种报错. 加上chose('close')之后，虽然打开不报错，但是这里面的OPtion的设置都无效 2013年11月1日16:32:03*/
                            display_selected_options: false
                        });

                        //S0:过滤从服务器返回的当前用户的渠道信息数据，将其转化为一个数字，后续才有办法操作
                        var arrChannelID = [];
                        $.each(retData.Data, function () {
                            //$(this).prop("selected", false);
                            arrChannelID.push(parseInt(this.ChannelID));
                        });

                        //S1:去掉option菜单的所有selected属性
                        $.each($(".multipleSelect option"), function () {
                            $(this).prop("selected", false);
                        });

                        //S3：将当前的option逐项的和当前用户的渠道信息对比，匹配上的则加上selected 属性，否则去掉
                        $.each($(".multipleSelect option"), function () {
                            if (arrChannelID.length > 0) {
                                var self = this;
                                var curCID = parseInt($(self).val());
                                //alert(JSON.stringify(curCID) + "arrChannelID" + JSON.stringify(arrChannelID));
                                //if ($.inArray(curCID, arrChannelID) > 0) {
                                if ($.inArray(curCID, arrChannelID) > -1) {//确定第一个参数在数组中的位置, 从0开始计数(如果没有找到则返回 -1 ).
                                    $(self).prop("selected", true);
                                }
                            }
                        });


                        //找了1个小时 吐血了，这个选项用来重新更新插件的状态，如果不调用这个选项，那么上一次选择的选项，将继续保留到下一次 2013年10月31日18:38:12
                        $(".multipleSelect").trigger("chosen:updated");

                        //关键一步：监听当前用户渠道的变动
                        var isChanged = false;
                        $(".multipleSelect").chosen().change(function (evt, params) {
                            //alert(JSON.stringify(params));//{"deselected":"15"}  {"selected":"15"}
                            //arrChannelID
                            isChanged = true;
                            var self = params;
                            if (typeof (self.selected) != "undefined") {
                                arrChannelID.push(self.selected);
                            } else if (typeof (self.deselected) != "undefined") {
                                arrChannelID.splice($.inArray(self.deselected, arrChannelID), 1);
                            }
                        });

                     
                        //$(".multipleSelect").chosen().showing_dropdown(function () {
                        //    //无效！JS直接报错
                        //});
                        //$(".multipleSelect").trigger("chosen:showing_dropdown");//无效
                        $("#channelSettingDiv").dialog({
                            title: 'Channels Setting',
                            width: 450,
                            height: 550,
                            closed: false,
                            cache: false,
                            modal: true,
                            buttons: [
                                {
                                    //Add by Lee 2013年12月3日10:29:20
                                    text: 'Disable Associated',
                                    iconCls: 'icon-undo',
                                    handler: function () {
                                        USOModel.UserStatus.UpdateUserChannelControl(User_Guid, false, function () {
                                            $("#channelSettingDiv").dialog("close");//关闭当前面板
                                            //$("#btnSearching").click();//重新刷新一次User面板
                                            $('#userManageDG').datagrid('reload');
                                        });
                                    }// end of Func handler
                                },

                                {
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
                                            }
                                        }).done(function (retData) {
                                            if (retData.Status === 1) {
                                                $.messager.alert('UserManagement-Channnels Setting', retData.Data, 'info');
                                            } else {
                                                $.messager.alert('UserManagement-Channnels Setting', retData.Data, 'error');
                                            }
                                        }).fail(function () {
                                            $.messager.alert('UserManagement-Channnels Setting', 'NetWork error when add new user', 'error');
                                        })
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
        }//end of OpenDialog
    }
});