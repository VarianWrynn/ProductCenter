/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var zTree = require('jquery.ztree');

    /// <summary>
    /// Click Left Tree Menu trigger this function
    /// </summary>
    /// <param name="event">event Object</param>
    /// <param name="treeId">zTree unique identifier: treeId, easy for users to control.</param>
    /// <param name="treeNode">JSON data object of the node which is clicked</param>
    /// <returns></returns>
    exports.menuClick = function (event, treeId, treeNode) {
        var nodeName = treeNode.name;
        if ($('#centerTabs').tabs('exists', nodeName)) {
            $('#centerTabs').tabs('select', nodeName);
        } else {
            var strHtml = "";
            strHtml += '<iframe id="frmWorkArea" width="100%" height="99%" frameborder="0" scrolling="auto" src="../' + treeNode.MenuUrl + '"></iframe>';
            $('#centerTabs').tabs('add', {
                title: nodeName,
                content: strHtml,
                closable: true
            });

            strHtml = "";
        }
    }
});