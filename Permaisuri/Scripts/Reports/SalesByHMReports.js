/// <summary>
/// Same columns as Sales by Product but SKU sales results rolled up together under common HM product identifier
/// Author: Lee Date:2013年10月14日10:55:43
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');
    var fancybox = require('FancyBox/jquery.fancybox');

    exports.RenderReport = {
        Render: function (data, callBack) {
            var sHtml = '';
            sHtml += '<table width="100%" border="0" cellpadding="0" cellspacing="0" class="reporttable">';
            sHtml += '<thead> <tr> <th width="20%" class="dataTableHeader" align="left" orderBy="1">HMNUM<span></span> </th>';
            sHtml += '<th width="35%" class="dataTableHeader" align="center" orderBy="2">HMNUM Name<span></span></th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="3">Sales Totals<span></span></th>';
            sHtml += '<th width="10%" class="dataTableHeader" align="center" orderBy="4">Units Sold<span></span> </th>';
            sHtml += '<th class="dataTableHeader" align="center" orderBy="5">Avg. Sale Amount <span></span> </th></tr> </thead>';
            sHtml += '<tbody>';
            $.each(data["ReportList"], function () {
                sHtml += '<tr>';
                sHtml += '<td valign="top"><span>' + this.HMNUM + '</span></td>';
                sHtml += '<td valign="top"><span>' + this.HMName + '</span></td>';
                sHtml += '<td valign="top" align="center">' + this.SalesTotal + '</td>';
                sHtml += '<td valign="top" align="center">' + this.UnitsSold + '</td>';
                sHtml += '<td valign="top" align="center">' + this.AvgAmont + '</td>';
                sHtml += '</tr>';
            })

            sHtml += '<tr id="totalSatis">';
            sHtml += '<td valign="top" ><span> Total </span></td>';
            sHtml += '<td valign="top" align="center"><span> </span></td>';
            sHtml += '<td valign="top" align="center"><span>' + data.SumSalesTotal + '</span></td>';
            sHtml += '<td valign="top" align="center"><span>' + data.SumUnitsSold + '</span></td>';
            sHtml += '<td valign="top" align="center"><span>' + data.SumAvgAmont + '</span></td>';
            sHtml += '</tr>';

            sHtml += '</tbody></table>';
            $(".salesblok").html(sHtml);
            $(".salesblok table tr:odd").addClass("odd_row");
            $(".salesblok table thead th").css("cursor", "pointer");

            $(".salesblok table thead th").on("click", function () {
                var orderBy = $(this).attr("orderBy");
                orderInfo["OrderBy"] = orderBy;//记录页面哪一列点击了排序
                orderInfo["OrderType"] = orderInfo["OrderType"] == "0" ? "1" : "0";
                callBack.apply(this);
            })


            //新增列的排序指示器 2013年10月29日15:19:24 Lee
            var orderColNum = parseInt(orderInfo["OrderBy"]);
            var typeCol = parseInt(orderInfo["OrderType"]);
            var curCol = $($(".salesblok table thead th").get(orderColNum - 1)).find("span");// 找到当前排序列的span元素
            curCol.addClass(typeCol == 0 ? "sort-asc" : "sort-desc");//给该元素加上逆序/顺序的图标


            //Next page
            /*$(".arrR").on("click", function () {
                var curPage = parseInt($.trim($("#aRow").text())) + 1;
                var totals = parseInt($.trim($("#hiddenTotal").val()));
                var maxRecords = curPage * parseInt($("#opPageSize").val());
                if (maxRecords > totals) {
                    return false;
                }
                debugger; //经过测试发现在Render之后绑定click会造成多次绑定运行，翻到第二页就执行了2次，第三页就执行4次，以此类推翻倍执行AJAX。。。。2013年9月6日15:48:12
                defaultData.page = curPage;
                callBack.apply(this, [defaultData, defaultData.orderBy, orderTypes]);
            });*/

            sHtml = null;
        }
    }
})