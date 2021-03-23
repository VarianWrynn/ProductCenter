define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    /// <summary>
    /// 注意：该页面已经被HMGroupConfig, HMGroupCreate页面引用，成为公共页面。2014年4月24日
    /// </summary>
    exports.HMConfig = {

        InitToolTip: function () {

            $("#tooltipMasterPack").tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">Indicator:A box can hold how many pieces.</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipFirstCost").tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">FOB cost of #item,Free On Board.</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipLandedCost").tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">The cost of #item from factory to destination warehouse,<br>including first cost and estimate freight cost </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipEstimateFreight").tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">The cost of ociean freigh and inbound freight </span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

           

            //直接上传HM的图像
            $("#btnHMImgUpload").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: -10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">Upload media for current HM# directly</span></span>',
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