/// <reference path="../jquery-1.7.1-vsdoc.js" />
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var highstock = require('highstock');
    var progress = require('Common/Prograss');
    var chosenCSS = require('PlugIn/Chosen/chosen.min.css');//dynamically loading css
    var chosen = require('PlugIn/Chosen/chosen.jquery.min');
    var RenderModel = require('./ReRender');
    var TagsModel = require('Common/OpenCMSTag');

    $(document).ready(function () {
        TagsModel.CMSTags.BackSpace();
        $(".multipleSelect").chosen({
            placeholder_text_multiple: "Click here to select Channels. 'Alt + Right click' to select more than one.",
            display_selected_options: false
        });

        $("#sTime").datebox({ formatter: formatDate });
        $("#eTime").datebox({ formatter: formatDate });

        function formatDate(date) {
            return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
        }


        $("#queryEcomOrder").off('click').on('click', function () {
            RenderModel.ReRender.QueryPOOrder();
        });


        var seriesOptions = [],
		yAxisOptions = [],
		seriesCounter = 0,
		//merchantIDs = ['Costco', 'AmazonDropShip', 'CSN'],
		colors = Highcharts.getOptions().colors;
        progress.show();
        $.ajax({
            url: "./GetPOOrders",
            type: "POST",   
            dataType: "json",
            traditional:true,
            data: {
                ChannelList: $(".multipleSelect").val(),
                sTime: $("#sTime").datebox('getValue'),
                eTime: $("#eTime").datebox('getValue')
            },
            success: function (retData) {
                progress.hide();
                if (retData.Status === 1) {

                    //processing highChatrs data.
                    //根据选中的value获取出对于的Name
                    var arrName = [];
                    $.each($(".multipleSelect option"), function () {
                        var oValue = $(this).val();
                        var oName = $(this).text();
                        $.each($(".multipleSelect").val(), function (i,v) {
                            if (v === oValue)//如果当前选中的渠道的Value和渠道集合里面的Value相同，则push它的名称
                            {
                                arrName.push(oName);
                            }
                        })
                    });
                    //alert(JSON.stringify(arrName));["Costco","AmazonDropShip","OverStock"]

                  //这一步主要过滤掉时间的格式问题
                  var filterData =   $.map(retData["Data"], function (item) {
                      return {
                          name: item.name,
                          x: parseFloat(item.x.replace("/Date(", "").replace(")/", ""), 10),
                          y: parseInt(item.y)
                      }
                    })
                   
                    $.each(arrName, function (i, v) {
                        var curChannel = [];
                        /*对当前的数组按照名称做过滤，比如把所有属于Costco的数据都统统都筛选出来*/
                        curChannel = $.grep(filterData, function (val, key) {
                            return val["name"] == v; //比如：v=="Costco"
                        });

                        var arrChannelData = [];
                        $.each(curChannel, function () {
                            arrChannelData.push([this.x, this.y]);
                        });
                        seriesOptions[i] = {//i =0,1,2,3....
                            name: v,//Costco
                            data: arrChannelData //arrCostco.push([this.x, this.y]);
                        };
                    });

                    /*旧的方式 2013年12月18日15:59:29
                    var Costco = $.grep(retData.Data, function (val, key) {
                        val.x = parseInt(val.x.replace("/Date(", "").replace(")/", ""), 10);
                        return val["name"] == "Costco"
                    });
                    var arrCostco = [];
                    $.each(Costco, function () {
                       //var parseDate =   parseInt(this.x.replace("/Date(", "").replace(")/", ""), 10);
                        arrCostco.push([this.x, this.y]);
                    });
                    seriesOptions[0] = {
                        name: "Costco",
                        data: arrCostco
                        //data: retData.Data
                    };*/

                    $('#container').highcharts('StockChart', {
                        chart: {
                            type: 'spline' //htt p://api.highcharts.com/highcharts#plotOptions.spline
                        },
                        credits: { enabled: false },
                        rangeSelector: {
                            selected: 4
                        },
                        xAxis: {
                            type: 'datetime'
                            //,
                            //min: Date.UTC(2005, 12, 1)
                        },

                        yAxis: {
                            title: {
                                text: 'Orders'
                            },
                            min: 0
                        },
                        /*
                         To set general options for all series in the chart, use plotOptions.series.
                         To set general options for a specific chart type, each chart type has its own collection of plotOptions.
                        */
                        plotOptions: {
                            series: {
                                //compare: 'value', 注意：这个compare一旦加上，y轴将以百分比形式展示，比如一个订单这个月是一万，
                                //但是相比开头1000，增长了1000%，则Y轴显示的是1000，而不是一万....如果加入百分比，需要考虑的是加入负y轴，2013年12月18日17:55:20，
                                //cropThreshold: 300,
                                cursor: "pointer",
                                tooltip: {
                                    valueDecimals: 0
                                }
                            }
                        },
                        title: {
                            text: 'Noble House Sale Performance'
                        },
                        tooltip: {
                             //pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.change}%)<br/>',
                             valueDecimals: 2
                        },

                        series: seriesOptions//The series object is an array, meaning it can contain several series.
                        //htt p://www.highcharts.com/docs/chart-concepts/series
                        //series: retData["Data"]
                    });

                    //Set Key Metrics
                    var KeyMetricsModel = require('./KeyMetrics');
                    KeyMetricsModel.Render.List(retData);


                    //初始化低库存面板
                    var InvAlterModel = require('./InventoryAlerts');
                    InvAlterModel.Render.List(retData);

                    //Product Development Queue
                    var ProductDevModel = require('./ProductDevQueue');
                    ProductDevModel.Render.List(retData);


                } else {
                    $.messager.alert('Dashboard-SalePerformance', retData.Data, 'error');
                }
            },
            error: function () {
                progress.hide();
                $.messager.alert('Dashboard-SalePerformance', 'NetWork error when initializing Dahsboard', 'error');
            }
        });

    });// end of document.ready
});