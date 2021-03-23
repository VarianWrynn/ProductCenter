define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var prograss = require('Common/Prograss');

    //var chosenCSS = require('PlugIn/Chosen/chosen.min.css');//dynamically loading css
    var chosen = require('PlugIn/Chosen/chosen.jquery.min');

    var isOpened = false;
    exports.DuplicateProduct = function () {
        var duplicateW = $(window).width();
        var duplicateH = $(window).height();
        var curSKU = $("#SKU").val();

        $(window).resize(function () {
            var duplicateW = $(window).width();
            var duplicateH = $(window).height();
            if (!$('#duplicateDIV').is(":hidden")) {
                $('#duplicateDIV').dialog('resize',{
                    width: duplicateW - 30,
                    height: duplicateH - 150
                });
            }
        });

        $("#duplicateDIV").show(); //必须先show，否则里面的div input等html结构会看不到....
        if (!isOpened) {
            $("#duplicateDIV").dialog({
                title: 'Duplicate SKU ' + curSKU,
                width: duplicateW - 30,
                height: duplicateH - 150,
                closed: false,
                cache: false,
                modal: true
            });
            isOpened = true;
        } else {
            $("#duplicateDIV").dialog('open');
        }

        $('.duplicateBrand').combobox({
            required: true,
            url: "../CMSCacheableData/BrandList?isNeedAll=false",
            valueField: 'Brand_Id',
            textField: 'Brand_Name',
            validType: "ValidateListLocal['.duplicateBrand','Brand_Name']",
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) > 0;
            }
        });

        prograss.show();
        $.ajax({
            url: "../Common/GetChannelList?isNeedAll=false",
            type: "POST",
            dataType: "json",
            success: function (retData) {
                prograss.hide();
                var selHTML = '';
                $.each(retData, function () {
                    selHTML += '<option value=' + this.ChannelID + '>' + this.ChannelName + '</option>';
                })
                $(".multipleSelect").append(selHTML);

                $(".multipleSelect").chosen({
                    width: "95%",
                    placeholder_text_multiple: "Click here to select Channels. 'Alt + Right click' to select more than one.",
                    //inherit_select_classes: true,/*这个选项非常坑爹，加上去会导致打开各种报错. 加上chose('close')之后，虽然打开不报错，但是这里面的OPtion的设置都无效 2013年11月1日16:32:03*/
                    display_selected_options: false
                });
            },
            error: function () {
                prograss.hide();
                $.messager.alert('DuplicateProduct-GetChannelList', 'NetWork Error,Please contact administrator.', 'error');
            }
        }); // end of ajax func


        //add button
        $('.duplicatAddBtn').off("click").on("click", function () {
            var fvalidate = $("#duplicateForm").form('validate');
            if (!fvalidate) {
                return;
            }
            var newSKUOrder = $("#deplicateSKUName").val();
            var newBrandID = $('.duplicateBrand').combobox("getValue");
            var dupChanels = $(".multipleSelect").val();
            if (dupChanels == null)
            {
                //$(".multipleSelect").click();
                $(".chosen-container .chosen-choices").find("input").click();
                return false;
            }
            
            prograss.show();
            $.ajax({
                url: "./DuplicateMultipleNewSKU",
                type: "POST",
                dataType: "json",
                traditional: true,//prevent ajax deep copy oject 
                data: {
                    newSKUOrder: newSKUOrder,
                    newBrandID: newBrandID,
                    ChannelList:$(".multipleSelect").val(),
                    SKUID: $("#hiddenSKUID").val()
                },
                success: function (retData) {
                    prograss.hide();
                    if (retData.Status === 1) {
                        var cmsTagsModel = require('Common/OpenCMSTag');
                        var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
                        TagsParam.URL = "../Reports/Index?ReportType=3&QueueStatus=7",
                        TagsParam.name = "Product Development Report";
                        TagsParam.title = "Product Development Report";

                        TagsParam.iframeID = "frmWorkArea";
                        TagsParam.reload = true;
                        cmsTagsModel.CMSTags.OpenTags(TagsParam);

                        $("#duplicateDIV").dialog("close");
                    } else {
                        $.messager.alert('DuplicateProduct', retData.Data, 'error');
                    }
                },
                error: function () {
                    prograss.hide();
                    $.messager.alert('DuplicateProduct', 'NetWork Error,Please contact administrator。', 'error');
                }
            }); // end of ajax func
        });

        //Cancel button
        $('.duplicatCancelBtn').on("click", function () {
            $("#deplicateSKUName").val("");
            $('.duplicateBrand').combobox("setValue", "");
            $('.deplicateChannel').combobox("setValue", "");
            $("#duplicateDIV").dialog("close");
        });

    }
});