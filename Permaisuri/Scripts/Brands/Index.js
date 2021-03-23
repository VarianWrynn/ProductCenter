define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var addBrand = require('./BrandAdd');
    var editBrand = require('./BrandEdit');
    var BrandDelete = require('./BrandDelete');
    var TagsModel = require('Common/OpenCMSTag');
    var progress = require('Common/prograss');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        var H = $('#userManageDG').height();
        $(window).resize(function () {
            var W = $(window).width();
            $('#brandManageDG').datagrid('resize', { width: W - 20, height: H });
        });


        $("#btnSearching").on("click", function () {
            $('#brandManageDG').datagrid('load', {
                Brand_Name: $.trim($('#shBrandName').val()),
                Short_Name: $.trim($('#shShortName').val()),
                // Active: parseInt($("#shStatus").combobox('getValue')),
                //MVC后台bool类型无法自动初始化value为0,1的参数，即使前端使用了parseInt
                bStatus: $("#shStatus").combobox('getValue')
            });
        });

        $("#btnReset").on("click", function () {
            $('#shBrandName').val("");
            $('#shShortName').val("");
            $('#shStatus').combobox('select', 1);
        });

        $("#brandManageDG").datagrid({
            title: "Brand Management",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            page: 1,
            rows: 10,
            remoteSort: false,
            idField: 'BrandID',
            pagination: true,
            rownumbers: true,
            singleSelect: true,
            queryParams: {//add by lee  2013年12月9日18:59:58
                //Active: parseInt($("#shStatus").combobox('getValue'))
                bStatus: $("#shStatus").combobox('getValue'),
            },
            url: './GetBrandList',
            onLoadSuccess: function () {
                $(this).datagrid("unselectAll");
            },
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("Get Brand Data", retData.Data, "warning");
                    return false;
                }
            },
            toolbar: [{
                iconCls: 'icon-add',
                text: 'Add',
                handler: function () {
                    //$.messager.alert("I gonan add a new user", "done");
                    addBrand.addBrand();
                }
            }, '-', {
                iconCls: 'icon-edit',
                text: 'Edit',
                handler: function () {
                    var row = $('#brandManageDG').datagrid('getSelected');
                    if (row == null) {
                        $.messager.alert('BrandManagement-BrandEdit', "Please choose a row ", 'warning');
                        return false;
                    }
                    editBrand.editBrand(row.Brand_Id, row.Brand_Name, row.Short_Name, row.Active);
                }
            }, '-', {
                iconCls: 'icon-no',
                text: 'delete',
                handler: function () {
                    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                        if (r) {
                            var row = $('#brandManageDG').datagrid('getSelected');
                            if (row == null) {
                                $.messager.alert('BrandManagement-BrandDeleted', "Please choose a row ", 'warning');
                                return false;
                            }
                            BrandDelete.deleteBrand(row.Brand_Id);
                        }
                    });
                }
            }],
            columns: [[
                    { field: 'Brand_ID', title: 'ID', width: 80, hidden: true },
                    { field: 'Brand_Name', title: 'Brand Name', width: 380 },
                    {
                        field: 'Short_Name', title: 'Short Name', width: 280,
                        editor: {
                            type: 'validatebox',
                            options: { required: true }
                        }
                    },
                    {
                        field: 'Active', title: 'Active', width: 100,
                        formatter: function (value, row, index) {
                            if (value == 1) {
                                return "Yes";
                            }
                            else {
                                return "No";
                            }
                        }
                    },
                    { field: 'Modifier', title: 'Modifier' }
                    ,
                    {
                        field: 'Modifier_Date', title: 'Modifier Date', align: 'right'
                    }
            ]]
        }) //end of datagrid

    });
})