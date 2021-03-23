define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    /// <summary>
    /// ProductConfiguration页面的字段提示，后期陆续补上
    ///  Author:Lee,Date:2013年12月27日11:24:43
    /// 注意：ProductCreate页面也开始引用这个页面的，现在是一个公用页面了 2014年4月23日
    /// </summary>
    /// <param name="StatusCode">产品的Status</param>
    /// <returns></returns>
    exports.SKU = {

        InitToolTip: function () {

            //Sale Price字段
            $("#tooltipMaterial").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: 0,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">Material:This filed\'s value base on HMNUM (from WEBPO),but you can ' +
                    '<br>input new item if neccessary. the same logic control for "Colour","Category","SubCategory" fields </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });


            //Retail字段
            $("#tooltipVisibility").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff"> Notice: this field open on Phase 2</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            //Retail字段
            $("#tooltipSKU_QTY").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">Onlien Inventory for current Channel. this value retrieve from current Channel <br>' +
                    'in real time. You can edit and save it to current Channel. Notice: this field open on Phase 2</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            //Retail字段
            $("#tooltipSalePrice").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">The negotiated prices of our comany <br> and third-party e-commerce websit</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            //SKUOrder运费：货物从仓库到客户手上的平均估计运费，作为成本之二考虑
            $("#tooltipEstimateFreight_SKU").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff"> Estimated,averaged cost of goods that delivering from our warehouse to customer.<br> One of the references for sale price. </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipFirstCost").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                //content: '<span style="color:#fff">The original cost of HMNUM ( Free On Board ) ,this item can not be edited directly.<br>But you can edit it under HMNUM Configuration page. </span>',
                content: '<span style="color:#fff">The orginal cost of SKU, this price= HMNUM first cost +Ocean Freight+ Drayage+ USA Handling Charge,<br>this item can not be edited directly.But you can edit it under HMNUM Configuration page. </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipLandedCost").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff"> Cost of goods from factory to warehouse,this item can not be edited directly.<br>But you can edit it under HMNUM Configuration page.  </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });


            //Sale Price字段
            $("#tooltipRetailPrice").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">This price is sold to custmer,pricing by channel.<br> CMS fetch the online price data via URL automatically.</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

           
            $("#btnSkuImgUpload").css({ "cursor": "pointer" });

            $("#tooltipMP").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: 0,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">MP:Master Pack. Indicator:A box can hold how many pieces.' +
                    '<br> <b> For example: if current SKU have 6 HM# pieces , and MP is 3,<br> then it must need 2 box to load (6/3=2)</b></span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });
           
        }// end of updatedBasiceInfo
    }
});