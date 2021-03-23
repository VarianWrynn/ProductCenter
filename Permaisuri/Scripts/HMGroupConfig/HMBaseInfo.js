/// <summary>
/// HMNUMConfiguration 基本信息模块的维护
///2013年11月14日14:27:22
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    //原始HM信息，该价格用来cancel按钮的时候复原
    var HMData = {
         //StockKey: $("#StockKey").val(),
        //Comments: $("#Comments").val(),
        //StatusID: $("#hiddenStatusID").val()
    }

    exports.HM = {

        Init: function () {
            var InitSelf = this;
           
            HMData.Comments = $("#Comments").val();
            HMData.StatusID = $("#hiddenStatusID").val();
            HMData.ShipViaType = $("#ShipViaType").combobox("getValue");
            HMData.NetWeight = $("#NetWeight").val();
            $("#ChildrenHMForm input").prop("readonly", true).css({ "color": "#666666" });
            $(".editHMNUM").off("click").on("click", function () {
                //S1:隐藏Edit按钮，显示done按钮
                $(".editHMNUM").hide();
                $(".submitHMNUM").show();
                $(".cancelHMNUM").show();
                $('#Status').combobox("enable");
                $('#ShipViaType').combobox("enable");
                $("#Comments,#NetWeight").prop("readonly", false).css({ "color": "red" });
                return false;
            });

            $(".submitHMNUM").off("click").on("click", function () {
                InitSelf.Edit(function () {
                    InitSelf.EndEditModel(
                    function () {//更新基础信息
                        //HMData.StockKey = $("#StockKey").val();
                        HMData.Comments = $("#Comments").val();
                        HMData.StatusID = $('#Status').combobox('getValue');
                        HMData.ShipViaType = $('#ShipViaType').combobox('getValue');
                        HMData.NetWeight = $('#NetWeight').val();
                    }
                )
                });
            });

            //取消按钮，复原原信息
            $(".cancelHMNUM").off("click").on("click", function () {
                InitSelf.EndEditModel(function () {//还原基础信息
                    //$("#StockKey").val(HMData["StockKey"]);
                    $("#Comments").val(HMData["Comments"]);
                    $('#Status').combobox('setValue', HMData["StatusID"]);
                    $('#ShipViaType').combobox('setValue', HMData["ShipViaType"]);
                    $("#NetWeight").val(HMData["NetWeight"]);
                });
            });
        },// end of FUNC init()

        //退出编辑模式之后还原原来的样式，多处需要引用，故而提取
        EndEditModel:function(callBack)
        {
            $(".editHMNUM").show();
            $(".submitHMNUM").hide();
            $(".cancelHMNUM").hide();
            $('#Status').combobox("disable");
            $('#ShipViaType').combobox("disable");
            $("#Comments,#NetWeight").prop("readonly", true).css({ "color": "#666666" });
            if (callBack)
            {
                callBack();
            }
        },

        Edit: function (callback) {
            //校验合法性 2013年12月10日10:14:00
            var fvalidate = $("#HMBaseForm").form('validate');
            if (!fvalidate) {
                return;
            }

            var postData = {
                ProductID: $("#hiddenProductID").val(),
                //StockKey: "0",//组合产品写死
                StockKey: $.trim($("#StockKey").val()),//必须送到后台，否则EF对象校验不过 2014年2月18日15:13:28
                Comments: $.trim($("#Comments").val()),
                StatusID: $("#Status").combobox("getValue"),
                ShipViaTypeID: $("#ShipViaType").combobox("getValue"),
                NetWeight: $.trim($("#NetWeight").val()),
            };
            progress.show();
            $.ajax({
                url: "./EditHMBasicInfo",
                type: "POST",
                dataType: "json",
                data: postData,
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        if (callback) {
                            callback();
                        }
                    } else {
                        $.messager.alert('HMConfiguration-EditHMNUMBasicInfo', retData.Data, 'error');
                        return false;
                    }
                },
                error: function () {
                    progress.hide();
                    $.messager.alert('HMConfiguration-EditHMNUMBasicInfo', 'Failed to edit,please contact administrator', 'error');
                    return false;
                }
            });
        }
    }
});