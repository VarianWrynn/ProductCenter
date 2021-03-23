define(function (require, exports, module) {

    var $ = jQuery = require('jquery');
    var ui = require('jquery-ui');
    exports.MediaHM = {
        AutoHM: function () {
            var HMAutoSelf = this;
            $("#AutoHM").autocomplete({
                source: function (request, response) {
                    $("#AutoHM").addClass('auto-loading');
                    //$("#AutoName").prop("disabled",false);
                    $.ajax({
                        url: "./GetProductInfo",
                        type: "POST",
                        dataType: "json",
                        data: {
                            SKUID: $(".modelID").val(),
                            ProductName: "",
                            HMNUM: request.term
                        },
                        success: function (retData) {
                            $("#AutoHM").removeClass('auto-loading');
                            //alert(JSON.stringify(aaa));
                            response($.map(retData["Data"], function (item) {
                                return {
                                    label: item.HMNUM,
                                    value: item.HMNUM,
                                    ProductID: item.ProductID,
                                    HMNUM: item.HMNUM,
                                    ProductName: item.ProductName,
                                    MaxImaSeq: item.MaxImaSeq
                                }
                            }));

                        },
                        error: function () {
                            $("#AutoHM").removeClass('auto-loading');
                            alert("NetWork Error,Please contact administrator");
                            return false;
                        }
                    }); // end of ajax func
                },// end of source function

                minLength: 1,
                select: function (event, ui) {
                    var self = this;
                    var beforHM = $("#AutoHM").val();
                    $("#hiddenAutoHM").val(ui.item.HMNUM);//防止用户故意乱输入一个不存在HMNUM提交来坑爹 2013年11月28日16:27:55
                    $("#AutoName").val(ui.item.ProductName);
                    $("#hiddenProductID").val(ui.item.ProductID);
                    $("#hiddenMaxImaSeq").val(ui.item.MaxImaSeq);
                    addMediaFileCounts = parseInt(ui.item.MaxImaSeq);
                    $("#addSpan").show();
                    //新增：rename的逻辑处理 2013年11月28日15:10:41
                    //$.each($("input[name='rename']"), function () {
                    //    ////将之前旧的HM的数据全部替换成新的HM
                    //    //var newRename = $(this).val().replace(beforHM, $("#AutoName").val());
                    //    //$(this).val(newRename);
                    //    $(this).parent().parent().parent().find(".btn btn-warning").click();//点击取消按钮
                    //});

                    //$("#fileupload .span7 .btn btn-warning cancel").click();//Cancel upload
                    //$($("#fileupload .span7 .btn")[2]).click();
                    $("#tdCancel").click();
                }
            });// end of autocomplete ;
        }
    }
});