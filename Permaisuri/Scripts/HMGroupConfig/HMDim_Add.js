/// <summary>
/// HMNUMConfiguration 的纸箱Cartons信息维护
///2013年11月14日14:27:22
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    exports.HMDim = {

        Init: function () {
            var InitSelf = this;

            $(".DimEditButtons").hide();//隐藏编辑系列按钮
            $(".DimAddButtons").show();//显示Add按钮


            $(".addDim").off("click").on("click", function () {
                InitSelf.Add();
            });
        },// end of FUNC init()
        Add: function () {
            var tHTML = '';
            tHTML += '<tr>';
            tHTML += '<td>  <input type="text" value="" name="DimTitle"></td>';
            tHTML += '<td>  <input type="text" value="0.00" name="DimCube"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="DimLength"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="DimWidth"> </td>';
            tHTML += '<td>  <input type="text" value="0.00" name="DimHeight"> </td>';
            tHTML += '<td>  <input type="text" value="" name="DimComment" style="width:100%"> </td>';
            tHTML += '<td>  <a class="aDimOK"><img src="../Content/images/yes16.png" /></a>  <a class="aDimDelete"><img src="../Content/images/trash-big.png" /></a> </td>';
            tHTML += '</tr>';
            $("#hmDims tbody").append(tHTML);

            //增加校验
            $("#hmDims input").not($("input[name$='DimComment']")).not($("input[name$='DimTitle']")).numberbox({
                required: true,
                min: 0,
                precision: 2
            });

            $("input[name='DimTitle']").validatebox({
                required: true
            });

            $(".aDimOK").off("click").on("click", function () {
                var fvalidate = $("#CTNForm").form('validate');
                if (!fvalidate) {
                    return;
                } 
                var postDate = {
                    ProductID: $("#hiddenProductID").val(),
                    HMNUM: $("#HMNUM").val(),
                    DimTitle: $(this).parent().parent().find("input[name='DimTitle']").val(),
                    DimWidth: $(this).parent().parent().find("input[name='DimWidth']").val(),
                    DimLength: $(this).parent().parent().find("input[name='DimLength']").val(),
                    DimHeight: $(this).parent().parent().find("input[name='DimHeight']").val(),
                    DimCube: $(this).parent().parent().find("input[name='DimCube']").val(),
                    DimComment: $(this).parent().parent().find("input[name='DimComment']").val()
                };
                var curTD = $(this).parent().parent();
                progress.show();
                $.ajax({
                    url: "./AddDimension",
                    type: "POST",
                    dataType: "json",
                    data: postDate
                }).done(function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        curTD.attr("value", retData["Data"]);
                    } else {
                        $.messager.alert('HMConfiguration-AddDimension', retData.Data, 'error');
                        return false;
                    }

                }).fail(function () {
                    progress.hide();
                    $.messager.alert('HMConfiguration-AddDimension', 'Failed to edit,please contact administrator', 'error');
                    return false;
                });
            }).css({ "cursor": "pointer" });

            $(".aDimDelete").off("click").on("click", function () {
                var curTD = $(this).parent().parent();
                var DimID = curTD.attr("value");
                if (parseInt(DimID) > 0) {
                    progress.show();
                    $.ajax({
                        url: "./DeleteDimension",
                        type: "POST",
                        dataType: "json",
                        data: { DimID: DimID}
                    }).done(function (retData) {
                        progress.hide();
                        if (retData.Status === 1) {
                            curTD.remove();
                        } else {
                            $.messager.alert('HMConfiguration-Deleteimension', retData.Data, 'error');
                            return false;
                        }
                    }).fail(function () {
                        progress.hide();
                        $.messager.alert('HMConfiguration-Deleteimension', 'Failed to edit,please contact administrator', 'error');
                        return false;
                    });
                } else {
                    curTD.remove();
                }
            }).css({ "cursor": "pointer" });

        }
    }
});