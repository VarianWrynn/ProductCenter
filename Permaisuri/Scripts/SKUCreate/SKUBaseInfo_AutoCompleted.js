define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var ui = require('jquery-ui');

    //这么调用 ACModel.AutoCompleted.Fileds($("#Material"),"Material");
    //而不是  ACModel.AutoCompleted.Fileds("#Material","Material");

    exports.AutoCompleted = {
        Fileds: function (selector, type) {
            var HMAutoSelf = this;
            selector.autocomplete({
                delay: 500,
                source: function (request, response) {
                    selector.addClass('auto-loading');
                    $.ajax({
                        url: "../ProductConfiguration/GetAutoCompeltedMCCC",
                        type: "POST",
                        dataType: "json",
                        data: {
                            term: $.trim(request.term),
                            type: type,//Material,Colour,Category,SubCategory
                            ParentID: $("#CategoryID").val()//只有查询SubCategory的时候这个字段才有意义
                        }
                    }).done(function (retData) {
                        selector.removeClass('auto-loading');
                        response($.map(retData["Data"], function (item) {
                            return {
                                label: item.Name,
                                value: item.Name,
                                ID: item.ID
                            }
                        }));
                    }).fail(function () {
                        selector.removeClass('auto-loading');
                        $.messager.alert('GetProductInfo', 'NetWork Error,Please contact administrator。', 'error');
                        return false;
                    });
                },// end of source function

                minLength: 1,
                select: function (event, ui) {
                    selector.val(ui.item);
                    if (type == "Category") {
                        $("#CategoryID").val(ui.item.ID);
                        $("#SubCategory").val("");
                    }
                }
            });// end of autocomplete ;
        }
    }
});