/// <summary>
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var prograss = require('Common/Prograss');
    var fancybox = require('FancyBox/jquery.fancybox');
    /// <summary>
    /// Dynamic render Report.
    /// 根据传递进来的数据动态展示报表 2013-09-04.
    /// </summary>
    /// <param name="data">remote date from Server-Side</param>
    /// <returns></returns>
    exports.RenderReport = {
        //用来存储各个列排序的情况（asc/desc),默认asc;
        Render: function (data, callBack) {
            var cmsURL = $("#hiddenCMSImgUrl").val();
            var sHtml = '';
            sHtml += '<table width="100%" border="0" cellpadding="0" cellspacing="0" class="reporttable">';
            sHtml += '<thead> <tr> <th width="10%" class="dataTableHeader" align="left" >ProductImage </th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="left" orderBy="1">SKU<span></span> </th>';
            sHtml += '<th width="20%" class="dataTableHeader" align="left" orderBy="2">ChannelName<span></span> </th>';
            sHtml += '<th width="15%" class="dataTableHeader" align="center" orderBy="3">Invetory<span></span></th>';
            sHtml += '<th width="25%" class="dataTableHeader" align="center" orderBy="4">Expected Inventory<span></span> </th>';
            sHtml += '<th width="" class="dataTableHeader" align="center" orderBy="5">Next Shipment<span></span></th>';
            //sHtml += '<th width="" class="dataTableHeader" align="center" orderBy="6">Affected SKUs</th></tr> </thead>';
            sHtml += '<tbody>';
            $.each(data["List"], function () {
                sHtml += '<tr>';
                var ImageInfo = ($.trim(this.pImage)).split(',');
                if (ImageInfo == "") {
                    sHtml += '<td valign="center" align="center"><img height="100px" width="100px" src="../Content/images/CMSDefault.jpg"></td>';
                } else {
                    //先获取到扩展名，再用_th+扩展名的方式替代
                    var imgExt = ImageInfo[1].substring(ImageInfo[1].lastIndexOf("."), ImageInfo[1].length);//.jpg
                    var thImg = ImageInfo[1].replace(imgExt, '_th' + imgExt);
                    sHtml += '<td valign="center" align="center">';
                    sHtml += '<a title="' + ImageInfo[1] + ' " href ="' + cmsURL + ImageInfo[0] + '/' + ImageInfo[1] + '" class="fancybox"> ';
                    sHtml += '<img height="100px" width="100px" src="' + cmsURL + ImageInfo[0] + '/' + thImg + '">';
                    sHtml += '</a></td>';
                }
                sHtml += '<td valign="center"><span>' + this.SKU + '</span></td>';
                sHtml += '<td valign="center"><span>' + this.ChannelName + '</span></td>';
                sHtml += '<td valign="center" align="center">' + this.Inventory + '</td>';
                sHtml += '<td valign="center" align="center">' + this.ExpectedInventory + '</td>';
                sHtml += '<td valign="center" align="center">' + this.StringNextShipment + '</td>';
                //sHtml += '<td valign="center" align="center" ><div class="reportAffectedSKU2">' + this.AffectedSKUS + '</div></td>';
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
                orderInfo["OrderBy"] = orderBy;//记录页面哪一列点击了排序
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