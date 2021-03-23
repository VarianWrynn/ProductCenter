define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var imagepicker = require('image-picker/image-picker.min.js');
    var ImagesModel = require('./HMMedia');

    exports.Media = {

        Init: function () {
            var MediaSelf = this;
            //图像上传
            $("#btnHMImgUpload").on("click", function () {
                MediaSelf.OpenDialog();
            });         

            $(".ImgUploadOKBtn").off("click").on("click", function () {
                //这里应该重新刷新加载一下图像栏目然后关闭
                //ImagesModel.Media.RerenderMedia();
                /*change1:由于这个click触发了dialog的close事件，在该事件里已经有了Loading图片的请求，所以这里注释掉避免请求两次 2014年5月16日17:07:59*/
                $("#ImgUploadDIV").dialog("close");
            });

            $(".ImgUploadCancelBtn").off("click").on("click", function () {
                $("#ImgUploadDIV").dialog("close");
            });
        },
        //打开图像选择的弹出框
        OpenDialog: function () {

            var W = $(window).width();
            var h = $(window).height();

            $(window).resize(function () {
                var nW = $(window).width();
                var nh = $(window).height();
                if (!$('#ImgUploadDIV').is(":hidden")) {
                    $('#ImgUploadDIV').dialog({ width: nW - 100, height: nh - 50 });//这个才是正确的。。。 2014年1月24日14:12:07
                    $("#ifmMediaUpload").css({ width: nW - 150, height: nh - 200 });
                }
            });

            $("#ImgUploadDIV").show(); //必须先show，否则里面的div input等html结构会看不到....

            $("#ImgUploadDIV").dialog({
                title: 'Upload media for HMNUM:   ' + $("#HMNUM").val(),
                width: W - 100,
                height: h - 50,
                closed: false,
                cache: false,
                modal: true,
                onOpen: function () {
                    /*原先这个iframe的地址是直接写在Razor页面的，导致的行为之一是一打开SKU页面就直接执行Media/FileUpload页面所有的脚本，现在改成每次打开动态赋予URL，关闭移除URL*/
                    var url = src = "../Media/FilesUpload?ReqIndicator=1&ProductID=" + $("#hiddenProductID").val();
                    $("#ifmMediaUpload").attr("src", url);
                },
                onClose: function () {
                    //这里应该重新刷新加载一下图像栏目然后关闭                    
                    ImagesModel.Media.RerenderMedia();
                }
            });
            $("#ifmMediaUpload").css({ width: W - 150, height: h - 200 });
        }
    }
});