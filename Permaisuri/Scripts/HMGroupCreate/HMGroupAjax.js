/// <summary>
/// HM#组合产品的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    exports.Ajax = {
        ///新增HMGroup基本信息操作，callback函数操作的是新增 基础HM#（非组合产品）的按钮等操作
        BaseInfoAdd: function (callback) {
            //校验合法性
            if ($.trim($("#HMNUM").val()) == "")
            {
                $("#HMNUM").val("");
            }
            if ($.trim($("#ProductName").val()) == "") {
                $("#ProductName").val("");
            }
            var fvalidate = $("#HMBaseForm").form('validate');
            if (!fvalidate) {
                return;
            }
            //隐藏submit button,其实发现没必要了，因为有遮罩层了
            $(".submitGPBaseInfo").hide();
            progress.show();
            $.ajax({
                url: "./HMGroupBaseInfoAdd",
                type: "POST",
                dataType: "json",
                data: {
                    HMNUM: $.trim($("#HMNUM").val()),
                    ProductName: $.trim($("#ProductName").val()),
                    Comments: $.trim($("#Comments").val()),
                    //CategoryID: $("#SubCategoryName").combobox('getValue'),
                    CategoryID: $("#CategoryName").combobox('getValue'),
                    StatusID: $("#Status").combobox("getValue"),
                    ShipViaTypeID: $("#ShipViaType").combobox("getValue"),
                    IsGroup: true,
                    NetWeight: $.trim($("#NetWeight").val()),
                },
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        //设置当前隐藏字段的Value,方便后续操作
                        $("#hiddenProductID").val(retData.Data);
                        if (callback) {
                            callback();
                        }
                    } else {
                        $(".submitGPBaseInfo").show();
                        $.messager.alert('HMGroup-BaseInfoAdd', retData.Data, 'error');
                        return false;
                    }
                },
                error: function () {
                    $(".submitGPBaseInfo").show();
                    progress.hide();
                    $.messager.alert('HMGroup-BaseInfoAdd', 'Failed to edit,please contact administrator', 'error');
                    return false;
                }
            });
        },// end of BaseInfoAdd Func

        //提取公共方法：添加基础产品 2013年11月20日11:25:56
        /*
         potions:{
            sender:object,事件的发送者句柄
            postData:{},Ajax提交数据
            successCallBack:function(){},//执行成功后的回调函数
            errorCallBack:function(){}//执行失败后的回调函数
         }
        */
        NewHMItemAdd: function (options)
        {
            progress.show();
            $.ajax({
                url: "./AddNewHM4Group",
                type: "POST",
                dataType: "json",
                data: options["postData"],
                success: function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        //必须先给RID赋值，再调用回调函数，原因是现在增加了一个新的功能，单MP大于1的时候，会自动选择设置QTY=MP，自动触发更新，这时候如果rid=0，后台
                        //不是拿去更新而是拿去插入，到时候一次选择一个子产品，到配置页面出现2个子产品。2014年3月18日
                        $(options["self"]).parent().parent().attr("RID", retData.Data["newID"]);
                        options["sender"].UpdateCosting(retData.Data["ChildrenCostList"]);

                        if (options.successCallBack) {
                            options.successCallBack();
                        }

                    } else {
                        if (options.errorCallBack) {
                            options.errorCallBack();
                        }
                        $.messager.alert('Add HM#', retData["Data"], 'error');
                    }
                },
                error: function () {
                    progress.hide();
                    if (options.errorCallBack) {
                        options.errorCallBack();
                    }
                    $.messager.alert('Add HM#', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        }
    }

});