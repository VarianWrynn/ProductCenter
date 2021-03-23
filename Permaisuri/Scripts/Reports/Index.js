define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');
    var salesByChannelReportsModel = require('./SalesByChannelReports');
    var ajaxModel = require('./GetReportsList');

    var getUrlVars = require('Common/getUrlVars');

    var tagsModel = require('Common/OpenCMSTag');

    //window全局变量，简单，粗暴，有效！一个地方改动，所有地方都生效...
    window.orderInfo = {
        OrderBy: 1,//默认按照第一列排序
        OrderType:0//默认按照顺序（asc)排序
    };

    $(document).ready(function () {
        tagsModel.CMSTags.BackSpace();
        $("#startTime").datebox({ formatter: formatDate });
        $("#endTime").datebox({ formatter: formatDate });
        function formatDate(date) {
            return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
        }
        var newDate = new Date();
        $("#startTime").datebox("setValue", formatDate(newDate));
        $("#endTime").datebox("setValue", formatDate(newDate));
        //prograss.show();
        $.ajax({
            url: "./InitReports",//when firt time open report page, init Channel and Brand fields 
            type: "POST",
            dataType: "json",
            success: function (retData) {
                //prograss.hide();
                if (retData.Status === 1) {
                    
                    var arrChannels = retData.Data["Channels"]
                    if (!retData.Data["IsChannelControl"])//如果表示需要控制Channel显示，则不能加ALL ,否则加All
                    {
                        var allItemChannel = { "ChannelID": 0, "ChannelName": "ALL" };
                        arrChannels.unshift(allItemChannel);
                    }
                   
                    $('#cbChannel').combobox({
                        data: arrChannels,
                        valueField: 'ChannelID',
                        textField: 'ChannelName'
                    });

                    if (!retData.Data["IsChannelControl"]) {
                        $('#cbChannel').combobox('select', 0);
                    } else {
                        //很好，当用户开启渠道关联却未选择关联信息的时候，自动查询讲被校验通不过 2013年11月4日12:38:17
                        $('#cbChannel').combobox('select', arrChannels[0]["ChannelID"]);
                    }

                    var arrBrands = retData.Data["Brands"];
                    var allItemBrand = { "Brand_Id": 0, "Brand_Name": "ALL" };
                    arrBrands.unshift(allItemBrand);
                    $('#cbBrand').combobox({
                        data: arrBrands,
                        valueField: 'Brand_Id',
                        textField: 'Brand_Name'
                    });
                    $('#cbBrand').combobox('select', 0);


                    var arrQueueStatus = retData.Data["QueueStatus"];
                    //从服务器返回的数组里面删除掉Active这个项
                    $.each(arrQueueStatus, function () {
                        var self = this;
                        if (this.StatusName == "Active")
                        {
                            arrQueueStatus.splice($.inArray(self, arrQueueStatus), 1);
                        }
                    });

                    arrQueueStatus.unshift({ "StatusName": "ALL", "StatusID": 0 });
                    $('#QueueStatus').combobox({
                        data: arrQueueStatus,
                        valueField: 'StatusID',
                        textField: 'StatusName'
                    });
                    $('#QueueStatus').combobox('select', 0);


                    var RType =parseInt($("#RType").val());
                    switch (RType)
                    {
                        case 1://Sales by Channel
                            $("#cbReportType").combobox('setValue', 1);
                            orderInfo["OrderBy"] = 3;//order by Units Sold
                            $("#reportSearch").click();
                            break;
                        case 2://Sales by Product
                            $("#cbReportType").combobox('setValue', 2);
                            orderInfo["OrderBy"] = 4;//order by Units Sold，注意，图像不做排序，需要忽略这一列开始计算
                            $("#reportSearch").click();
                            break;
                        case 3://Product Development Report
                            var reqStats = $.getUrlVar('QueueStatus');//Add by Lee,For SKU batch duplicated
                            if (typeof (reqStats) != "undefined") {
                                if (parseInt(reqStats) == 7)
                                {
                                    $('#QueueStatus').combobox('select', 7)
                                }
                            }

                            $("#cbReportType").combobox('setValue', 3);
                            orderInfo["OrderBy"] = 3;//order by Channel
                            $("#reportSearch").click();
                            break;
                        case 4://Low Inventory Report
                            $("#cbReportType").combobox('setValue', 4);
                            orderInfo["OrderBy"] = 2;//order by HM# 
                            $("#reportSearch").click();
                            break;
                        case 5://Sales by HM#
                            $("#cbReportType").combobox('setValue', 5);
                            orderInfo["OrderBy"] = 4;//order by Unites Sold
                            $("#reportSearch").click();
                            break;
                        case 6://Low Inventory Report By SKU
                            $("#cbReportType").combobox('setValue', 6);
                            orderInfo["OrderBy"] = 3;//Order by inventory
                            $("#reportSearch").click();
                            break;

                        case 7://Sales by Product
                            $("#cbReportType").combobox('setValue', 7);
                            orderInfo["OrderBy"] = 1;//order by SKUOrder，注意，图像不做排序，需要忽略这一列开始计算
                            $("#reportSearch").click();
                            break;

                        default:
                            $("#cbReportType").combobox('setValue', 1);
                            $("#reportSearch").click();
                            break;
                    }

                } else {
                    $.messager.alert('InitReports', retData.Data, 'error');
                }
            },
            error: function () {
                prograss.hide();
                $.messager.alert('InitReports', 'NetWork Error,Please contact administrator。', 'error');
            }
        }); // end of ajax func

        //Search button
        var reqParams = {};
        $("#reportSearch").on("click", function () {
            //orderInfo.OrderBy = 1;
            //orderInfo.OrderType = 0;
            var startTime = $("#startTime").datebox("getValue");
            var endTime = $("#endTime").datebox("getValue");
            reqParams = {
                page: 1,
                rows: parseInt($("#opPageSize").val()),
                ReportType: $("#cbReportType").combobox('getValue'),
                Brand: $("#cbBrand").combobox('getValue'),
                Channel: $("#cbChannel").combobox('getValue'),
                startTime: startTime,
                endTime: endTime,
                QueueStatus: $("#QueueStatus").combobox('getValue'),
                AffectedSKU: $("#AffectedSKU").val(),
                SKUOrder: $("#SKUOrder").val(),
                HMNUM: $("#HMNUM").val(),
                OrderBy: orderInfo["OrderBy"],
                OrderType: orderInfo["OrderType"]
            }
            ajaxModel.GetReportsModel.GetReportsList(reqParams);
            $("#aRow").text("1");//如果是用户点击Search按钮，则默认从第一页开始查询，并且重置排序规则
            //var rhHtml = '';
            //rhHtml += '<h3><strong>Report:</strong> Sales Totals</h3>';
            //rhHtml += '<h3><strong>From:</strong> ' + startTime + '</h3>';
            //rhHtml += '<h3><strong>To:</strong> ' + endTime + '</h3>';
            $(".reasulthaed h3").remove();
            //$("#aPrintBtn").before(rhHtml);
            return false;
        });

        //Previous page
        $(".arrL").on("click", function () {
            var curPage = parseInt($.trim($("#aRow").text())) - 1;
            if (curPage < 1) {
                return false;
            }
            $("#aRow").text(curPage);
            ajaxModel.GetReportsModel.GetReportsList();
        });

        //Next page
        $(".arrR").on("click", function () {
            var curPage = parseInt($.trim($("#aRow").text())) + 1; 
            var totals = parseInt($.trim($("#hiddenTotal").val()));
            var maxRecords = (curPage-1) * parseInt($("#opPageSize").val());
            //debugger;
            if (maxRecords > totals) {
                return false;
            }
            $("#aRow").text(curPage);
            ajaxModel.GetReportsModel.GetReportsList();
        });

        //dropdownlist 
        $("#opPageSize").change(function () {
            ajaxModel.GetReportsModel.GetReportsList();
        });

        $("#cbReportType").combobox({
            onChange: function (newValue, oldValue) {
                var choseValue = parseInt(newValue);
                switch (choseValue) {
                    case 1://Sales by Channel 注意，如果是第一次进来，默认选择的是 Sales by channel，无法触发onChange事件，需要对这个做单独处理 2014年1月20日11:39:54
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().show();
                        tdFrom.show();

                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().hide();
                        tdASKU.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().hide();
                        tdHM.hide();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().hide();
                        tdQueueStatus.hide();


                        orderInfo["OrderBy"] = 3;//order by Units Sold
                       // $("#aExport").show();
                        break
                    case 2://Sales by Product
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().show();
                        tdFrom.show();

                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().hide();
                        tdASKU.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().show();
                        tdSKU.show();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().hide();
                        tdHM.hide();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().hide();
                        tdQueueStatus.hide();

                        orderInfo["OrderBy"] = 4;//order by Units Sold

                        //$("#aExport").show();
                        break
                    case 3://Product Development Report
                        /*隐藏时间、受影响SKU的查询，显示产品生命周期状态查询*/
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().hide();
                        tdFrom.hide();

                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().hide();
                        tdASKU.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().hide();
                        tdHM.hide();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().show();
                        tdQueueStatus.show();
                        orderInfo["OrderBy"] = 3;//order by Channel
                       // $("#aExport").hide();
                        break
                    case 4://Low Inventory Report
                        /*隐藏时间、产品生命周期的查询，显示受影响SKU查询*/
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().hide();
                        tdFrom.hide();

                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().show();
                        tdASKU.show();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().hide();
                        tdHM.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().hide();
                        tdQueueStatus.hide();

                        orderInfo["OrderBy"] = 2;//order by HM# 
                        //$("#aExport").show();
                        break;
                    case 5://Sales by HM#

                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().show();
                        tdFrom.show();

                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().hide();
                        tdASKU.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().show();
                        tdHM.show();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().hide();
                        tdQueueStatus.hide();

                        orderInfo["OrderBy"] = 4;//order by Units Sold
                        break;

                    case 6://Low Inventory Report By SKU
                        /*隐藏时间、产品生命周期的查询，隐藏受影响SKU查询*/
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().hide();
                        tdFrom.hide();


                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().hide();
                        tdASKU.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().hide();
                        tdHM.hide();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().hide();
                        tdQueueStatus.hide();

                        orderInfo["OrderBy"] = 3;//Order by inventory
                        //$("#aExport").show();
                        break;

                    case 7://Cost and Margin by Product

                        /*隐藏时间*/
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().hide();
                        tdFrom.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdHM = $("#HMNUM").parent();
                        tdHM.prev().hide();
                        tdHM.hide();

                        orderInfo["OrderBy"] = 1;//Order by SKUOrder
                       // $("#aExport").show();
                        break;
                    default:
                        var tdFrom = $("#startTime").parent();
                        tdFrom.prev().show();
                        tdFrom.show();

                        var tdASKU = $("#AffectedSKU").parent();
                        tdASKU.prev().hide();
                        tdASKU.hide();

                        var tdSKU = $("#SKUOrder").parent();
                        tdSKU.prev().hide();
                        tdSKU.hide();

                        var tdQueueStatus = $("#QueueStatus").parent();
                        tdQueueStatus.prev().hide();
                        tdQueueStatus.hide();

                        //$("#aExport").hide();
                        break;
                }
            }
        });


        $("#aPrintBtn").on("click", function () {
           // alert("Print");
        });

        $("#aExport").on("click", function () {
            var params = "?ReportType=" + $("#cbReportType").combobox('getValue');
            params += "&Brand=" + $("#cbBrand").combobox('getValue');
            params += "&Channel=" + $("#cbChannel").combobox('getValue');
            params += "&startTime=" + $("#startTime").datebox("getValue");
            params += "&endTime=" + $("#endTime").datebox("getValue");
            params += "&QueueStatus=" + $("#QueueStatus").combobox('getValue');
            params += "&AffectedSKU=" + $("#AffectedSKU").val();
            params += "&SKUOrder=" + $("#SKUOrder").val();
            params += "&HMNUM=" + $("#HMNUM").val();
            params += "&OrderBy=" + orderInfo["OrderBy"];
            params += "&OrderType=" + orderInfo["OrderType"];
            var url = "./ExportToExcel" + params;
            window.location.href = url;

            //$.ajax({
            //    url: "./ExportToExcel",
            //    type: "GET",
            //    data: reqParams,
            //    contentType: "application/ms-txt; charset=utf-8",
            //    dataType: "html"
            //}).done(
            //function (data) {
            //    //window.location.href = data;
            //    alert("aa");
            //}

            //).fail(
            //    function ()
            //    {
            //        alert("a");
            //    }
            //);
           
        });

    });
})