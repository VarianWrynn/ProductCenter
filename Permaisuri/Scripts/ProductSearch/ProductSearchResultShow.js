// ReSharper disable UnusedParameter
define(function (require, exports, module) {
// ReSharper restore UnusedParameter
    var $ = require('jquery');
// ReSharper disable UnusedLocals
    var easyui = require('jquery.easyui.min');
// ReSharper restore UnusedLocals
    var prograss = require('Common/Prograss');

    ///根据AJAX请求，每次动态返回库存信息。
    //新增参数reqType来支持报表的导出。2014年2月17日10:55:01
    exports.requestProductData = function (thisModel,reqType) {
        //var newData =  $(paramData).extend({
        //    SKU: $.trim($(".searchSKU").val()),
        //    MultiplePart: $("#cbMultiplePart").combo('getValue')
        //});

        var fvalidate = $("#searchProductFilterForm").form('validate');
        if (!fvalidate) {
            return false;
        }

        var paramData = {
            page: parseInt($.trim($("#aRow").text())),
            rows: parseInt($("#opPageSize").val()),
            Keywords: $.trim($("#keywordbg").val()),
            SKUOrder: $.trim($(".searchSKU").val()),
            UpdateBy: $.trim($("#UpdateBy").val()),//2014年2月14日16:10:01
            BrandID: $("#cbBrand").combobox('getValue'),
            ChannelID: $("#cbChannel").combobox('getValue'),
            multiplePartType: $("#cbMultiplePart").combobox('getValue'),
            Status: $('#cbStatus').combobox('getValue'),
            CategoryID: $("#cbCategory").combobox('getValue'),
            InventoryType: $('#cbInventory').combobox('getValue'),
            OrderBy: orderInfo["OrderBy"],
            OrderType: orderInfo["OrderType"]
        };

        if (typeof (reqType) != "undefined")
        {
            if (reqType == "ExporteCVS")//说明是为了导出报表，需要特殊处理
            {
                var url = './ExportToExcel?postData=' + JSON.stringify(paramData);
                window.location.href = url;
                return false;
            }
        }

        prograss.show();
        $.ajax({
            url: "./GetProductDGData",
            type: "POST",
            dataType: "json",
            data: paramData,
            success: function (retData) {
                prograss.hide();
                if (retData.Status === 1) {
                    thisModel.showResultTable(retData.Data, paramData);
                } else {
                    $.messager.alert('SearchProducts', retData.Data, 'error');
                }
            },
            error: function () {
                prograss.hide();
                $.messager.alert('SearchProducts', 'NetWork Error,Please contact administrator。', 'error');
            }
        }); // end of ajax func
        return false;
    }; /// <summary>
    /// Dynamic displaying search product table with json data
    /// </summary>
    /// <param name="dgData">local data (json formate)</param>
    /// <returns></returns>
// ReSharper disable UnusedParameter
    exports.showResultTable = function (data, params, isInit) {
// ReSharper restore UnusedParameter
        var dgData = data["rows"];
        var totalRecord = data["total"];
        var startIndex = ((params["page"] - 1) * params["rows"])+1;// 第二页： （2-1)*10-1 =11;
        var endIndex = params["page"] * params["rows"];//2*10=20
        var dgHtml = "";
        $.each(dgData, function () {
            var skuSelf = this;
            dgHtml += '<tr>';
            if (this.StockByPcs > 30) {
                dgHtml += ' <td valign="top" align="center"> <img src="../Content/Products/images/greendot.png"> </td>';
            } else if (this.StockByPcs > 0) {
                dgHtml += ' <td valign="top" align="center"> <img src="../Content/Products/images/orangedot.png"> </td>';
            } else {
                dgHtml += ' <td valign="top" align="center"> <img src="../Content/Products/images/reddot-new.png"> </td>';
            }
            if (this.IsGroup) {
                dgHtml += '<td valign="top" align="center"><a class="show_hide" value="' + skuSelf.SKUID + '" href="JavaScript:void(0);" style="display: inline;">';
                dgHtml += '<img width="13" height="14" src="../Content/Products/images/expand-ico.png"></a></td> ';
            } else {
                dgHtml += ' <td valign="top" align="center"> <img src="../Content/Products/images/edit-pen.png"> </td>';
            }
            dgHtml += ' <td valign="top" align="left" class="SKUOrderName" value=' + this.SKUID + ' ><span>' + this.ProductName + '</span></td>';
            dgHtml += ' <td valign="top" align="left" class="SKUOrderTD" value=' + this.SKUID  + ' ><span>' + this.SKU + '</span></td>';
            //dgHtml += ' <td valign="top" align="left"> <input type="text" value=' + this.SKU_QTY + ' id="textfield" class="skuinput" name="textfield"></td>';
            //dgHtml += ' <td valign="top" align="left">' + this.SKU_QTY + ' </td>';先注释掉这一话 2014年3月7日，避免让用户产生误解。
   
            if (this.StockByPcs > 30) {
                dgHtml += ' <td valign="top" align="left">In Stock (QTY:' + this.StockByPcs + ')</td>';
            } else if (this.StockByPcs > 0) {
                dgHtml += ' <td valign="top" align="left"><font>Low Inventory (QTY:' + this.StockByPcs + ')</font></td>';
            } else {
                dgHtml += ' <td valign="top" align="left"><span class="strongtxt">Out of Stock (QTY:' + this.StockByPcs + ')</span></td>';
            }
            //dgHtml += ' <td valign="top" align="left"><input type="text" value=' + this.Price + ' id="textfield" class="skuinput" name="textfield"></td>';
            dgHtml += ' <td valign="top" align="left">'+this.strPrice+'</td>';
            dgHtml += ' <td valign="top" align="left">' + this.ChannelName + '</td>';

            dgHtml += ' <td valign="top" align="left">' + this.UpdateOn + '</td>';

            dgHtml += '</tr>';
            if (this.IsGroup) {  //注意，现在是列表状态，当前这个状态如果是组合产品，则，这个CMS_HMNUM是父产品的信息
                dgHtml += '<tr style="padding: 0; margin: 0;">';
                dgHtml += '<td valign="top" align="left" style="padding: 0; margin: 0;" colspan="8">';
                dgHtml += '<table width="100%" border="0" cellspacing="0" cellpadding="5" class="slidingDiv' + skuSelf.SKUID + '" style="display: none;"><tbody>';
                $.each(this.SKU_HM_Relation.CMS_HMNUM.Children_CMS_HMNUM_List, function () {//注意,如果当前是组合
                    dgHtml += '<tr>';
                    dgHtml += '<td width="3%">&nbsp;</td><td width="3%">&nbsp;</td>';
                    dgHtml += '<td width="30%"><img src="../Content/Products/images/arrow.png">' + this.ProductName + '</td>';
                    dgHtml += '<td width="25%"><img src="../Content/Products/images/arrow.png">' + this.HMNUM + '</td>';
                    dgHtml += '<td width="10%" align="right" class="rightpad"><span class="strongtxt" style="display:block;text-align:center">' + this.StockKeyQTY + '</span> </td>';
                    if (this.Comments == null || this.Comments == "") {

                        dgHtml += '<td width="40%"></td>';
                    } else {
                        dgHtml += '<td width="40%">NOTE: ' + this.Comments + '</td>';
                    }
                    dgHtml += '</tr>';
                });
                dgHtml += '</tbody> </table> </td></tr>';
            }
        });
        $("#searchResultBody").html(dgHtml);
        $("#searchResultBody tr:odd").addClass("odd_row");
        $(".resulthld").html('<p>Results ' + startIndex + ' - ' + endIndex + ' of ' + totalRecord + '</p>');
        $("#hiddenTotal").val(totalRecord);
        $("#aRow").html(params["page"]);
        $('.show_hide').click(function () {
            var id = $(this).attr("value");
            $(".slidingDiv" + id).slideToggle();
        });
        $(".SKUOrderName,.SKUOrderTD").css("cursor", "pointer").on("click", function () {
            var paramsVal = $(this).attr("value");
             var strHtml = "";
            //var params = paramsVal.split('|');
            //var strReq = "?SKU=" + params[0] + "&ChannelID=" + params[1];
             strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src="../ProductConfiguration/ProductConfiguration?SKUID=' + paramsVal + '"></iframe>';
            //notices:if the iframe name use id="frmWorkArea" , it will never create new tab but will replace old searhProduct page. This is not user want.. 2013/08/28.
            if (window.parent.jQuery('#centerTabs').tabs('exists', "ProductConfiguration")) {
                //window.parent.jQuery('#centerTabs').tabs('select', "ProductConfiguration");
                //window.parent.jQuery("#frmTag").attr("src", '../ProductConfiguration/ProductConfiguration?SKUID=' + paramsVal);
                //由于管理tab下的iframe十分混乱，以后一律先关闭再打开！
                window.parent.jQuery('#centerTabs').tabs('close', "ProductConfiguration");
                window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "ProductConfiguration",
                    content: strHtml,
                    closable: true
                });

            } else {
                window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "ProductConfiguration",
                    content: strHtml,
                    closable: true
                });
            }
        });
        dgHtml = "";//just for set the memory free....
    };
});