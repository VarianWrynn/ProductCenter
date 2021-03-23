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
            //绑定查看大图像Event
            $(".fancybox").fancybox({
                tpl: {
                    error: '<p class="fancybox-error">There is no picture for this item</p>',
                }
            });

            //双击和单击冲突，改成右击
            $(".liberyhld").contextmenu(function (e) {
                var imageSelf = this;
                //说明当前点击的图片是Primary image,无需做任何改动
                if (($(imageSelf).find(".primaryFlags")).length > 0) {
                    return false;
                }
                $.messager.confirm('Confirm', 'Are you sure to set this image as primary iamge?', function (r) {
                    if (r) {
                        mediaSelf.SetPrimaryImage(imageSelf);
                    }
                });
                return false;
            });

            $(".primaryFlags").parents(".liberyhld").css({ "border": "3px solid blue" });//给Primary Image 加边框

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

        SetPrimaryImage: function (imageSelf) {
            progress.show();
            var imageID = $(imageSelf).attr("PrimaryImagesID");
            $.ajax({
                url: "./SetPrimaryImageForHMNUM",
                type: "POST",
                dataType: "json",
                data: {
                    ProductID: $("#hiddenProductID").val(),
                    MediaID: imageID
                },
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        //查询当前SKU的Primary Image 如果有则清除掉
                        $(".primaryFlags").parents(".liberyhld").css({ "border": "" });
                        $(".primaryFlags").remove();
                        //设置当前被点击的image 为 primary image
                        $(imageSelf).append('<span class="primaryFlags"> </span>');
                        $(".primaryFlags").parents(".liberyhld").css({ "border": "3px solid blue" });//给Primary Image 加边框

                    } else {
                        $.messager.alert('SetPrimaryImage', retData.Data, 'error');
                    }
                },
                error: function () {
                    progress.hide();
                    $.messager.alert('SetPrimaryImage', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        },
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
                        strHTML += '<a class="fancybox" href="' + this.Pic + '" title="Come from ' + this.SystemName + '" >';
                        strHTML += '<img src="' + this.SmallPic + '"  onerror="this.src=\'../Content/images/NoPic.jpg\' ">';
                        strHTML += '</a> ';
                        strHTML += '</div>';
                    });
                    $("#OtherSystemImagesDiv").html(strHTML);
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
                url: "./GetImageListByProductID",
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
                            strHTML += '<div  class="bubbleInfo" id="hidMediaID' + this.MediaID + '" PrimaryImagesID ="' + this.MediaID + '">';
                            strHTML += '<img  src="' + cmsURL + this.HMNUM + '/' + thfName + '">';
                            strHTML += '</div>';
                            if (this.IsPrimaryImages == true) {
                                strHTML += '<span class="primaryFlags"></span>';
                            }
                            strHTML += '</div>';
                        });
                        $("#divMediaList").html(strHTML);

                        //BubbleInfoModel.Media.Bubble($("#hiddenCMSImgUrl").val(), retData["Data"]["List"], function () {
                        //    RenderSelf.RerenderMedia();
                        //}, $('#CMSImagesForms'), true, function (_imageSelf) {
                        //    //需要注意的是这里的_imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
                        //    // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
                        //    RenderSelf.SetPrimaryImage(_imageSelf);
                        //});

                        var opts = {
                            cmsURL: $("#hiddenCMSImgUrl").val(),
                            Media_Data_list: retData["Data"]["List"],
                            DeletedCallBack: function () {
                                RenderSelf.RerenderMedia();
                            },
                            overlayElement: $('#CMSImagesForms'),
                            IsNeedPrimaryImage: true,
                            PrimaryImageCallBack: function (_imageSelf) {
                                //需要注意的是这里的_imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
                                // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
                                RenderSelf.SetPrimaryImage(_imageSelf);
                            }
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