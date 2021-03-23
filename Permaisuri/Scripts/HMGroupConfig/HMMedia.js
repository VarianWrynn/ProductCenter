/// <summary>
/// HMNUMConfiguration 的纸箱Cartons信息维护
///2013年12月25日10:43:35
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var fancybox = require('FancyBox/jquery.fancybox');
    var BubbleInfoModel = require('Media/BubbleInfo');

    exports.Media = {
        //绑定事件，比如CMS图像的删除，点击放大等事件
        Init: function () {
            var mediaSelf = this;
            //绑定图像放大的Event
            $(".fancybox").fancybox({
                tpl: {
                    error: '<p class="fancybox-error">There is no picture for this item</p>',
                }
            });
            //绑定图像删除事件
            $(".CMSMediaTrash").off("click").on("click", function () {
                var thisObj = $(this);
                var MediaID = thisObj.attr("value");
                $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                    if (r) {
                        progress.show();
                        $.ajax({
                            url: "./DeleteCMSMedia",
                            type: "POST",
                            dataType: "json",
                            data: { "MediaID": MediaID }
                        }).done(function (retData) {
                            progress.hide();
                            if (retData.Status === 1) {
                                thisObj.parent().parent().remove();
                            } else {
                                $.messager.alert('HMConfig-DeleteCMSMedia', retData.Data, 'error');
                                return false;
                            }
                        }).fail(function () {
                            progress.hide();
                            $.messager.alert('HMConfig-DeleteCMSMedia', 'Failed to edit,please contact administrator', 'error');
                            return false;
                        });
                    }
                });
                return false; //阻止事件冒泡
            }).css({"cursor":"pointer"});
        },// end of Init init() 

        //从WebPO,Ecom获取图像（根据HM#）
        GetImagesFromOtherSystem: function () {
            $("#CMSImageDIV").addClass('auto-loading');
            $.ajax({
                url: "./GetImagesFromOtherSystem",
                type: "POST",
                dataType: "json",
                data: { "HMNUM": $("#HMNUM").val()}
            }).done(function (retData) {
                $("#CMSImageDIV").removeClass('auto-loading');
                if (retData.Status === 1) {
                    var strHTML = '<p class="OtherSystem">WebPO / Ecom Images</p>';
                    $.each(retData["Data"], function () {//瓶装其他系统的图像
                        strHTML += '<div class="liberyhld">';
                        strHTML += '<a class="fancybox" href="' + this.Pic + '" title="Come from ' + this.SystemName + '">';
                        strHTML += '<img src="' + this.SmallPic + '"  onerror="this.src=\'../Content/images/NoPic.jpg\' ">';
                        strHTML += '</a> ';
                        strHTML += '</div>';
                    });
                    $("#OtherSystemImagesDiv").html(strHTML);
                    //$(".fancybox").fancybox();//rebind event
                } else {
                    $.messager.alert('HMConfig-GetImagesFromOtherSystem', retData.Data, 'error');
                    return false;
                }
            }).fail(function () {
                $("#CMSImageDIV").removeClass('auto-loading');
                $.messager.alert('HMConfig-GetImagesFromOtherSystem', 'Failed to edit,please contact administrator', 'error');
                return false;
            });
        },

        RerenderMedia: function () {
            var RenderSelf = this;
            progress.show();
            $.ajax({
                url: "../HMConfig/GetImageListByProductID",
                type: "POST",
                dataType: "json",
                data: {
                    StockKeyID: $("#hiddenStockKeyID").val()
                },
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        var cmsURL = $("#hiddenCMSImgUrl").val();
                        var strHTML = "";
                        $.each(retData["Data"]["List"], function () {
                            var fName = this.ImgName + this.fileFormat;
                            var thfName = this.ImgName + "_th" + this.fileFormat;
                            strHTML += '<div class="liberyhld" id="' + this.MediaID + '">';
                            strHTML += '<div  class="bubbleInfo" id="hidMediaID' + this.MediaID + '">';
                            //strHTML += '<a class="fancybox" href="' + cmsURL + this.HMNUM + '/' + fName + '" title="' + this.ImgName + '">';
                            strHTML += '<img  src="' + cmsURL + this.HMNUM + '/' + thfName + '">';
                            //strHTML += '<span><img class="CMSMediaTrash" value="' + this.MediaID + '" src="../Content/Products/images/trash.png"></span>';
                            strHTML += '</div>';
                            strHTML += '</div>';
                        });
                        $("#divMediaList").html(strHTML);
                        //BubbleInfoModel.Media.Bubble($("#hiddenCMSImgUrl").val(), retData["Data"]["List"], function () {
                        //    RenderSelf.RerenderMedia();
                        //}, $('#CMSImagesForms'));

                        var opts = {
                            cmsURL: $("#hiddenCMSImgUrl").val(),
                            Media_Data_list: retData["Data"]["List"],
                            DeletedCallBack: function () {
                                RenderSelf.RerenderMedia();
                            },
                            overlayElement: $('#CMSImagesForms')
                        };
                        BubbleInfoModel.Media.Bubble(opts);

                        RenderSelf.Init();

                    } else {
                        $.messager.alert('UpdatedProduct', retData.Data, 'error');
                    }
                },
                error: function () {
                    progress.hide();
                    $.messager.alert('UpdatedProduct', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }
    }
});