define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var DFormatterModel = require('Common/DecimalFormatter');

    var overlaycss = require('PlugIn/Overlay/overlay.css');
    var overlayModel = require('PlugIn/Overlay/loading-overlay.min');


    exports.HMNUM = {
        Init: function (SKUInfo) {
            $("#HMDetail").data("SKUInfo", SKUInfo);//存储信息，获取 $("#HMDetail").data("SKUInfo");
            HMSelf = this;
            //非组合产品的QTY的下拉单事件绑定
            $(".BasicQTY").combobox({
                required: true,
                onSelect: function (rec) {
                    var fvalidate = $("#HMDetail").form('validate');
                    if (!fvalidate) {
                        return;
                    }
                    var comboQTYSelf = this;
                    $.messager.confirm('Confirm', 'Are you sure to change this sellsets?', function (r) {
                        if (r) {
                            $('#productPiecesTable').loadingOverlay({
                                loadingText: 'Updating'              // Text within loading overlay
                            });
                            $.ajax({
                                url: "./UpdateSellPack",
                                type: "POST",
                                dataType: "json",
                                data: {
                                    SKUID: $("#hiddenSKUID").val(),
                                    ProductID: $("#hiddenProductID").val(),
                                    //StockKeyID: $("#hiddenStockKey").val(),
                                    StockKeyID: $("#hiddenStockKeyID").val(),
                                    R_QTY: rec.value
                                }
                            }).done(function (retData) {
                                $('#productPiecesTable').loadingOverlay('remove');
                                if (retData.Status === 1) {

                                    var newSKUInfo = $("#HMDetail").data("SKUInfo");

                                    var curTD = $(comboQTYSelf).parent().parent();
                                    var newBoxNum = rec.value / newSKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["MasterPack"];
                                    curTD.find(".hiddenSellPack").val(rec.value);
                                    curTD.find(".BoxNum").val(newBoxNum);

                                    /*更新HMNUM的各种信息...*/
                                    newSKUInfo["SKU_HM_Relation"]["R_QTY"] = rec.value;
                                    newSKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["BoxNum"] = newBoxNum;

                                    HMSelf.StatisticHMNUM(newSKUInfo);
                                    $("#HMDetail").data("SKUInfo", newSKUInfo);
                                } else {
                                    $(".BasicQTY").combobox('setValue', "");
                                    $.messager.alert('Product Pieces', retData["Data"], 'error');
                                }
                            }).fail(function () {
                                $('#productPiecesTable').loadingOverlay('remove');
                                $(".BasicQTY").combobox('setValue', "");
                                $.messager.alert('Product Pieces', 'NetWork Error,Please contact administrator。', 'error');
                            });
                        } else {
                            //找出hiddenValue,进行回滚...
                            var curTr = $(comboQTYSelf).parent().parent();
                            var curSellPack = curTr.find(".hiddenSellPack").val();
                            var curCombo = curTr.find(".BasicQTY");
                            curCombo.combobox("setValue", curSellPack);
                        }
                    });
                }// end of onSelect function
            });

            $("#gotoHMNUMConfig").off("click").on("click", function () {
                var strHtml = "";
                var tempURL = "";
                var tagName = "";
                if (SKUInfo.IsGroup) {
                    tagName = "HMGroupConfig";
                    tempURL = '../HMGroupConfig/Config?ProductID=' + SKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["ProductID"];
                } else {
                    tagName = "HMConfig";
                    tempURL = '../HMConfig/Index?ProductID=' + SKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["ProductID"];
                }
                strHtml += '<iframe id="frmTag" width="100%" height="99%" frameborder="0" scrolling="auto" src=' + tempURL + '></iframe>';
                if (window.parent.jQuery('#centerTabs').tabs('exists', tagName)) {
                    window.parent.jQuery('#centerTabs').tabs('close', tagName);
                } window.parent.jQuery('#centerTabs').tabs('add', {
                    title: tagName,
                    content: strHtml,
                    closable: true
                });
                strHtml = "";
            });

            HMSelf.StatisticHMNUM(SKUInfo);

        },

        StatisticHMNUM:function(SKUInfo)
        {
            var statSelf = this;
            statSelf._UpdateCosting(SKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["HM_Costing"], SKUInfo["SKU_HM_Relation"]["R_QTY"]);

            if (SKUInfo.IsGroup) {
                statSelf._StatisticGroup(SKUInfo)
            } else
            {
                statSelf._StatisticBasic(SKUInfo)
            }
        },

        _StatisticBasic: function (SKUInfo) {
            var totalPieces = 0;
            var totalWeight = 0;
            var totalBox = 0;
            var totalBoxDesc = "";

          
            totalPieces = SKUInfo["SKU_HM_Relation"]["R_QTY"];
            var boxN = SKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["BoxNum"];

            totalBox = boxN;
            $.each(SKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["CTNList"], function () {
                totalWeight += (this.CTNWeight * boxN);
                totalBoxDesc += '<div>Box ' + this["CTNTitle"] + ': <strong> ' + this["CTNLength"] + ' x ' + this["CTNWidth"] + 'x ' + this["CTNHeight"] + ' (x' + boxN + ') </strong></div>';
            });
            $(".totalPieces").html(totalPieces);
            $(".totalWeight").html(DFormatterModel.DecimalFormate.toDecimal2(totalWeight) + " (lbs)");
            $(".totalBox").html(totalBox);
            $(".totalBoxDesc").html(totalBoxDesc);
        },

        _StatisticGroup: function (SKUInfo) {
            var totalPieces = 0;
            var totalWeight = 0;
            var totalBox = 0;
            var totalBoxDesc = "";

            var sellPack = SKUInfo["SKU_HM_Relation"]["R_QTY"];

            $.each(SKUInfo["SKU_HM_Relation"]["CMS_HMNUM"]["Children_CMS_HMNUM_List"], function () {
                totalPieces += parseInt(this.SellSets) * parseInt(sellPack);
                var boxN = parseInt(this.BoxNum) * parseInt(sellPack);
                totalBox += boxN;
                $.each(this.CTNList, function () {
                    totalWeight += (this.CTNWeight * boxN);//一个纸箱的重量乘以纸箱数
                    totalBoxDesc += '<div>Box ' + this["CTNTitle"] + ': <strong> ' + this["CTNLength"] + ' x ' + this["CTNWidth"] + 'x ' + this["CTNHeight"] + ' (x' + boxN + ') </strong></div>';
                });
            });
            $(".totalPieces").html(totalPieces);
            $(".totalWeight").html(DFormatterModel.DecimalFormate.toDecimal2(totalWeight) + " (lbs)");
            $(".totalBox").html(totalBox);
            $(".totalBoxDesc").html(totalBoxDesc);
        },
     
        _UpdateCosting: function (cost, SellPack) {
            var tFirstCost = 0;//t==>total
            var tLandedCost = 0;
            var tEstimateFreight = 0;

            var tOceanFreight = 0;
            var tUSAHandlingCharge = 0;
            var tDrayage = 0;

            var totalCost = 0;

            //对每一个同类型的价格相加 tr.replace(/\-/g,"!")则可以全部替换掉匹配的字符(g为全局标志)。 
            //以下这种方式对于接续科学格式（eg:"99,999,999,999.00")会解析出BUG，最后接续得出只有99! 2014年2月19日
            //tFirstCost += parseFloat((cost.FirstCost).replace('$', '')) * parseInt(QTY);
            tFirstCost += parseFloat((cost.FirstCost).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(SellPack);
            tLandedCost += parseFloat((cost.LandedCost).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(SellPack);;
            tEstimateFreight += parseFloat((cost.EstimateFreight).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(SellPack);

           
            tOceanFreight += parseFloat((cost.OceanFreight).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(SellPack);
            tUSAHandlingCharge += parseFloat((cost.USAHandlingCharge).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(SellPack);
            tDrayage += parseFloat((cost.Drayage).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(SellPack);

            //totalCost = tLandedCost + tEstimateFreight
            totalCost = tFirstCost + tOceanFreight + tUSAHandlingCharge + tDrayage;
            $("#totalCost").html("$" + DFormatterModel.DecimalFormate.toDecimal2(totalCost));
            $("#FirstCost").val('$' + DFormatterModel.DecimalFormate.toDecimal2(totalCost));//2014年4月24日18:01:57  你懂得
            //$("#LandedCost").val('$' + DFormatterModel.DecimalFormate.toDecimal2(tLandedCost));
            $("#EstimateFreight").val('$' + DFormatterModel.DecimalFormate.toDecimal2(tEstimateFreight));
        }

    }
});