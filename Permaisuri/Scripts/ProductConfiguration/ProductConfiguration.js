
define(function (require, exports, module) {
    var $ = require('jquery');
    //var ui = require('jquery-ui');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/prograss');
    var fancybox = require('FancyBox/jquery.fancybox');
    var UIExtModel = require('Common/EasyUIExt');
    var chosen = require('PlugIn/Chosen/chosen.jquery.min');   
    var TooltipModel = require('./ProductTooltip');
    var RelatedProductsModels = require('./ProductConfigAction/RelatedProducts');
    var DuplicateProductModle = require('./ProductConfigAction/DuplicateProduct');
    var ImagesModel = require('./ProductConfigAction/ImageOperations');
    var SKUBaseModel = require('./ProductConfigAction/SKUBaseInfo');
    var CostModel = require('./ProductConfigAction/SKUCosting');
    var SKUMediaUploadModel = require('./ProductConfigAction/SKUMediaUpload');

    var CopyImgModel = require('./ProductConfigAction/CopyImagesFromChannel');

    var HMNUMModel = require('./ProductConfigAction/HMNUM');
    var UIExtModel = require('Common/EasyUIExt');
    var TagsModel = require('Common/OpenCMSTag');
    var BubbleInfoCSS = require('Media/BubbleInfo.css');
    var BubbleInfoModel = require('Media/BubbleInfo');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        //各种字段的提示信息
        TooltipModel.SKU.InitToolTip();
        //校验
        UIExtModel.Ext.ValidateListLocal();

        //Master Pack整除校验
        UIExtModel.Ext.ValidateExactDivided();



        $("#btnCopySKUImages").on("click", function () {
            CopyImgModel.CopyImage.OpenDialog();
        });

        //单击展示图片
        $(".fancybox").fancybox({
            tpl: {
                error: '<p class="fancybox-error">There is no picture for this item</p>',
            }
        });

        $($(".chosen-select option").get(0)).attr("disabled", "disabled");
        $($(".chosen-select option").get(6)).attr("disabled", "disabled");//禁止用户再重新选择回NewDuplicated来改变ProductPieces...
        $(".chosen-select").chosen({
            disable_search:true,
            no_results_text: "Oops, nothing found!",
            display_disabled_options: true,
            inherit_select_classes: true,
            disabled:true
        });

        $("#opVisibiliy").combobox({
            required: true,
            disabled: true
        });

        $('#SKU_QTY').numberbox({
            deltaX: -60,
            required: true,
            precision: 0
        });

        var brandW = $("#SubCategory").width();
        $('#cbBrand').combobox({
            width: brandW,
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbBrand','Brand_Name']"
        });

        var shipViaTypeW = $("#ProductName").width()+20;
        $('#ShipViaType').combobox({
            width:shipViaTypeW,
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#ShipViaType','ShipViaTypeName']"
        });

        

        //产品复制
        $("#btnDuplicate").on("click", function () {
            DuplicateProductModle.DuplicateProduct();
        });


        SKUBaseModel.SKU.Init();

        CostModel.SKUCosting.Init();

        RelatedProductsModels.RelatedProducts.Builting();

        SKUMediaUploadModel.SKUMedia.Init();

        ImagesModel.ImagesModel.MediaEvents();

        var SKUInfos = $.parseJSON($("#hiddenSKUInfo").val());
        HMNUMModel.HMNUM.Init(SKUInfos);


        //BubbleInfoModel.Media.Bubble($("#hiddenCMSImgUrl").val(), SKUInfos.channelMedias, function () {
        //    ImagesModel.ImagesModel.RerenderSKUMedia($("#hiddenSKUID").val());
        //}, $('#CMSImagesForms'), true, function (_imageSelf) {
        //    //需要注意的是这里的imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
        //    // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
        //    HMMediaModel.ImagesModel.SetPrimaryImage(_imageSelf);
        //});


        var opts = {
            cmsURL: $("#hiddenCMSImgUrl").val(),
            Media_Data_list: SKUInfos.channelMedias,
            IsNeedDeleteButton:false,
            overlayElement: $('#chanelImageDIV'),
            IsNeedPrimaryImage: true,
            UnattachedSKUButton: true,
            UnattachedCallBack: function (_MediaID) {
                ImagesModel.ImagesModel.UnattachSKUMedia(_MediaID);
            },
            PrimaryImageCallBack: function (_imageSelf) {
                //需要注意的是这里的_imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
                // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
                ImagesModel.ImagesModel.SetPrimaryImage(_imageSelf);
            }
        };
        BubbleInfoModel.Media.Bubble(opts);

    });// end of document.ready
});