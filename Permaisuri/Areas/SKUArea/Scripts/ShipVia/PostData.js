define(function (require, exports, module) {
    var $ = require('jquery');
    var easyui = require('jquery.easyui.min');
  
    exports.PostData = {
        //获取查询的参数信息
        queryPostData: function () {
            return {
                ShipViaTypeID: $("#queryShipViaTypeID").combo('getValue'),
                IsDefaultShipViaInd: $("#queryIsDefaultShipViaInd").combo('getValue'),
                ShipVia: $.trim($("#queryShipVia").val())
            }
        },
        //重置查询参数（在界面上）
        ResetQueryPostData: function () {
            $("#queryShipVia").val("");
            $('#queryShipViaTypeID').combobox('select', "0");
            $('#queryIsDefaultShipViaInd').combobox('select', "-1");
        },
    
        //获取新增or编辑的参数信息
        DlgPostData: function () {
            return {
                ShipViaID:$("#dlgShipViaID").val(),
                ShipVia: $("#dlgShipVia").val(),
                ShipViaTypeID: $("#dlgShipViaType").combobox('getValue'),
                IsDefaultShipVia: $("#dlgIsDefaultShipVia").combobox('getValue'),
                ExpressNumLength: $("#dlgExpressNumLenght").val(),
                CarrierRouting: $("#dlgCarrierRouting").val(),
                CarrierCode: $("#dlgCarrierCode").val(),
                ExpressRule: $("#dlgExpressRule").val()
            }
        },

        ResetDlgPostData: function () {
            $("#dlgShipViaID").val("0"),
            $("#dlgShipVia").val("");
            $("#dlgShipViaType").combobox('select','');
            $("#dlgIsDefaultShipVia").combobox('select', '');
            $("#dlgExpressNumLenght").val("");
            $("#dlgCarrierRouting").val("");
            $("#dlgCarrierCode").val("");
            $("#dlgExpressRule").val("")
        },

        //当点击某个列打开的时候给予给个字段赋值...
        SetDlgPostData: function (rowData) {
            //console.error(JSON.stringify(rowData));
            $("#dlgShipViaID").val(rowData.SHIPVIAID),
            $("#dlgShipVia").val(rowData.SHIPVIA);//ShipVia注意大小写
            $("#dlgShipViaType").combobox('select', rowData.ShipViaTypeID);
            var YesOrNo = rowData.IsDefaultShipVia==true?"Yes":"No"
            $("#dlgIsDefaultShipVia").combobox('select', YesOrNo);
            //$("#dlgIsDefaultShipVia").combobox('setValue', rowData.IsDefaultShipVia);
            $("#dlgExpressNumLenght").val(rowData.ExpressNumLength);
            $("#dlgCarrierRouting").val(rowData.CarrierRouting);
            $("#dlgCarrierCode").val(rowData.CarrierCode);
            $("#dlgExpressRule").val(rowData.ExpressRule)
        }
    }
});