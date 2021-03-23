/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var cmsTagsModel = require('Common/OpenCMSTag');
    var progress = require('Common/Prograss');

    /// <summary>
    /// 从Dashboard主页面剥离出来的，用于展示KeyMetrics面板.
    /// CreateDate:2014年2月11日11:39:40
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.Render = {
        List: function (retData) {
            var keyData = retData.Metrics;
            var keyHtml = "";
            keyHtml += '<div class="keyblk" name="AvgOrdersDiv" ><h2>' + keyData.AvgOrders + '</h2><p  value="AvgOrders" >Avg. daily orders</p></div>'
            //keyHtml += '<div class="keyblk"><h2 style="font-size:40px !important;padding-bottom:22px !important;">' + keyData.AvgAmt + '</h2><p  value="AvgAmt" >Average order amount</p></div>'
            keyHtml += '<div class="keyblk" name="AvgAmtDiv"><h2>' + keyData.AvgAmt + '</h2><p  value="AvgAmt" >Average order amount</p></div>'
            keyHtml += '<div class="keyblk"><h2>' + keyData.ProductDev + '</h2><p value="ProductDev" >Products in Development</p></div>'
            keyHtml += '<div class="keyblk"><h2>' + keyData.ItemAtt + '</h2><p value="ItemAtt">Items requiring attention</p></div>'
            $("#matricsarea").html(keyHtml);

            //bind Key Mertices Event  2013年10月25日9:45:50 Lee
            $(".keyblk p:gt(1)").css({
                color: "blue",
                cursor: "pointer"
            }).on("click", function () {
                var type = $(this).attr("value");
                var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                TagsParam.name = "Reports";
                TagsParam.title = "Reports";
                TagsParam.iframeID = "frmTag";
                TagsParam.reload = true;

                switch (type) {
                    case "AvgOrders":
                        //TagsParam.URL = "../Reports/Index?ReportType=1";
                        //cmsTagsModel.CMSTags.OpenTags(TagsParam);
                        break;
                    case "AvgAmt":
                        //TagsParam.URL = "../Reports/Index?ReportType=1";
                        //cmsTagsModel.CMSTags.OpenTags(TagsParam);
                        break;
                    case "ProductDev":
                        TagsParam.URL = "../Reports/Index?ReportType=3";
                        cmsTagsModel.CMSTags.OpenTags(TagsParam);
                        break;
                    case "ItemAtt":
                        TagsParam.name = "Search Products";
                        TagsParam.title = "Search Products";
                        TagsParam.URL = "../ProductSearch/ProductSearch?reqStatus=10";
                        cmsTagsModel.CMSTags.OpenTags(TagsParam);
                        break;
                    default:
                        break;
                }
            })

        }
    }
});