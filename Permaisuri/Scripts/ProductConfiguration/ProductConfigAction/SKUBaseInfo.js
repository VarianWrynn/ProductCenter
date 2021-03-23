/// <summary>
/// HMNUMConfiguration 基本信息模块的维护
///2013年11月14日14:27:22
/// </summary>
define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    var ACModel = require('./SKUBaseInfo_AutoCompleted');
   

    //原始HM信息，该价格用来cancel按钮的时候复原
    var SKUData = {
        //UPC: $.trim($("#UPC").val()),
        //ProductDesc: $.trim($("#ProductDesc").val()),
        //Specifications: $.trim($("#Specifications").val()),
        //Keywords: $.trim($("#Keywords").val()),
        //Visibility: $.trim($("#opVisibiliy").val()),
        //URL: $.trim($("#URL").val()),
        ////BrandID: $('#cbBrand').combobox('getValue')//用这种方法老是脚本报错，我猜测combox初始化是异步的，还在Init的时候获取就报错： $.data(...) is undefined  2013年11月24日14:59:58
        //BrandID: $("#hiddenBrandID").val(),
        //ProductName: $.trim($("#ProductName").val()),//Only for duplicated product
        //SKU: $.trim($("#SKU").val()),
        //SKU_QTY: $.trim($("#SKU_QTY").val()),
        //Material: $('#Material').val(),
        //Colour: $('#Colour').val(),
        //Category: $('#Category').val(),
        //SubCategory: $('#SubCategory').val(),
        //opStatus:$("#hiddenStatusID").val()
    }
    var dStatus = 0;
    var SKUElement = [];

    exports.SKU = {
        Init: function () {
            var InitSelf = this;

            SKUData.UPC = $.trim($("#UPC").val());
            SKUData.ProductDesc = $.trim($("#ProductDesc").val());
            SKUData.Specifications = $.trim($("#Specifications").val());
            SKUData.Keywords = $.trim($("#Keywords").val());
            SKUData.Visibility = $.trim($("#opVisibiliy").val());
            SKUData.URL = $.trim($("#URL").val());
            SKUData.BrandID = $("#hiddenBrandID").val();
            SKUData.ProductName = $.trim($("#ProductName").val());
            SKUData.SKU = $.trim($("#SKU").val());
            SKUData.SKU_QTY = $.trim($("#SKU_QTY").val());
            SKUData.Material = $('#Material').val();
            SKUData.Colour = $('#Colour').val();
            SKUData.Category = $('#Category').val();
            SKUData.SubCategory = $('#SubCategory').val();
            SKUData.opStatus = $("#hiddenStatusID").val();
            SKUData.ShipViaTypeID = $("#ShipViaType").combobox('getValue');

            dStatus = parseInt($("#hiddenStatusID").val());

            SKUElement = [$("#UPC"), $("#ProductDesc"), $("#Specifications"), $("#Keywords"), $("#opVisibiliy"), $("#URL"), $("#cbBrand"),
                $("#SKU_QTY"), $('#Material'), $('#Colour'), $('#Category'), $('#SubCategory'), $("#ShipViaType")];

            $('#ProductDesc').validatebox({
                required:"required"
            });

            $('#Specifications').validatebox({
                deltaX: -260,
                deltay:-100,
                required: "required",
                validType: 'length[0,1000]'
            });

            $('#URL').validatebox({
                deltaX: -300,
                validType: 'url'
            });

            
            ACModel.AutoCompleted.Fileds($("#Material"), "Material");
            ACModel.AutoCompleted.Fileds($("#Colour"), "Colour");
            ACModel.AutoCompleted.Fileds($("#Category"), "Category");
            ACModel.AutoCompleted.Fileds($("#SubCategory"), "SubCategory");

            /*为什么子类型放在这里disable而不是放在HTML，因为如果放在页面设置，放你选择父类别，子类别会处于Disable状态无法选择，我觉得这是easyUI的bug 2014年1月13日17:39:49*/
            //$('#SubCategory').combobox("disable");

            $(".editSKU").off("click").on("click", function () {
                //S1:隐藏Edit按钮，显示done按钮
                $(".editSKU").hide();
                $(".submitSKU").show();
                $(".cancelSKU").show();

                $.each(SKUElement, function () {
                    this.prop("readonly", "").css({ "color": "red" });
                });
                $('#cbBrand').combobox("enable");
                $('#ShipViaType').combobox("enable");
                //$('#opVisibiliy').combobox("enable"); 二期再开启
                $("#SKU_QTY").prop("readonly", "readonly").css({ "color": "" })//二期开启 David,2014/1/14
                 
                $("#opStatus").prop("disabled", "");
                $("#opStatus").trigger("chosen:updated");

                //New Duplicated && New
                if (dStatus == 7 || dStatus==1)
                {
                    $("#ProductName").prop("readonly", "").css({ "color": "red" })
                    $("#SKU").prop("readonly", "").css({ "color": "red" })
                }
                return false;
            });

            $(".submitSKU").off("click").on("click", function () {
                InitSelf.Edit(function () {
                    InitSelf.EndEditModel(
                    function () {//更新基础信息
                        SKUData.UPC = $.trim($("#UPC").val());
                        SKUData.ProductDesc = $.trim($("#ProductDesc").val());
                        SKUData.Specifications = $.trim($("#Specifications").val());
                        SKUData.Keywords = $.trim($("#Keywords").val());
                        SKUData.URL = $.trim($("URL").val());
                        SKUData.ProductName = $.trim($("#ProductName").val());
                        SKUData.SKU = $.trim($("#SKU").val());
                        SKUData.SKU_QTY = $.trim($("#SKU_QTY").val());

                        SKUData.Visibility = $('#opVisibiliy').combobox('getValue');
                        SKUData.BrandID = $('#cbBrand').combobox('getValue');
                        SKUData.ShipViaTypeID = $('#ShipViaType').combobox('getValue');

                        SKUData.opStatus = $("#opStatus").val();

                        SKUData.Material = $('#Material').val();
                        SKUData.Colour = $('#Colour').val();
                        SKUData.Category = $('#Category').val();
                        SKUData.SubCategory = $('#SubCategory').val();
                    })
                });
            });

            //点击撤销按钮
            $(".cancelSKU").off("click").on("click", function () {
                InitSelf.EndEditModel(function () {//还原基础信息
                    $("#UPC").val(SKUData.UPC);
                    $("#ProductDesc").val(SKUData.ProductDesc);
                    $("#Specifications").val(SKUData.Specifications);
                    $("#Keywords").val(SKUData.Keywords);
                    $("#URL").val(SKUData.URL);
                    $("#ProductName").val(SKUData.ProductName);
                    $("#SKU").val(SKUData.SKU);
                    $("#SKU_QTY").val(SKUData.SKU_QTY);
                    $('#opVisibiliy').combobox('setValue', SKUData.Visibility);
                    $('#cbBrand').combobox('setValue', SKUData.BrandID);
                    $('#ShipViaType').combobox('setValue', SKUData.ShipViaTypeID);

                    $('#opStatus').val(SKUData.opStatus);
                    $("#opStatus").trigger("chosen:updated");

                    $('#Material').val(SKUData.Material);
                    $('#Colour').val(SKUData.Colour);
                    $('#Category').val(SKUData.Category);
                    $('#SubCategory').val(SKUData.SubCategory);
                });
            });
        },// end of FUNC init()

        //退出编辑模式之后还原原来的样式，多处需要引用，故而提取
        EndEditModel:function(callBack)
        {
            $(".editSKU").show();
            $(".submitSKU").hide();
            $(".cancelSKU").hide();
            $.each(SKUElement, function () {
                this.prop("readonly", "readonly").css({ "color": "#666666" })
            });

            $('#cbBrand').combobox("disable");
            $('#ShipViaType').combobox("disable");
            $('#opVisibiliy').combobox("disable");
            
            $("#ProductName").prop("readonly", "readonly").css({ "color": "#666666" })
            $("#SKU").prop("readonly", "readonly").css({ "color": "#666666" })

            $("#opStatus").prop("disabled", "disabled");
            $("#opStatus").trigger("chosen:updated");

            if (callBack)
            {
                callBack();
            }
        },

        Edit: function (callback) {
            var editSelf = this;
            $.each(SKUElement, function () {//阻止空格....2014年3月26日
                this.val($.trim(this.val()))
            });

            var fvalidate = $("#ProductConfigurationForm").form('validate');
            if (!fvalidate) {
                return;
            }
            var Status = $("#opStatus").val();
            //if (Status == 7) {
            //    Status = 3 //MarketingDevelopment
            //}
            var postData = {
                SKUID: $("#hiddenSKUID").val(),//主键ID，不可或缺
                UPC: $.trim($("#UPC").val()),
                ProductDesc: $.trim($("#ProductDesc").val()),
                Specifications: $.trim($("#Specifications").val()),
                Keywords: $.trim($("#Keywords").val()),
                URL: $.trim($("#URL").val()),
                StatusID: Status,
                SKU_QTY: $.trim($("#SKU_QTY").val()),
                Visibility: $('#opVisibiliy').combobox('getValue'),
                BrandID: $('#cbBrand').combobox('getValue'),
                ShipViaTypeID: $('#ShipViaType').combobox('getValue'),
                Material: $('#Material').val(),
                Colour: $("#Colour").val(),
                Category: $("#Category").val(),
                SubCategory: $("#SubCategory").val(),
            };
            progress.show();
            var dtd = $.Deferred(); // 新建一个Deferred对象,注意这个对象只能定义在Edit方法内部，如果定义在方法外部，会出现第一次执行正常，第二次开始全部异常。
            $.when(editSelf.CheckMCC(dtd)).done(function () {
                $.ajax({
                    url: "./UpdatedProduct",
                    type: "POST",
                    dataType: "json",
                    data: postData
                }).done(function (retData) {
                    progress.hide();
                    if (retData.Status === 1) {
                        if (callback) {
                            callback();
                        }
                        //if (dStatus == 7 || dStatus == 1) {
                        //    dStatus = 3;
                        //    $("#hiddenStatusID").val("3")
                        //    //$("#opStatus").val("3")
                        //    //$("#opStatus").trigger("chosen:updated");
                        //}

                    } else {
                        $.messager.alert('editSKUBasicInfo', retData.Data, 'error');
                        return false;
                    }
                }).fail(function () {
                    progress.hide();
                    $.messager.alert('editSKUBasicInfo', 'Failed to edit,please contact administrator', 'error');
                    return false;
                });
            }).fail(function () {
                $.messager.alert('Colour,Material,Category', 'Check Material,Colour Or Category Failed!', 'error');
            });
        },//end of Edit

        //CheckMaterial,Colour,Category的有效性
        CheckMCC: function (dtd) {
            $.ajax({
                url: "./CheckMCC",
                type: "POST",
                dataType: "json",
                data: {
                    Material: $('#Material').val(),//combobox('getText'),
                    Colour: $("#Colour").val(),
                    Category: $("#Category").val(),
                    SubCategory: $("#SubCategory").val(),
                }
            }).done(function (retData) {
                progress.hide();
                if (retData.Status === 1) {
                    dtd.resolve(); // 改变Deferred对象的执行状态 
                } else {
                    $.messager.confirm('Confirm', 'the value of <b> ' + retData.Data + ' filed </b> does not exist,do you want to create automatically?', function (r) {
                        if (r) {
                            dtd.resolve();
                        }
                    });
                   // dtd.reject();
                }
            }).fail(function () {
                dtd.reject(); // 改变Deferred对象的执行状态
                progress.hide();
                $.messager.alert('editSKUBasicInfo', 'Failed to edit,please contact administrator', 'error');
            });
            return dtd;
        }
    }
});