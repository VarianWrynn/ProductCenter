define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    exports.UserStatus = {
        UpdateUserStatus: function (Status, User_Guid) {
            prograss.show();
            $.ajax({
                url: "./UpdateUserStats",
                type: "POST",
                dataType: "json",
                data: {
                    Status: Status,
                    User_Guid: User_Guid
                },
                success: function (retData) {
                    prograss.hide();
                    if (retData.Status === 1) {

                    } else {
                        $.messager.alert('UserManagement-UpdateUserStats', retData.Data, 'error');
                    }
                },
                error: function () {
                    prograss.hide();
                    $.messager.alert('UserManagement-UpdateUserStats', 'fail to operate,please contact administrator', 'error');
                }
            });
        },

        /// <summary>
        ///更新当前用户是否打开/关闭关联Channels的设置
        /// Author:Lee,Date:2013年11月4日9:10:17
        /// </summary>
        /// <param name="User_Guid">用户唯一标识</param>
        /// <param name="Status">true-开发，false-关闭</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        UpdateUserChannelControl: function (User_Guid,Status,callBack) {
            prograss.show();
            $.ajax({
                url: "./UpdateUserChannelControl",
                type: "POST",
                dataType: "json",
                data: {
                    User_Guid: User_Guid,
                    IsChannelControl: Status
                },
                success: function (retData) {
                    prograss.hide();
                    if (retData.Status === 1) {
                        if (callBack) {
                            callBack();
                        }
                    } else {
                        $.messager.alert('UserManagement-UpdateUserChannelControl', retData.Data, 'error');
                    }
                },
                error: function () {
                    prograss.hide();
                    $.messager.alert('UserManagement-UpdateUserChannelControl', 'fail to operate,please contact administrator', 'error');
                }
            });
        }
    }
});