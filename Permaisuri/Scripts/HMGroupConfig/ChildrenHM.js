define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    exports.ChildrenHM = {

        Init: function () {
            $("#ChildrenHMForm .ChildrenQTY").combobox({
                onSelect: function (record) {
                    var self = this;
                    var ChildrenProductID = $(this).parent().parent().attr("value");//获取当前子产品的ID
                    var fvalidate = $("#ChildrenHMForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }
                    $.messager.confirm('Confirm', 'Are you sure to change this sellsets?', function (r) {
                        if (r) {
                            $.ajax({
                                url: "./UpdateSellSets",
                                type: "POST",
                                dataType: "json",
                                data: {
                                    ProductID: $("#hiddenProductID").val(),
                                    ChildrenProductID: ChildrenProductID,
                                    SellSets: record.value
                                }
                            }).done(function (retData) {
                                if (retData.Status === 1) {
                                    var curTr = $(self).parent().parent();
                                    curTr.find("input[name='SellSets']").val(record.value);//设置隐藏的那个SellSets为最新值
                                } else {
                                    $.messager.alert('HMConfiguration-UpdateSellSets', retData.Data, 'error');
                                    return false;
                                }
                            }).fail(function () {
                                $.messager.alert('HMConfiguration-UpdateSellSets', 'Failed to edit,please contact administrator', 'error');
                                return false;
                            });
                        } else {
                            //找出hiddenValue,进行回滚...
                            var curTr = $(self).parent().parent();
                            var curSellSet = curTr.find("input[name='SellSets']").val();
                            var curCombo = curTr.find(".ChildrenQTY");
                            curCombo.combobox("setValue", curSellSet);
                        }
                    });//$.messager.confirm

                }
            });
        }

    }//end of exports.ChildrenHM
});
