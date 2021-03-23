
define(function (require, exports, module) {
    var $ = require('jquery');
    var fancybox = require('FancyBox/jquery.fancybox');
    var ChangeNameModel = require('./MediaNameChanged');
    var ui = require('jquery-ui');
    var overlayModel = require('PlugIn/Overlay/loading-overlay.min');
    var BubbleInfoModel = require('./BubbleInfo');

    /// <summary>
    /// 动态渲染图像及其信息。
    /// Dynamic render the image and imgae's information via server data
    /// </summary>
    /// <param name="retData">retrieve data from Server-side</param>
    /// <param name="postData">request pareameters</param>
    /// <returns></returns>
    exports.MediaLibraryExt = {
        DynamicShow: function (retData, postData) {
            var mediaSelf = this;
            var cmsURL = $("#hiddenCMSImgUrl").val();
            var cloudURL = $("#CloudPrefix").val();

            $("#div_picture").html('');
            if (!$("#dpop").is(":hidden")) {
                $("#dpop").hide();
            }
            var count = parseInt(retData["count"]);
            $("#p_showResults").html("Results " + (postData.page == 1 ? 1 : (postData.page - 1) * postData.rows) + " - " + (postData.rows * postData.page < count ? postData.rows * postData.page : count) + " of <span id='span_Result_Count'>" + count + "</span>");
            if (count == 0) {
                if (!$("#pagina_hld").is(":hidden")) {
                    $("#pagina_hld").hide();
                }
                $("#div_picture").html("No Image View");
                return false;
            }
            else if ($("#pagina_hld").attr("display") == undefined) {
                $("#pagina_hld").show()
            }
            var Media_Data_list = retData["list"];

            $.each(Media_Data_list, function () {
                var pic_div = '';
                pic_div += '<div class="bubbleInfo" id="hidMediaID' + this.MediaID + '"><img height="160" width="160" alt="' + this.ImgName;
                pic_div += '" class="trigger" src="' + cmsURL + this.HMNUM + '/' + this.ImgName + '_th' + this.fileFormat;
                pic_div += '" /></div>';
                $("#div_picture").append(pic_div);
            });


            //BubbleInfoModel.Media.Bubble(cmsURL, Media_Data_list, function () { $(".btnSearch").click(); }, $('#mainarea'));
            var opts = {
                cmsURL: cmsURL,
                Media_Data_list: Media_Data_list,
                DeletedCallBack: function () {
                    $(".btnSearch").click();
                },
                overlayElement: $('#mainarea')
            };
            BubbleInfoModel.Media.Bubble(opts);

            //$("#div_picture").html("");
            //$.each(dList, function () {
            //    var pic_div = '';
            //    pic_div += '<div class="bubbleInfo" id="hidMediaID' + this.MediaID + '"><img height="160" width="160" alt="' + this.ImgName;
            //    pic_div += '" class="trigger" src="' + cmsURL + this.HMNUM + '/' + this.ImgName + '_th' + this.fileFormat;
            //    pic_div += '" /></div>';
            //    $("#div_picture").append(pic_div);
            //    $("#hidMediaID" + this.MediaID).data("Media_Data", this);//$("#ID").data("CMS_SKU")
            //});

            //$('.bubbleInfo').each(function () {
            //    $(this).mouseover(function () {
            //        var MediaData = $("#" + this.id).data("Media_Data");
            //        var fname = MediaData.ImgName;
            //        var CloudStatusID = MediaData.CloudStatusID;
            //        var imgUrl = (CloudStatusID == 4 ? cloudURL : cmsURL) + MediaData.HMNUM + "/" + fname + MediaData.fileFormat;//原图
            //        $("#dynamicImg").attr("src", $('img', this).attr("src")).attr("width", 100).attr("height", 100);
            //        $("#single_1").attr("href", imgUrl).attr("title", fname);
            //        var oldText = '';
            //        var mediaID = MediaData.MediaID;
            //        var fileExt = MediaData.fileFormat;
            //        $("#span_Name").text(fname);
            //        $(".deleteImage").off().on("click", function () {
            //            mediaSelf.DeleteCMSMedia(mediaID);
            //        });
            
            //        $("#td_Format").text(MediaData.fileFormat);
            //        $("#td_Size").text(MediaData.fileSize);
            //        $("#td_Date").text(MediaData.strCreateOn);
            //        $("#td_Resolution").text(MediaData.fileWidth + ' x ' + MediaData.fileHeight);                 
            //        $("#h2_url").text(imgUrl);
            //        var wspHtml = '';
            //        $.each(MediaData.CMS_SKU, function () {
            //            wspHtml += '<tr>';
            //            wspHtml += '<td><span class="span_SKU">' + this.SKU + '</span></td>';
            //            wspHtml += '<td><span class="span_ProductName">' + this.ProductName + '</span></td>';
            //            wspHtml += '<td class="td_Channel"><span>' + this.ChannelName + '</span></td>';
            //            wspHtml += '</tr>';
            //        });
            //        $(".productstable tbody").html(wspHtml);
            //        wspHtml = null;
            //        var y = $(this).offset().top - 40;
            //        var x = $(this).offset().left;
            //        $("#dpop").css("left", x).css("top", y).fadeIn()
            //        .mouseleave(function () {
            //            var mSelf = this;
            //            $(mSelf).off().hide();
            //            //$(this).off().fadeOut();有问题，多指几次会一闪一闪 
            //        });

            //        $(".single_1").fancybox({
            //            helpers: {
            //                title: {
            //                    type: 'float'
            //                }
            //            }
            //        });
            //    })
            //});// end of $('.bubbleInfo').each(function ()
        },// end of DynamicShow


        //DeleteCMSMedia: function (MediaID) {
        //    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
        //        if (r) {
        //            var mediaSelf = this;
        //            $('#mainarea').loadingOverlay({
        //                loadingText: 'deleting media....'
        //            });
        //            $.ajax({
        //                url: "./DeleteCMSMedia",
        //                type: "post",
        //                dataType: "json",
        //                data: {
        //                    MediaID: MediaID
        //                }
        //            }).done(function (retData) {
        //                $('#mainarea').loadingOverlay('remove');
        //                if (retData.Status == 1) {
        //                    $(".btnSearch").click();
        //                }
        //                else {
        //                    $.messager.alert('DeleteCMSMedia', retData.Data, 'error');
        //                }
        //            }).fail(function () {
        //                $('#mainarea').loadingOverlay('remove');
        //                $.messager.alert('DeleteCMSMedia', 'NetWork error when edit new channel', 'error');
        //            })
        //        }
        //    });
        //}
    }
});