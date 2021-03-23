/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var cmsTagsModel = require('Common/OpenCMSTag');

    /// <summary>
    /// 从Dashboard主页面剥离出来的，用于展示开发中的SKU.
    /// CreateDate:2014年2月11日11:39:40
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.Render = {
        List: function (retData) {
            var proDveData = retData.ProductDevList;
            var pdHtml = '';
            $.each(proDveData, function () {
                pdHtml += '<tr class="proDevTD" value=' + this.SKUID + '> ';
                pdHtml += '<td valign="top"  align="left"><span>#' + this.SKU + '</span></td>';
                pdHtml += '<td valign="top"  align="left"><span>' + this.ChannelName + '</span></td>';
                pdHtml += '<td valign="top"  align="left"><span>' + this.StatusName + '</span></td>';
                pdHtml += '<td valign="top"  align="right"><span>' + this.strModify_Date + '</span></td>';
                pdHtml += '</tr>';
            });
            $("#productDve").html(pdHtml);
            $("#productDve tr:odd").addClass("odd_row");
            var pCount = retData.Metrics.ProductDev;
            $(".productDevCount").html('total:' + pCount + ' records <span>More...</span>');
            $(".productDevCount span").on("click", function () {
                var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                TagsParam.name = "Reports";
                TagsParam.title = "Reports";
                TagsParam.iframeID = "frmTag";
                TagsParam.reload = true;
                TagsParam.URL = "../Reports/Index?ReportType=3";
                cmsTagsModel.CMSTags.OpenTags(TagsParam);
            });
            $(".proDevTD").css("cursor", "pointer").bind("click", function () {
                var SKUID = $(this).attr('value');
                var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                TagsParam.name = "ProductConfiguration";
                TagsParam.title = "ProductConfiguration";
                TagsParam.iframeID = "frmTag";
                TagsParam.reload = true;
                TagsParam.URL = "../ProductConfiguration/ProductConfiguration?SKUID=" + SKUID;
                cmsTagsModel.CMSTags.OpenTags(TagsParam);
            });

        }
    }
});