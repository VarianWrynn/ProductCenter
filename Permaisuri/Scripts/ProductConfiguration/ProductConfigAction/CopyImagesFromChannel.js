define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var imagepicker = require('image-picker/image-picker.min.js');
    var imgOperModel = require('./ImageOperations');

    exports.CopyImage = {

        //打开图像选择的弹出框
        OpenDialog: function () {
           var  CopyImageSelf = this;
            $(".copyOtherlOKBtn").off("click").on("click", function () {
                $("#copyOtherModal").dialog("close");
            });

            $(".copyOtherCancelBtn").off("click").on("click", function () {
                $("#copyOtherModal").dialog("close");
            });

            var W = $(window).width();
            var h = $(window).height();
            var curSKU = $("#SKU").val();

            $(window).resize(function () {
                var W = $(window).width();
                var h = $(window).height();
                if (!$('#copyOtherModal').is(":hidden")) {
                    $('#copyOtherModal').dialog({
                        width: W - 20,
                        height: h - 20
                    });
                }
            });

            $("#copyOtherModal").show(); //必须先show，否则里面的div input等html结构会看不到....

            $("#copyOtherModal").dialog({
                title: 'Copy Images from Other Channel for SKU:' + $("#SKU").val(),
                width: W - 30,
                height: h - 20,
                closed: false,
                cache: false,
                modal: true,
                onOpen: function () {
                    CopyImageSelf.GetChannelsByHM(); 
                },
                onClose: function () {
                    
                },
                buttons:[{
                    text:'Save',
                    handler: function () {

                        var copySKUID = $("#hiddenCopySKUID").val();
                        if (parseInt(copySKUID) === 0)//说明没有选择Channel或者选择那个item根本就木有图片！
                        {
                            $("#copyOtherModal").dialog("close");
                            return false;
                        }
                        CopyImageSelf.CopyImagesFromOtherSKUID(copySKUID)
                    }
                },{
                    text:'Close',
                    handler: function () {
                        $("#copyOtherModal").dialog("close");
                    }
                }]


            });
        },

        //获取当和当前SKU引用了共同HMNUM的其他Channel的SKU列表
        GetChannelsByHM: function () {
            var copySelf = this;
            $("#OtherChannel").addClass('auto-loading');
            $.ajax({
                url: "./GetChannelsByHM",
                type: "POST",
                dataType: "json",
                data: {
                    StockKey: $("#hiddenStockKey").val()
                }
            }).done(function (retData) {
                $("#OtherChannel").removeClass('auto-loading');
                if (retData.Status === 1) {
                    var selHTML = '';
                    var curSKUID = $("#hiddenSKUID").val();
                    $.each(retData["Data"], function () {

                        selHTML += '<option value="0"> </option>';
                        if (curSKUID == this.SKUID) {
                            //selHTML += '<option value=' + this.SKUID + ' selected >' + this.ChannelName + '</option>';
                        } else {
                            selHTML += '<option value=' + this.SKUID + '>' + this.ChannelName + '</option>';
                        }
                    })
                    $("#OtherChannel").html(selHTML);
                    $("#OtherChannel").chosen({
                        placeholder_text_multiple: "Click here to select Channels.",
                        display_selected_options: false,
                        //selected:false
                    });
                    $("#OtherChannel").on('change', function () {
                        var SKUID = $(this).val();
                        if (parseInt(SKUID) === 0)
                        {
                            return;
                        }
                        //根据当前的SKUID获取Images 展示到 copyDisplayDiv DIV上
                        copySelf.GetImageList(SKUID);
                    });

                } else {
                    $.messager.alert('CopyImagesFormOtherChannel', retData.Data, 'error');
                }
            }).fail(function () {
                $("#OtherChannel").removeClass('auto-loading');
                $.messager.alert('CopyImagesFormOtherChannel', 'NetWork Error,Please contact administrator。', 'error');
            })// end of ajax func
        }, // end of func

        GetImageList: function (SKUID) {
            $.ajax({
                url: "./GetImageListByWPID",
                type: "POST",
                dataType: "json",
                data: {
                    SKUID: SKUID
                },
                success: function (retData) {
                    if (retData.Status === 1) {
                        var strHTML = "";
                        var cmsURL = $("#hiddenCMSImgUrl").val();
                        $.each(retData["Data"]["List"], function () {
                            var fName = this.ImgName + this.fileFormat;
                            var thfName = this.ImgName + "_th" + this.fileFormat;
                            strHTML += '<div class="liberyhld" id="' + this.MediaID + '">';
                            strHTML += '<img  src="' + cmsURL + this.HMNUM + '/' + thfName + '">';
                            strHTML += '</div>';
                        });
                        if (strHTML == "") {
                            strHTML = "<h1>This item does not have contain any images</h1>";
                            $("#hiddenCopySKUID").val("0");
                        }
                        $("#copyDisplayDiv").html(strHTML);
                        //alert(SKUID);
                        $("#hiddenCopySKUID").val(SKUID);
                        
                    } else {
                        $.messager.alert('GetImageList', retData.Data, 'error');
                    }
                },
                error: function () {
                    $.messager.alert('GetImageList', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }, // func: GetImageList


        CopyImagesFromOtherSKUID: function (OtherSKUID) {
            var SKUID = $("#hiddenSKUID").val();
            $.ajax({
                url: "./CopyImagesFromOtherSKUID",
                type: "POST",
                dataType: "json",
                data: {
                    SKUID: SKUID,
                    OtherSKUID: OtherSKUID
                },
                success: function (retData) {
                    if (retData.Status === 1) {
                        imgOperModel.ImagesModel.RerenderSKUMedia(SKUID);
                        $("#copyOtherModal").dialog("close");

                    } else {
                        $.messager.alert('CopyImagesFromOtherSKU', retData.Data, 'error');
                    }
                },
                error: function () {
                    $.messager.alert('CopyImagesFromOtherSKU', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }, // func: GetImageList
    }
});