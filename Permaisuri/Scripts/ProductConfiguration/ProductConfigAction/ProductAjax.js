define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    /// <summary>
    /// 更新产品的信息到服务器，产品Status下拉单和其他input类型的改变都会触发该事件
    ///  Author:Lee,Date:2013年10月24日15:12:17
    ///     StatusCode:
    ///     New = 1,
    ///     MediaCreation = 2,
    ///     MarketingDevelopment = 3,
    ///     Complete = 4,
    ///     Active = 5,
    ///     Discontinued = 6,
    ///     NewDuplicated = 7
    /// </summary>
    /// <param name="StatusCode">产品的Status</param>
    /// <returns></returns>
    //var SKUElement = [$("#UPC"), $("#ProductDesc"), $("#Specifications"), $("#Keywords"), $("#opVisibiliy"), $("#URL")];
    exports.Ajaxs = {
        updatedBasiceInfo: function () {
            var fvalidate = $("#ProductConfigurationForm").form('validate');
            if (!fvalidate) {
                return;
            }
            prograss.show();
            $.ajax({
                url: "./UpdatedProduct",
                type: "POST",
                dataType: "json",
                data: {
                    SKUID: $("#hiddenSKUID").val(),
                    UPC: $.trim($("#UPC").val()),
                    ProductDesc: $.trim($("#ProductDesc").val()),
                    Specifications: $.trim($("#Specifications").val()),
                    Keywords: $.trim($("#Keywords").val()),
                    Visibility: $.trim($("#opVisibiliy").val()),
                    URL: $.trim($("#URL").val()),
                    BrandID: $('#cbBrand').combobox('getValue'),
                    StatusID: $("#opStatus").val(),
                    SKU_QTY: $.trim($("#SKU_QTY").val()),
                    Visibiliy: $("#opVisibiliy").combobox('getValue'),
                    Material: $('#Material').combobox('getText'),
                    Colour: $("#Colour").combobox('getText'),
                    Category: $("#Category").combobox('getText'),
                    SubCategory: $("#SubCategory").combobox('getText'),
                },
                success: function (retData) {
                    prograss.hide();
                    if (retData.Status === 1) {
                        //$($("#opStatus option").get(StatusCode)).attr("selected", true);
                        //$(".chosen-select").trigger("chosen:updated");
                        var curStatus = parseInt($("#hiddenStatusID").val());
                        if (curStatus == 7 || curStatus == 1) {
                            $("#hiddenStatusID").val("3");
                        }
                    } else {
                        $.messager.alert('UpdatedProduct', retData.Data, 'error');
                    }
                },
                error: function () {
                    prograss.hide();
                    $.messager.alert('UpdatedProduct', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }// end of updatedBasiceInfo
    }
});