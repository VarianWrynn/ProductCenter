/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    /// <summary>
    /// 从Dashboard主页面剥离出来的，用于展示渠道库存低库存信息的模块
    /// CreateDate:2014年2月11日11:39:40
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.Render = {
        List: function (retData) {
            //low Inventory table
            var lowInvData = retData.LowInventory;
            var lowInvHtml = ' <tbody>';
            $.each(lowInvData, function () {
                lowInvHtml += '<tr class="SKUOrderTD" value=' + this.SKUID + '> ';
                if (this.StockBySet_PreSale > 0) {
                    lowInvHtml += '<td valign="top"> <img src="../Content/Dashboard/images/orangedot.png"></td>';
                    lowInvHtml += '<td valign="top"  align="left"><span class="autocut"> SKU ' + this.SKU + '&nbsp;&nbsp;&nbsp;&nbsp;' + this.ProductName + '</span></td>';
                    lowInvHtml += '<td valign="top"  align="right"><font>Low Inventory</font></td>';
                } else {
                    lowInvHtml += '<td valign="top"> <img src="../Content/Dashboard/images/reddot.png"></td>';
                    lowInvHtml += '<td valign="top"  align="left"><span class="autocut"> SKU ' + this.SKU + '&nbsp;&nbsp;&nbsp;&nbsp;' + this.ProductName + '</span></td>';
                    lowInvHtml += '<td valign="top"  align="right"><font  class="strongtxt">Oversold</font></td>';
                }
                lowInvHtml += '</tr>';
            });
            lowInvHtml += ' </tbody>';
            $("#lowInvTable").html(lowInvHtml);
            $("#lowInvTable tr:odd").addClass("odd_row");
            $(".lowInvRecord").html('total:' + retData.TotalRecord + ' records <span>More...</span>');
            $(".lowInvRecord span").on("click", function () {
                var strHtml = "";
                var strReq = "../ProductSearch/ProductSearch?reqFrom=lowInventory";
                strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src=' + strReq + '></iframe>';
                var paramsVal = $(this).attr('value');
                if (window.parent.jQuery('#centerTabs').tabs('exists', "Search Products")) {
                    window.parent.jQuery('#centerTabs').tabs('select', "Search Products");
                    window.parent.jQuery("#frmTag").attr("src", strReq);
                } else {
                    window.parent.jQuery('#centerTabs').tabs('add', {
                        title: "Search Products",
                        content: strHtml,
                        closable: true
                    });
                }
                strHtml = "";
            });
            $(".SKUOrderTD").bind("click", function () {
                var paramsVal = $(this).attr('value');
                var strHtml = "";
                strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src="../ProductConfiguration/ProductConfiguration?SKUID=' + paramsVal + '"></iframe>';
                //notices:if the iframe name use id="frmWorkArea" , it will never create new tab but will replace old searhProduct page. This is not user want.. 2013/08/28.
                var paramsVal = $(this).attr('value');
                if (window.parent.jQuery('#centerTabs').tabs('exists', "ProductConfiguration")) {
                    window.parent.jQuery('#centerTabs').tabs('close', "ProductConfiguration");
                }
                window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "ProductConfiguration",
                    content: strHtml,
                    closable: true
                });
                strHtml = "";
            });

        }
    }
});