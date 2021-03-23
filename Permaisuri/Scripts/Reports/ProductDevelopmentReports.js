/// <summary>
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
            sHtml += '<thead> <tr>  <th class="dataTableHeader" align="left" orderBy="1">Product<span></span></th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="2">Brand<span></span></th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="3">Channel<span></span> </th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="4">QueueStatus<span></span></th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="5">LastUpdated<span></span></th></tr> </thead>';
            sHtml += '<tbody>';
            var qStatus = $("#QueueStatus").combobox('getData');
            $.each(data["List"], function () {
                sHtml += '<tr>';
                sHtml += '<td valign="top"><span class="pDevSpan" SKUID="' + this.SKUID + '">' + this.Product + '</span></td>';
                sHtml += '<td valign="top" align="center">' + this.Brand + '</td>';
                sHtml += '<td valign="top" align="center">' + this.Channel + '</td>';
                var curObj = this;
                $.each(qStatus, function () {
                    if (this.StatusID == curObj.QueueStatus)
                    {
                        sHtml += '<td valign="top" align="center">' + this.StatusName + '</td>';
                    }
                })
                sHtml += '<td valign="top" align="center" >' + this.LastUpdated + '</td>';
                sHtml += '</tr>';
            })
            sHtml += '</tbody></table>';
            $(".salesblok").html(sHtml);
            $(".salesblok table tr:odd").addClass("odd_row");
            $(".salesblok table thead th ").css("cursor", "pointer").on("click", function () {
                var orderBy = $(this).attr("orderBy");
                orderInfo["OrderBy"] = orderBy;//记录页面哪一列点击了排序
                orderInfo["OrderType"] = orderInfo["OrderType"] == "0" ? "1" : "0";
                callBack.apply(this);
            })
            sHtml = null;

            //新增列的排序指示器 2013年10月29日15:19:24 Lee
            var orderColNum = parseInt(orderInfo["OrderBy"]);
            var typeCol = parseInt(orderInfo["OrderType"]);
            var curCol = $($(".salesblok table thead th").get(orderColNum - 1)).find("span");// 找到当前排序列的span元素
            curCol.addClass(typeCol == 0 ? "sort-asc" : "sort-desc");//给该元素加上逆序/顺序的图标



            $(".pDevSpan").css("cursor", "pointer").on("click", function () {
                var SKUID = $(this).attr("SKUID");
                var strHtml = "";
                strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src="../ProductConfiguration/ProductConfiguration?SKUID=' + SKUID + '"></iframe>';
                //notices:if the iframe name use id="frmWorkArea" , it will never create new tab but will replace old searhProduct page. This is not user want.. 2013/08/28.
                var paramsVal = $(this).attr('value');
                if (window.parent.jQuery('#centerTabs').tabs('exists', "ProductConfiguration")) {
                    window.parent.jQuery('#centerTabs').tabs('close', "ProductConfiguration");
                } window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "ProductConfiguration",
                    content: strHtml,
                    closable: true
                });
                strHtml = "";
            });
        }
    }

})