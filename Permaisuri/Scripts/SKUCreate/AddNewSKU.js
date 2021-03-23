/// <summary>
/// HM#组合产品的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    exports.Ajax = {
        ///新增HMGroup基本信息操作，callback函数操作的是新增 基础HM#（非组合产品）的按钮等操作
        AddNewSKU: function (callback) {
            baseSelf = this;
            //校验合法性
            var fvalidate = $("#NewSKUBaseForm").form('validate');
            if (!fvalidate) {
                return;
            }
            progress.show();

            var dtd = $.Deferred();
            $.when(baseSelf.CheckMCC(dtd)).done(function () {
                $.ajax({
                    url: "./AddNewSKU",
                    type: "POST",
                    cache: false,
                    dataType: "json",
                    data: {
                        ProductID: $("#ProductID").val(),
                        R_QTY: $("#comboQTY").combobox('getValue'),
                        URL: $("#URL").val(),
                        ProductName: $.trim($("#ProductName").val()),
                        SKU: $.trim($("#SKU").val()),
                        UPC: $.trim($("#UPC").val()),
                        ProductDesc: $.trim($("#ProductDesc").val()),
                        Specifications: $.trim($("#Specifications").val()),
                        SKU_QTY: $.trim($("#SKU_QTY").val()),
                        Keywords: $.trim($("#Keywords").val()),
                        StatusID: $.trim($("#opStatus").val()),
                        Visibiliy: $("#opVisibiliy").combobox('getValue'),
                        BrandID: $('#cbBrand').combobox('getValue'),
                        ChannelID: $('#cbChannel').combobox('getValue'),
                        Material: $('#Material').val(),
                        Colour: $("#Colour").val(),
                        Category: $("#Category").val(),
                        SubCategory: $("#SubCategory").val(),
                        ShipViaTypeID: $("#ShipViaType").combobox('getValue')
                    }
                }).done(function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        $("#hiddenSKUID").val(retData.Data);
                        //回调函数
                        $("#aSKUAdd").hide();
                        $("#autoHMNUM").attr('readonly', 'readonly');//禁用掉HMNUM

                        var cmsTagsModel = require('Common/OpenCMSTag');
                        var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                        var SKUID = $("#hiddenSKUID").val();
                        if (parseInt(SKUID) < 1) {
                            $.messager.alert('Go to configuration page erro', "SKU basic infomation updated error ...", 'error');
                            return;
                        }

                        TagsParam.URL = "../ProductConfiguration/ProductConfiguration?SKUID=" + SKUID;
                        TagsParam.name = "ProductConfiguration";
                        TagsParam.title = "ProductConfiguration";
                        TagsParam.iframeID = "frmTag";//--新建而不是覆盖...
                        TagsParam.reload = true;
                        cmsTagsModel.CMSTags.OpenTags(TagsParam);

                        //关闭当前页面
                        window.parent.jQuery('#centerTabs').tabs('close', 'Add New SKU');
                    } else {
                        $.messager.alert('AddProduct', retData.Data, 'error');
                    }
                }).fail(function () {
                    progress.hide();
                    $.messager.alert('AddProduct', 'NetWork Error,Please contact administrator。', 'error');
                });
            }).fail(function () {
                progress.hide();
                $.messager.alert('Colour,Material,Category', 'Check Material,Colour Or Category Failed!', 'error');
            });

        },// end of BaseInfoAdd Func

        //CheckMaterial,Colour,Category的有效性
        CheckMCC: function (dtd) {
            $.ajax({
                url: "../ProductConfiguration/CheckMCC",
                type: "POST",
                dataType: "json",
                data: {
                    Material: $('#Material').val(),
                    Colour: $("#Colour").val(),
                    Category: $("#Category").val(),
                    SubCategory: $("#SubCategory").val()
                }
            }).done(function (retData) {
                //progress.hide();
                if (retData.Status === 1) {
                    dtd.resolve(); // 改变Deferred对象的执行状态 
                } else {
                    $.messager.confirm('Confirm', 'the value of <b> ' + retData.Data + ' filed </b> does not exist,do you want to create automatically?', function (r) {
                        if (r) {
                            dtd.resolve();
                        } else {
                            dtd.reject();
                        }
                    });
                    // dtd.reject();
                }
            }).fail(function () {
                dtd.reject(); // 改变Deferred对象的执行状态
                //progress.hide();
                $.messager.alert('editSKUBasicInfo', 'Failed to edit,please contact administrator', 'error');
            });
            return dtd;
        }
    }

});