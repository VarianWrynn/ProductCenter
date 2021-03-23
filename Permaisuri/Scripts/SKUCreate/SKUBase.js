/// <summary>
/// HM#组合产品的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var ui = require('jquery-ui');
    var AddModel = require('./AddNewSKU');
    var CostModel = require('./SKUCosting');
    var DFormatterModel = require('Common/DecimalFormatter');
    var UIExtModel = require('Common/EasyUIExt');

    var ACModel = require('./SKUBaseInfo_AutoCompleted');

    exports.SKUBase = {
        //第一阶段： 初始化
        Init: function () {
            var InitSelf = this;

            //校验
            UIExtModel.Ext.ValidateListLocal();
            var brandW = $("#tdBrand").width() - 10;
            $('#cbBrand').combobox({
                width: brandW,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
                },
                validType: "ValidateListLocal['#cbBrand','Brand_Name']"
            });


            var channelW = $("#ProductName").width() + 10;
            $('#cbChannel').combobox({
                width: channelW,
                onChange:function(newValue, oldValue)
                {
                    if($("#SKU").val()!="")
                    {
                        //如果SKU有值，则触发校验让其去后台查询是否有重复 2014年6月11日17:51:02
                        var fvalidate = $("#NewSKUBaseForm").form('validate');
                        if (!fvalidate) {
                            return;
                        }
                    }
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
                },
                validType: "ValidateListLocal['#cbChannel','ChannelName']"
            });


            var shipViaTypeW = $("#tdShipViaType").width() * 0.75;
            $('#ShipViaType').combobox({
                width: shipViaTypeW,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
                },
                validType: "ValidateListLocal['#ShipViaType','ShipViaTypeName']"
            });

            ACModel.AutoCompleted.Fileds($("#Material"), "Material");
            ACModel.AutoCompleted.Fileds($("#Colour"), "Colour");
            ACModel.AutoCompleted.Fileds($("#Category"), "Category");
            ACModel.AutoCompleted.Fileds($("#SubCategory"), "SubCategory");

            CostModel.SKUCosting.Init();

            //QTY下拉单
            InitSelf.ConstructQTY();

            InitSelf.HMAutoComplete();

            //Binding submit event 
            $("#aSKUAdd").on("click", function () {
                var fvalidate = $("#NewSKUBaseForm").form('validate');
                if (!fvalidate) {
                    return;
                }
                AddModel.Ajax.AddNewSKU()
              
            });
        },

        ConstructQTY: function () {
            var QTYSelf = this;
            var comboQTY = [];
            for (var i = 1; i < 16; i++) {
                comboQTY.push(
                    {
                        QTYName: "QTY:" + i,
                        QTYValue: i,
                        selected: i === 1 ? true : false//默认就让程序选中第一个选项，避免出现各种问题 2013年12月26日10:35:38
                    });
            };
            $("input[name='comboQTY']").combobox({
                required: true,
                data: comboQTY,
                valueField: 'QTYValue',
                textField: 'QTYName',
                onSelect: function (rec) {
                    var fvalidate = $("#AddHMForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }

                }// end of onSelect function
            });// end of combobox event binding
        },//end of  ConstructQTY

        HMAutoComplete: function () {
            var HMGroupSelf = this;
            var isNeedCheckHM = true;//用来指示是否需要对用户输入的内容进行离开焦点校验，如果是从下拉单选择则不需要叫校验了
            var AutoHMData = {};
            $("input[name='autoHMNUM']").autocomplete({
                source: function (request, response) {
                    $("input[name='autoHMNUM']").addClass('auto-loading');
                    isNeedCheckHM = true;//reset 焦点指示器
                    AutoHMData = {};//reset
                    $.ajax({
                        url: "./GetProductInfo",
                        type: "POST",
                        dataType: "json",
                        //如果不设置为false,用户很喜欢Copy-Past然后鼠标离开触发onBlure事件，在异步的情况下，在该事件里面无法保证服务器的数据已经返回了，
                        // deferred对做不到 呵呵！【一句话，设置false是为了确保AutoHMData在onBlur的时候有数据】 2014年6月4日17:18:42
                        async: false,
                        data: {
                            ProductName: "",
                            HMNUM: request.term,
                            StatusID: 2//1－unaudited; 2-audited,3-disabled
                        }
                    }).done(function (retData) {
                        $("input[name='autoHMNUM']").removeClass('auto-loading');
                        AutoHMData = retData["Data"];
                        response($.map(retData["Data"], function (item) {
                            return {
                                label: item.HMNUM,
                                value: item.HMNUM,
                                ProductID: item.ProductID,
                                MasterPack: item.MasterPack,
                                strMasterPack: item.strMasterPack,
                                ProductName: item.ProductName,
                                StockKey: item.StockKey,
                                StockKeyID: item.StockKeyID,
                                HMNUM: item.HMNUM,
                                HMCostID: item.HMCostID,
                                StockKeyQTY: item.StockKeyQTY
                            }
                        }));
                    }).fail(function () {
                        $("input[name='autoHMNUM']").removeClass('auto-loading');
                        $.messager.alert('Add HM#', 'NetWork Error,Please contact administrator。', 'error');
                        return false;
                    });
                },// end of source function

                minLength: 1,
                delay:800,//延迟0.8秒 2014年6月11日17:31:04
                select: function (event, ui) {
                    var obj = {};
                    $.each(AutoHMData, function () {
                        if (this.HMNUM == ui.item.HMNUM) {
                            obj = this;
                        }
                    });
                    //$("input[name='autoHMNUM']").val(ui.item.HMNUM);
                    HMGroupSelf.SetHMInfo(obj);
                }
            }).off("blur").on("blur", function () { // end of aut alert(JSON.stringify(AutoHMData));
                var curValue = $(this).val();
                var IsValueIncluded = false;
                $.each(AutoHMData, function () {
                    if (this.HMNUM == curValue) {
                        $("#ProductID").val(this.ProductID);
                        IsValueIncluded = true;
                    }
                });
                if (IsValueIncluded == false) {
                    $("input[name='autoHMNUM']").val("")
                    $("#ProductID").val("0");
                }
                var fvalidate = $("#AddHMForm").form('validate');
                if (!fvalidate) {
                    return;
                }
                //alert(IsValueIncluded);

            });//end of blur event;
        },// end of Func HMAutoComplete

        //从WebPO,Ecom获取图像（根据HM#）
        GetImagesFromOtherSystem: function (HMNUM) {
            $("#WEBPOImgDIV").addClass('auto-loading');
            $.ajax({
                url: "../HMGroupConfig/GetImagesFromOtherSystem",
                type: "POST",
                dataType: "json",
                data: { "HMNUM": HMNUM }
            }).done(function (retData) {
                $("#WEBPOImgDIV").removeClass('auto-loading');
                if (retData.Status === 1) {
                    var strHTML = '';
                    strHTML += '<a class="fancybox" href="' + retData["Data"][0]["Pic"] + '" title="Come from ' + retData["Data"][0]["SystemName"] + '">';
                    strHTML += '<img src="' + retData["Data"][0]["SmallPic"] + '"  onerror="this.src=\'../Content/images/NoPic.jpg\' ">';
                    strHTML += '</a> ';;
                    $("#WEBPOImgDIV").html(strHTML);
                } else {
                    $.messager.alert('HMConfig-GetImagesFromOtherSystem', retData.Data, 'error');
                    return false;
                }
            }).fail(function () {
                $("#WEBPOImgDIV").removeClass('auto-loading');
                $.messager.alert('HMConfig-GetImagesFromOtherSystem', 'Failed to edit,please contact administrator', 'error');
                return false;
            });
        },

        //2014年2月18日11:40:03
        //HMObj:货号信息；hmType指示当前是HMNUM做变动还是Name做变动；  selectSelf：当前发生变动的对象（通常是一个ID）
        SetHMInfo: function (HMObj) {
            var SetSelf = this;
            $("#comboQTY").combobox('setValue', HMObj.MasterPack);
            $("#MasterPack").val(HMObj.strMasterPack);
            $("#ProductID").val(HMObj.ProductID);
            $("#autoHMNUM").val(HMObj.HMNUM);
            $("#autoProductName").val(HMObj.ProductName);
            $("#Inventory").val(HMObj.StockKeyQTY);

            var strHTML = '';
            if (HMObj["webSystemImage"] != null) {

                strHTML += '<a class="fancybox" href="' + HMObj["webSystemImage"]["Pic"] + '" title="Come from ' + HMObj["webSystemImage"]["SystemName"] + '">';
                strHTML += '<img src="' + HMObj["webSystemImage"]["SmallPic"] + '"  onerror="this.src=\'../Content/images/NoPic.jpg\' ">';
                strHTML += '</a> ';

            } else {
                strHTML += '<a class="fancybox" href="../Content/images/NoPic.jpg" title=" CMS default picture">';
                strHTML += '<img src="../Content/images/NoPic.jpg" >';
                strHTML += '</a> ';
            }
            $("#WEBPOImgDIV").html(strHTML);
            strHTML = null;

            $("#Material").val(HMObj.HMMaterial == null ? "" : HMObj.HMMaterial.MaterialName);
            $("#Colour").val(HMObj.HMColour == null ? "" : HMObj.HMColour.ColourName);
            $("#Category").val(HMObj.Category == null ? "" : HMObj.Category.ParentCategoryName);
            $("#CategoryID").val(HMObj.Category == null ? "0" : HMObj.Category.ParentCategoryID);
            $("#SubCategory").val(HMObj.Category == null ? "" : HMObj.Category.CategoryName);
            $("#ShipViaType").combobox('setValue', HMObj.CMS_ShipVia_Type == null ? "" :
                (HMObj.CMS_ShipVia_Type.ShipViaTypeID == 0 ? "" : HMObj.CMS_ShipVia_Type.ShipViaTypeID));
            
        }
    }
});