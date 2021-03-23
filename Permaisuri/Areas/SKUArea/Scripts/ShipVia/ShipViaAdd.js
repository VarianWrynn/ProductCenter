define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var postDataModel = require('./PostData');

    exports.ShipVia =  {

        Add: function () {
            $("#dlgShipViaDiv").show();

            //之所以放这里做resize是因为在dialog没有被show()之前，所有该页面的元素获取到的width()都是0，也就无从resize了 2014年5月23日
            var len = $("#TDdlgIsDefaultShipVia").width() * 0.93;
            $('#dlgIsDefaultShipVia').combobox('resize', len);
            $('#dlgShipViaType').combobox('resize', len);

            var digWidth = $(window).width();
            var digHeight = $(window).height();

            $(window).resize(function () {
                var digWidth = $(window).width();
                var digHeight = $(window).height();

                var len = $("#TDdlgIsDefaultShipVia").width() * 0.95;
                $('#dlgIsDefaultShipVia').combobox('resize', len);
                $('#dlgShipViaType').combobox('resize', len);
                
                $('#dlgShipViaDiv').dialog('resize', {
                    width: digWidth - 50,
                    height: digHeight - 50,
                });
            });


            $('#dlgShipViaDiv').dialog({
                title: 'Add New ShiVia',
                width: digWidth - 50,
                height: digHeight - 50,
                closed: false,
                cache: false,
                modal: true,
                onClose: function () {
                    postDataModel.PostData.ResetDlgPostData();
                },
                buttons: [{
                    text: 'Save',
                    iconCls: 'icon-save',
                    handler: function () {
                        var fvalidate = $("#dlgShipViaForm").form('validate');
                        if (!fvalidate) {
                            return;
                        }
                        $.ajax({
                            url: "./AddShipVia",
                            type: "POST",
                            dataType: "json",
                            data: postDataModel.PostData.DlgPostData(),
                            success: function (retData) {
                                if (retData.Status === 1) {
                                    $("#dlgShipViaDiv").dialog("close");
                                    var queryParams = postDataModel.PostData.queryPostData();
                                    $("#SHipViaDG").datagrid('reload', queryParams);
                                } else {
                                    $.messager.alert('ShipVia-Add', retData.Data, 'error');
                                }
                            },
                            error: function () {
                                $.messager.alert('ShipVia-Add', 'NetWork error when add new role', 'error');
                            }
                        });
                    }
                }, {
                    text: 'Close',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $("#dlgShipViaDiv").dialog("close");
                    }
                }]
            });
        }
    }
});