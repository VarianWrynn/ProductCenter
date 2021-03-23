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
            $("#hmDims input").prop("readonly", "readonly").css({ "color": "#666666" });
            //增加数字类型校验,这种方法会导致更改撤销的时候，返回原值无效
            //$("#hmDims input").numberbox({
            //    required: true,
            //    min: 0,
            //    precision: 2
            //});


            /*JQuery 限制文本框只能输入数字和小数点,需要注意off()会取消所有的事件，包括keyup*/
            //获取ID为hmDims内的所有的input元素，除了id为DimComment结尾(^表示开头)的input元素 2013年12月10日11:04:35
            $("#hmDims input").not($("input[id$='DimComment']")).keyup(function () {
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).bind("paste", function () {  //CTR+V事件处理    
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).css("ime-mode", "disabled"); //CSS设置输入法不可用    


            InitSelf.RefreshDimCTNs();//第一次进来，需要对Carton数组进行初始化。
            $(".editDim").off("click").on("click", function () {
                //S1:隐藏Edit按钮，显示done按钮
                $(".editDim").hide();
                $(".submitDim").show();
                $(".cancelDim").show();
                $("#hmDims input").prop("readonly", "").css({ "color": "red" });

                return false;
            });

            $(".submitDim").off("click").on("click", function () {
                if ($("#DimForm table tbody tr").length == 0)//说明当前没有信息 不可提交Server 否则会报错！2013年11月20日19:11:14
                {
                    InitSelf.EndEditModel();//退出编辑模式，传空参数代表不做任何回调函数
                    return false;
                }
                var fvalidate = $("#DimForm").form('validate');
                if (!fvalidate) {
                    return;
                }

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
            $("#hmDims input").prop("readonly", "readonly").css({ "color": "#666666" });
            if (callBack) {
                callBack();
            }
        }
    }
});