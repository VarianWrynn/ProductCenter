/// <summary>
/// HMNUMConfiguration 的纸箱Cartons信息维护
///2013年11月14日14:27:22
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    //原始HM信息，该价格用来cancel按钮的时候复原
    var CTNList = [
    ];

    exports.HMCTN = {
        //该方法用于第一次初始化或者更新之后获取纸箱的信息
        RefreshHMCTNs: function ()
        {
            //alert("没有空格"+$("input[name='CTNList[0].CTNCube']").length);
            //alert("有空格"+$("input [name='CTNList[0].CTNCube']").length);
            var inputs = $("#hmCTNs tbody tr input");//一个纸箱代表一个tr
            $.each(inputs, function () {

                //trSelf = this;
                //var curID = trSelf.id;
                //获取当前tr的value(实际上是绑定的Carton的唯一标识CTNID）

                //定义结构单个纸箱的基本信息结构,注意必须在each循环内定义，如果在循环外，每次保存的都是最后一条信息...2013年11月14日17:34:33
                //var CTN = {
                //    'CTNList[' +curID+ '].CTNID': 0,
                //    CTNCube: 0,
                //    CTNLength: 0,
                //    CTNWidth: 0,
                //    CTNHeight: 0,
                //    CTNComment: ""
                //};
                //CTN.CTNID = $(trSelf).attr("value");
                ////分别获取当前tr下属的input的各种纸箱信息
                //CTN.CTNCube = $("#" + curID + " input[name='CTNList[" + curID + "].CTNCube']").val();//CTNList[0].CTNCube
                //CTN.CTNLength = $("#" + curID + " input[name='CTNList[" + curID + "].CTNLength']").val();
                //CTN.CTNWidth = $("#" + curID + " input[name='CTNList[" + curID + "].CTNWidth']").val();
                //CTN.CTNHeight = $("#" + curID + " input[name='CTNList[" + curID + "].CTNHeight']").val();
                //CTN.CTNComment = $("#" + curID + " input[name='CTNList[" + curID + "].CTNComment']").val();
                //将当前纸箱信息推入到数组CTN中
                //CTNList.push(CTN);
                //如此坑爹复杂的结构，用JS拼凑已经无法阻止脚步了，只能用$.data了.....

                /*下面作用是保存当前各项input 的Value,用于用户做undo的时候给各项重新赋旧值*/
                var inputName = this.name;
                var inptVal = this.value;
                //var tempV = $("input name=[" + inputName + "]").data("CTNParams", inptVal); //  alert($("input[name='CTNList[0].CTNCube']").length);单引号.... 空格
                $("input[name='" + inputName + "']").data("CTNParams", inptVal);
               
            });
        },

        Init: function () {
            var InitSelf = this;

            $("#hmCTNs input").prop("readonly", "readonly").css({ "color": "#666666" });//默认打开禁用

            //增加数字类型校验,这种方法会导致更改撤销的时候，返回原值无效
            //$("#hmCTNs input").numberbox({
            //    required:true,
            //    min: 0,
            //    precision: 2
            //});

            /*JQuery 限制文本框只能输入数字和小数点,需要注意off()会取消所有的事件，包括keyup*/
            //获取ID为hmCTNs内的所有的input元素，除了id为CTNComment结尾(^表示开头)的input元素 2013年12月10日11:04:38
            $("#hmCTNs input").not($("input[id$='CTNComment']")).keyup(function () {
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).bind("paste", function () {  //CTR+V事件处理    
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
            }).css("ime-mode", "disabled"); //CSS设置输入法不可用    


            InitSelf.RefreshHMCTNs();//第一次进来，需要对Carton数组进行初始化。
            $(".editCTN").off("click").on("click", function () {
                //S1:隐藏Edit按钮，显示done按钮
                $(".editCTN").hide();
                $(".submitCTN").show();
                $(".cancelCTN").show();
                $("#hmCTNs input").prop("readonly", "").css({ "color": "red" });

                return false;
            });

            $(".submitCTN").off("click").on("click", function () {
                if ($("#CTNForm table tbody tr").length == 0)//说明当前没有纸箱 不可提交Server 否则会报错！2013年11月20日19:11:14
                {
                    InitSelf.EndEditModel();//退出编辑模式，传空参数代表不做任何回调函数
                    return false;
                }

                var fvalidate = $("#CTNForm").form('validate');
                if (!fvalidate) {
                    return;
                }

                progress.show();
                $.ajax({
                    url: "./EditHMCartons",
                    type: "POST",
                    dataType: "json",
                    data: $("#CTNForm").serialize(),
                    success: function (retData) {
                        progress.hide();
                        if (retData.Status === 1) {
                            InitSelf.RefreshHMCTNs();//刷新纸箱信息
                            InitSelf.EndEditModel();//退出编辑模式，传空参数代表不做任何回调函数
                        } else {
                            $.messager.alert('HMConfiguration-EditHMCartons', retData.Data, 'error');
                            return false;
                        }
                    },
                    error: function () {
                        progress.hide();
                        $.messager.alert('HMConfiguration-EditHMCartons', 'Failed to edit,please contact administrator', 'error');
                        return false;
                    }
                });
            });

            //取消价格变化，复原原价格
            $(".cancelCTN").off("click").on("click", function () {
                InitSelf.EndEditModel(function () {//还原纸箱信息，我屮艸芔茻....
                    var inputs = $("#hmCTNs tbody tr input");//一个纸箱代表一个tr
                    $.each(inputs, function () {
                        var inputName = this.name;
                        var oldValue = $("input[name='" + inputName + "']").data("CTNParams");
                        $("input[name='" + inputName + "']").val(oldValue)
                    });
                });
            });
        },// end of FUNC init()

        //退出编辑模式之后还原原来的样式，多处需要引用，故而提取
        EndEditModel: function (callBack) {
            $(".editCTN").show();
            $(".submitCTN").hide();
            $(".cancelCTN").hide();
            $("#hmCTNs input").prop("readonly", "readonly").css({ "color": "#666666" });
            if (callBack) {
                callBack();
            }
        }
    }
});