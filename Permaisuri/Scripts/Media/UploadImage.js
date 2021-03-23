
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var ajaxForm = require('./jquery.form');
    var imgTools = require('./imageTools');
    var W = $(window).width();
    var h = $(window).height();
    $(document).ready(function () {
        $(".filebtn").on("change", function () {
            $('#user_head_upload_box').hide();
            $('#user_head_show_box').show();
            $('#user_head_form').submit();
        });

        $(".btnno").on("click", function () {
            imgTools.cancelImage();
        });

        $("#user_head_form").ajaxForm({
            success: function (data) {
                $('#user_head_upload_box').show();
                $('#user_head_show_box').hide();
                if (data != undefined && data != null) {
                    if (data.msg == 0) {
                        //showreward("<span class=\"g_error\">请上传图片！</span>");
                        $.messager.alert("ImageUploadingg", "please chose a image！", "error");
                    } else if (data.msg == -1) {
                        //showreward("<span class=\"g_error\">文件格式不对！</span>");
                        $.messager.alert("ImageUploadingg", "file formate is not correct！", "error");
                    } else if (data.msg == -2) {
                        //showreward("<span class=\"g_error\">上传图片不能超过10M！</span>");
                        $.messager.alert("ImageUploadingg", "imgae size more than 10M！", "error");
                    } else if (data.msg == -3) {
                        //showreward("<span class=\"g_error\">出现异常，请稍后再试！！</span>");
                        $.messager.alert("ImageUploadingg", "Upload image exception!", "error");
                    } else {
                        var path = "../MediaLib/UploadImage/" + data.msg;
                        $("#head_name").val(data.msg);
                        imgTools.ImagesUtil.initialize(path);   
                    }
                }
            }
        });


        $("#user_head_param_form").ajaxForm({
            success: function (data) {
                if (data.Status === 1) {
                    var customerMediaPath = $("#CustomerMediaPath").val();
                    $('img#user_head_origin').imgAreaSelect({ remove: true });
                    $("#user_head_show_box").hide();
                    $("#user_head_upload_box").show();
                    var urls = window.location.origin + customerMediaPath+"/100/" + data.Data["name"] + "_100" + data.Data["ext"];
                    window.parent.jQuery(".addmediabtn").before('<div class="liberyhld" id=' + data.Data["newID"] + '> <img src=' + urls + '><span><img  class="mediaTrash" value=' + data.Data["newID"] + '  src="../Content/Products/images/trash.png"></span></div>');

                    //re-bind image event
                    window.parent.jQuery(".mediaTrash").css("cursor", "pointer").on("click", function () {
                        var imgID = $(this).attr("value");
                        window.parent.jQuery.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                            if (r) {
                                //window.parent.RemoveImageModel.RemoveImage($.trim(imgID));
                                window.parent.ImagesModel.ImagesModel.OpenImgDialog($.trim(imgID));
                            }
                        });
                    });
                    window.parent.jQuery('#imgUploadDiv').window('close');
                } else {
                    $.messager.alert("ImageSave", data.Data, "error");
                }
            }
        });

    });// end of documet.ready
});