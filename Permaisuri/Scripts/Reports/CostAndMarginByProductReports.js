/// <summary>
/// CreateDate:2014年1月14日15:36:33
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    var fancybox = require('FancyBox/jquery.fancybox');

    /// <summary>
    /// Dynamic render Report.
    /// 根据传递进来的数据动态展示报表(SaleByProduct 2013-09-05)
    /// </summary>
    /// <param name="data">remote date from Server-Side</param>
    /// <param name="ajaxF">由于GetReportsList 和本页面的方法相互调用，无法直接使用Seajs引用（直接被seajs杀死）,所以这里直接接收
    ///                     GetReportsList的方法，然后在本页面直接调用
    /// </param>
    /// <returns></returns>
    exports.RenderReport = {
        Render: function (data, callBack) {
            var cmsURL = $("#hiddenCMSImgUrl").val();
            var sHtml = '';
            sHtml += '<table width="100%" border="0" cellpadding="0" cellspacing="0" class="reporttable">';
            sHtml += '<thead> <tr> <th width="10%" class="dataTableHeader" align="left" >PrimaryImages </th>';
            sHtml += '<th width="10%" class="dataTableHeader" align="left" orderBy="1">SKUOrder <span></span></th>';
            sHtml += '<th width="20%" class="dataTableHeader" align="left" orderBy="2">ProductName <span></span></th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="3">Channel <span></span></th>';
            sHtml += '<th width="10%" class="dataTableHeader" align="center" orderBy="4">Brand<span></span></th>';
            sHtml += '<th width="7%" class="dataTableHeader" align="center" orderBy="5">Retail<span></span></th>';
            sHtml += '<th width="5%" class="dataTableHeader" align="center" orderBy="6">COG<span></span> </th>';
            sHtml += '<th width="" class="dataTableHeader" align="center" orderBy="7">Margin<span></span></th>';
            sHtml += '<th width="" class="dataTableHeader" align="center" orderBy="8">Inventory<span></span></th></tr> </thead>';
            sHtml += '<tbody>';
            $.each(data["List"], function () {
                sHtml += '<tr>';
                var ImageInfo = ($.trim(this.ImageName)).split(',');
                if (ImageInfo == "") {
                    sHtml += '<td valign="center" align="center"><img height="100px" width="100px" src="../Content/images/CMSDefault.jpg"></td>';
                } else {
                    //先获取到扩展名，再用_th+扩展名的方式替代
                    var imgExt = ImageInfo[1].substring(ImageInfo[1].lastIndexOf("."), ImageInfo[1].length);//.jpg
                    var thImg = ImageInfo[1].replace(imgExt, '_th' + imgExt);
                    sHtml += '<td valign="center" align="center">';
                    sHtml += '<a title="' + ImageInfo[1] + ' " href ="'+cmsURL + ImageInfo[0] + '/' + ImageInfo[1] + '" class="fancybox"> ';
                    sHtml += '<img height="100px" width="100px" src="' + cmsURL + ImageInfo[0] + '/' + thImg + '">';
                    sHtml += '</a></td>';
                }
                sHtml += '<td valign="center" align="center">' + this.SKUOrder + '</td>';
                sHtml += '<td valign="center" align="center">' + this.Name + '</td>';
                sHtml += '<td valign="center" align="center">' + this.ChannelName + '</td>';
                sHtml += '<td valign="center" align="center">' + this.BrandName + '</td>';
                sHtml += '<td valign="center" align="center">' + this.NormalSelling + '</td>';
                sHtml += '<td valign="center" align="center">' + this.COG + '</td>';
                sHtml += '<td valign="center" align="center">' + this.Margin + '</td>';
                if (this.Inventory > 30) {
                    sHtml += ' <td valign="center" align="center">In Stock(QTY:' + this.Inventory + ')</td>';
                } else if (this.Inventory > 0) {
                    sHtml += ' <td valign="center" align="center"><font>Low Inventory(QTY:' + this.Inventory + ')</font></td>';
                } else {
                    sHtml += ' <td valign="center" align="center"><span class="strongtxt">Out of Stock(QTY:' + this.Inventory + ')</span></td>';
                }
                sHtml += '</tr>';
            })
            sHtml += '</tbody></table>';
            $(".salesblok").html(sHtml);
            $(".salesblok table tr:odd").addClass("odd_row");

            //绑定图像发放Event
            $(".fancybox").fancybox({
                tpl: {
                    error: '<p class="fancybox-error">There is no picture for this item</p>',
                }
            });

            $(".salesblok table thead th:gt(0)").css("cursor", "pointer").on("click", function () {
                var orderBy = $(this).attr("orderBy");
                orderInfo["OrderBy"] = orderBy;
                // orderInfo["OrderType"] == "0" ? "1" : "0";//切换排列
                orderInfo["OrderType"] = orderInfo["OrderType"] == "0" ? "1" : "0";
                callBack.apply(this);
            })

            //新增列的排序指示器 2013年10月29日15:19:24 Lee
            var orderColNum = parseInt(orderInfo["OrderBy"]);
            var typeCol = parseInt(orderInfo["OrderType"]);
            var curCol = $($(".salesblok table thead th").get(orderColNum)).find("span");// 找到当前排序列的span元素
            curCol.addClass(typeCol == 0 ? "sort-asc" : "sort-desc");//给该元素加上逆序/顺序的图标
            sHtml = null;
        }
    }
})