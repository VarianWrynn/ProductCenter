
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var UIExtModel = require('Common/EasyUIExt');
    var ACModel = require('./FiledsAutoCompleted');
    var TagsModel = require('Common/OpenCMSTag');
    var PostParamsModel = require('./PostParams');
    var progress = require('Common/prograss');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        //校验
        UIExtModel.Ext.ValidateListLocal();
        $("#keywordbg").validatebox({
            validType: 'length[3,200]'
        });

        $(".searchSKU").css("background-repeat", "").css("background-image", "").addClass("easyui-validatebox").validatebox({
            validType: 'length[3,200]'
        });

        $("#cbBrand").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbBrand','Brand_Name']"
        });

        $('#cbChannel').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbChannel','ChannelName']"
        });

        $('#cbCategory').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbCategory','CategoryName']"
        });

        $("#cbMultiplePart").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbMultiplePart','text']"
        });

        $("#cbInventory").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbInventory','text']"
        });

        $("#cbStatus").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbStatus','StatusName']"
        });


        ACModel.AutoCompleted.Fileds($("#keywordbg"), 1);//1:HMNUM 
        ACModel.AutoCompleted.Fileds($(".searchSKU"), 2);//2:SKU;
        ACModel.AutoCompleted.Fileds($("#UpdateBy"), 3);//3:ModifiedUser

        $(".btnSearch").on("click", function () {
            $('#skuDG').datagrid('load', PostParamsModel.Get.PostParams());
            return false;
        });

        $(".btnReset").on("click", function () {
            $('#keywordbg').val("");
            $('.searchSKU').val("");
            $('#cbBrand').combobox('select', 0);
            $('#cbChannel').combobox('select', 0);
            $('#cbCategory').combobox('select', 0);
            $('#cbInventory').combobox('select', 0);
            $('#cbStatus').combobox('select', "0");
            $('#cbMultiplePart').combobox('select', 0);
            $("#UpdateBy").val("");
            return false;
        });

        var H = $('#skuDG').height();
        $(window).resize(function () {
            var W = $(window).width();
            $('#skuDG').datagrid('resize', { width: W - 20, height: H });
        });

        $('#skuDG').datagrid({
            title: "SKU List",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            remoteSort: false,
            idField: 'SKUID',
            pagination: true,
            rownumbers: true,
            url: './GetSKUList',
            queryParams:PostParamsModel.Get.PostParams(),
            onLoadSuccess: function () {
                $(this).datagrid("unselectAll");//解决当选中一条记录，翻页的时候则自动全部被选中的显示问题 2013年10月22日17:24:54

                ////事件绑定：触发当前用户是否允许登入CMS系统
                //$(".cbMenu").on("click", function () {
                //    var uGUID = $(this).attr("Value");
                //    var uStatus = 2;
                //    if ($(this).prop("checked") == true) {
                //        uStatus = 1;
                //    }
                //    USOModel.UserStatus.UpdateUserStatus(uStatus, uGUID);
                //});

                //$("a[name='ChannelsControl']").on("click", function () {
                //    var self = this;
                //    var uGUID = $(this).attr("UserID");
                //    USOModel.UserStatus.UpdateUserChannelControl(uGUID, true, function () {
                //        $(self).next().show();
                //        $(self).hide();
                //    });
                //});

            },
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("Get User Data", retData.Data, "warning");
                    return false;
                }
            },
            toolbar: [
                {
                    iconCls: 'icon-reload',
                    text: 'SendToEcom',
                    handler: function () {
                        var rows = $('#skuDG').datagrid('getSelections');
                        if (rows.length == 0) {
                            $.messager.alert('eCom Synchronized', "Please select a row ", 'warning');
                            return false;
                        }
                        var SKUIDList = [];
                        $.each(rows, function () {
                            SKUIDList.push(this.SKUID);
                        });

                        $('#skuDG').datagrid('loading');//loaded
                        $.ajax({
                            url: "./EcomSynchronized",
                            type: "POST",
                            dataType: "json",
                            traditional: true,
                            data: {
                                //SKUList: JSON.stringify(testArr)
                                SKUIDList: SKUIDList
                            }
                        }).done(function () {
                            //$('#skuDG').datagrid('loaded');
                            $('#skuDG').datagrid('reload')
                        }).fail(function () {
                            $('#skuDG').datagrid('loaded');
                            $.messager.alert('SendToEcomWithMultiple', 'Failed to edit,please contact administrator', 'error');
                            return false;
                        });
                    }
                }
            ],
            striped: true,
            columns: [[
                    { field: 'SKUID', title: 'SKUID', checkbox: true, width: 80 },
                    {
                        field: 'SKU', title: 'SKUOrder', width: 150
                    },

                     {
                         field: 'ChannelName', title: 'Channel Name', width: 150,
                         formatter: function (value, row, index) {
                             return row.ChannelName;
                         }
                     },

                    {
                        field: 'ProductName', title: 'Product Name', width: 250
                    },

                    {
                        field: 'HMNUM', title: 'HMNUM', width: 150,
                        formatter: function (value, row, index) {
                            return row.SKU_HM_Relation.CMS_HMNUM.HMNUM;
                        }
                    },

                    {
                        field: 'Synch Status', title: 'Status', width: 150,
                        formatter: function (value, row, index) {
                            return row.CMS_Ecom_Sync.StatusDesc;
                        }
                    },
                    {
                        field: 'Comments', title: 'Comments',
                         formatter: function (value, row, index) {
                             return row.CMS_Ecom_Sync.Comments;
                         }
                     }
            ]]
        });//end of datagrid

    }); //end of document.ready

});