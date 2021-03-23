/// <summary>
/// HMNUMConfiguration 的纸箱Cartons信息维护
///2013年11月14日14:27:22
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    exports.HMCTN = {

        Init: function () {
            var InitSelf = this;

            $(".CTNEditButtons").hide();//隐藏编辑系列按钮
            $(".CTNAddButtons").show();//显示Add按钮

            $(".addCTN").off("click").on("click", function () {
                InitSelf.Add();
            });

        },// end of FUNC init()
        Add: function () {
            var tHTML = '';
            tHTML += '<tr value="0">';
            tHTML += '<td>  <input type="text" value="" name="CTNTitle"></td>';
            tHTML += '<td>  <input type="text" value="0.00" name="CTNCube"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="CTNLength"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="CTNWidth"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="CTNHeight"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="CTNWeight"> </td>';
            tHTML += '<td>  <input type="text" value="" name="CTNComment" style="width:100%"> </td>';
            tHTML += '<td>  <a class="aCTNOK"><img src="../Content/images/yes16.png" /></a>  <a class="aCTNDelete"><img src="../Content/images/trash-big.png" /></a> </td>';
            tHTML += '</tr>';
            $("#hmCTNs tbody").append(tHTML);

            //增加校验
            $("#hmCTNs input").not($("input[name$='CTNComment']")).not($("input[name$='CTNTitle']")).numberbox({
                required: true,
                min: 0,
                precision: 2
            });

            $("input[name='CTNTitle']").validatebox({
                required: true
            });

            $(".aCTNOK").off("click").on("click", function () {
                var fvalidate = $("#CTNForm").form('validate');
                if (!fvalidate) {
                    return;
                } 
                var postDate = {
                    ProductID: $("#hiddenProductID").val(),
                    HMNUM: $("#HMNUM").val(),
                    CTNTitle: $(this).parent().parent().find("input[name='CTNTitle']").val(),
                    CTNLength: $(this).parent().parent().find("input[name='CTNLength']").val(),
                    CTNWidth: $(this).parent().parent().find("input[name='CTNWidth']").val(),
                    CTNHeight: $(this).parent().parent().find("input[name='CTNHeight']").val(),
                    CTNWeight: $(this).parent().parent().find("input[name='CTNWeight']").val(),
                    CTNCube: $(this).parent().parent().find("input[name='CTNCube']").val(),
                    CTNComment: $(this).parent().parent().find("input[name='CTNComment']").val()
                };

                var curTD = $(this).parent().parent();

                progress.show();
                $.ajax({
                    url: "./AddHMCarton",
                    type: "POST",
                    dataType: "json",
                    data: postDate
                }).done(function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        //如果更新成功，则应在当前列的tr加上value值，用于后续的删除操作
                        curTD.attr("value", retData["Data"]);
                    } else {
                        $.messager.alert('HMConfiguration-AddCarton', retData.Data, 'error');
                        return false;
                    }

                }).fail(function () {
                    progress.hide();
                    $.messager.alert('HMConfiguration-AddCarton', 'Failed to edit,please contact administrator', 'error');
                    return false;
                });
            }).css({ "cursor": "pointer" });

            $(".aCTNDelete").off("click").on("click", function () {
                var curTD = $(this).parent().parent();
                var CTNID = curTD.attr("value");
                if (parseInt(CTNID) > 0) {
                    progress.show();
                    $.ajax({
                        url: "./DeleteCarton",
                        type: "POST",
                        dataType: "json",
                        data: { CTNID: CTNID }
                    }).done(function (retData) {
                        progress.hide();
                        if (retData.Status === 1) {
                            curTD.remove();
                        } else {
                            $.messager.alert('HMConfiguration-DeleteCarton', retData.Data, 'error');
                            return false;
                        }
                    }).fail(function () {
                        progress.hide();
                        $.messager.alert('HMConfiguration-DeleteCarton', 'Failed to edit,please contact administrator', 'error');
                        return false;
                    });
                } else {
                    curTD.remove();
                }
            }).css({ "cursor": "pointer" });

        }
    }
});