/// <summary>
/// HMNUMConfiguration 价格模块的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var UIExtModel = require('Common/EasyUIExt');

    //原始价格，该价格用来cancel按钮的时候复原，注意Ajax提交价格之后，需要重新赋值
    var OriginalCost = {
        //SalePrice: $("#SalePrice").val(),
        //EstimateFreight_SKU: $("#EstimateFreight_SKU").val()
    }

    exports.SKUCosting = {

        Init: function () {
            var InitSelf = this;

            OriginalCost.SalePrice = $("#SalePrice").val();
            OriginalCost.EstimateFreight_SKU = $("#EstimateFreight_SKU").val();

            /*JQuery 限制文本框只能输入数字和小数点,需要注意off()会取消所有的事件，包括keyup*/
            $(".hmCost").keyup(function () {
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).bind("paste", function () {  //CTR+V事件处理    
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).css("ime-mode", "disabled"); //CSS设置输入法不可用    


            //校验
            UIExtModel.Ext.ValidateCosting();
            $("#SalePrice").validatebox({
                required: true,
                validType: "ValidateCosting['#FirstCost']"
            });

            $(".editCosting").off("click").on("click", function () {
                //S1:隐藏Edit按钮，显示done按钮
                $(".editCosting").hide();
                $(".submitCosting").show();
                $(".cancelCosting").show();

                //需要注意off('click') 和off('blur')很重要 否则会出现多次blure,导致最后展示的value出现多个$符号...
                $("#SalePrice,#EstimateFreight_SKU").prop("disabled", false).off('click').on("click", function () {
                    var self = this;
                    //将当前文本框的美元符号替代掉
                    $(self).val($(self).val().replace('$', ''));

                    $(self).off('blur').on('blur', function () {
                        var newVal = $(this).val().replace('$', '');//不知道什么原因还是会出现多个$， 我屮艸芔茻
                        $(self).val("$" + newVal);
                    });
                }).css({ "color": "red" });
                $("#SalePrice").focus().click();
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
                $("#SalePrice").val(OriginalCost["SalePrice"]);
                $("#EstimateFreight_SKU").val(OriginalCost["EstimateFreight_SKU"]);
                InitSelf.EndEditModel();
            });
        },// end of FUNC init()

        //退出编辑模式之后还原原来的样式，多处需要引用，故而提取
        EndEditModel: function () {
            $(".editCosting").show();
            $(".submitCosting").hide();
            $(".cancelCosting").hide();
            $("#SalePrice,#EstimateFreight_SKU").prop("disabled", true).css({ "color": "#666666" });
        },

        EditCosting: function (callback) {
            var temPrice = $.trim($("#SalePrice").val().replace('$', ''));//解决用户clear输入框，blur时候，里面还有一个$号提交为空的问题
            if (temPrice == "")
            {
                $("#SalePrice").val("");
            }

            var temPrice = $.trim($("#EstimateFreight_SKU").val().replace('$', ''));//解决用户clear输入框，blur时候，里面还有一个$号提交为空的问题
            if (temPrice == "") {
                $("#EstimateFreight_SKU").val("");
            }

            var fvalidate = $("#HMCostingForm").form('validate');
            if (!fvalidate) {
                return;
            }

            var fvalidate2 = $("#AddHMForm").form('validate');
            if (!fvalidate2) {
                return;
            }

            var postData = {
                SKUID: $("#hiddenWPID").val(),
                SalePrice: $.trim($("#SalePrice").val().replace('$', '')),
                EstimateFreight: $.trim($("#EstimateFreight_SKU").val().replace('$', ''))
            };
            progress.show();
            $.ajax({
                url: "./EditSKUCosting",
                type: "POST",
                dataType: "json",
                data: postData,
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        //价格成功更新回来后，直接显示服务器返回的最新的价格
                        $("#SalePrice").val(retData.Data["SalePrice"]);
                        $("#EstimateFreight_SKU").val(retData.Data["EstimateFreight"]);
                        //，同时更新局部变量 OriginalCost,这样子方便不关闭当前页面进行第二次价格编辑时候，还原的是正确的Value!
                        OriginalCost.SalePrice = retData.Data["SalePrice"];
                        OriginalCost.EstimateFreight_SKU = retData.Data["EstimateFreight"];

                        if (callback) {
                            callback();
                        }

                    } else {
                        $.messager.alert('AddNewProduct-EditCosting', retData.Data, 'error');
                        return false;
                    }
                },
                error: function () {
                    progress.hide();
                    $.messager.alert('AddNewProduct-EditCosting', 'Failed to edit,please contact administrator', 'error');
                    return false;
                }
            });
        }
    }
});