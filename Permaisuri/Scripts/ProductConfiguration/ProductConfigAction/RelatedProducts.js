define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    ///构建 Related Products页面 （在ProductConfiguraton页面上）
    ///Change1:在点击View之前，增加一个URL是否有效的判断（2014年2月14日15:14:54 Chinese Team Testing Feedback )
    exports.RelatedProducts = {
        Builting: function (relatedPData) {

            if (typeof (relatedPData) != "undefined") {
                var relatedHtml = '';
                $.each(relatedPData, function () {
                    relatedHtml += '<tr><td width="36%"><a class="aRelProduct" value="' + this.SKUID + '">' + this.ProductName + '</a></td>';
                    relatedHtml += ' <td width="16%">SKU: <span>' + this.SKU + '</span></td>';
                    relatedHtml += ' <td width="24%">Brand: <strong>' + this.BrandName + '</strong></td>';
                    relatedHtml += ' <td width="20%">Channel: ' + this.ChannelName + '</td>';
                    relatedHtml += ' <td width="5%"> <a class="aGoToSite" target value="' + this.URL + '">View</a> </td>';
                    relatedHtml += ' <td width="2%">&nbsp;</td>';
                });
                $(".relatestable").html(relatedHtml);
            }


            $(".aGoToSite").css("cursor", "pointer").css("color", "blue").attr('target', '_blank').off().on("click", function () {
                var url = $("#URL").val();
                if ($.trim(url) == "") {
                    $.messager.alert('Error', 'Selected item url is not valid, please  update the product configeration.', 'error', function () {
                       
                    });
                    return false;
                }
                window.open(url);
                return false;
            });

            $(".aRelProduct").css("cursor", "pointer").css("color", "blue").off().on("click", function () {
                var sParams = $(this).attr("value");
                var strHtml = "";
                $.messager.confirm('Confirm', 'Are you sure to leave current page?', function (r) {
                    if (r) {
                        var strReq = "?SKUID=" + sParams;
                        strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src="../ProductConfiguration/ProductConfiguration' + strReq + '"></iframe>';
                        //notices:if the iframe name use id="frmWorkArea" , it will never create new tab but will replace old searhProduct page. This is not user want.. 2013/08/28.

                        var parentObj = window.parent.jQuery('#centerTabs');
                        if (parentObj.tabs('exists', "ProductConfiguration")) {

                            var tab = parentObj.tabs('getTab', "ProductConfiguration");

                            parentObj.tabs('update', {
                                tab: tab,
                                options: {
                                    title: 'ProductConfiguration',
                                    content: strHtml,
                                }
                            });
                        }
                        strHtml = "";
                    }
                });
            });
        }
    }
});