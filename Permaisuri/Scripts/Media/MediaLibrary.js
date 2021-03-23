define(function (require, exports, module) {
    var $ = require('jquery');
    var prograss = require('Common/Prograss');
    var easyui = require('jquery.easyui.min');
    var DateFormatterModel = require('Common/DateFormatter');
    var UIExtModel = require('Common/EasyUIExt');
    var MediaLibraryExtModel = require('./MediaLibraryExt');
    var ACModel = require('../ProductSearch/FiledsAutoCompleted');
    var TagsModel = require('Common/OpenCMSTag');
    var PostDataModel = require('./PostData');

    var overlaycss = require('PlugIn/Overlay/overlay.css');
    var overlayModel = require('PlugIn/Overlay/loading-overlay.min');

    function loading() {
        $("#progressBar").show();
        $("#add_media_bg").show();
    }
    function loadingOver() {
        $("#progressBar").hide();
        $("#add_media_bg").hide();
    }


    $(function () {
        //progress.hide();
        TagsModel.CMSTags.BackSpace();
        //$("#txttimeStart").datebox({ formatter: formatDate });
        //$("#txtTimeEnd").datebox({ formatter: formatDate });

        //var newDate = new Date();
        //$("#txttimeStart").datebox("setValue", formatDate(newDate));
        //$("#txtTimeEnd").datebox("setValue", formatDate(newDate));

        //function formatDate(date) {
        //    //if (isCurrent) {
        //    //    return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
        //    //}
        //    return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
        //}

        //-----------------some action↓-------------------------------//
        var postData = PostDataModel.PostData.Get();
        prograss.show();
        $.ajax({  //get cbChannel value ajax
            url: "./InitMediaLibrary",
            type: "POST",
            dataType: "json",
            data: postData,
            success: function (retData) {
                prograss.hide();
                if (retData.Status === 1) {
                    MediaLibraryExtModel.MediaLibraryExt.DynamicShow(retData.Data["MediaData"], postData);
                } else {
                    prograss.hide();
                    $.messager.alert('InitMediaLibrary', retData.Data, 'error');
                }
            }
        }); // end of ajax func

        ACModel.AutoCompleted.Fileds($("#HMNUM"), 4);
        ACModel.AutoCompleted.Fileds($("#SKUOrder"), 5);

        //MediaLibraryExtModel.MediaLibraryExt.HMAutoComplete();

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
            $("#cbCloudStatus").combobox('select',0);
            return false;
        });

        //search click begin
        $(".btnSearch").on("click", function () {
            var isValidated = $("#mediaLibForm").form('validate')
            if (!isValidated) {
                return false;
            }     
           
            var postData = PostDataModel.PostData.Get();
            //prograss.show();
            
            $('#mainarea').loadingOverlay({
                loadingText: 'Loading media'              // Text within loading overlay
            });
            $.ajax({
                url: "./SearchMediaLibrary",
                type: "POST",
                dataType: "json",
                //contentType: "application/json", //Invalid JSON primitive: page
                data: postData,
                success: function (retData) {
                    $('#mainarea').loadingOverlay('remove');
                    if (retData.Status === 1) {
                        MediaLibraryExtModel.MediaLibraryExt.DynamicShow(retData.Data, postData);
                    } else {
                        prograss.hide();
                        $.messager.alert('SearchMediaLibrary', retData.Data, 'error');
                    }
                },
                error: function (event, jqXHR, ajaxSettings, thrownError) {
                    //console.log(JSON.stringify(event));
                    $('#mainarea').loadingOverlay('remove');
                    $.messager.alert('SearchMediaLibrary', 'NetWork Error,Please contact administrator。', 'error');
                }
            });// end of ajax function

            return false;//can not be remove 

        })//search click end

        $("#arrL").click(function () {
            if ($("#aRow").text() == 1) {
                return;
            }
            $("#aRow").text(
                $("#aRow").text() - 1
                )
            $(".btnSearch").click();
        })

        $("#arrR").click(function () {
            if (parseInt($("#aRow").text()) * $("#opPageSize").val() >= $("#span_Result_Count").text()) {
                return;
            }
            $("#aRow").text(
                parseInt($("#aRow").text()) + 1
                )
            $(".btnSearch").click();
        })

        $("#opPageSize").change(function () {
            $("#aRow").text(1);
            $(".btnSearch").click();
        })

        $("#btn_addmedia").click(function () {
            var strHtml = "";
            strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src="../Media/FilesUpload"></iframe>';
            //notices:if the iframe name use id="frmWorkArea" , it will never create new tab but will replace old searhProduct page. This is not user want.. 2013/08/28.
            if (window.parent.jQuery('#centerTabs').tabs('exists', "Add Media")) {
                window.parent.jQuery('#centerTabs').tabs('select', "Add Media");
                //window.parent.jQuery("#frmTag").attr("src", '../ProductConfiguration/ProductConfiguration' + strReq);
            } else {
                window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "Add Media",
                    content: strHtml,
                    closable: true
                });
            }
            strHtml = "";
        })

        $.extend($.fn.validatebox.defaults.rules, {
            lessThan: {
                validator: function (value, param) {
                    //return value < $(param[0]).val();
                    //return DateFormatterModel.comparet2TimeBool(value, $(param[0]).val());
                    return DateFormatterModel.comparet2TimeBool(value, $(param[0]).datebox('getValue'));
                },
                message: 'start time must equal or less than end time.'
            }
        });


        $("#single_1").fancybox({
            helpers: {
                title: {
                    type: 'float'
                }
            }
        });

    });
})
