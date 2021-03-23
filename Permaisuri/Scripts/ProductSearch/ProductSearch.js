/// <reference path="../jquery-1.7.1-vsdoc.js" />
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var searchResultModele = require('./ProductSearchResultShow');
    var prograss = require('Common/Prograss');
    var tooTipModel = require('./ProductSearchTooltip.js');
    var UIExtModel = require('Common/EasyUIExt');
    var TagsModel = require('Common/OpenCMSTag');
    var ACModel = require('./FiledsAutoCompleted');
    
   //window全局变量
    window.orderInfo = {
        OrderBy: 7,//默认按照第修改时间列排序(ModeifedTime)
        OrderType:1//1：逆序排列（Desc)
    };
    $(document).ready(function () {
        TagsModel.CMSTags.BackSpace();
        $(".slidingDiv").hide();
        $(".show_hide").show();
        $('.show_hide').click(function () {
            $(".slidingDiv").slideToggle();
        });
        var reqFrom = $("#hiddenReqFrom").val();//标识是来自哪个页面的请求。
        var Inventory = 0;
        if (reqFrom == "lowInventory")//来自Dashboard页面的低库存查询请求
        {
            Inventory = 4;
        }
        var reqStatus = $.trim($("#reqStatus").val());
        if ((typeof (reqStatus) == "undefined") || reqStatus == "") {
            reqStatus = 0;
        }

        //单击标题列排序查询
        $(".reporttable thead th:gt(1)").css({
            "cursor": "pointer"
        }).on("click", function () {
            var orderBy = $(this).attr("orderBy");
            orderInfo["OrderBy"] = orderBy;//记录页面哪一列点击了排序
            orderInfo["OrderType"] = orderInfo["OrderType"] == "0" ? "1" : "0";
            searchResultModele.requestProductData(searchResultModele);

            //先移除掉所有的排序样式 
            $(this).parent().children().find(".sort-asc").removeClass("sort-asc");
            $(this).parent().children().find(".sort-desc").removeClass("sort-desc");
            //alert(orderInfo["OrderType"]);
            $(this).find("span").addClass(parseInt(orderInfo["OrderType"]) == 0 ? "sort-asc" : "sort-desc");//给该元素加上逆序/顺序的图标;

        });

        ACModel.AutoCompleted.Fileds($("#keywordbg"), 1);//1:HMNUM 
        ACModel.AutoCompleted.Fileds($(".searchSKU"), 2);//2:SKU;
        ACModel.AutoCompleted.Fileds($("#UpdateBy"), 3);//3:ModifiedUser
        tooTipModel.SKUList.InitToolTip();

        $(".btnReset").on("click", function () {
            $('#keywordbg').val("");
            $('.searchSKU').val("");
            $('#cbBrand').combobox('select', 0);
            $('#cbChannel').combobox('select', 0);
            $('#cbCategory').combobox('select', 0);
            $('#cbInventory').combobox('select', 0);
            $('#cbStatus').combobox('select', "0");
            $('#cbMultiplePart').combobox('select',0);
            $("#UpdateBy").val("");

            return false;
        });

        //校验
        UIExtModel.Ext.ValidateListLocal();
        $("#keywordbg").validatebox({
            validType: 'length[3,200]'
        });

        $(".searchSKU").css("background-repeat", "").css("background-image", "").addClass("easyui-validatebox").validatebox({
            validType: 'length[3,200]'
        });

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

        $('#cbCategory').combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbCategory','CategoryName']"
        });

        $("#cbMultiplePart").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbMultiplePart','text']"
        });

        $("#cbInventory").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbInventory','text']"
        });

        $("#cbStatus").combobox({
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
            },
            validType: "ValidateListLocal['#cbStatus','StatusName']"
        });
        $('#cbStatus').combobox('select', reqStatus);

        if (reqFrom == "lowInventory")//来自Dashboard页面的低库存查询请求
        {
            $('#cbInventory').combobox('select', 4);// set dropdown list default selected
        }


        //新增按钮事件绑定
        $(".addlt").on("click", function () {
            var strHtml = "";
            strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src="../SKUCreate/Index"></iframe>';
            if (window.parent.jQuery('#centerTabs').tabs('exists', "Add New Product")) {
                //由于管理tab下的iframe十分混乱，以后一律先关闭再打开！
                window.parent.jQuery('#centerTabs').tabs('close', "Add New Product");
                window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "Add New Product",
                    content: strHtml,
                    closable: true
                });
            } else {
                window.parent.jQuery('#centerTabs').tabs('add', {
                    title: "Add New Product",
                    content: strHtml,
                    closable: true
                });
            }
            strHtml = "";
        })

        prograss.show();
        $.ajax({
            url: "./InitSearchProducts",
            type: "POST",
            dataType: "json",
            data: {
                page: 1,
                rows: parseInt($("#opPageSize").val()),
                Keywords: $.trim($("#keywordbg").val()),
                BrandID: 0,
                ChannelID: 0,
                InventoryType: Inventory,
                Status: reqStatus,
                OrderBy: orderInfo["OrderBy"],
                OrderType: orderInfo["OrderType"]
            },
            success: function (retData) {
                prograss.hide();
                if (retData.Status === 1) {
                    var dgData = retData.Data["dgDatas"]["rows"];
                    var totalRecord = retData.Data["dgDatas"]["rows"];
                    searchResultModele.showResultTable(retData.Data["dgDatas"], { page: 1, rows: parseInt($("#opPageSize").val()) }, true);
                } else {
                    $.messager.alert('SearchProducts', retData.Data, 'error');
                }
            },
            error: function () {
                prograss.hide();
                $.messager.alert('SearchProducts', 'NetWork Error,Please contact administrator。', 'error');
            }
        }); // end of ajax func


        //Previous page
        $(".arrL").on("click", function () {
            if ($("#aRow").text() == 1) {
                return;
            }
            var curPage = parseInt($.trim($("#aRow").text())) - 1;
            $("#aRow").text(curPage);
            if ( parseInt(curPage) < 1) {
                return false;
            }
            searchResultModele.requestProductData(searchResultModele);
        });

        //Next page
        $(".arrR").on("click", function () {
            var curPage = parseInt($.trim($("#aRow").text()));
            $("#aRow").text(curPage);
            var totals = parseInt($.trim($("#hiddenTotal").val()));
            var maxRecords = curPage * parseInt($("#opPageSize").val());
            if (maxRecords > totals) {
                return false;
            }
            $("#aRow").text(curPage+1);
            searchResultModele.requestProductData(searchResultModele);
        });

        //dropdownlist 
        $("#opPageSize").change(function () {
            searchResultModele.requestProductData(searchResultModele);
        });

        $(".btnSearch").on("click", function () {
            $("#aRow").text("1");//Changed by Lee 2013-10-07
            searchResultModele.requestProductData(searchResultModele);
            return false;//2014年4月1日18:35:08 不加这句出BUG了...
          
        });


        //导出按钮
        $("#aExport").off("click").on("click", function () {       
            searchResultModele.requestProductData(searchResultModele, "ExporteCVS");
        });


    });// end of document.ready
});