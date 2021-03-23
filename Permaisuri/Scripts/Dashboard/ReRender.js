/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var highstock = require('highstock');

    var seriesOptions = [];
    /// <summary>
    /// 这个方法是用于Dashboard页面重新查询订单信息（点击页面Query按钮）的时候用的
    /// CreateDate:2013年12月23日17:12:25
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.ReRender = {
        QueryPOOrder: function () {
            var RenderSelf = this;
            progress.show();
            $.ajax({
                url: "./QueryPOOrder",
                type: "POST",
                dataType: "json",
                traditional: true,
                data: {
                    ChannelList: $(".multipleSelect").val(),
                    sTime: $("#sTime").datebox('getValue'),
                    eTime: $("#eTime").datebox('getValue')
                }
            }).done(function (retData) {
                progress.hide();
                if (retData.Status === 1) {
                    RenderSelf.HightChartRender(retData["Data"]);
                    RenderSelf.KeyMerticsReder(retData["Metrics"]);
                } else {
                    $.messager.alert('Dashboard-QueryPOOrder', retData.Data, 'error');
                    return false;
                }

            }).fail(function () {
                progress.hide();
                $.messager.alert('Dashboard-QueryPOOrder', 'Failed to query data', 'error');
                return false;
            });
        },


        HightChartRender: function (datas) {
            //processing highChatrs data.
            //根据选中的value获取出对于的Name
            var arrName = [];
            $.each($(".multipleSelect option"), function () {
                var oValue = $(this).val();
                var oName = $(this).text();
                $.each($(".multipleSelect").val(), function (i, v) {
                    if (v === oValue)//如果当前选中的渠道的Value和渠道集合里面的Value相同，则push它的名称
                    {
                        arrName.push(oName);
                    }
                })
            });
            //alert(JSON.stringify(arrName));["Costco","AmazonDropShip","OverStock"]

            //这一步主要过滤掉时间的格式问题
            var filterData = $.map(datas, function (item) {
                return {
                    name: item.name,
                    x: parseFloat(item.x.replace("/Date(", "").replace(")/", ""), 10),
                    y: parseInt(item.y)
                }
            })

            seriesOptions = [];
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
        },

        KeyMerticsReder: function (metricsData) {
            $("div[name='AvgOrdersDiv'] h2").text(metricsData["AvgOrders"]);
            $("div[name='AvgAmtDiv'] h2").text(metricsData["AvgAmt"]);
        }
    }
});