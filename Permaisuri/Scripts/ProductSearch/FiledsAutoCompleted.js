
/*现在这个页面已经变成一个公用页面,MediaLibrary页面也调用到它了 2014年4月8日*/
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
                        url: "../../ProductSearch/GetAutoCompeltedInfo",
                        type: "POST",
                        dataType: "json",
                        async:true,
                        data: {
                            term: request.term,
                            type: type//1:HMNUM + HMName 2:SKU+ SKUName;3:ModifiedUser,4:HMNUM Only,5 SKUOrder Only
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