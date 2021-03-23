/// <summary>
/// UserManagement Model Configuration
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var addUser = require('./UserAdd');
    var editUser = require('./UserEdit');
    var UserDelete = require('./UserDelete');
    var UserInlineEdit = require('./UserInlineEdit');
    var UserRoleSetting = require('./UserRoleUpdate');
    var UserChannelModel = require('./UserChannel');
    var UserChannelModel = require('./UserChannel');
    var iCheck = require('jquery.icheck');
    var urLoadModel = require('./UserRoleLoading');
    var USOModel = require('./UserStatusOperation');
    var TagsModel = require('Common/OpenCMSTag');
    var progress = require('Common/prograss');

    $(document).ready(function () {
        progress.hide();
        TagsModel.CMSTags.BackSpace();
        var AllUserStatus =$.parseJSON($("#AllUserStatus").val());
        $('#shStatus').combobox({
            data : AllUserStatus,
            valueField: 'UserStaustID',
            textField: 'UserStatusName'
        });

        /*multipleChose 多下拉的初始化，用于设置用户的Channel*/
        var ChnnaleData = $.parseJSON($("#ChannelHidden").val());
        var selHMTL = '';
        $.each(ChnnaleData, function () {
            selHMTL += '<option value=' + this.ChannelID + '>' + this.ChannelName + '</option>';
        });
        $(".multipleSelect").append(selHMTL);
        selHMTL = null;


        $("#btnSearching").on("click", function () {
            $('#userManageDG').datagrid('load', {
                User_Account: $.trim($('#shUserAccount').val()),
                Display_Name: $.trim($('#shDisplayName').val()),
                UserStatusID: $("#shStatus").combo('getValue'),
                Primary_Email: $.trim($("#shEmail").val())
            });
        });

        $("#btnReset").on("click", function () {
            $('#shUserAccount').val("");
            $('#shDisplayName').val("");
            $("#shEmail").val("");
            $('#shStatus').combobox('select', "");
        });

        var H = $('#userManageDG').height();
        $(window).resize(function () {
            var W = $(window).width();
           // var H = $(window).height();//这个高度将导致datagrid的Status bar和row分离的很远（row数据少的情况下）
            //var H = $('#userManageDG').height();//Height不能在resize的时候再计算
            //$('#userManageDG').datagrid('resize', { width: W - 20, height: H });
            $('#userManageDG').datagrid('resize', { width: W - 20, height: H });
        });

        $('#userManageDG').datagrid({
            title: "User Management",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            remoteSort: false,
            idField: 'User_GUID',
            pagination: true,
            rownumbers: true,
            singleSelect: true,
            url: './GetUserList',
            onLoadSuccess: function () {
                $(this).datagrid("unselectAll");//解决当选中一条记录，翻页的时候则自动全部被选中的显示问题 2013年10月22日17:24:54

                //事件绑定：触发当前用户是否允许登入CMS系统
                $(".cbMenu").on("click", function () {
                    var uGUID = $(this).attr("Value");
                    var uStatus = 2;
                    //注意：$(this).attr("checked")已经毫无效果 2013年10月25日15:59:06 Lee
                    if ($(this).prop("checked") == true) {
                        uStatus = 1;
                    }
                    USOModel.UserStatus.UpdateUserStatus(uStatus, uGUID);
                });

                $("a[name='ChannelsControl']").on("click", function () {
                    var self = this;
                    var uGUID = $(this).attr("UserID");
                    USOModel.UserStatus.UpdateUserChannelControl(uGUID, true, function () {
                        $(self).next().show();
                        $(self).hide();
                    });
                    //UserChannelModel.UserChannel.OpenDialog(uID, uAcc);
                });

                $("a[name='ChannelsSetting']").on("click", function () {
                    var uID = $(this).attr("UserID");
                    var uAcc = $(this).attr("userAcc");      
                    UserChannelModel.UserChannel.OpenDialog(uID, uAcc);
                });
                
                $("a[name='RolesSetting']").on("click", function () {
                    var uID = $(this).attr("UserID"); 
                    var uAcc = $(this).attr("userAcc");
                    $("#roleSettingTitle").text("Setting " + uAcc + " 's Roles");
                    UserRoleSetting.UpdateUserRole(uID);
                });

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
                //    iconCls: 'icon-add',
                //    text: 'Add',
                //    handler: function () {
                //        //$.messager.alert("I gonan add a new user", "done");
                //        addUser.addUser();
                //    }
                //}, '-', {
                //    iconCls: 'icon-edit',
                //    text: 'Edit',                
                //    handler: function () {
                //        var row = $('#userManageDG').datagrid('getSelected');
                //        if (row == null) {
                //            $.messager.alert('UserManagement-UserEdit', "Please choose a row ", 'warning');
                //            return false;
                //        }                                  
                //        editUser.editUser(row.User_Guid, row.User_Account, row.Display_Name, row.Primary_Email, row.Status, row.User_Pwd);
                //    }
                //}, '-', {
                //    iconCls: 'icon-no',
                //    text: 'delete',
                //    handler: function () {
                //        $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                //            if (r) {
                //                var row = $('#userManageDG').datagrid('getSelected');
                //                if (row == null) {
                //                    $.messager.alert('UserManagement-UserDeleted', "Please choose a row ", 'warning');
                //                    return false;
                //                }
                //                UserDelete.deleteUser(row.User_Guid);
                //            }
                //        });
                //    }
                //}, '-', {
                {
                    iconCls: 'icon-remove',
                    text: 'RoleSetting',
                    handler: function () {
                        var row = $('#userManageDG').datagrid('getSelected');
                        if (row == null) {
                            $.messager.alert('UserManagement-UserDeleted', "Please choose a row ", 'warning');
                            return false;
                        }
                        //urLoadModel.initRoleList(row.User_Guid);
                        $("#roleSettingTitle").text("Setting "+row.User_Account+" 's Roles"); 
                        UserRoleSetting.UpdateUserRole(row.User_Guid);
                    }
                }, '-',
            {
                iconCls: 'icon-reload',
                text: 'Synchronizing WebPO',
                handler: function () {
                    var UserSynch = require('./UserSynchronizer');
                    UserSynch.UserSynchronizing();
                }
            }
            ],
            onDblClickCell: function (rowIndex, field, value) {
                UserInlineEdit.beginEditing(rowIndex, field, value);
            },
            columns: [[
                    { field: 'User_Guid', title: 'GUID', width: 80, hidden: true },
                    { field: 'User_Account', title: 'User Account', width: 180 },
                    {
                        field: 'Display_Name', title: 'Display Name', width: 200,
                        editor: {
                            type: 'validatebox',
                            options: { required: true }
                        }
                    },
                    {
                        field: 'UserStatusID', title: 'Allow Login CMS', width: 150,
                        formatter: function (value, row, index) {
                            var sFormate = '';
                            if (parseInt(value) == 1) {
                                sFormate = '<input type="checkbox" name="cbMenu" checked="checked" class="cbMenu" value=' + row.User_Guid + ' />'
                            } else {
                                sFormate = '<input type="checkbox" name="cbMenu" class="cbMenu"  value=' + row.User_Guid + ' />'
                            }

                            return sFormate;
                        }
                    },
                    { field: 'Last_Logon', title: 'Last Logon', width: 150 }
                    ,
                     {
                         field: 'Operation', title: 'Roles Setting', align: 'center',
                         formatter: function (value, row, index) {
                             var tempHTML = '';
                             tempHTML += '<div class="buttons">';//如果多按钮需要加长该列的长度才能到达多个按钮并排不叠加的效果
                             tempHTML += '<a class="regular" name="RolesSetting" userID=' + row.User_Guid + ' userAcc=' + row.User_Account + '><img src="../Content/Common/images/textfield_key.png" alt=""/>Roles Setting</a>';
                             tempHTML += '</div>';
                             return tempHTML
                         }
                     },
                    {
                        field: 'ChannelsSetting', title: 'ChannelsSetting', align: 'center',
                        formatter: function (value, row, index) {
                            var tempHTML = '';
                            tempHTML += '<div class="buttons">';//如果多按钮需要加长该列的长度才能到达多个按钮并排不叠加的效果
                            if (row.IsChannelControl) {
                                //tempHTML += '<a class="positive" name="ChannelsControl" ><img src="../Content/Common/images/apply2.png" alt=""/>IsChannelControl</a>';
                                tempHTML += '<a class="regular" name="ChannelsSetting" userID=' + row.User_Guid + ' userAcc=' + row.User_Account + '><img src="../Content/Common/images/textfield_key.png" alt=""/>Channels Setting</a>';
                            } else {
                                tempHTML += '<a class="positive" name="ChannelsControl" userID=' + row.User_Guid + ' ><img src="../Content/Common/images/apply2.png" alt=""/>IsChannelControl</a>';
                                tempHTML += '<a class="regular" style="display:none;" name="ChannelsSetting" userID=' + row.User_Guid + ' userAcc=' + row.User_Account + '><img src="../Content/Common/images/textfield_key.png" alt=""/>Channels Setting</a>';
                            }
                            tempHTML += '</div>';
                            return tempHTML
                        }
                    }
            ]]
        });//end of datagrid

    }); //end of document.ready

});