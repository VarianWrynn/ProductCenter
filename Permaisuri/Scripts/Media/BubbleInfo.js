
define(function (require, exports, module) {
    var $ = require('jquery');
    var fancybox = require('FancyBox/jquery.fancybox');
    var ChangeNameModel = require('./MediaNameChanged');
    //var ui = require('jquery-ui');//export this ui will conflict with easyUI, especially with the dialog(add media) 2014年5月16日16:19:02
    var overlayModel = require('PlugIn/Overlay/loading-overlay.min');

    //var zclip = require('PlugIn/zclip/jquery.zclip');


    /// <summary>
    /// Extract funtion: onMouseOver PopUp media infomation. 2014年5月16日10:51:48
    /// Dynamic render the image and imgae's information via server data
    /// </summary>
    /// <param name="cmsURL">CMS展示图片的前缀地址，该地址源自于WebConfig的配置项</param>
    /// <param name="Media_Data_list">从后台库表里面获取的图像的JSON列表</param>
    /// <param name="DeletedCallBack">成功删除当前一个图片之后需要执行的步骤，比如刷新当前图像栏</param>
    /// <param name="overlayElement">删除图像之后需要loading遮罩的范围,比如 $('#mainarea')</param>
    /// <param name="IsNeedPrimaryImage">指示当前页面是否需要展示“PirmaryImages",SKU页面和非组合产品页面都需要</param>
    /// <param name="PrimaryImageCallBack">当 IsNeedPrimaryImage 为true的时候，单击设置按钮执行的回调函数</param>
    /// <param name="cloudURL">云端的地址</param>
    /// <returns></returns>
    exports.Media = {
        //Bubble: function (cmsURL, Media_Data_list, DeletedCallBack, overlayElement, IsNeedPrimaryImage,PrimaryImageCallBack) {
        Bubble: function (options) {
            var mediaSelf = this;
            var defaults = {
                cmsURL: '',
                Media_Data_list: {},
                IsNeedCopyUrlButton: false,//是否需要CopyUrl的button
                CopyUrlButtonCallBack:function(){},//点击CopyURL之后执行的回调函数
                IsNeedDeleteButton:true,//是否需要删除按钮，默认是ture
                DeletedCallBack: function () { },
                overlayElement: $('body'),
                IsNeedPrimaryImage: false,
                PrimaryImageCallBack: function () { },
                UnattachedSKUButton: false,//是否需要“UnattachedSKUButton”
                UnattachedCallBack: function () { },// UnattachedSKUButton 回调函数
                cloudURL: "https://s3-us-west-1.amazonaws.com/media.cms.noblehouse/"
            };
            var opts = $.extend({}, defaults, options);
            mediaSelf.PopupHtmlBuilt(opts);
            $.each(opts.Media_Data_list, function () {
                $("#hidMediaID" + this.MediaID).data("Media_Data", this);//$("#ID").data("CMS_SKU")
            });

            if (!$("#dpop").is(":hidden")) {
                $("#dpop").hide();
            }

            $('.bubbleInfo').each(function () {
                var selfBubble = this;
                $(this).mouseover(function () {
                    var MediaData = $("#" + this.id).data("Media_Data");
                    var fname = MediaData.ImgName;
                    var CloudStatusID = MediaData.CloudStatusID;
                    var imgUrl = (CloudStatusID == 4 ? opts.cloudURL : opts.cmsURL) + MediaData.HMNUM + "/" + fname + MediaData.fileFormat;//原图
                    $("#dynamicImg").attr("src", $('img', this).attr("src")).attr("width", 140).attr("height", 140);
                    $("#single_1").attr("href", imgUrl).attr("title", fname);
                    var oldText = '';
                    var mediaID = MediaData.MediaID;
                    var fileExt = MediaData.fileFormat;
                    $("#span_Name").text(fname);

                    $(".copyUrl").off().on("click", function () {
                        mediaSelf.CopyUrl("aaaaaaaaaaaa");
                    });

                    $(".deleteImage").off().on("click", function () {
                        mediaSelf.DeleteCMSMedia(mediaID, opts.DeletedCallBack, opts.overlayElement);
                    });

                    $(".unattachSKU").off().on("click", function () {
                        $.messager.confirm('Confirm', 'Are you sure to unattach this media from current SKU?', function (r) {
                            if (r) {
                                if (opts.UnattachedCallBack) {
                                    opts.UnattachedCallBack(MediaData.MediaID);
                                }
                            }
                        });
                    });

                    $(".primaryImage").off().on("click", function () {
                        $.messager.confirm('Confirm', 'Are you sure to set this image as primary iamge?', function (r) {
                            if (r) {
                                if (opts.PrimaryImageCallBack) {
                                    //这里用当前的对象赋予整个方法 2014年5月19日10:55:45 有点难理解 需要耐心
                                    opts.PrimaryImageCallBack(selfBubble);
                                }
                            }
                        });
                    });

                    $("#td_Format").text(MediaData.fileFormat);
                    $("#td_Size").text(MediaData.fileSize);
                    $("#td_Date").text(MediaData.strCreateOn);
                    $("#td_Resolution").text(MediaData.fileWidth + ' x ' + MediaData.fileHeight);
                    $("#h2_url").text(imgUrl);
                    var wspHtml = '';
                    $.each(MediaData.CMS_SKU, function () {
                        wspHtml += '<tr>';
                        wspHtml += '<td><span class="span_SKU">' + this.SKU + '</span></td>';
                        wspHtml += '<td><span class="span_ProductName">' + this.ProductName + '</span></td>';
                        wspHtml += '<td class="td_Channel"><span>' + this.ChannelName + '</span></td>';
                        wspHtml += '</tr>';
                    });
                    $(".productstable tbody").html(wspHtml);
                    wspHtml = null;
                    var y = $(this).offset().top - 40;
                    var x = $(this).offset().left;

                    //鼠标移除层事件...
                    $("#dpop")
                        .css("left", x).css("top", y).fadeIn()
                        .mouseleave(function () {
                            var mSelf = this;
                            $(mSelf).off().hide();
                            //$(this).off().fadeOut();有问题，多指几次会一闪一闪 
                        });

                    $(".single_1").fancybox({
                        helpers: {
                            title: {
                                type: 'float'
                            }
                        }
                    });
                })
            });// end of $('.bubbleInfo').each(function ()
        },// end of DynamicShow

        //构建弹出层的展示信息，比如展示url ,分辨率 and so on.
        PopupHtmlBuilt: function (opts) {
            var popHmtl = '';
            popHmtl += '<table id="dpop" class="popup" style="opacity: 1; top: 20px; left: 30px; display: none">';
            popHmtl += '  <tbody>';
            popHmtl += '       <tr>';
            popHmtl += '        <td id="topleft" class="corner"></td>';
            popHmtl += '                 <td class="top"></td>';
            popHmtl += '                <td id="topright" class="corner"></td>';
            popHmtl += '              </tr>';
            popHmtl += '            <tr>';
            popHmtl += '               <td class="left"></td>';
            popHmtl += '               <td>';
            popHmtl += '                   <div style="float: left">';
            popHmtl += '                        <a id="single_1" class="single_1" href="" title="">';
            popHmtl += '                            <img id="dynamicImg" alt="" style="float: left; margin-right: 20px;">';
            popHmtl += '                       </a>';
            popHmtl += '                    </div>';
            popHmtl += '                   <table class="popupname" width="60%" cellspacing="0" cellpadding="5" border="0">';
            popHmtl += '                       <tbody>';
            popHmtl += '                          <tr>';
            popHmtl += '                              <td height="25px">Name:</td>';
            popHmtl += '                             <td><span id="span_Name"></span></td>';
            popHmtl += '                        </tr>';
            popHmtl += '                        <tr>';
            popHmtl += '                            <td height="25px">Format:</td>';
            popHmtl += '                           <td id="td_Format"></td>';
            popHmtl += '                      </tr>';
            popHmtl += '                       <tr>';
            popHmtl += '                           <td height="25px">Size:</td>';
            popHmtl += '                          <td id="td_Size"></td>';
            popHmtl += '                    </tr>';
            popHmtl += '                    <tr>';
            popHmtl += '                        <td height="25px">Date:</td>';
            popHmtl += '                       <td id="td_Date"></td>';
            popHmtl += '                    </tr>';
            popHmtl += '                    <tr>';
            popHmtl += '                        <td height="25px">Resolution:</td>';
            popHmtl += '                         <td id="td_Resolution"></td>';
            popHmtl += '                     </tr>';
            popHmtl += '                  </tbody>';
            popHmtl += '              </table>';
            popHmtl += '              <div style ="clear:both">';
            if (opts.IsNeedCopyUrlButton) {
                popHmtl += '                    <span><a class="buttonCMS greenCMS copyUrl" style="margin: 3px 10px 0 0; ">Copy Url </a></span>';
            }
            if (opts.IsNeedDeleteButton) {
                popHmtl += '                    <span><a class="buttonCMS greenCMS deleteImage" style="margin: 3px 10px 0 0; ">Delete </a></span>';
            }
            if (opts.UnattachedSKUButton) {
                popHmtl += '                    <span><a class="buttonCMS greenCMS unattachSKU" style="margin: 3px 10px 0 0; ">Unattach SKU </a></span>';
            }
            if (opts.IsNeedPrimaryImage) {
                popHmtl += '                    <span><a class="buttonCMS greenCMS primaryImage" style="margin: 3px 0 0 0; "> Set Primary</a></span>';
            }
            popHmtl += '              </div>';
            popHmtl += '              <h2 id="h2_url"></h2>';
            popHmtl += '              <p class="p_showing">Attached to SKUs:</p>';
            popHmtl += '             <table class="productstable" width="100%" cellspacing="0" cellpadding="5" border="0">';
            popHmtl += '                 <thead>';
            popHmtl += '                       <tr>';
            popHmtl += '                          <td>SKU</td>';
            popHmtl += '                       <td>SKUName</td>';
            popHmtl += '                        <td>Channel</td>';
            popHmtl += '                    </tr>';
            popHmtl += '                  </thead>';
            popHmtl += '                  <tbody></tbody>';
            popHmtl += '              </table>';
            popHmtl += '           </td>';
            popHmtl += '           <td class="rignt"></td>';
            popHmtl += '       </tr>';
            popHmtl += '       <tr>';
            popHmtl += '          <td name="topleft" class="corner"></td>';
            popHmtl += '          <td class="top"></td>';
            popHmtl += '          <td name="topright" class="corner"></td>';
            popHmtl += '      </tr>';
            popHmtl += '   </tbody>';
            popHmtl += '   </table>';
            $("#div_showOrhide").html(popHmtl);
        },


        DeleteCMSMedia: function (MediaID, DeletedCallBack, overlayElement) {
            $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                if (r) {
                    var mediaSelf = this;
                    overlayElement.loadingOverlay({
                        loadingText: 'deleting media....'
                    });
                    $.ajax({
                        url: "../Media/DeleteCMSMedia",
                        type: "post",
                        dataType: "json",
                        data: {
                            MediaID: MediaID
                        }
                    }).done(function (retData) {
                        overlayElement.loadingOverlay('remove');
                        if (retData.Status == 1) {
                            if (DeletedCallBack) {
                                DeletedCallBack();
                            }
                        }
                        else {
                            $.messager.alert('DeleteCMSMedia', retData.Data, 'error');
                        }
                    }).fail(function () {
                        overlayElement.loadingOverlay('remove');
                        $.messager.alert('DeleteCMSMedia', 'NetWork error when edit new channel', 'error');
                    })
                }
            });
        },


        CopyUrl: function (urlContent) {
        //    if (typeof (urlContent) == "undefined") {
        //        return false;
        //    }

        //    $(".copyUrl").zclip({
        //        path: '../../Scripts/PlugIn/zclip/ZeroClipboard.swf',
        //        copy: "aaa"
        //    });
        }

    }
});