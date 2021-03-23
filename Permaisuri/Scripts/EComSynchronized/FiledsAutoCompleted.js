define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var ui = require('jquery-ui');

    exports.AutoCompleted = {
        Fileds: function (selector, type) {
            var HMAutoSelf = this;
            selector.autocomplete({
                delay: 500,
                source: function (request, response) {
                    selector.addClass('auto-loading');
                    $.ajax({
                        url: "../ProductSearch/GetAutoCompeltedInfo",
                        type: "POST",
                        dataType: "json",
                        data: {
                            term: request.term,
                            type: type//1:HMNUM 2:SKU;3:ModifiedUser
                        }
                    }).done(function (retData) {
                        selector.removeClass('auto-loading');
                        response($.map(retData["Data"], function (item) {
                            return {
                                label: item,
                                value: item
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
                }
            });// end of autocomplete ;
        }
    }
});