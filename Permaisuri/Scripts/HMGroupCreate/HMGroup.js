/// <summary>
/// HM#组合产品的维护
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');
    var UIExtModel = require('Common/EasyUIExt');
    var DFormatterModel = require('Common/DecimalFormatter');
    var ui = require('jquery-ui');
    var AjaxModel = require('./HMGroupAjax');

    exports.HMGroup = {
        //第一阶段： 初始化
        Init: function () {
            var InitSelf = this;

            //下拉单校验
            UIExtModel.Ext.ValidateListLocal();

            //整除校验
            UIExtModel.Ext.ValidateExactDivided();

            //初始化Category
            var curW = $("#HMNUM").width();
            var CategoryName = $('#CategoryName').combobox({
                required: true,
                width: curW+20,
                url: '../CMSCacheableData/GetGroupCategoryFromWEBPO',
                method:'POST',
                valueField: 'CategoryID',
                //textField: 'CategoryName',
                textField: 'ParentCategoryName',
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) > -1;
                },
                validType: "ValidateListLocal['#CategoryName','ParentCategoryName']"
            });

            $('#CategoryName').combobox("select", 78);//78....因为WEBPO是78


            var curShipVW = $("#ProductName").width();
            $("#ShipViaType").combobox({
                width: curShipVW + 35,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) == 0;
                },
                validType: "ValidateListLocal['#ShipViaType','ShipViaTypeName']"
            });

            $(window).resize(function () {
                var curW = $("#HMNUM").width()+10;
                $('#CategoryName').combobox('resize', curW);
                //$("#CategoryName").combobox({ 这种方法会有BUG，当用户选择了一个选项，再次触发resize之后，选择项会被清空！
                //    width: curW + 15
                //});

                var curShipVW = $("#ProductName").width()+10;
                $('#ShipViaType').combobox('resize', curShipVW);
            });
          



            $("#HMNUM").off("blur").on("blur", function () {
        
                if ($("#HMNUM").val().indexOf('.') == 0)//标点符号在第一个位置
                {
                    alert("HMNUM first character can be a '.'")
                    return;
                } else if (($("#HMNUM").val().lastIndexOf('.') + 1) == ($("#HMNUM").val().length))
                {
                    alert("HMNUM last character can be a '.' ")
                    return;
                }
                $("#StockKey").val($("#HMNUM").val());
            });

            //隐藏掉Costing,Cartons Dimensions 和 Add HM# 按钮
            $("#AddHMForm,#HMCostingForm,#productCTN,#productDim").hide();

            //设置Status的初始状态
            $('#Status').combobox({
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) > -1;
                },
                validType: "ValidateListLocal['#Status','StatusName']"
            });


            //提交组合产品的基本信息到服务器保存
            $(".submitGPBaseInfo").off("click").on("click", function () {
                if ($("#HMNUM").val().indexOf('.') == 0)//标点符号在第一个位置
                {
                    alert("HMNUM first character can be a '.'")
                    return;
                } else if (($("#HMNUM").val().lastIndexOf('.') + 1) == ($("#HMNUM").val().length)) {
                    alert("HMNUM last character can be a '.' ")
                    return;
                }
                AjaxModel.Ajax.BaseInfoAdd(function () {
                    InitSelf.BaseHMAdd();
                });
            });

            //最后确定按钮
            $("#Go2Confirm").on("click", function () {
                var fvalidate = $("#HMBaseForm").form('validate');
                if (!fvalidate) {
                    return;
                }
                var fvalidate = $("#AddHMForm").form('validate');
                if (!fvalidate) {
                    return;
                }
                var fvalidate = $("#HMCostingForm").form('validate');
                if (!fvalidate) {
                    return;
                }
                $.messager.confirm('Confirm', 'Are you sure <b>leave</b> this page and go to ProductConfiguration?', function (r) {
                    if (r) {

                        //在离开本页面之前，应该对当前价格做一次更新，防止出现当前价格有值，到了配置页面依然是0的情景 2014年3月25日
                        var costingModel = require('./HMGPCosting');
                        
                        //costingModel.HMCosting.EditCosting(InitSelf.GotoConfig());
                        costingModel.HMCosting.EditCosting(function () {
                            InitSelf.GotoConfig()
                        });
                    }
                });
            });
        },

        //第2阶段：添加组合产品的基本产品信息
        BaseHMAdd: function () {
            var BaseSelf = this;
            //禁止基础字段修改,隐藏提交按钮
            $("#HMBaseForm input,HMBaseForm textarea").prop("disabled", true);
            $("#CategoryName").combobox('disable');
            $("#Status").combobox('disable');
            $("#ShipViaType").combobox('disable');
            $("#submitGPBaseInfo").hide();
            $("#Status").combobox('disable');

            ////显示基础字段的编辑、撤销按钮,暂时不做2013年11月20日15:58:53
            //$(".editGPBaseInfo, .cancelGPBaseInfo").show();
            //绑定编辑、撤销按钮的事件....later...

            //显示 “添加基础HM#表单”，并且绑定添加HM#的按钮事件
            $("#AddHMForm,#HMCostingForm").show();

            //绑定Costing逻辑控制
            var CostModel = require('./HMGPCosting');
            CostModel.HMCosting.Init();

            //QTY下拉单
            BaseSelf.ConstructQTY();
            //绑定HM，ProductName的autoCompleted事件;
            BaseSelf.HMAutoComplete();
            //BaseSelf.ProductNameAutoComplete();
            BaseSelf.CartonTrash();

            //绑定AddHM# button's event:
            $(".addNewHM").off("click").on("click", function () {
                //校验前面那个HM#是否正确的选择完毕了，如果没有，则不让其新增。这么做是为了控制逻辑走向，否则这里的提交逻辑就要做大变动，必须以list形式提交
                //到后台做初始化，而不是目前一个一个的新增这种形式。Lee 2013年11月19日15:06:25

                var fvalidate = $("#AddHMForm").form('validate');
                if (!fvalidate) {
                    return;
                }

                var newHMHtml = '';
                newHMHtml += '<tr cpid="0" RID="0">';

                //image column -start,这里展示的是WEBPO的图像 而不是CMS本身自带的图像,由于组合产品是包含多张图片，回调的时候需要知道哪张图对于哪个HM#，所以ID在这里需要加上HM#的标识
                newHMHtml += '<td>'
                newHMHtml += '<div name="WEBPOImgDIV">';
                newHMHtml += '</td>'
                //image column -end

                newHMHtml += '<td> <input type="text" name="comboQTY" class="comboQTY" style="width: 120px; height: 28px;"></td>';
                newHMHtml += '<td> <input type="text" name="MasterPack" style="width: 70px; height: 28px;" value="N/A" disabled="disabled" > </td>';
                newHMHtml += '<td> <input type="text" name="autoHMNUM" data-options="required:true" class="easyui-validatebox" style="width: 180px; height: 28px;" value=""></td>';
                newHMHtml += '<td> <input type="text" name="autoProductName"  disabled="disabled" style="width: 260px; height: 28px;" value=""></td>';
                newHMHtml += '<td> <input type="text" name="Inventory" style="width:50px;height:28px;" disabled="disabled" value="0" ></td>';
                newHMHtml += '<td> <img class="cartonTrash" src="../Content/Products/images/trash-big.png" style="cursor: pointer;"></td>';
                newHMHtml += '</tr>';

                /*这里调试了N久，注意了 是在body上直接append,append的意思就是在body里面 而不是后面 append上这个元素  2013年11月19日16:22:21*/
                $(".addHMArea table tbody").append(newHMHtml);

                //QTY下拉单
                BaseSelf.ConstructQTY();
                //绑定HM，ProductName的autoCompleted事件;
                BaseSelf.HMAutoComplete();
                //BaseSelf.ProductNameAutoComplete();

                BaseSelf.CartonTrash();
                //$.parser.parse();//需要加上这句话才能使easyUI的校验重新打开
                $.parser.parse("#AddHMForm");//2014年5月8日17:52:38
            });

        },

        //第3阶段：组合产品本身的价格更新(每一次新增，删除，更新子产品的信息之后触发）
        UpdateCosting: function (costDataList) {
            var tFirstCost = 0;//t==>total
            var tLandedCost = 0;
            var tEstimateFreight = 0;
            //对每一个同类型的价格相加
            $.each(costDataList, function () {
                //tr.replace(/\-/g,"!")则可以全部替换掉匹配的字符(g为全局标志)。 
                tFirstCost += parseFloat((this.FirstCost).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(this.SellSets);
                tLandedCost += parseFloat((this.LandedCost).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(this.SellSets);;
                tEstimateFreight += parseFloat((this.EstimateFreight).replace(/\$/g, '').replace(/\,/g, '')) * parseInt(this.SellSets);;
            });

            $("#FirstCost").val('$' + DFormatterModel.DecimalFormate.toDecimal2(tFirstCost));
            $("#LandedCost").val('$' + DFormatterModel.DecimalFormate.toDecimal2(tLandedCost));
            $("#EstimateFreight").val('$' + DFormatterModel.DecimalFormate.toDecimal2(tEstimateFreight));

        },


        //第4阶段：成功选择回显价格后，开始转跳到配置页面
        GotoConfig: function () {
            var cmsTagsModel = require('Common/OpenCMSTag');
            var TagsParam = cmsTagsModel.CMSTags.getTagOptions();
            var ProductID = $("#hiddenProductID").val();
            if (parseInt(ProductID) < 1) {
                $.messager.alert('error', "please select a validated HMNUM!", 'error');
            }
            TagsParam.URL = "../HMGroupConfig/Config?ProductID=" + ProductID;
            TagsParam.name = "HMGroupConfig";
            TagsParam.title = "HMGroupConfig";
            TagsParam.iframeID = "frmTag";//--新建而不是覆盖...
            cmsTagsModel.CMSTags.OpenTags(TagsParam);
            //关闭当前页面
            window.parent.jQuery('#centerTabs').tabs('close', 'Create HM Group');

        },

        ConstructQTY: function () {
            var QTYSelf = this;
            var comboQTY = [];
            for (var i = 1; i < 16; i++) {
                comboQTY.push(
                    {
                        QTYName: "QTY:" + i,
                        QTYValue: i,
                        selected: i === 1 ? true : false
                    });
            };
            $("input[name='comboQTY']").combobox({
                disabled:true,//2014年3月17日 Lee
                required: true,
                data: comboQTY,
                valueField: 'QTYValue',
                textField: 'QTYName',
                onSelect: function (rec) {
                    var fvalidate = $("#AddHMForm").form('validate');
                    if (!fvalidate) {
                        return;
                    }
                    var selfItem = this;
                    var cpid = $(selfItem).parent().parent().attr("cpid");
                    var rID = $(selfItem).parent().parent().attr("rid");
                    if (parseInt(cpid) < 1)
                    {
                        //防止客户端随便乱输HM#后选择QTY提交后台
                        $.messager.alert('Add HM#', 'Please select correct HM# item before submit this item', 'error');
                        return false;
                    }

                    var potions = {
                        sender: QTYSelf,
                        self: selfItem,
                        postData: {
                            RID: rID,
                            ProductID: $("#hiddenProductID").val(),//这个item已经在用户第一阶段提交之后自动做了更新，确保不会为0了
                            ChildrenProductID: cpid,
                            SellSets: rec.QTYValue
                        },
                        successCallBack: function () {
                           
                        },
                        errorCallBack: function () {
                            //执行失败，则清空
                            $(selfItem).val('');
                        }
                    };
                    
                    AjaxModel.Ajax.NewHMItemAdd(potions);
                }// end of onSelect function
            });// end of combobox event binding
        },//end of  ConstructQTY

        CartonTrash: function () {
            var HMGroupSelf = this;
            //绑定删除按钮
            $(".cartonTrash").css("cursor", "pointer").off("click").on("click", function () {
                //关于删除。现在考虑的是需要区分当前列是否已经成功插入到了数据库，如果是删除的时候需要直接删除数据库，否则直接remove掉当前item
                //给当前tr的cPid(ChildrenProductID)赋值钱
                var self = this;
                var cTR = $(self).parent().parent()
                var RID = cTR.attr("RID");
                if (parseInt(RID) < 1)//直接remove，不需要和Server交互
                {
                    cTR.remove();
                    return false;
                }
                $.messager.confirm('Confirm', 'Are you sure to delete this item?', function (r) {
                    if (r) {
                        progress.show();
                        $.ajax({
                            url: "./DeleteChildrenHM",
                            type: "POST",
                            dataType: "json",
                            data: {
                                RID: RID,
                                ProductID: $("#hiddenProductID").val()//用于提取价格
                            },
                            success: function (retData) {
                                progress.hide();
                                if (retData.Status === 1) {
                                    HMGroupSelf.UpdateCosting(retData.Data["ChildrenCostList"]);
                                    cTR.remove();
                                } else {
                                    $.messager.alert('Product Pieces Delete', retData["Data"], 'error');
                                }
                            },
                            error: function () {
                                progress.hide();
                                $.messager.alert('Product Pieces Delete', 'NetWork Error,Please contact administrator。', 'error');
                            }
                        }); // end of ajax func
                    }
                });// end of   $.messager.confirm

                return false;//for the Safari
            }); //end of  //绑定删除按钮
        },

        HMAutoComplete: function () {
            var HMGroupSelf = this;
            var isNeedCheckHM = true;//用来指示是否需要对用户输入的内容进行离开焦点校验，如果是从下拉单选择则不需要叫校验了
            $("input[name='autoHMNUM']").autocomplete({
                delay: 500,
                source: function (request, response) {
                    $("input[name='autoHMNUM']").addClass('auto-loading');
                    isNeedCheckHM = true;//reset 焦点指示器
                    $.ajax({
                        url: "./GetProductInfo",
                        type: "POST",
                        dataType: "json",
                        data: {
                            ProductName:"",
                            HMNUM: request.term,
                            ProductID:$("#hiddenProductID").val(),//排除掉已经选择HMNUM 2014年3月17日
                            HMType: 1
                        },
                        success: function (retData) {
                            $("input[name='autoHMNUM']").removeClass('auto-loading');
                            response($.map(retData["Data"], function (item) {
                                return {
                                    label: item.HMNUM,
                                    value: item.HMNUM,
                                    ProductID: item.ProductID,
                                    MasterPack:item.MasterPack,
                                    strMasterPack: item.strMasterPack,
                                    ProductName: item.ProductName,
                                    StockKey: item.StockKey,
                                    HMNUM: item.HMNUM,
                                    HMCostID: item.HMCostID,
                                    StockKeyQTY: item.StockKeyQTY
                                }
                            }));

                        },// end of success
                        error: function () {
                            $("input[name='autoHMNUM']").removeClass('auto-loading');
                            $.messager.alert('Add HM#', 'NetWork Error,Please contact administrator。', 'error');
                            return false;
                        }
                    }); // end of ajax func
                },// end of source function

                minLength: 1,
                select: function (event, ui) {
                    isNeedCheckHM = false;
                    
                    HMGroupSelf.SetNewHMInfo(ui.item, "HMNUM", this);
                }
            }).off("blur").on("blur", function () { // end of autocomplete ;
                if (!isNeedCheckHM) {
                    return false;
                }
                
                var selfText = this;
                $(selfText).addClass('auto-loading').prop("disabled", true);
                $.ajax({
                    url: "../Common/CheckHMNUM",
                    type: "POST",
                    dataType: "json",
                    data: {
                        HMNUM: $(selfText).val()
                    }
                }).done(function (retData) {
                    $(selfText).removeClass('auto-loading').prop("disabled", false);
                    if (retData.Status === 1) {
                        HMGroupSelf.SetNewHMInfo(retData["Data"], "HMNUM", selfText);
                    } else {
                        //这里再做一次检验，以期待解决那个“选择了某个HMNUM，但是又被自动清空，但是图像什么都出来的问题”2014年3月26日
                        if (!isNeedCheckHM) {
                            return false;
                        }
                        //不存在这个对象，清空
                        $(selfText).val("");
                        $(selfText).validatebox('validate');
                    }
                }).fail(function () {
                    $("input[name='comboHMNUM']").removeClass('auto-loading').prop("disabled", false);
                    $.messager.alert('Check HMNUM', 'NetWork Error,Please contact administrator。', 'error');
                })
            });//end of blur event;
        },// end of Func HMAutoComplete

        //从WebPO,Ecom获取图像（根据HM#）
        GetImagesFromOtherSystem: function (HMNUM, inputSelf) {
            //$("#WEBPOImgDIV").addClass('auto-loading');
            var curImgDiv = inputSelf.parent().parent().find("div[name='WEBPOImgDIV']");
            curImgDiv.addClass('auto-loading');
            $.ajax({
                url: "../HMGroupConfig/GetImagesFromOtherSystem",
                type: "POST",
                dataType: "json",
                data: { "HMNUM": HMNUM }
            }).done(function (retData) {
                //$("#WEBPOImgDIV").removeClass('auto-loading');
                curImgDiv.removeClass('auto-loading');
                if (retData.Status === 1) {
                    var strHTML = '';
                    strHTML += '<a class="fancybox" href="' + retData["Data"][0]["Pic"] + '" title="Come from ' + retData["Data"][0]["SystemName"] + '">';
                    strHTML += '<img src="' + retData["Data"][0]["SmallPic"] + '"  onerror="this.src=\'../Content/images/NoPic.jpg\' ">';
                    strHTML += '</a> ';;
                    curImgDiv.html(strHTML);
                } else {
                    $.messager.alert('HMConfig-GetImagesFromOtherSystem', retData.Data, 'error');
                    return false;
                }
            }).fail(function () {
                //$("#WEBPOImgDIV").removeClass('auto-loading');
                curImgDiv.removeClass('auto-loading');
                $.messager.alert('HMConfig-GetImagesFromOtherSystem', 'Failed to edit,please contact administrator', 'error');
                return false;
            });
        },

        ContructToolTip: function () {
            $("#tooltipMP").css({ "cursor": "pointer" }).tooltip({
                position: 'top',
                deltaX: 0,//负数代表向X轴的左方移动
                deltaY: 10,//负数代表向Y轴的上方移动。。。。
                content: '<span style="color:#fff">MP:Master Pack. Indicator:A box can hold how many pieces.' +
                    '<br> <b> For example: if current SKU have 6 HM# pieces , and MP is 3,<br> then it must need 2 box to load (6/3=2)</b></span>',
                onShow: function () {
                    $(this).tooltip('tip').css({
                        backgroundColor: '#666',
                        borderColor: '#666'
                    });
                }
            });
        },

        //2014年2月18日11:40:03
        //HMObj:货号信息；hmType指示当前是HMNUM做变动还是Name做变动；  selectSelf：当前发生变动的对象（通常是一个ID）
        /*
            potions:{
              sender:object,事件的发送者句柄,HMGroupSelf
              self:object,触发事件的某个元素,selectSelf
              postData:{},Ajax提交数据
              successCallBack:function(){},//执行成功后的回调函数
              errorCallBack:function(){}//执行失败后的回调函数
            }
        */
        SetNewHMInfo: function (HMObj, hmType, selectSelf) {
            var HMGroupSelf = this;
            //确定当前HM#用户选择的数量QTY：
            var curQTY = $(selectSelf).parent().parent().find("input[name='comboQTY']").val();
            var rID = $(selectSelf).parent().parent().attr("RID");
            //2013年12月26日14:40:36 增加库存信息，增加WEBPO图片信息
            $(selectSelf).parent().parent().find("input[name='Inventory']").val(HMObj.StockKeyQTY);
            $(selectSelf).parent().parent().find("input[name='MasterPack']").val(HMObj.strMasterPack);//2014年2月22日6:47:58

            HMGroupSelf.GetImagesFromOtherSystem(HMObj.HMNUM, $(selectSelf));
            var potions = {
                sender: HMGroupSelf,
                self: selectSelf,
                postData: {
                    RID: rID,
                    ProductID: $("#hiddenProductID").val(),//这个item已经在用户第一阶段提交之后自动做了更新，确保不会为0了
                    ChildrenProductID: HMObj.ProductID,
                    SellSets: curQTY
                },
                successCallBack: function () {
                    //这里应该设置2个属性，一个cPid用来给QTY下拉单变动的时候与后台交互使用。cPid(ChildrenProductID)
                    ///另一个是RID用来用于在删除时候区分当前列是否已经成功插入到了数据库，如果是删除的时候需要直接删除数据库，否则直接remove掉当前item
                    $(selectSelf).parent().next().find("input[name='autoProductName']").val(HMObj.ProductName);
                    $(selectSelf).parent().parent().attr("cpid", HMObj.ProductID);

                    /*设置默认的QTY数量的最小值，= MasterPack x 1,同时设置当前列的那个QTY下拉单为可用(enbaled)*/
                    //var curRowComboQTY = $(selectSelf).parent().parent().find("*[name='comboQTY']"); 完全无效....
                    var curRowComboQTY = $(selectSelf).parent().parent().find(".comboQTY");
                    //return false;
                    curRowComboQTY.combobox({
                        disabled: false,
                        validType: 'ExactDivided[' + HMObj.MasterPack + ']',//对于已经赋值的combobox进行校验MasterPack
                    });
                    curRowComboQTY.combobox('select', HMObj.MasterPack);  
                },
                errorCallBack: function () {
                    //执行失败，则清空HM#和ProductName
                    $("#MasterPack").val("N/A");
                    $(selectSelf).val("");
                    $(selectSelf).parent().next().find("input[name='autoProductName']").val("");
                }
            };
           
            AjaxModel.Ajax.NewHMItemAdd(potions);//这里面包含了价格的更新
        }
    }
});