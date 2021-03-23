/// <summary>
/// Notice: this function have been discarded
/// 菜单维护界面：当用户在双节列的时候触发列编辑事件
/// Menu Management page：When double click row ，it will trigger inline eidt events.
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');



    var editIndex = undefined;
    exports.beginEditing = function (row, isSave) {
        debugger;
        //if (row.MenuID != editIndex) {
        //    if (endEditing()) {
        //        //通知框架要编辑id为row.MenuID的行
        //        $('#memuManageDG').treegrid('beginEdit', row.MenuID);
        //        editIndex = row.MenuID;
        //        //Get the specified row editors
        //        var ed = $('#memuManageDG').datagrid('getEditors', row.MenuID);
        //        //ed.target is undefiend,cause now ed is a array, you may get data like this ed[0].target...
        //        var curRow = $('#memuManageDG').datagrid('getSelected');
        //        var obj = null;
        //        $.each($(".datagrid-row"), function () {
        //            if ($(this).attr('node-id') == curRow.MenuID) {
        //                obj = this;
        //            }
        //        });

        //        $(obj).bind("blur", function () {
        //            alert("See Ua");
        //        })
        //    } else {
        //        $('#userManageDG').treegrid('selectRow', row.MenuID);
        //    }
        //}        
        if (!isSave.isSave) {
            if (row.MenuID != editIndex) {
                if (endEditing()) {
                    //通知框架要编辑id为row.MenuID的行
                    $('#memuManageDG').treegrid('beginEdit', row.MenuID);
                    isSave.isSave = true;
                    isSave.rowMr_ID = row.MR_ID;
                    isSave.rowMenuID = row.MenuID;
                }
            }
        }
        else {
            var ed = $('#memuManageDG').datagrid('getEditors', isSave.rowMenuID);
            var postData = {
                MR_ID: isSave.rowMr_ID,
                MenuName: ed[0].target[0].value,
                ParentMenuID: $(".combo-value").val(),
                SortNo:ed[2].target[0].value
            }
            $.post("./EditMenu", postData,
                function (status, data) {
                    $('#memuManageDG').treegrid('endEdit', isSave.rowMenuID);
                    isSave.isSave = false;
                    isSave.rowMenuID = null;
                    isSave.rowMr_ID = null;
            });               
        }
    }

    var endEditing = function () {
        if (editIndex == undefined) { return true }
        if ($('#memuManageDG').treegrid('validateRow', editIndex)) {
            var ed = $('#memuManageDG').treegrid('getEditors', editIndex);
            $('#memuManageDG').treegrid('endEdit', editIndex);

            // var productname = $(ed.target).combobox('getText');

            // $('#memuManageDG').treegrid('reload', editIndex);
            editIndex = undefined;
            return true;
        } else {
            return false;
        }
    }
});