/// <summary>
/// 点击“Channels Setting”设置当前用户和渠道关联
/// Author:Lee, Date:2013-11-1
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');
    var PostDataModel = require('./PostData');

    exports.ShipVia = {


        /// <summary>
        /// 为ShipeViaType设置一个默认的ShipVia。比如为快递Express设置一家默认的快递类型，比如FEDX_1D。2014年5月12日9:45:34
        /// CMS只需要知道SHIPVIAType（物流or快递）就足够了，但是同步到eCom需要确切知道type下面的子类型
        /// </summary>
        /// <param name="User_Guid">用于动态查询当前用户的渠道信息的参数</param>
        /// <param name="User_Account">用于Dialog面板展示使用</param>
        UpdateDefaultShipVia: function (SHIPVIAID, ShipViaTypeID) {
            var UpdateThis = this;
            $("#SHipViaDG").datagrid('loading');
            $.ajax({
                url: "./UpdateDefaultShipVia",
                type: "POST",
                dataType: "json",
                cache: false,
                data: {
                    SHIPVIAID: SHIPVIAID,
                    ShipViaTypeID: ShipViaTypeID
                }
            }).done(function (retData) {
                if (retData.Status === 1) {
                    var queryParams = PostDataModel.PostData.queryPostData();
                    $("#SHipViaDG").datagrid('reload', queryParams);
                } else {
                    $.messager.alert("ShipVia -setting default shipvia type", retData.Data, "error");
                    return false;
                }
               
            }).fail(function () {
                $.messager.alert('ShipVia -setting default shipvia type', 'Error try to  set default shipvia type', 'error');
            });
        }// end of property:UpdateDefaultShipVia
    }
});