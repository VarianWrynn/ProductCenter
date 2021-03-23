/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var zTree = require('jquery.ztree');
    var zTMenuClick = require('./zTreeMenuClick');
    var UserLogoutModel = require('./UserLogout');

    //var progress = require('Common/prograss');

    var setting = {
        data: {
            simpleData: {
                enable: true,
                idKey: "MenuID",
                pIdKey: "ParentMenuID",
                rootPId: "0"
            }
        },
        callback: {
            onClick: zTMenuClick.menuClick
        },
        edit: {
            enable: false
        }
    };

    $(document).ready(function () {

        //progress.show();
        $("#aLogout").bind("click", function () {
            UserLogoutModel.Logout();
        });

        $('#cc').layout();
        $.ajax({
            url: "./GetMenuTrees", // ./代表了当前当前包含了Contriller的目录，所以这里只要Action?
            type: "POST",
            dataType: "json",
            success: function (retData) {
                if (retData.Status === 1) {
                    var zTreeObj = $.fn.zTree.init($("#MenuTree"), setting, retData.Data);
                    /*设置当前第一个节点被选中并且默认展开子节点（不展开孙节点）*/
                    var nodes = zTreeObj.getNodes();
                    if (nodes.length > 0) {
                        var treeNode = nodes[0];
                        zTreeObj.selectNode(treeNode);
                        zTreeObj.expandNode(treeNode, true, false, true);

                        //默认打开Dashboard
                        var nodeName = treeNode.name;
                        if ($('#centerTabs').tabs('exists', nodeName)) {
                            $('#centerTabs').tabs('select', nodeName);
                        } else {
                            var strHtml = "";
                            strHtml += '<iframe id="frmWorkArea" width="100%" height="99%" frameborder="0" scrolling="auto" src="../' + treeNode.MenuUrl + '"></iframe>';
                            //strHtml += '<iframe id="frmWorkArea" width="100%" height="99%" frameborder="0" scrolling="auto" src="../Media/FilesUpload"></iframe>';
                            $('#centerTabs').tabs('add', {
                                title: nodeName,
                                content: strHtml,
                                closable: true
                            });
                        }
                        strHtml = "";
                    }
                } else {
                    $.messager.alert('GetMenuTrees', retData.Data, 'error');
                }
            },
            error: function () {
                $.messager.alert('GetMenuTrees', 'NetWork Error,Please contact administrator。', 'error');
            }
        }); // end of ajax func

    }).keydown(function (event) {

        //获取当前按键的键值   
        //jQuery的event对象上有一个which的属性可以获得键盘按键的键值   
        
        var keycode = event.which;
        if (keycode == 8) {
            
            //var tabs = $('#centerTabs').tabs('tabs');//Return all tab panels.

            //var tab = $('#centerTabs').tabs('getSelected');
            //var index = $('#centerTabs').tabs('getTabIndex', tab);
            //alert(index);
            //$('#centerTabs').tabs('select', index-1);

            //console.info(JSON.stringify(tabs));
            //here,return false does never work..
            event.preventDefault();
        }
    });
});