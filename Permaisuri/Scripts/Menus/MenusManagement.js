
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/prograss');
    var MenuAddModel = require('./MenuAdd');
    /*  var UserDelete = require('./UserDelete');
    */
    var MenuInlineEditModel = require('./MenuInlineEdit');

    var TagsModel = require('Common/OpenCMSTag');

    var W = $(window).width();
    var h = $(window).height();
    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        $(window).resize(function () {
            var W = $(window);
            var h = W.height();
            $('#memuManageDG').datagrid('resize', { width: W - 20, height: h-20 });
        });

        $('#memuManageDG').treegrid({            
           // fit: true,
            width: 'auto',
            //height: h - h2 - 50,
            height: 'auto',
            title: "Manu Management",
            autoRowHeight: false,
            resizeHandle: "right",
            striped: true,
            remoteSort: false,
            idField: 'MenuID',
            treeField: 'name',
            pagination: false,
            rownumbers: true,
            singleSelect: true,
            url: './GetMenuListForTreeGrid',
            onLoadSuccess: function (row, data)
            {
                $("a[name='editrow']").on("click", function () {
                    $('#memuManageDG').treegrid('beginEdit', $(this).attr('value'));
                });
                $("a[name='deleterow']").off("click").on("click", function () {
                   var rowId =  $(this).attr('value');
                    $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                        if (r) {
                            $('#memuManageDG').treegrid('remove', rowId);
                        }
                    });
                });

            },
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else if (typeof (retData.Data) == "undefiend") {//when insert new node, loadFilter will be fire agian...
                    return retData;
                } else {
                    $.messager.alert("Get Role List", retData.Data, "warning");
                    return false;
                }
            },
            toolbar: [{
                iconCls: 'icon-add',
                text: 'Add',
                handler: function () {
                    MenuAddModel.MenuAdd();
                }
            }
            ],
            columns: [[
                   { field: 'MR_ID', title: 'GUID', width: 0, hidden: true },
                    {
                     field: 'name', title: 'MenuName', width: 280,
                     editor: {
                         type: 'validatebox',
                         options: { required: true }
                     }
                    },
                    { field: 'MenuID', title: 'ID', width: 80 },
                    {
                        field: 'ParentMenuID', title: 'ParentID', width: 100
                       
                    },
                    {
                        field: 'SortNo', title: 'Sort Number', width: 80,
                        editor:{type:'numberbox',
                            options: { required: true}
                        }
                    },
                    {
                        field: 'MenuUrl', title: 'Menu Url', width: 300,
                        editor: {
                            type: 'text'
                        }
                    },
                    { field: 'iconSkin', title: 'iconSkin', width: 80 },
                    {
                        field: 'Visible', title: 'Visible', width: 50,
                        //editor: {
                        //    type: 'checkbox'
                        //},
                        formatter: function (value, row, index) {
                            return '<input type="checkbox" name="cbMenu" />'
                        }
                    },
                    {
                        field: 'action', title: 'Action', width: 80, align: 'center',
                        formatter: function (value, row, index) {
                            //return '<a href="javascript:void(0)" name="deleterow" value="' + row.MR_ID + '" >Delete</a>';
                            if (row.editing) {
                                var s = '<a href="#" name="saverow" value="' + row.MenuID + '">Save</a> ';
                                var c = '<a href="#" name="cancelrow" value="' + row.MenuID + '">Cancel</a>';
                                return s + c;
                            } else {
                                var e = '<a href="#" name="editrow" value="' + row.MenuID + '">Edit</a> ';
                                var d = '<a href="#" name="deleterow" value="' + row.MenuID + '">Delete</a>';
                                return e + d;
                            }
                        }
                    }

            ]],
            onBeforeEdit: function (row) {
                row.editing = true;
                updateActions(row);
            },
            onAfterEdit: function (row) {
               /* alert("do AJAX update failure!");
                $('#memuManageDG').treegrid('beginEdit', row.MenuID);
                return false;*/

                var postData = {
                    MR_ID: row.MR_ID,
                    MenuName: row.name,
                    ParentMenuID: row.ParentMenuID,
                    SortNo: row.SortNo,
                    iconSkin: row.iconSkin,
                    MenuUrl: row.MenuUrl,
                    Visible: row.Visible,
                    MenuID: row.MenuID
                };

                $.ajax({
                    url: "./UpdateMenu",
                    type: "POST",
                    dataType: "json",
                    data: postData,
                    success: function (retData) {
                        if (retData.Status === 1) {
                            row.editing = false;
                            updateActions(row);
                        } else {
                            $.messager.alert('MenuEdit', retData.Data, 'error');
                            $('#memuManageDG').treegrid('beginEdit', row.MenuID);
                            return false;
                        }
                    },
                    error: function () {
                        $.messager.alert('MenuEdit', 'Fail to eidt this item,please contact administrator', 'error');
                        $('#memuManageDG').treegrid('beginEdit', row.MenuID);
                        return false;
                    }
                });
              
            },
            onCancelEdit: function (row) {
                row.editing = false;
                updateActions(row);
            }
        });//end of datagrid    

        ///this function must be executed,or the node status will never be updated...(Status change: Edit-->Save)
        function updateActions(row) {
            $('#memuManageDG').treegrid('update', {
                id: row.MenuID,
                row: {
                  
                }
            });
            
            $("a[name='editrow']").off("click").on("click", function () {
                $('#memuManageDG').treegrid('beginEdit', $(this).attr('value'));
            });
            $("a[name='deleterow']").off("click").on("click", function () {
                var rowId = $(this).attr('value');
                $.messager.confirm('Confirm', 'Are you sure to detele this item?', function (r) {
                    if (r) {
                        $('#memuManageDG').treegrid('remove', rowId);
                    }
                });
            });

            $("a[name='saverow']").off("click").on("click", function () {
                $('#memuManageDG').treegrid('endEdit', $(this).attr('value'));
            });
            $("a[name='cancelrow']").off("click").on("click", function () {
                $('#memuManageDG').treegrid('cancelEdit', $(this).attr('value'));
            });

        }
    });// end of documet.ready
});