define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');
    var ChannelReportsModel = require('./SalesByChannelReports');
   
    var LowInventoryReportsModel = require('./LowInventoryReports');
    var ProductDevelopmentReportsModel = require('./ProductDevelopmentReports');

    exports.GetReportsModel = {
        /// <summary>
        /// 根据查询条件动态展示不同数据/不同的报表类型 2013-9-6
        /// </summary>
        /// <param name="reqParams">前端报表查询参数，用于后端查询类的实例化</param>
        /// <returns></returns>
        GetReportsList: function (reqParams) {
            var fvalidate = $("#reportFilterForm").form('validate');
            if (!fvalidate) {
                return;
            }
            var params = null;
            if (typeof (reqParams) != "undefined") {
                params = reqParams;
            } else {
                params = {
                    page: $.trim($("#aRow").text()),
                    rows: parseInt($("#opPageSize").val()),
                    ReportType: $("#cbReportType").combobox('getValue'),
                    Brand: $("#cbBrand").combobox('getValue'),
                    Channel: $("#cbChannel").combobox('getValue'),
                    startTime: $("#startTime").datebox("getValue"),
                    endTime: $("#endTime").datebox("getValue"),
                    QueueStatus: $("#QueueStatus").combobox('getValue'),
                    AffectedSKU: $("#AffectedSKU").val(),
                    SKUOrder: $("#SKUOrder").val(),
                    OrderBy: orderInfo["OrderBy"],
                    OrderType: orderInfo["OrderType"]
                }
            };
            prograss.show();
            $.ajax({
                url: "./GetReportsList",
                type: "POST",
                dataType: "json",
                data: params,
                success: function (retData) {
                    prograss.hide();
                    if (retData.Status === 1) {
                        var reportType = parseInt($("#cbReportType").combobox('getValue'));
                        switch (reportType) {
                            case 1://Sales by Channel
                                ChannelReportsModel.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            case 2://Sales by Product
                                var ProductReportsModel = require('./SalesByProductReports');
                                ProductReportsModel.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            case 3://Product Development Report
                                ProductDevelopmentReportsModel.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            case 4://Low Inventory Report
                                LowInventoryReportsModel.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            case 5://Sales by HM
                                var SalesByHMModel = require('./SalesByHMReports');
                                SalesByHMModel.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            case 6://Low Inventory Report by SKU
                                var LIBSRModel = require('./LowInventoryBySKUReports');
                                LIBSRModel.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            case 7://Cost and Margin by Product
                                var CMP = require('./CostAndMarginByProductReports');
                                CMP.RenderReport.Render(retData.Data, exports.GetReportsModel.GetReportsList);
                                break;
                            default: break;
                        }
                        var ipage =parseInt($("#aRow").text());
                        var irows = parseInt($("#opPageSize").val());
                        var starIndex = (ipage - 1) * irows + 1;
                        var endIndex = ipage * irows;
                        endIndex = parseInt(retData.Data["Count"]) < endIndex ? parseInt(retData.Data["Count"]) : endIndex;
                        $("#hiddenTotal").val(retData.Data["Count"]);
                        $(".resulthld").html('<p>Results ' + starIndex + ' - ' + endIndex + ' of ' + retData.Data["Count"] + '</p>');

                    } else {
                        $.messager.alert('GetReportsList', retData.Data, 'error');
                    }
                },
                error: function () {
                    prograss.hide();
                    $.messager.alert('GetReportsList', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }// end func of Get ReportsList
    }// end of model
})