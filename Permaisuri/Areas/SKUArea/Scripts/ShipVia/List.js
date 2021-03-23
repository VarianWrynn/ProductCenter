/// <reference path="../jquery-1.7.1-vsdoc.js" />
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var UIExtModel = require('Common/EasyUIExt');
    var progress = require('Common/Prograss');
    var PostDataModel = require('./PostData');
    var DefaultShipViaModel = require('./DefaultShipVia');
    var ShipViaAddModel = require('./ShipViaAdd');
    var ShipViaEditModel = require('./ShipViaEdit');
    var ShipViaDeleteModel = require('./ShipViaDelete');
    

    $(document).ready(function () {
        progress.hide();
        ////校验
        UIExtModel.Ext.ValidateListLocal();
        $("#queryShipViaTypeID").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#queryShipViaTypeID','ShipViaTypeName']"
        });

        $('#queryIsDefaultShipViaInd').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#queryIsDefaultShipViaInd','text']"
        });


        $("#dlgShipViaType").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#dlgShipViaType','ShipViaTypeName']"
        });

        $("#dlgIsDefaultShipVia").combobox({
            //width:len,
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#dlgIsDefaultShipVia','text']"
        });


        $(".btnReset").on("click", function () {
            PostDataModel.PostData.ResetQueryPostData();
            return false;
        });

        //search click begin
        $(".btnSearch").on("click", function () {
            var isValidated = $("#shipViaListForm").form('validate')
            if (!isValidated) {
                return false;
            }
            var postData = PostDataModel.PostData.queryPostData();
            $('#SHipViaDG').datagrid('load', postData);
            return false;//can not be remove 

        })//search click end

        var postData = PostDataModel.PostData.queryPostData();
        $("#SHipViaDG").datagrid({
            title: "ShipVia List",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            remoteSort: false,
            idField: 'SHIPVIAID',
            singleSelect:true,
            pagination: true,
            rownumbers: true,
            queryParams: postData,
            url: './GetShipViaList',
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    $("body").data("ShipViaDataCache", retData.Data);
                    //var oldValue = $("body]").data("ShipViaDataCache");
                    return retData.Data;
                } else {
                    $.messager.alert("Get User Data", retData.Data, "warning");
                    return false;
                }
            },
            onLoadSuccess: function () {
                $("a[name='DefaultShipeVia']").on("click", function () {
                    var SHIPVIAID = $(this).attr("SHIPVIAID");
                    var ShipViaTypeID = $(this).attr("ShipViaTypeID");
                    DefaultShipViaModel.ShipVia.UpdateDefaultShipVia(SHIPVIAID, ShipViaTypeID);
                });
            },
            toolbar: [{
                iconCls: 'icon-add',
                text: 'Add',
                handler: function () {
                    ShipViaAddModel.ShipVia.Add();
                }
            }, '-', {
                iconCls: 'icon-edit',
                text: 'Edit',
                handler: function () {
                    var row = $('#SHipViaDG').datagrid('getSelected');
                    if (row == null) {
                        $.messager.alert('ShipVia-Edit', "Please choose a row ", 'warning');
                        return false;
                    }
                    ShipViaEditModel.ShipVia.Edit(row);
                }
            }, '-', {
                iconCls: 'icon-no',
                text: 'delete',
                handler: function () {
                    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                        if (r) {

                            var row = $('#SHipViaDG').datagrid('getSelected');
                            if (row == null) {
                                $.messager.alert('ShipVia-Deleted', "Please choose a row ", 'warning');
                                return false;
                            }
                            ShipViaDeleteModel.ShipVia.Delete(row);
                        }
                    });
                }
            }],
            onDblClickCell: function (rowIndex, field, value) {
                var row = $('#SHipViaDG').datagrid('getSelected');
                if (row == null) {
                    $.messager.alert('ShipVia-Edit', "Please choose a row ", 'warning');
                    return false;
                }
                ShipViaEditModel.ShipVia.Edit(row);
            },
            columns: [[
                    { field: 'SHIPVIAID', title: 'SHIPVIAID', hidden: true, width: 80 },
                    {
                        field: 'SHIPVIA', title: 'SHIPVIA', width: 180
                    },
                    {
                        field: 'CarrierRouting', title: 'CarrierRouting', width: 150
                    },
                    { field: 'CarrierCode', title: 'CarrierCode', width: 80 },
                    { field: 'ExpressNumLength', title: 'ExpressNumLength', width: 100 },
                    { field: 'ExpressRule', title: 'ExpressRule', width: 320 },
                    {
                        field: 'CMS_ShipViaType', title: 'CMS_ShipViaType', width: 120,
                        formatter: function (value, row, index) {
                            return value.ShipViaTypeName;
                        }
                    },
                    {
                        field: 'IsDefaultShipVia', title: 'IsDefaultShipVia', align: 'center',
                        formatter: function (value, row, index) {
                            var tempHTML = '';
                            tempHTML += '<div class="buttons">';//如果多按钮需要加长该列的长度才能到达多个按钮并排不叠加的效果
                            if (row.IsDefaultShipVia) {

                                tempHTML += '<a class="regular" name="DefaultShipeVia" SHIPVIAID=' + row.SHIPVIAID + ' ShipViaTypeID = ' + row.ShipViaTypeID + '><img src="../../Content/Common/images/textfield_key.png" alt=""/>  Yes</a>';
                            } else {
                                tempHTML += '<a class="positive" name="DefaultShipeVia" SHIPVIAID=' + row.SHIPVIAID + ' ShipViaTypeID = ' + row.ShipViaTypeID + ' ><img src="../../Content/Common/images/apply2.png" alt=""/>  No</a>';
                            }
                            tempHTML += '</div>';
                            return tempHTML
                        }
                    }
            ]]
        });//end of datagrid
      
    });// end of document.ready
});