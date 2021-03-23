define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var PostDataModel = require('./PostData');

    exports.ShipVia = {
        Edit: function (rowData) {
            $("#dlgShipViaDiv").show();
            $("#dlgShipVia").attr("readonly", "readonly").css("color", "#666666");
            //设置Edit页面字段信息
            PostDataModel.PostData.SetDlgPostData(rowData);

            //之所以放这里做resize是因为在dialog没有被show()之前，所有该页面的元素获取到的width()都是0，
            //也就无从resize了 2014年5月23日
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
            var fvalidate = $("#dlgShipViaForm").form('validate');//这一句话仅仅只是为了消除先点击Add按钮导致Edit的时候校验红框框出现在Edit页面上的小问题 2014年5月26日
            $('#dlgShipViaDiv').dialog({
                title: 'Edit ShiVia',
                width: digWidth - 50,
                height: digHeight - 50,
                closed: false,
                cache: false,
                modal: true,
                onClose: function () {
                    $("#dlgShipVia").removeAttr("readonly").css("color", "");;
                    PostDataModel.PostData.ResetDlgPostData();
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
                            url: './EditShipVia',
                            type: 'POST',
                            dataType: 'json',
                            data: PostDataModel.PostData.DlgPostData(),
                            success: function (retData) {
                                if (retData.Status === 1) {
                                    $("#dlgShipViaDiv").dialog("close");
                                    var queryParams = PostDataModel.PostData.queryPostData();
                                    $("#SHipViaDG").datagrid('reload', queryParams);
                                }
                                else {
                                    $.messager.alert('ShipVia-Edit', retData.Data, 'error');
                                }
                            },
                            error: function () {
                                $.messager.alert('ShipVia-Edit', 'NetWork error when edit role', 'error');
                            }
                        })
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
})