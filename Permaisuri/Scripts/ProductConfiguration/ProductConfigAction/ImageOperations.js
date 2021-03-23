define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var imagepicker = require('image-picker/image-picker.min.js');

    var overlaycss = require('PlugIn/Overlay/overlay.css');
    var overlayModel = require('PlugIn/Overlay/loading-overlay.min');

    exports.ImagesModel = {

        MediaEvents:function()
        {
            var Self = this;
            //双击和单击冲突，改成右击
            $(".liberyhld").contextmenu(function (e) {
                var imageSelf = this;
                //说明当前点击的图片是Primary image,无需做任何改动
                if (($(this).find(".primaryFlags")).length > 0) {
                    return false;
                }
                $.messager.confirm('Confirm', 'Are you sure to set this image as primary iamge?', function (r) {
                    if (r) {
                        Self.SetPrimaryImage(imageSelf);
                    }
                });
                return false;
            });

            //$(".mediaTrash").css("cursor", "pointer").off("click").on("click", function (event) {
            //    var thisObj = $(this);
            //    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
            //        if (r) {
            //            Self.RemoveImage(thisObj);
            //        }
            //    });
            //    event.stopPropagation();//阻止事件继续冒泡，导致在点击删除按钮的时候又触发了图片Set Primary images的事件 2013-10-08
            //    // return false;
            //});

            $(".primaryFlags").parents(".liberyhld").css({ "border": "3px solid blue" });//给Primary Image 加边框

            $(".attachMediabtn").off("click").on("click", function () {
                Self.OpenImgDialog($("#hiddenProductID").val(), $("#hiddenStockKey").val());
            }).css({"cursor":"pointer"});

            $(".myModalCancel").off("click").on("click", function () {
                Self.CloseImgDialog();
            });

            //选中图像之后，点确定按钮，触发SKU和图像关联动作
            $(".myModalOK").off("click").on("click", function () {
                var arrMediaID = $("#selectSKUImg").val();
                if (arrMediaID === null) {
                    $.messager.alert('AttachImagesToSKU', "Please choose at least one image", 'error');
                    return false;
                }
               
                var arrWPID = $("#multMediaChannels").val();
                if (arrWPID === null) {
                    $.messager.alert('AttachImagesToSKU', "Please choose at least one Channel", 'error');
                    return false;
                }

                $.ajax({
                    url: "./AttachImagesToSKUWithBatch",
                    type: "POST",
                    dataType: "json",
                    data: {
                        mediaIDList: arrMediaID,
                        WPIDList: arrWPID
                    },
                    traditional: true,//prevent ajax deep copy oject 
                }).done(function (retData) {
                    if (retData.Status === 1) {

                    } else {
                        $.messager.alert('AttachImagesToSKU', retData.Data, 'error');
                    }
                }).fail(function () {
                    $.messager.alert('AttachImagesToSKU', 'NetWork Error,Please contact administrator。', 'error');
                }).done(function () {
                    Self.RerenderSKUMedia($("#hiddenSKUID").val());
                    Self.CloseImgDialog();
                })// end of ajax func

            });// end of  $(".myModalOK").on bangding
        },
        
        //打开图像选择的弹出框
        OpenImgDialog: function (curProductID, stockKey) {

            var  ImageSelf = this;
            var W = $(window).width();
            var h = $(window).height();
            $(window).resize(function () {
                var nW = $(window).width();
                var nh = $(window).height();
                if (!$('#multipleChannelMediaModal').is(":hidden")) {
                    $('#multipleChannelMediaModal').dialog({ width: nW - 100, height: nh - 50 });
                }
            });

            $("#multipleChannelMediaModal").show(); //必须先show，否则里面的div input等html结构会看不到....
            $("#multipleChannelMediaModal").dialog({
                title: 'Attach media for  SKU:' + $("#SKU").val(),
                width: W - 100,
                height: h - 50,
                closed: false,
                cache: false,
                modal: true,
                onClose: function () {
                    ImageSelf.CloseImgDialog();
                }
            });

            ImageSelf.GetImageList(curProductID);
            ImageSelf.GetChannelsByHM(stockKey);
        },

        //curProductID:获取当SKU关联的HM#的ID
        GetImageList: function (curProductID) {
            $.ajax({
                url: "./GetImageList",
                type: "POST",
                dataType: "json",
                data: {
                    page: 1,
                    rows: 300,//。。。本来要全部展示...为了兼容MediaLibrabry页面，这些展示前100个。。。把 2013年11月29日15:06:12
                    Keywords: $.trim($("#imgKeyWords").val()),
                    IsExcludeSKU: false,
                    SKUID: $("#hiddenSKUID").val(),
                    SKUOrder: $(".hiddenSKU").val(),
                    ProductID: curProductID
                },
                success: function (retData) {
                    if (retData.Status === 1) {

                        var cmsURL = $("#hiddenCMSImgUrl").val();

                        var gpHtml = '';
                        gpHtml += '<select multiple class="image-picker show-html" id="selectSKUImg">';
                        $.each(retData.Data["groupByList"], function () {//先按照图像大小建立分组信息，注意id
                            gpHtml += '<optgroup label="' + this.fileWidth + '*' + this.fileHeight + '" id="' + this.fileWidth + 'x' + this.fileHeight + '">';
                            gpHtml += '</optgroup>';
                        });
                        gpHtml += '</select>';//注意select 放置在循环之外，属于最高层
                        $("#divSkuImg").html(gpHtml);//填充到图像栏目的DIV下面。

                        $.each(retData.Data["List"], function () {//按照图像大小，分别append到对于的组别下面
                            var strHtml = '';
                            strHtml += '<option data-img-src="' + cmsURL + this.HMNUM + "/" + this.ImgName + '_th' + this.fileFormat + '" value=' + this.MediaID + '></option>';
                            $("#" + this.fileWidth + 'x' + this.fileHeight + "").append(strHtml);
                        });

                        //$("#selectSKUImg").imagepicker({ show_label: true });
                        $("#selectSKUImg").imagepicker();
                        strHtml = null;
                        gpHtml = null;
                    } else {
                        $.messager.alert('GetImageList', retData.Data, 'error');
                    }
                },
                error: function () {
                    $.messager.alert('GetImageList', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }, // func: GetImageList

        SetPrimaryImage: function (imageSelf) {
            var imageID = $(imageSelf).attr("PrimaryImagesID");
            $('#chanelImageDIV').loadingOverlay({
                loadingText: 'Setting primary image'
            });
            $.ajax({
                url: "./SetPrimaryImage",
                type: "POST",
                dataType: "json",
                data: {
                    SKUID: $("#hiddenSKUID").val(),
                    MediaID: imageID
                },
                success: function (retData) {
                    $('#chanelImageDIV').loadingOverlay('remove');
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
                    $('#chanelImageDIV').loadingOverlay('remove');
                    $.messager.alert('SetPrimaryImage', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        },

        //在ProductConfiguration页面删除掉关联SKU的图像
        // obj :Refer to 当前被删除图像的父类DIV
        UnattachSKUMedia: function (MediaID) {
            var unattachSelf = this;
            $('#chanelImageDIV').loadingOverlay({
                loadingText: 'Unattaching media from SKU...'
            });
            $.ajax({
                url: "./RemoveSKUMedia",
                type: "POST",
                dataType: "json",
                data: {
                    SKUID: $("#hiddenSKUID").val(),
                    MediaID: MediaID
                }
            }).done(function (retData) {
                $('#chanelImageDIV').loadingOverlay('remove');
                if (retData.Status === 1) {
                    //obj.parent().parent().remove();
                    unattachSelf.RerenderSKUMedia($("#hiddenSKUID").val());
                } else {
                    $.messager.alert('UnattachSKUMedia', retData.Data, 'error');
                }
            }).fail(function () {
                $('#chanelImageDIV').loadingOverlay('remove');
                $.messager.alert('UnattachSKUMedia', 'NetWork Error,Please contact administrator。', 'error');
            });
        },

        //关掉当前图像选择的弹出框
        //CreateDate:2014年1月6日10:15:28
        CloseImgDialog: function () {
            if (!$("#multipleChannelMediaModal").is(":hidden")) {
                //$("#imgKeyWords").val("");
                //$("#add_media_bg").fadeOut();
                //$("#myModal").fadeOut();
                $("#multipleChannelMediaModal").dialog("close");
            }
        },

        //新功能：当关联图像的时候连同渠道一并关联
        GetChannelsByHM: function (stockKey) {
            $("#multMediaChannels").addClass('auto-loading');
            $.ajax({
                url: "./GetChannelsByHM",
                type: "POST",
                dataType: "json",
                data: {
                    StockKey: stockKey
                }
            }).done(function (retData) {
                $("#multMediaChannels").removeClass('auto-loading');
                if (retData.Status === 1) {
                   
                    var selHTML = '';
                    var curSKUID = $("#hiddenSKUID").val();
                    $.each(retData["Data"], function () {
                        if (curSKUID == this.SKUID) {
                            selHTML += '<option value=' + this.SKUID + ' selected >' + this.ChannelName + '</option>';
                        } else {
                            selHTML += '<option value=' + this.SKUID + '>' + this.ChannelName + '</option>';
                        }
                    })
                    //$("#multMediaChannels").append(selHTML);
                    /*不要取消掉这行注释，将append改成html是为了消除重复发SKUID数据的问题2014年5月5日17:00:16*/
                    /*  加了默认选中当前设置之后，每次打开Attach一次就会多传送一次当前被选择中的SKUID，导致后台数据重复！
                           WPIDList	9334
                           WPIDList	9334
                           WPIDList	9334
                           mediaIDList	11
                           mediaIDList	10
                           mediaIDList	9
                           mediaIDList	8
                           mediaIDList	7
                   */
                    $("#multMediaChannels").html(selHTML);
                    $("#multMediaChannels").chosen({
                        placeholder_text_multiple: "Click here to select Channels. 'Alt + Right click' to select more than one.",
                        display_selected_options: false
                    });

                } else {
                    $.messager.alert('GetChannels', retData.Data, 'error');
                }
            }).fail(function () {
                $("#multMediaChannels").removeClass('auto-loading');
                $.messager.alert('GetChannels', 'NetWork Error,Please contact administrator。', 'error');
            })// end of ajax func
        }, // func: GetImageList


        //根据SKUID重新刷新产品对应的图像
        RerenderSKUMedia: function (SKUID) {
            var RenderSelf = this;
            $('#chanelImageDIV').loadingOverlay({
                loadingText: 'refereshing media...'
            });
            $.ajax({
                url: "./GetImageListByWPID",
                type: "POST",
                dataType: "json",
                data: {
                    SKUID: SKUID
                }
            }).done(function (retData) {
                $('#chanelImageDIV').loadingOverlay('remove');
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
                            strHTML += '  <span class="primaryFlags"> </span> ';
                        }
                        strHTML += '</div>';
                    });
                    $("#divMediaList").html(strHTML);

                    var opts = {
                        cmsURL: $("#hiddenCMSImgUrl").val(),
                        Media_Data_list: retData["Data"]["List"],
                        IsNeedDeleteButton: false,
                        overlayElement: $('#chanelImageDIV'),
                        IsNeedPrimaryImage: true,
                        UnattachedSKUButton: true,
                        UnattachedCallBack: function (_MediaID) {
                            RenderSelf.UnattachSKUMedia(_MediaID);
                        },
                        PrimaryImageCallBack: function (_imageSelf) {
                            //需要注意的是这里的_imageSelf是一个undefined对象，该对象在bubbuleInfo onMouseOver的时候被覆盖赋予了当前图像div的对象
                            // 同理，在图像做ReRender的时候，也许要这样延迟赋值。2014年5月19日10:57:00
                            RenderSelf.SetPrimaryImage(_imageSelf);
                        }
                    };
                    var BubbleInfoModel = require('Media/BubbleInfo');
                    BubbleInfoModel.Media.Bubble(opts);

                    RenderSelf.MediaEvents();

                } else {
                    $.messager.alert('UpdatedProduct', retData.Data, 'error');
                }
            }).fail(function () {
                $('#chanelImageDIV').loadingOverlay('remove');
                $.messager.alert('UpdatedProduct', 'NetWork Error,Please contact administrator。', 'error');
            })
        }
    }
});