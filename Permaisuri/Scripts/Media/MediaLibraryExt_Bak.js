
define(function (require, exports, module) {
    var $ = require('jquery');
    var fancybox = require('FancyBox/jquery.fancybox');
    var ChangeNameModel = require('./MediaNameChanged');
    var ui = require('jquery-ui');
    var overlayModel = require('PlugIn/Overlay/loading-overlay.min');


    /// <summary>
    /// 动态渲染图像及其信息。
    /// Dynamic render the image and imgae's information by server data
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
            var dList = retData["list"];
            $("#div_picture").html("");
            $.each(dList, function () {
                var pic_div = '';
                pic_div += '<div class="bubbleInfo" id="hidMediaID' + this.MediaID + '"><img height="160" width="160" alt="' + this.ImgName;
                pic_div += '" oName="' + this.ImgName;
                pic_div += '" hidMediaID=' + this.MediaID;
                pic_div += ' date="' + this.strCreateOn;
                pic_div += '" size="' + this.fileSize;
                pic_div += '" HMNUM="' + this.HMNUM;
                pic_div += '" format="' + this.fileFormat;
                pic_div += '" CloudStatusID="' + this.CloudStatusID;

                pic_div += '" resolution="' + this.fileWidth + ' x ' + this.fileHeight;
                
                pic_div += '" class="trigger" src="' + cmsURL + this.HMNUM + '/' + this.ImgName + '_th' + this.fileFormat;
                pic_div += '" /></div>';
                $("#div_picture").append(pic_div);
                $("#hidMediaID" + this.MediaID).data("CMS_SKU", this.CMS_SKU);//$("#ID").data("CMS_SKU")
            });

            $('.bubbleInfo').each(function () {
                $(this).mouseover(function () {
                    
                    var fname = $('img', this).attr("oName");
                    var CloudStatusID = $('img',this ).attr('CloudStatusID');
                  
                    var imgUrl = (CloudStatusID==4?cloudURL:cmsURL) + $('img', this).attr("HMNUM") + "/" + $('img', this).attr("oName") + $('img', this).attr("format");//原图
                    $("#dynamicImg").attr("src", $('img', this).attr("src")).attr("width", 100).attr("height", 100);
                   
                    $("#single_1").attr("href", imgUrl).attr("title", fname);
                    var oldText = '';
                    var mediaID = $('img', this).attr("hidMediaID");
                    var fileExt = $('img', this).attr("format");
                    $("#span_Name").text($('img', this).attr("oName"));

                    $(".deleteImage").off().on("click", function () {
                        mediaSelf.DeleteCMSMedia(mediaID);
                    });
                    //$("#span_Name").text($('img', this).attr("oName")).off().on("click", function () {
                    //    //找到当前鼠标点击的td,this对应的就是响应了click的那个td
                    //    var tdObj = $(this);
                    //    if (tdObj.children("input").length > 0) {
                    //        //当前td中input，不执行click处理
                    //        return false;
                    //    }
                    //    var oldText = tdObj.html();
                    //    tdObj.html("");
                    //    var inputObj = $("<input type='text'>").css("border-width", "1")
                    //        .width(tdObj.width()-10)
                    //        //.css("background-color", tdObj.css("background-color"))
                    //        .val(oldText).appendTo(tdObj);
                    //    //是文本框插入之后就被选中
                    //    inputObj.trigger("focus").trigger("select");
                    //    inputObj.click(function () {
                    //        return false;
                    //    });
                    //    //处理文本框上回车和esc按键的操作
                    //    inputObj.keyup(function (event) {
                    //        //获取当前按下键盘的键值
                    //        var keycode = event.which;
                    //        //处理回车的情况
                    //        if (keycode == 13) {
                    //            //获取当当前文本框中的内容
                    //            var inputtext = $(this).val();
                    //            //将td的内容修改成文本框中的内容
                    //            tdObj.html(inputtext);
                    //        }
                    //        //处理esc的情况
                    //        if (keycode == 27) {
                    //            //将td中的内容还原成text
                    //            tdObj.html(oldText);
                    //        }
                    //    }).on("blur", function () {
                    //        var inPutObj = this;//切换到onBlure的时候，this由原来的td切换到了现在的input
                    //        var curText = $.trim($(inPutObj).val());
                    //        var fileNamepatn = /\||<|>|\?|\*|:|\/|\\|"/;//匹配文件名是否合法
                    //        if (curText == "") {
                    //            $(inPutObj).parent().html(oldText);
                    //        } else if (fileNamepatn.test(curText)) {
                    //            $(inPutObj).parent().html(oldText);
                    //        } else if (oldText == curText) {
                    //            {
                    //                $(inPutObj).parent().html(curText);

                    //            }
                    //        } else {
                    //            var postData = {
                    //                mediaID: mediaID,
                    //                newFileName: curText,
                    //                oldFileName: oldText,
                    //                fileExt: fileExt
                    //            };
                    //            ChangeNameModel.ChangedNameObj.ChangedImageName(postData, $(inPutObj).parent());
                    //        }
                    //    });
                    //});
                    $("#td_Format").text($('img', this).attr("format"));
                    $("#td_Size").text($('img', this).attr("size"));
                    $("#td_Date").text($('img', this).attr("date")); 
                    $("#td_Resolution").text($('img', this).attr("resolution"));
                    //var oldDesc = '';
                    //$("#td_Desc").text($('img', this).attr("desc")).off().on("click", function () {
                    //    var tdObj = $(this);
                    //    if (tdObj.children("input").length > 0) {return false;}
                    //    var oldDesc = tdObj.html();
                    //    tdObj.html("");
                    //    var inputObj = $("<input type='text'>").css("border-width", "1").width(tdObj.width() - 10).val(oldDesc).appendTo(tdObj);
                    //    inputObj.trigger("focus").trigger("select");
                    //    inputObj.click(function () { return false;});
                    //    inputObj.keyup(function (event) {
                    //        var keycode = event.which;
                    //        if (keycode == 13) {
                    //            var inputtext = $(this).val();
                    //            tdObj.html(inputtext);
                    //        }
                    //        if (keycode == 27) { tdObj.html(oldDesc); }
                    //    }).on("blur", function () {
                    //        var inPutObj = this;
                    //        var curText = $.trim($(inPutObj).val());
                    //        if (curText == "" || oldDesc == curText) {
                    //            $(inPutObj).parent().html(oldDesc);
                    //        } else {
                    //            var postData = {
                    //                mediaID: mediaID,
                    //                newDesc: curText,
                    //                oldDesc: oldDesc
                    //            };
                    //            ChangeNameModel.ChangedNameObj.ChangedDescriptionName(postData,$(inPutObj).parent());
                    //        }
                    //    });
                    //});
                    $("#h2_url").text(imgUrl);
                    //alert(JSON.stringify($("#" + this.id).data("CMS_SKU")));
                    var CMS_SKU = $("#" + this.id).data("CMS_SKU");
                    var wspHtml = '';
                    $.each(CMS_SKU, function () {
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

                    $("#dpop").css("left", x).css("top", y).fadeIn()
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


        DeleteCMSMedia: function (MediaID) {
            $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                if (r) {
                    var mediaSelf = this;
                    $('#mainarea').loadingOverlay({
                        loadingText: 'deleting media....'
                    });
                    $.ajax({
                        url: "./DeleteCMSMedia",
                        type: "post",
                        dataType: "json",
                        data: {
                            MediaID: MediaID
                        }
                    }).done(function (retData) {
                        $('#mainarea').loadingOverlay('remove');
                        if (retData.Status == 1) {
                            $(".btnSearch").click();
                        }
                        else {
                            $.messager.alert('DeleteCMSMedia', retData.Data, 'error');
                        }
                    }).fail(function () {
                        $('#mainarea').loadingOverlay('remove');
                        $.messager.alert('DeleteCMSMedia', 'NetWork error when edit new channel', 'error');
                    })
                }
            });
        }
    }
});