define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    /// <summary>
    /// ProductConfiguration页面的字段提示，后期陆续补上
    ///  Author:Lee,Date:2013年12月27日11:24:43
    /// </summary>
    exports.SKUList = {

        InitToolTip: function () {
            

            $("#toolTipQueryHM").tooltip({
                position: 'top',
                deltaX: 55,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">allow you enter HM# or HM name <br> to search result,for the system\'s performance,<br>please enter at least 3 characters</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });


            $("#toolTipQuerySKU").tooltip({
                position: 'top',
                deltaX: 30,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">allow you enter SKU# or SKU name <br> to search result，for the system\'s performance, <br>please enter at least 3 characters</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#toolTipQueryModifedByUser").tooltip({
                position: 'top',
                deltaX: 30,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">Query SKU data which have been modifed by this user </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });         

            $("#tooltipProductName").tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">SKUOrder\'s name, notice this name is <br> different from HM#\'s name</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

 
            $("#tooltipSKUOrder").tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">SKUOrder</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipSKUQty").tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">Current SKUOrder\'s online inventory</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipInventory").tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">HM#\'s real inventory in warehouse which  <br> assocaited with current Channel\'s SKUOrder</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });


            $("#tooltipSalePrice").tooltip({
                position: 'top',
                deltaX: -25,//负数代表向X轴的左方移动
                deltaY: 0,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff"> The negotiated prices of our comany <br> and thrid-party e-commerce websit </span>',
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