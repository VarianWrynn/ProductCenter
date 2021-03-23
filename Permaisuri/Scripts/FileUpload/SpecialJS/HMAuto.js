
window.MediaHM = {
    AutoHM: function () {
        var isSelected = false;
        var HMAutoSelf = this;
        $("#AutoHM").autocomplete({
            source: function (request, response) {
                isSelected = false;//reset;
                $("#AutoHM").addClass('auto-loading');
                //$("#AutoName").prop("disabled",false);
                $.ajax({
                    url: "./GetProductInfo",
                    type: "POST",
                    dataType: "json",
                    data: {
                        SKUID: $(".modelID").val(),
                        ProductName: "",
                        HMNUM: request.term
                    },
                    success: function (retData) {
                        $("#AutoHM").removeClass('auto-loading');
                        response($.map(retData["Data"], function (item) {
                            return {
                                label: item.HMNUM,
                                value: item.HMNUM,
                                ProductID: item.ProductID,
                                HMNUM: item.HMNUM,
                                ProductName: item.ProductName,
                                MaxImaSeq: item.MaxImaSeq
                            }
                        }));
                    },
                    error: function () {
                        $("#AutoHM").removeClass('auto-loading');
                        alert("NetWork Error,Please contact administrator");
                        return false;
                    }
                }); // end of ajax func
            },// end of source function

            minLength: 1,
            select: function (event, ui) {
              
                isSelected = true;
                //console.error("selected:" + isSelected);
                HMAutoSelf.SetNewHMInfo(ui.item);
            }
        }).off("blur").on("blur", function () { // end of autocomplete ;
            var selfText = this;
            if ($.trim($(selfText).val()) == "") {
                $("#addSpan").show();
                $("#tdCancel").click();
                return false;
            }
            if (isSelected) {
                return false;
            }
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
                    //校验通过
                    HMAutoSelf.SetNewHMInfo(retData["Data"]);
                } else {
                    //这里在新增一次校验，加入用户选择了HMNUM,触发了Select，则不要再做校验来清空
                    //console.error("aaaaaa");
                    if (isSelected) {
                        //console.error(isSelected);
                        return false;
                    }
                    //console.error("bbbbbb");
                    //不存在这个对象，清空
                    $(selfText).val("");
                    $("#AutoName").val("");
                    $("#tdCancel").click();
                    $("#addSpan").hide();
                }
            }).fail(function () {
                $("input[name='comboHMNUM']").removeClass('auto-loading').prop("disabled", false);
                $.messager.alert('Check HMNUM', 'NetWork Error,Please contact administrator。', 'error');
            })
        });//end of blur event
    },

    //从WebPO,Ecom获取图像（根据HM#）
    GetImagesFromOtherSystem: function (HMNUM, id) {
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
                //$(".fancybox").fancybox();//rebind event
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
    SetNewHMInfo: function (HMObj)
    {
        var HMAutoSelf = this;
        //获取图像
        HMAutoSelf.GetImagesFromOtherSystem(HMObj.HMNUM);
        var beforHM = $("#AutoHM").val();
        $("#hiddenAutoHM").val(HMObj.HMNUM);//防止用户故意乱输入一个不存在HMNUM提交来坑爹 2013年11月28日16:27:55
        $("#AutoName").val(HMObj.ProductName);
        $("#hiddenProductID").val(HMObj.ProductID);
        $("#hiddenMaxImaSeq").val(HMObj.MaxImaSeq);
        addMediaFileCounts = parseInt(HMObj.MaxImaSeq);
        $("#addSpan").show();
        $("#tdCancel").click();
    }
}