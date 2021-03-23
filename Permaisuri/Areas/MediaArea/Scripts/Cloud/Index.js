/// <reference path="../jquery-1.7.1-vsdoc.js" />
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var UIExtModel = require('Common/EasyUIExt');
    var fancybox = require('FancyBox/jquery.fancybox');
    var ACModel = require('../../../../Scripts/ProductSearch/FiledsAutoCompleted');
    
    $(document).ready(function () {
        progress.hide();
        ACModel.AutoCompleted.Fileds($("#HMNUM"), 4);
        ACModel.AutoCompleted.Fileds($("#SKUOrder"), 5);

        var CMSImgUrl = $("#hiddenCMSImgUrl").val();
            

        //绑定图像发放Event
        $(".fancybox").fancybox({
            tpl: {
                error: '<p class="fancybox-error">There is no picture for this item</p>',
            }
        });

        //校验
        UIExtModel.Ext.ValidateListLocal();
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

        $('#cbCloudStatus').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbCloudStatus','CloudStatusName']"
        });

        $(".btnReset").on("click", function () {
            $('#SKUOrder').val("");
            $('#HMNUM').val("");
            $('#cbChannel').combobox('select', 0);
            $('#cbFormat').combobox('select', 0);
            $('#cbStatus').combobox('select', 0);
            $('#cbBrand').combobox('select', 0);

            return false;
        });

        //search click begin
        $(".btnSearch").on("click", function () {
            var isValidated = $("#mediaLibForm").form('validate')
            if (!isValidated) {
                return false;
            }
            var postData = {};
            postData.page = $("#aRow").text();
            postData.rows = $("#opPageSize").val();
            postData.SKUOrder = $("#SKUOrder").val();
            postData.HMNUM = $("#HMNUM").val();
            postData.Channel = $("#cbChannel").combobox('getValue');
            postData.Format = $("#cbFormat").combobox('getValue');
            postData.Status = $("#cbStatus").combobox('getValue');
            postData.Brand = $("#cbBrand").combobox('getValue');
            postData.CloudStatusId = $("#cbCloudStatus").combobox('getValue');

            $('#mediaDG').datagrid('load', postData);

            return false;//can not be remove 

        })//search click end

        $("#mediaDG").datagrid({
            title: "Media List",
            autoRowHeight: true,
            resizeHandle: "right",
            striped: true,
            remoteSort: false,
            idField: 'MediaID',
            pagination: true,
            rownumbers: true,
            url: './GetMediaList',
            loadFilter: function (retData) {
                if (retData.Status === 1) {
                    return retData.Data;
                } else {
                    $.messager.alert("Get User Data", retData.Data, "warning");
                    return false;
                }
            },
            onLoadSuccess: function () {

                // 调用当前页的fancybox，这里的(live:false)是必须的，默认是live:true
                /*If set to true, fancyBox uses "live" to assign click event. Set to "false", http://fancyapps.com/fancybox/#docs
                if you need to apply only to current collection, e.g., if using something like*/
                $(".fancybox").fancybox({ type: "image", 'hideOnContentClick': true, live: false });
                //// 以下代码调用父窗口的fancybox
                //$(".fancybox").click(function () {
                //    parent.$.fancybox.open([{ href: $(this).attr('href') }]);
                //    return false;
                //});
            },
            toolbar: [
               {
                   iconCls: 'icon-reload',
                   text: 'Upload to Cloud',
                   handler: function () {
                       var rows = $('#mediaDG').datagrid('getSelections');
                       if (rows.length == 0) {
                           $.messager.alert('Upload to Cloud', "Please select a row ", 'warning');
                           return false;
                       }
                       var MediaIDList = [];
                       $.each(rows, function () {
                           MediaIDList.push(this.MediaID);
                       });

                       $('#mediaDG').datagrid('loading');//loaded
                       //console.info(JSON.stringify(MediaIDList));
                      
                       $.ajax({
                           url: "./CloudUploadWithMediaList",
                           type: "POST",
                           dataType: "json",
                           traditional: true,
                           data: {
                               MediaIDList: MediaIDList
                           }
                       }).done(function () {
                           $('#mediaDG').datagrid('reload')
                       }).fail(function () {
                           $('#mediaDG').datagrid('loaded');
                           $.messager.alert('SendToEcomWithMultiple', 'Failed to edit,please contact administrator', 'error');
                           return false;
                       });
                   }
               }
            ],
            columns: [[
                    {
                        field: 'MediaID', title: 'MediaID', checkbox: true, width: 80
                    },
                    {
                        field: 'Image', title: 'Image', width: 150,
                        formatter: function (value, row, index) {
                            var tempHTML = '';
                            tempHTML += '<div>';
                            tempHTML += '<a title="' + row.ImgName + '" href="' + CMSImgUrl + row.HMNUM + "/" + row.ImgName + row.fileFormat + '" class="fancybox">';
                            tempHTML += '<img width="100px" src="' + CMSImgUrl + row.HMNUM + "/" + row.ImgName + "_th" + row.fileFormat + '">';
                            tempHTML += '</a>';
                            tempHTML += '</div>';
                            return tempHTML
                        }
                    },

                    {
                        field: 'ImgName', title: 'ImgName', width: 150,
                        formatter: function (value, row, index) {
                            return row.ImgName + row.fileFormat;        
                        }
                    },
                    {
                         field: 'HMNUM', title: 'HMNUM', width: 150
                    },
                    { field: 'MediaType', title: 'MediaType', width: 80 },
                    { field: 'fileSize', title: 'fileSize', width: 120 },
                    { field: 'fileWidth', title: 'fileWidth', width: 100 },
                    { field: 'fileHeight', title: 'fileHeight', width: 100 },
                    {
                        field: 'MediaCloudStatus', title: 'CloudStatus', width: 150,
                        formatter: function (value, row, index) {
                            return value.CloudStatusName;
                        }
                    }
                    
                   
            ]]
        });//end of datagrid
      
    });// end of document.ready
});