define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');
    /// <summary>
    /// 触发CMS和WebPO的账号进行数据同步。由于一开始CMS设置了自己的用户信息，并且使用GUID进行关联。后来要求用户统一在WEBPO进行设置
    /// 而WebPO采用自增长而非GUID的方式记录数据。因此需要采取一种同步机制而不是简单的View视图关联....
    /// Author:Lee Date:2013年10月22日11:56:03
    /// </summary>
    /// <param name="??">placeholder</param>
    /// <returns></returns>
    exports.UserSynchronizing = function () {
        prograss.show();
        $.ajax({
            url: "./UserSynchWithWebPO",
            type: "POST",
            dataType: "json",
            data: {
            },
            success: function (retData) {
                prograss.hide();
                if (retData.Status === 1) {
                    $('#userManageDG').datagrid('reload');
                } else {
                    $.messager.alert('UserManagement-Synchronizing User Data', retData.Data, 'error');
                }
            },
            error: function () {
                prograss.hide();
                $.messager.alert('UserManagement-Synchronizing User Data', 'NetWork Error when Synchronizing User Data', 'error');
            }
        });
    }
});