/// <summary>
/// HMNUMConfiguration 的尺寸Dimensions信息维护
/// 2013年11月16日10:39:42
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    //原始HM信息，该价格用来cancel按钮的时候复原
    var DimList = [
    ];

    exports.HMDim = {
        //给外界暴露接口
        getDimList: function () {
            return DimList;
        },
        
        //该方法用于第一次初始化或者更新之后获取纸箱的信息
        RefreshDimCTNs: function () {
            var inputs = $("#hmDims tbody tr input");//一个纸箱代表一个tr
            $.each(inputs, function () {
                var inputName = this.name;
                var inptVal = this.value;
                $("input[name='" + inputName + "']").data("DimParams", inptVal);
            });
        },

        Init: function () {
            var InitSelf = this;
            InitSelf.RefreshDimCTNs();//第一次进来，需要对Carton数组进行初始化。
            $(".editDim").off("click").on("click", function () {
                //S1:隐藏Edit按钮，显示done按钮
                $(".editDim").hide();
                $(".submitDim").show();
                $(".cancelDim").show();
                $("#hmDims input").prop("disabled", false).css({ "color": "red" });

                return false;
            });

            $(".submitDim").off("click").on("click", function () {
                progress.show();
                $.ajax({
                    url: "./EditHMDimensions",
                    type: "POST",
                    dataType: "json",
                    data: $("#DimForm").serialize(),
                    success: function (retData) {
                        progress.hide();
                        if (retData.Status === 1) {
                            InitSelf.RefreshDimCTNs();//刷新纸箱信息
                            InitSelf.EndEditModel();//退出编辑模式，传空参数代表不做任何回调函数
                        } else {
                            $.messager.alert('HMConfiguration-EditHMDimensions', retData.Data, 'error');
                            return false;
                        }
                    },
                    error: function () {
                        progress.hide();
                        $.messager.alert('HMConfiguration-EditHMDimensions', 'Failed to edit,please contact administrator', 'error');
                        return false;
                    }
                });
            });

            //取消价格变化，复原原价格
            $(".cancelDim").off("click").on("click", function () {
                InitSelf.EndEditModel(function () {//还原纸箱信息，我屮艸芔茻....
                    var inputs = $("#hmDims tbody tr input");//一个纸箱代表一个tr
                    $.each(inputs, function () {
                        var inputName = this.name;
                        var oldValue = $("input[name='" + inputName + "']").data("DimParams");
                        $("input[name='" + inputName + "']").val(oldValue)
                    });
                });
            });
        },// end of FUNC init()

        //退出编辑模式之后还原原来的样式，多处需要引用，故而提取
        EndEditModel: function (callBack) {
            $(".editDim").show();
            $(".submitDim").hide();
            $(".cancelDim").hide();
            $("#hmDims input").prop("disabled", true).css({ "color": "#666666" });
            if (callBack) {
                callBack();
            }
        }
    }
});