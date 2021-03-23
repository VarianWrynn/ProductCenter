/// <reference path="../jquery-1.7.1-vsdoc.js" />
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/prograss');
    var SKUBaseModel = require('./SKUBase');
    var fancybox = require('FancyBox/jquery.fancybox');
    var TagsModel = require('Common/OpenCMSTag');

    var TooltipModel = require('../ProductConfiguration/ProductTooltip');
    var UIExtModel = require('Common/EasyUIExt');

    $(document).ready(function () {
        progress.hide();
        //各种字段的提示信息
        TooltipModel.SKU.InitToolTip();

        TagsModel.CMSTags.BackSpace();
        //单击展示图片
        $(".fancybox").fancybox({
            tpl: {
                error: '<p class="fancybox-error">There is no picture for this item</p>',
            }
        });

        $("#opStatus").combobox({
            required: true,
            disabled: true
        });


        $("#opVisibiliy").combobox({
            required: true
        });



        $('#ProductDesc').validatebox({
            required: "required"
        });

        $('#Specifications').validatebox({
            deltaX: -260,
            deltay: -100,
            required: "required",
            validType: 'length[0,1000]'
        });

       
        $('#URL').validatebox({
            deltaX: -300,
            validType: 'url'
        });

        UIExtModel.Ext.ValidateRemoteIsExist();
        $("#SKU").validatebox({
            validType: ['length[3, 100]', 'RemoteIsExist["../SKUCreate/CheckSKUExist","SKU"]'],
            //invalidMessage: "This item already exists",
            delay:500
        });


        $('#SKU_QTY').numberbox({
            deltaX: -60,
            required: true,
            precision: 0
        });

        $("#ProductName,#SKU,#UPC").validatebox({
            required:true
        });

        SKUBaseModel.SKUBase.Init();

    });// end of document.ready
});