/// <summary>
/// 说明:报表的翻页和下拉单事件都是在各自报报表的JS页面里面，在成功Render完之后再绑定的，为什么不提取到index页面或者做一个公共页面呢？
///      原因是因为各个页面的列不同，orderby 和 orderType也不相同，即使提取到公共页面用污染全局的window.变量名也不一定能区别出来，同时还要
///      对每种类型的报表进行第一次渲染和非第一次渲染的判断，导致排序逻辑提出取来会十分复杂，遂放入各个报表JS页面各自处理。
///      2013年9月6日11:58:04  王力 以上的注释作废，找到解决方案，把私有变量 orderTypes 暴露出去 2013年9月6日15:59:41
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var prograss = require('Common/Prograss');

    /// <summary>
    /// Dynamic render Report.
    /// 根据传递进来的数据动态展示报表 2013-09-04.
    /// </summary>
    /// <param name="data">remote date from Server-Side</param>
    /// <returns></returns>
    exports.RenderReport = {
        //用来存储各个列排序的情况（asc/desc),默认asc;
        Render: function (data, callBack) {
            var sHtml = '';
            sHtml += '<table width="100%" border="0" cellpadding="0" cellspacing="0" class="reporttable">';
            sHtml += '<thead> <tr> <th width="25%" class="dataTableHeader" align="left" orderBy="1">Channel<span></span> </th>';
            sHtml += '<th width="25%" class="dataTableHeader" align="center" orderBy="2">Sales Totals<span></span></th>';
            sHtml += '<th width="25%" class="dataTableHeader" align="center" orderBy="3">Units Sold<span></span> </th>';
            sHtml += '<th width="25%" class="dataTableHeader" align="center" orderBy="4">Avg. Sale Amount <span></span> </th></tr> </thead>';
            sHtml += '<tbody>';
            $.each(data["ReportList"], function () {
                sHtml += '<tr>';
                sHtml += '<td valign="top"><span>' + this.Channel + '</span></td>';
                sHtml += '<td valign="top" align="center">' + this.SalesTotal + '</td>';
                sHtml += '<td valign="top" align="center">' + this.UnitsSold + '</td>';
                sHtml += '<td valign="top" align="center">' + this.AvgAmont + '</td>';
                sHtml += '</tr>';
            })

            sHtml += '<tr id="totalSatis">';
            sHtml += '<td valign="top" ><span> Total </span></td>';
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
                //  orderInfo["OrderType"] == "0" ? "1" : "0";//切换排列，把自己坑了~~~
                orderInfo["OrderType"]  =   orderInfo["OrderType"] == "0" ? "1" : "0";
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