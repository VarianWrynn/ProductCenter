define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');

    var arrcxRoles = []; //private data
    exports.rolesData=arrcxRoles; //public data
    exports.initRoleList = function (user_Guid) {
        // arrcxRoles = [];//原来打算在这里执行：每次打开都重新reset掉权限列表 ,但是外面调用roleData的时候永远都是[]，不知道为什么！
        exports.rolesData =arrcxRoles= [];
        $.ajax({
            url: "./GetRoleInUser",
            type: "POST",
            dataType: "json",
            data: {
                User_Guid: user_Guid
            },
            success: function (retData) {
                if (retData.Status === 1) {
                    var brifHtml = "";
                    $.each(retData.Data, function (i, val) {
                        if (i == 0) {
                            brifHtml += "<tr>";
                        }
                        else if (i == retData.Data.length) {
                            brifHtml += "</tr>";
                        } else if (i % 2 == 0) {
                            brifHtml += "</tr><tr>";
                        }
                        if (retData.Data[i]["User_Checked"]) {//该用户已经拥有的权限
                            arrcxRoles.push(retData.Data[i]["Role_GUID"]);
                            brifHtml += "<td style='width:50%'> <input name='roleListTab' checked type='checkbox' value='" + retData.Data[i]["Role_GUID"] + "'>";
                        } else {
                            brifHtml += "<td style='width:50%'> <input name='roleListTab' type='checkbox' value='" + retData.Data[i]["Role_GUID"] + "'>";
                        }
                        brifHtml += "<label>" + retData.Data[i]["Role_Name"] + "</label><td>";

                    });
                    
                   // $("body").data("rolesData", arrcxRoles);//由于跨iframe了，无法用elemnt方式来绑定，只能绑定到body上，尚未找到解决方法
                    $("#roleListTab").html(brifHtml);

                    $("input[name='roleListTab']").each(function () {
                        var self = $(this),
                        label = self.next(),
                        label_text = label.text();
                        label.remove();
                        self.iCheck({
                            checkboxClass: 'icheckbox_line-blue',
                            radioClass: 'iradio_line-blue',
                            insert: '<div class="icheck_line-icon"></div>' + label_text
                        });
                    });//end of each func

                    //参考地址：https://github.com/fronteed/iCheck
                    // ifChecked and ifUnchecked 都是iChecked的 Callbacks
                    $("input[name='roleListTab']").on('ifChecked', function (event) {
                        arrcxRoles.push($(this).val());
                        exports.rolesData = arrcxRoles;
                        //$("body").data("rolesData", arrcxRoles);
                    });

                    $("input[name='roleListTab']").on('ifUnchecked', function (event) {
                        arrcxRoles.splice($.inArray($(this).val(), arrcxRoles), 1);
                        exports.rolesData = arrcxRoles;
                       //$("body").data("rolesData", arrcxRoles);
                    });

                } else {
                    $.messager.alert('UserManagement-GetRoleInUser', retData.Data, 'error');
                }
            },
            error: function () {
                $.messager.alert('UserManagement-GetRoleInUser', 'NetWork error when GetRoleInUser', 'error');
            }
        });

    }

});