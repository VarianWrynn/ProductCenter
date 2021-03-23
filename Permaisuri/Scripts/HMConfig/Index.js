/// <summary>
/// HMNUMConfiguration模块的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var CostModel = require('./HMCosting');
    var HMModel = require('./HMBaseInfo');
    var HMCTNModel = require('./HMCTN');
    var HMDimModel = require('./HMDim');
    var cmsTagsModel = require('Common/OpenCMSTag');
    var HMCTN_AddModel = require('./HMCTN_Add');
    var HMDim_AddModel = require('./HMDim_Add');
    var HMMediaModel = require('./HMMedia');
    var UploadModel = require('./HMMediaUpload');
    var TagsModel = require('Common/OpenCMSTag');
    var UIExtModel = require('Common/EasyUIExt');

    var BubbleInfoCSS = require('Media/BubbleInfo.css');
    var BubbleInfoModel = require('Media/BubbleInfo');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();

        //设置字段提醒
        var tooltipModel = require('./HMConfigTooltip');
        tooltipModel.HMConfig.InitToolTip();

        //校验
        UIExtModel.Ext.ValidateListLocal();

        $("#ShipViaType").combobox({
            width: $("#tooltipShipViaType").width() - ($("#tooltipShipViaType").width()* 0.01),
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#ShipViaType','ShipViaTypeName']"
        });


        $(window).resize(function () {
            var curW = $("#ProductName").width()+10;
            $('#ShipViaType').combobox('resize', curW);
        });


        //设置Status的初始状态
       $('#Status').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) > -1;
            },
            validType: "ValidateListLocal['#Status','StatusName']"
        });

        $(".aRelatedSKU").off("click").on("click", function () {
            var pID = $(this).attr("value");
            var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
            TagsParam.URL = "../ProductConfiguration/ProductConfiguration?SKUID=" + pID;
            TagsParam.name = "ProductConfiguration";
            TagsParam.title = "ProductConfiguration";
            TagsParam.iframeID = "frmTag";
            TagsParam.reload = true;
            cmsTagsModel.CMSTags.OpenTags(TagsParam);
        });

        $(".aRelatedParentHM").off("click").on("click", function () {
            var ProductID = $(this).attr("value");
            var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
            TagsParam.URL = "../HMGroupConfig/Config?ProductID=" + ProductID;
            TagsParam.name = "HMGroupConfig";
            TagsParam.title = "HMGroupConfig";
            TagsParam.iframeID = "frmTag";
            TagsParam.reload = true;
            cmsTagsModel.CMSTags.OpenTags(TagsParam);
        });

        $("input[name$='Comment']").css({ "width": "100%" });//改变comments的长度

        HMModel.HM.Init();//基础信息模块
        CostModel.HMCosting.Init();
        HMMediaModel.Media.Init();//图像事情初始化模块
        HMMediaModel.Media.GetImagesFromOtherSystem();
        UploadModel.Media.Init();// 图像上传模块 2014年2月19日

        //判断当前纸箱的个数，如果是0个，则进入添加逻辑页面，否则进入编辑逻辑页面 2013年12月10日16:19:24
        var countCTN = parseInt($("#hiddenCountCTN").val());
        if (countCTN > 0) {
            HMCTNModel.HMCTN.Init();
        } else {
            HMCTN_AddModel.HMCTN.Init();
        }

        var countDim = parseInt($("#hiddenCountDim").val());
        if (countDim > 0) {
            HMDimModel.HMDim.Init();
        } else {
            HMDim_AddModel.HMDim.Init();
        }

        var Media_Data_list = $.parseJSON($("#Media_Data_list").val());
        //BubbleInfoModel.Media.Bubble($("#hiddenCMSImgUrl").val(), Media_Data_list, function () {
        //    HMMediaModel.Media.RerenderMedia();
        //}, $('#CMSImagesForms'), true, function (_imageSelf) {
        //    //需要注意的是这里的_imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
        //    // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
        //    HMMediaModel.Media.SetPrimaryImage(_imageSelf);
        //});

        var opts = {
            cmsURL: $("#hiddenCMSImgUrl").val(),
            Media_Data_list: Media_Data_list,
            DeletedCallBack: function () {
                HMMediaModel.Media.RerenderMedia();
            },
            overlayElement: $('#CMSImagesForms'),
            IsNeedPrimaryImage: true,
            PrimaryImageCallBack: function (_imageSelf) {
                //需要注意的是这里的_imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
                // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
                HMMediaModel.Media.SetPrimaryImage(_imageSelf);
            }
        };
        BubbleInfoModel.Media.Bubble(opts);
    }); //end of document.ready
});