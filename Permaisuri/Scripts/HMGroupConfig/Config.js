/// <summary>
/// HMNUMConfiguration模块的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var UIExtModel = require('Common/EasyUIExt');
    var CostModel = require('./HMCosting');
    var HMModel = require('./HMBaseInfo');
    var HMCTNModel = require('./HMCTN');
    var HMDimModel = require('./HMDim');
    var cmsTagsModel = require('Common/OpenCMSTag');
    var HMMediaModel = require('./HMMedia');
    var UploadModel = require('./HMMediaUpload');
    var ChildrenHMModel = require('./ChildrenHM');//新增ChildrenHM模块 2014年3月14日
    var TagsModel = require('Common/OpenCMSTag');

    var BubbleInfoCSS = require('Media/BubbleInfo.css');
    var BubbleInfoModel = require('Media/BubbleInfo');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        //整除校验
        UIExtModel.Ext.ValidateExactDivided();
        UIExtModel.Ext.ValidateListLocal();

        //设置字段提醒
        var tooltipModel = require('../HMConfig/HMConfigTooltip');
        tooltipModel.HMConfig.InitToolTip();

        $("#ShipViaType").combobox({
            width: $("#tooltipShipViaType").width() - ($("#tooltipShipViaType").width() * 0.01),
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#ShipViaType','ShipViaTypeName']"
        });

        $(window).resize(function () {
            var curShipVW = $("#ProductName").width() + 10;
            $('#ShipViaType').combobox('resize', curShipVW);
        });


        //设置Status的初始状态
       $('#Status').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) > -1;
            },
            validType: "ValidateListLocal['#Status','StatusName']"
        });

        $(".aChildrenHM").off("click").on("click", function () {
            var pID = $(this).attr("value");
            var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
            TagsParam.URL = "../HMConfig/Index?ProductID=" + pID;
            TagsParam.name = "HMCofig";
            TagsParam.title = "HMCofig";
            TagsParam.iframeID = "frmTag";
            TagsParam.reload = true;
            cmsTagsModel.CMSTags.OpenTags(TagsParam);
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

        //$("#ChildrenHMForm input not:[name='ChildrenQTY']").prop("disabled", true);

        HMModel.HM.Init();//基础信息模块
        CostModel.HMCosting.Init();
        HMCTNModel.HMCTN.Init();
        HMDimModel.HMDim.Init();

        HMMediaModel.Media.Init(); //图像事情初始化模块
        HMMediaModel.Media.GetImagesFromOtherSystem();
        UploadModel.Media.Init();//图像上传模块 2014年2月21日


        ChildrenHMModel.ChildrenHM.Init();//2014年3月14日

        var Media_Data_list = $.parseJSON($("#Media_Data_list").val())
        //BubbleInfoModel.Media.Bubble($("#hiddenCMSImgUrl").val(), Media_Data_list, function () {
        //    HMMediaModel.Media.RerenderMedia();
        //}, $('#CMSImagesForms'));

        var opts = {
            cmsURL: $("#hiddenCMSImgUrl").val(),
            Media_Data_list: Media_Data_list,
            DeletedCallBack: function () {
                HMMediaModel.Media.RerenderMedia();
            },
            overlayElement: $('#CMSImagesForms')
        };
        BubbleInfoModel.Media.Bubble(opts);



        //判断当前纸箱的个数，如果是0个，则进入添加逻辑页面，否则进入编辑逻辑页面 2013年12月10日16:19:24
        var countCTN = parseInt($("#hiddenCountCTN").val());
        if (countCTN > 0) {
            HMCTNModel.HMCTN.Init();
        } else {
            var HMCTN_AddModel = require('./HMCTN_Add');
            HMCTN_AddModel.HMCTN.Init();
        }

        var countDim = parseInt($("#hiddenCountDim").val());
        if (countDim > 0) {
            HMDimModel.HMDim.Init();
        } else {
            var HMDim_AddModel = require('./HMDim_Add');
            HMDim_AddModel.HMDim.Init();
        }

    }); //end of document.ready
});