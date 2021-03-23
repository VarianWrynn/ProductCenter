/// <summary>
/// HMNUM 组合产品 价格模块的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    //原始价格，该价格用来cancel按钮的时候复原，注意Ajax提交价格之后，需要重新赋值
    var OriginalCost = {
        //FirstCost: $("#FirstCost").val(),
        //OceanFreight: $("#OceanFreight").val(),
        //Drayage: $("#Drayage").val(),
        //USAHandlingCharge: $("#USAHandlingCharge").val()
    }

    exports.HMCosting = {
        Init: function () {
            var InitSelf = this;

            OriginalCost.FirstCost = $("#FirstCost").val();
            OriginalCost.OceanFreight = $("#OceanFreight").val();
            OriginalCost.Drayage = $("#Drayage").val();
            OriginalCost.USAHandlingCharge = $("#USAHandlingCharge").val();

            /*JQuery 限制文本框只能输入数字和小数点,需要注意off()会取消所有的事件，包括keyup*/
            $(".hmCost").keyup(function () {
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).bind("paste", function () {  //CTR+V事件处理    
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).css("ime-mode", "disabled"); //CSS设置输入法不可用    

            $(".editCosting").off("click").on("click", function () {
                $("#Status").val("Adjust Costing");
                //S1:隐藏Edit按钮，显示done按钮
                $(".editCosting").hide();
                $(".submitCosting").show();
                $(".cancelCosting").show();

                //需要注意off('clicl') 和off('blur')很重要 否则会出现多次blure,导致最后展示的value出现多个$符号...
                $(".hmCost").prop("disabled", false).off('click').on("click", function () {
                    var self = this;
                    //将当前文本框的美元符号替代掉
                    $(self).val($(self).val().replace('$', ''));

                    $(self).off('blur').on('blur', function () {
                        var newVal = $(this).val().replace('$', '');//不知道什么原因还是会出现多个$， 我屮艸芔茻
                        $(self).val("$" + newVal);
                    });
                }).css({ "color": "red" });
                $("#FirstCost").focus().click();
                return false;
            });

            $(".submitCosting").off("click").on("click", function () {
                //s1:获取价格提交通过AJAX提交到Server
                //S?:隐藏done/cacel按钮，显示Edit按钮
                //Ajax updateing
                InitSelf.EditCosting(function () {
                    InitSelf.EndEditModel();
                });

            });

            //取消价格变化，复原原价格
            $(".cancelCosting").off("click").on("click", function () {
                $("#FirstCost").val(OriginalCost["FirstCost"]);
                $("#OceanFreight").val(OriginalCost["OceanFreight"]);
                $("#Drayage").val(OriginalCost["Drayage"]);
                $("#USAHandlingCharge").val(OriginalCost["USAHandlingCharge"]);
                InitSelf.EndEditModel();
            });
        },// end of FUNC init()

        //退出编辑模式之后还原原来的样式，多处需要引用，故而提取
        EndEditModel:function()
        {
            $(".editCosting").show();
            $(".submitCosting").hide();
            $(".cancelCosting").hide();
            $(".hmCost").prop("disabled", true).css({ "color": "#666666" });
        },

        EditCosting: function (callback) {
            var postData = {
                ProductID: $("#hiddenProductID").val(),
                FirstCost: $.trim($("#FirstCost").val().replace('$', '')),
                OceanFreight: $.trim($("#OceanFreight").val().replace('$', '')),
                Drayage: $.trim($("#Drayage").val().replace('$', '')),
                USAHandlingCharge: $.trim($("#USAHandlingCharge").val().replace('$', '')),
                HMNUM: $.trim($("#HMNUM").val())
            };
            progress.show();
            $.ajax({
                url: "./EditHMGroupCosting",
                type: "POST",
                dataType: "json",
                data: postData,
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        //价格成功更新回来后，直接显示服务器返回的最新的价格
                        $("#FirstCost").val(retData.Data["FirstCost"]);
                        $("#OceanFreight").val(retData.Data["OceanFreight"]);
                        $("#Drayage").val(retData.Data["Drayage"]);
                        $("#USAHandlingCharge").val(retData.Data["USAHandlingCharge"]);



                        //，同时更新局部变量 OriginalCost,这样子方便不关闭当前页面进行第二次价格编辑时候，还原的是正确的Value!
                        OriginalCost.FirstCost = retData.Data["FirstCost"];
                        OriginalCost.OceanFreight = retData.Data["OceanFreight"];
                        OriginalCost.Drayage = retData.Data["Drayage"];
                        OriginalCost.USAHandlingCharge = retData.Data["USAHandlingCharge"];

                        if (callback)
                        {
                            callback();
                        }

                    } else {
                        $.messager.alert('HMConfiguration-EditHMNUM', retData.Data, 'error');
                        return false;
                    }
                },
                error: function () {
                    progress.hide();
                    $.messager.alert('HMConfiguration-EditHMNUM', 'Failed to edit,please contact administrator', 'error');
                    return false;
                }
            });
        }
    }
});