define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    /// <summary>
    /// 注意：该页面已经被HMGroupConfig, HMGroupCreate页面引用，成为公共页面。2014年4月24日
    /// </summary>
    exports.HMList = {

        InitToolTip: function () {

            $("#tooltipOrphanHM").tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">If checked, only HMNUMs without associated with any SKU will be searched.</span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });

            $("#tooltipExcludedSubHMNUM").tooltip({
                position: 'top',
                deltaX: -35,//负数代表向X轴的左方移动
                deltaY: -15,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff"> For example: there is a group HM# "235980" combination of Sub-HMs# <br>' +
                           '"41678.00" and "49388.00IRN". So if you check this checkbox , the HMNUM of "41678.00" and <br>' +
                           ' "49388.00IRN"  should be prevented from listing on the list.' +
                          '</span>',
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