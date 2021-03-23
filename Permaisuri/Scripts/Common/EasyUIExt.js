/// <reference path="../jquery-1.7.1-vsdoc.js" />

define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
    var progress = require('Common/Prograss');

    /// <summary>
    /// 扩展EasyUI.....
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    exports.Ext = {

        /// <summary>
        /// 本地校验当前输入框的内容是否存在，不存在则提示。2014年2月17日16:40:45
        /// 调用方式：
        /*
         $('#cbBrand').combobox({
             valueField: 'Brand_Id',
             textField: 'Brand_Name',
             validType: "ValidateList['#cbBrand','Brand_Name']"
            }
        */
        /// </summary>
        /// <param name="value"> 当前输入框的value</param>
        /// <param name="param"> 一般是数组形式传递进来，第一个参数代表当前输入框的ID，第二个代表需要哪里面哪个字段做校验</param>
        /// <returns></returns>
        ValidateListLocal: function () {
            $.extend($.fn.validatebox.defaults.rules, {
                ValidateListLocal: {
                    //value:被校验字段的value! param:被校验字段对于的传递进来的那个参数 注意必须用【双/单引号】引用起来！
                    //比如validType: "Test['#SKU_QTY']"  #SKU_QTY外面包含了一对引号
                    validator: function (value, param) {
                        //这里必须返回Bool类型，true代表校验通过；false代表校验失败，则弹出下面的message.
                        var list = $(param[0]).combobox('getData');
                        //$.messager.alert("aaa", JSON.stringify($(param[0]).combobox('getData')), "error");
                        var curTextName = param[1];//Brand_Name
                        //var curTextValue = $(param[0]).combobox('getText');
                        var vResult = false;
                        $.each(list, function () {
                            //对于本地（不通过后台获取）的combobox,结构：this={selected:"selected",text:"All",value:"0"} 2014年5月22日
                            if (value == this[curTextName]) {
                                vResult = true;
                            }
                        });
                        return vResult;
                    },
                    message: 'Please select a correct option!'
                }
            });
        },

        //价格校验，用于比对零售价是否大于成本价，不大于则校验不通过
        ValidateCosting: function ()
        {
            $.extend($.fn.validatebox.defaults.rules, {
                ValidateCosting: {
                    //value:被校验字段的value! param:被校验字段对于的传递进来的那个参数 注意必须用【双/单引号】引用起来！
                    //比如validType: "Test['#SKU_QTY']"  #SKU_QTY外面包含了一对引号
                    validator: function (value, param) {
                        var firstCost = ($(param[0]).val()).replace(/\$/g, '').replace(/\,/g, '');
                        var retailCost = value.replace(/\$/g, '').replace(/\,/g, '');
                        return parseFloat(retailCost) > parseFloat(firstCost);
                    },
                    message: 'Retail prices must be greater than Landed cost!'
                }
            });
        },

        ValidateCostNoDollor: function () {
            $.extend($.fn.validatebox.defaults.rules, {
                ValidateCostNoDollor: {
                    //value:被校验字段的value! param:被校验字段对于的传递进来的那个参数 注意必须用【双/单引号】引用起来！
                    //比如validType: "Test['#SKU_QTY']"  #SKU_QTY外面包含了一对引号
                    validator: function (value, param) {
                        var firstCost = param[0];
                        var landedCost = value;
                        return parseFloat(landedCost) > parseFloat(firstCost);
                    },
                    message: 'LandedCost prices must be greater than First cost!'
                }
            });
        },

        ValidateFolder: function () {
            $.extend($.fn.validatebox.defaults.rules, {
                ValidateFolder: {
                    validator: function (value, param) {
                        //var reg = /[\\/:*?,"<>|%&!@#+()]/;//注意逗号也不可以的！ &属于MVC建议禁用范畴，所以也加上去 2014年5月30日
                        var reg = /^[\w._-]+(\.[\w]+)?$/;
                        return reg.test(value)
                    },
                    message: 'HMNUM contain illegal character.'
                }
            });
        },


        ValidateExactDivided: function () {
            $.extend($.fn.validatebox.defaults.rules, {
                ExactDivided: {
                    validator: function (value, param) {

                        var MPMode = value.replace('QTY:', '') % param[0];//必须replace掉传递进来的带有非数字的字符串...
                        return MPMode == 0;
                    },
                    message: 'Pieces must be exact divided by master pack.'
                }
            });
        },


        ValidateRemoteIsExist: function () {
            $.extend($.fn.validatebox.defaults.rules, {
                RemoteIsExist: {
                    validator: function (value, param) {
                        var channelId = $("#cbChannel").combobox('getValue');
                        if (channelId == "")
                        {
                            //Linda反馈，如果Channel未选择就先通过不校验
                            return true;
                        }
                        var postdata = {};
                        
                        //var result = $.ajax({
                        //    url: param[0],
                        //    dataType: "json",
                        //    data: postdata,
                        //    async: false,//不能为true...
                        //    cache: false,
                        //    type: "post"
                        //}).responseText;
                        var result = false;
                        $.ajax({
                            url: param[0],
                            type: "POST",
                            async: false,
                            cache: false,
                            dataType: "json",
                            data: {
                                SKU: value,
                                ChannelID: channelId
                            }
                        }).done(function (retData) {
                            if (retData.Status === 1)
                            {
                                result = true;
                            }
                        }).fail(function (err) {

                        });
    
                        return result;
                    },
                    message: "This item already exists."
                }
            });
        }

    }
});