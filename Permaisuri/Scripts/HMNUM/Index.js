/// <summary>
/// HMNUM模块的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var UIExtModel = require('Common/EasyUIExt');
    var cmsTagsModel = require('Common/OpenCMSTag');
    var getUrlVars = require('Common/getUrlVars');
    var PostDataModel = require('./PostData');
    var ACModel = require('../ProductSearch/FiledsAutoCompleted');
    var TooltipModel = require('./HMTooltip');

    $(document).ready(function () {
        progress.hide();
        cmsTagsModel.CMSTags.BackSpace();
        TooltipModel.HMList.InitToolTip();//2014年6月3日
        ACModel.AutoCompleted.Fileds($("#HMNUM"), 4);
        $("#btnSearching").on("click", function () {
            
            var fvalidate = $(".filterbg").form('validate');
            if (!fvalidate) {
                return;
            }
            $('#HMNUMManageDG').datagrid('load', PostDataModel.PostData.Get());
        });

        $("#btnReset").on("click", function () {
            $('#HMNUM').val("");
            $('#ProductName').val("");
            $("#StockKey").val("");
            $("#queryIsGroup").combobox('select', "0");
            $("#Status").combobox('select', "0");
            $("#ISOrphan").removeProp("checked");
            $("#IsExcludedSubHMNUM").removeProp("checked");
        });

        var H = $('#HMNUMManageDG').height();
        $(window).resize(function () {
            var W = $(window).width();
            $('#HMNUMManageDG').datagrid('resize', { width: W - 20, height: H });
        });

        UIExtModel.Ext.ValidateListLocal();


        // 该字段用来配合StatusID查询的 0：全部查询 ； 
        // 1：代表值查询未审核unaudit的数据； 2代表查询已审核通过的数据，包括audited 和 disabled
        var requestType = $.getUrlVar('RequestType');
        $("#Status").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            loadFilter: function (retData) {
                var reqType = parseInt(requestType);
                switch (reqType) {
                    case 1://delete audited, disabled
                        $.each(retData, function () {
                            var self = this;
                            if (this.StatusID === 2 || this.StatusID === 3) {
                                retData.splice($.inArray(self, retData), 1);
                            }
                        });

                        return retData;
                        break;
                    case 2: // delete   unaudit
                        $.each(retData, function () {
                            var self = this;
                            if (this.StatusID === 1) {
                                retData.splice($.inArray(self, retData), 1);
                            }
                        });

                        return retData;

                        break;
                    default:
                        return retData;
                        break;
                }
            },
            validType: "ValidateListLocal['#Status','StatusName']"
        });
        $("#queryIsGroup").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#queryIsGroup','text']"
        });
        

        $('#HMNUMManageDG').datagrid({
            title: "HM# Management",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            remoteSort: false,
            idField: 'ProductID',
            pagination: true,
            rownumbers: true,
            singleSelect: true,
            onSortColumn: function (sort, order) {
                alert("sort:"+sort+",order："+order+"");
            },
            queryParams: PostDataModel.PostData.Get(),
            url: './GetHMNUMList',
            onLoadSuccess: function () {
                $("a[name='editrow']").on("click", function () {
                    $('#HMNUMManageDG').datagrid('beginEdit', $(this).attr('rowInex'));
                    //该方法可以防止在笔记本分辨率较小的情况下，焦点重新被重置到页面的最前方，引发操作不便 
                    //原理未知。。2013年12月9日17:28:42
                    return false;
                });

                $("a[name='config']").on("click", function () {
                    var ProductID = $(this).attr("ProductID");//用prop返回undefined
                    var IsGroup = $(this).attr("IsGroup");
                    var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                    //根据IsGroup字段判断是基础产品还是组合产品 然后转跳到不同的页面去..
                    if (IsGroup == "true") {
                        TagsParam.URL = "../HMGroupConfig/Config?ProductID=" + ProductID;
                        TagsParam.name = "HMGroupConfig";
                        TagsParam.title = "HMGroupConfig";
                    } else {
                        TagsParam.URL = "../HMConfig/Index?ProductID=" + ProductID;
                        TagsParam.name = "HMCofig";
                        TagsParam.title = "HMCofig";
                    }

                    TagsParam.iframeID = "frmTag";
                    TagsParam.reload = true;
                    cmsTagsModel.CMSTags.OpenTags(TagsParam);
                    // return false;如果这里设置了false,则单击该列的时候，该列不再触发被选择的动作
                });

            },
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("GetHMNUMList", retData.Data, "error");
                    return false;
                }
            },
            toolbar: [
               {
                   iconCls: 'icon-edit',
                   text: 'HMNUM Configuration',
                   handler: function () {
                       var row = $('#HMNUMManageDG').datagrid('getSelected');
                       if (row == null) {
                           $.messager.alert('HM#-Edit', "Please choose a row ", 'warning');
                           return false;
                       }
                       var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                       //根据IsGroup字段判断是基础产品还是组合产品 然后转跳到不同的页面去....2013年11月20日16:07:49
                       if (row.IsGroup) {
                           TagsParam.URL = "../HMGroupConfig/Config?ProductID=" + row.ProductID;
                           TagsParam.name = "HMGroupConfig";
                           TagsParam.title = "HMGroupConfig";
                       } else {
                           TagsParam.URL = "../HMConfig/Index?ProductID=" + row.ProductID;
                           TagsParam.name = "HMCofig";
                           TagsParam.title = "HMCofig";
                       }

                       TagsParam.iframeID = "frmTag";
                       TagsParam.reload = true;
                       cmsTagsModel.CMSTags.OpenTags(TagsParam);
                   }
               }, '-',
            {
                iconCls: 'icon-reload',
                text: 'Synchronizing HM Data',
                handler: function () {
                    var HMSynch = require('./HMOperation');
                    HMSynch.HM.SynchHMNewData();
                }
            }

            ],
            onDblClickRow: function (rowIndex, rowData) {
                var ProductID = rowData["ProductID"];//用prop返回undefined
                var IsGroup = rowData["IsGroup"];

                var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                //根据IsGroup字段判断是基础产品还是组合产品 然后转跳到不同的页面去..
                if (IsGroup == true) {
                    TagsParam.URL = "../HMGroupConfig/Config?ProductID=" + ProductID;
                    TagsParam.name = "HMGroupConfig";
                    TagsParam.title = "HMGroupConfig";
                } else {
                    TagsParam.URL = "../HMConfig/Index?ProductID=" + ProductID;
                    TagsParam.name = "HMCofig";
                    TagsParam.title = "HMCofig";
                }

                TagsParam.iframeID = "frmTag";
                TagsParam.reload = true;
                cmsTagsModel.CMSTags.OpenTags(TagsParam);
            },
            columns: [[
                    { field: 'ProductID', title: 'ProductID', width: 0, hidden: true },
                    {
                        field: 'HMNUM', title: 'HM#', width: 200, sortable: true,

                    },
                    {
                        field: 'ProductName', title: 'Product Name', width: 350,sortable:true,
                    },
                     {
                         field: 'StockKey', title: 'StockKey', align: 'center', width: 200
                     },
                     {
                         field: 'MasterPack', title: 'MP', align: 'center', width: 40
                     },
                    {
                        field: 'FirstCost', title: 'First Cost', align: 'left', width: 80,
                        formatter: function (value, row, index) {
                            //注意 如果field改成HM_Costing，然后return value.FirstCost,那么下面几个列引用HM_Costing将都变成FirstCost的值
                            return row.HM_Costing.FirstCost;
                        },
                        editor: {
                            type: 'validatebox',
                            options: { required: true }
                        }
                    },

                     {
                         field: 'Status', title: 'Status', align: 'center', width: 100,
                         formatter: function (value, row, index) {
                             return row.CMS_HMNUM_Status.StatusName;
                         },
                     },

                     //{
                     //    field: 'OceanFreight', title: 'Ocean Freight', align: 'left',
                     //    formatter: function (value, row, index) {
                     //        return row.HM_Costing.OceanFreight;
                     //    },
                     //    editor: {
                     //        type: 'numberbox',
                     //        options: {
                     //            required: true,
                     //            min: 0,
                     //            precision: 2
                     //        }
                     //    }
                     //},
                     

                       
                    //{
                    //    field: 'HM#Operation', title: 'Edit Cost', align: 'center',
                    //    formatter: function (value, row, index) {
                    //        if (row.editing) {
                    //            var s = '<a href="#" name="saverow" value="' + row.ProductID + '" rowInex="' + index + '">Save</a> ';
                    //            var c = '<a href="#" name="cancelrow" value="' + row.ProductID + '" rowInex="' + index + '">Cancel</a>';
                    //            return s + c;
                    //        } else {
                    //            return '<a href="#" name="editrow" value="' + row.ProductID + '" rowInex="' + index + '">Edit Cost</a> ';
                    //        }
                    //    }
                    //},
                     {
                         field: 'HM#Config', title: 'HM#Config', align: 'center',
                         formatter: function (value, row, index) {
                             var tempHTML = '';
                             tempHTML += '<div class="buttons">';//如果多按钮需要加长该列的长度才能到达多个按钮并排不叠加的效果
                             tempHTML += '<a class="regular" name="config"  IsGroup="' + row.IsGroup + '" ProductID="' + row.ProductID + '"><img src="../Content/Common/images/textfield_key.png" alt=""/>Config</a>';
                             tempHTML += '</div>';
                             return tempHTML
                         }
                     }
            ]],
            onBeforeEdit: function (rowIndex, rowData) {
                //$.messager.alert('UserManagement-AddRole', rowIndex, 'error');
                rowData.editing = true;
                updateActions(rowIndex, rowData);
            },
            onAfterEdit: function (rowIndex, rowData, changes) {
                //$.messager.alert('UserManagement-AddRole', JSON.stringify(rowData), 'error');
                //return false;

                if (parseFloat(rowData.LandedCost) < parseFloat(rowData.FirstCost)) {
                    $.messager.alert('Costing Update', "Landed cost  can not be less than first cost", 'error');
                    $('#HMNUMManageDG').datagrid('beginEdit', rowIndex);
                    return false;
                }


                Object.size = function (obj) {
                    var size = 0, key;
                    for (key in obj) {
                        if (obj.hasOwnProperty(key)) size++;
                    }
                    return size;
                };
                var size = Object.size(changes);
                if (size === 0)//没有发生价格变动
                {
                    rowData.editing = false;
                    updateActions(rowIndex, rowData);
                    return false;
                }

                var postData = {
                    ProductID: rowData.ProductID,
                    StockKey: rowData.StockKey,
                    FirstCost: rowData.FirstCost,
                    LandedCost: rowData.LandedCost,
                    EstimateFreight: rowData.EstimateFreight,
                    HMNUM: rowData.HMNUM,
                    StockKey: rowData.StockKey
                };
                progress.show();
                $.ajax({
                    url: "./EditHMNUMCosting",
                    type: "POST",
                    dataType: "json",
                    data: postData,
                    success: function (retData) {
                        progress.hide();
                        if (retData.Status === 1) {
                            rowData.editing = false;

                            rowData.HM_Costing.FirstCost = rowData.FirstCost;
                            rowData.HM_Costing.LandedCost = rowData.LandedCost;
                            rowData.HM_Costing.EstimateFreight = rowData.EstimateFreight;

                            updateActions(rowIndex, rowData);
                        } else {
                            $.messager.alert('HM#-EditHMNUM', retData.Data, 'error');
                            $('#HMNUMManageDG').datagrid('beginEdit', rowIndex);
                            return false;
                        }
                    },
                    error: function () {
                        progress.hide();
                        $.messager.alert('HM#-EditHMNUM', 'NetWork error when add new role', 'error');
                        $('#HMNUMManageDG').datagrid('beginEdit', rowIndex);
                        return false;
                    }
                });

            },
            onCancelEdit: function (rowIndex, rowData) {
                rowData.editing = false;
                updateActions(rowIndex, rowData);
            }
        });//end of datagrid

        /// <summary>
        /// 每次操作的参数（这里指价格）的更新，包括点击Edit的时候（需要展示去掉美元符号的参数），点击Save/Cancel后（需要展示包含了美元符号的价格）的展示方式
        /// ChangeDate:2013年12月9日19:41:50 
        /// </summary>
        /// <param name="rowIndex">当前点击的列</param>
        /// <param name="rowData">参数，这里只要是价格的几个参数</param>
        /// <returns></returns>
        function updateActions(rowIndex, rowData) {
            //$.messager.alert('UserManagement-AddRole', JSON.stringify(rowData), 'error');
            if (rowData.editing) {
                //在开始编辑的时候，把货币前面的美元符号给去掉，这样子editor为validateNumber的时候可以被识别出来 2013年11月13日10:41:34
                rowData.HM_Costing.FirstCost = rowData.HM_Costing.FirstCost.replace('$', '');
                rowData.HM_Costing.LandedCost = rowData.HM_Costing.LandedCost.replace('$', '');
                rowData.HM_Costing.EstimateFreight = rowData.HM_Costing.EstimateFreight.replace('$', '');
            } else {
                //更新这里的价格，这样调用UpdateActions之后，列的内容展示是新价格，否则展示还是旧的价格
                //Lee 2013年11月13日6:56:06
                rowData.HM_Costing.FirstCost = '$' + rowData.HM_Costing.FirstCost;
                rowData.HM_Costing.LandedCost = '$' + rowData.HM_Costing.LandedCost;
                rowData.HM_Costing.EstimateFreight = '$' + rowData.HM_Costing.EstimateFreight;
            }

            $('#HMNUMManageDG').datagrid('updateRow', {
                index: parseInt(rowIndex),
                row: {
                    FirstCost: rowData.HM_Costing.FirstCost,
                    LandedCost: rowData.HM_Costing.LandedCost,
                    EstimateFreight: rowData.HM_Costing.EstimateFreight
                }
            });
            //$(ed.target).focus();
          
            $("a[name='editrow']").off().on("click", function () {
                $('#HMNUMManageDG').datagrid('beginEdit', $(this).attr('rowInex'));
                //该方法可以防止在笔记本分辨率较小的情况下，焦点重新被充值到页面的最前方，引发操作不便 
                //原理未知。。2013年12月9日17:28:42
                return false;
            });

            $("a[name='saverow']").off().on("click", function () {
                $('#HMNUMManageDG').datagrid('endEdit', $(this).attr('rowInex'));
                //该方法可以防止在笔记本分辨率较小的情况下，焦点重新被充值到页面的最前方，引发操作不便 
                //原理未知。。2013年12月9日17:28:42
                return false;
            });
            $("a[name='cancelrow']").off().on("click", function () {
                $('#HMNUMManageDG').datagrid('cancelEdit', $(this).attr('rowInex'));

                //该方法可以防止在笔记本分辨率较小的情况下，焦点重新被充值到页面的最前方，引发操作不便 
                //原理未知。。2013年12月9日17:28:42
                return false;
            });

            //如果这里不重新绑定，当用户线点击Edit Cost之后，再点击Config则会变得毫无反应！2013年12月10日9:56:38 Lee
            $("a[name='config']").off().on("click", function () {
                var ProductID = $(this).attr("ProductID");//用prop返回undefined
                var IsGroup = $(this).attr("IsGroup");
                var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                //根据IsGroup字段判断是基础产品还是组合产品 然后转跳到不同的页面去..
                if (IsGroup == "true") {
                    TagsParam.URL = "../HMGroupConfig/Config?ProductID=" + ProductID;
                    TagsParam.name = "HMGroupConfig";
                    TagsParam.title = "HMGroupConfig";
                } else {
                    TagsParam.URL = "../HMConfig/Index?ProductID=" + ProductID;
                    TagsParam.name = "HMCofig";
                    TagsParam.title = "HMCofig";
                }

                TagsParam.iframeID = "frmTag";
                TagsParam.reload = true;
                cmsTagsModel.CMSTags.OpenTags(TagsParam);
                // return false;如果这里设置了false,则单击该列的时候，该列不再触发被选择的动作
            });
           
        }

    }); //end of document.ready

});