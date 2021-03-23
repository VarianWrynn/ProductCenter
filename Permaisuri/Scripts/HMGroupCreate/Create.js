/// <summary>
/// HM#组合产品的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var HMGroupModel = require('./HMGroup');
    var fancybox = require('FancyBox/jquery.fancybox');

    var UIExtModel = require('Common/EasyUIExt');
    var TagsModel = require('Common/OpenCMSTag');
    $(document).ready(function () {
        progress.hide();
        //设置字段提醒
        var tooltipModel = require('../HMConfig/HMConfigTooltip');
        tooltipModel.HMConfig.InitToolTip();

        TagsModel.CMSTags.BackSpace();
        //单击展示图片
        $(".fancybox").fancybox({
            tpl: {
                error: '<p class="fancybox-error">There is no picture for this item</p>',
            }
        });

        //校验
        UIExtModel.Ext.ValidateFolder();
        $("#HMNUM").validatebox({
            validType: ["ValidateFolder"]
        });

        HMGroupModel.HMGroup.Init();

    }); //end of document.ready
});