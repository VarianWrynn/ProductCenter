define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var editIndex = undefined;

     exports.beginEditing = function (rowIndex, field, value) {
        if (field != "Display_Name")
            return;

        if (rowIndex != editIndex) {
            if (endEditing()) {
                $('#userManageDG').datagrid('beginEdit', rowIndex);
                editIndex = rowIndex;
                var ed = $('#userManageDG').datagrid('getEditor', { index: rowIndex, field: 'Display_Name' });
                $(ed.target).focus().bind('blur', function () {
                    endEditing();
                });
            } else {
                $('#userManageDG').datagrid('selectRow', editIndex);
            }
        }
    }

    var endEditing = function () {
        if (editIndex == undefined) { return true }
        if ($('#userManageDG').datagrid('validateRow', editIndex)) {
            var ed = $('#userManageDG').datagrid('getEditor', { index: editIndex, field: 'Display_Name' });
            var number = $(ed.target).text('getValue');
            $('#userManageDG').datagrid('getRows')[editIndex]['Display_Name'] = number;
            $('#userManageDG').datagrid('endEdit', editIndex);
            $('#userManageDG').datagrid('selectRow', editIndex);
            editIndex = undefined;
            return true;
        } else {
            return false;
        }
    }
});