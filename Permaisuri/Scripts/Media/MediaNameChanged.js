
define(function (require, exports, module) {
    var $ = require('jquery');
    var prograss = require('Common/Prograss');
    /// <summary>
    ///  修改视图、图片的名字
    ///  Change name of image,video etc.
    /// </summary>
    /// <param name="postData">postData to Server</param>
    /// <param name="tdObj">当前td元素对象，用于ajax成功或者失败后的不同赋值</param>
    /// <returns>true or false</returns>
    exports.ChangedNameObj = {
        ChangedImageName: function (postData, tdObj) {
            //prograss.show();
            $.ajax({
                url: "EditMediaName",//使用Edit而不是用change，是因为后台对Edit这个关键字做了listen,可以记录到数据库去
                type: "POST",
                dataType: "json",
                data: postData,
                success: function (retData) {
                    //prograss.hide();不能做遮罩，一旦做了，马上失去焦点无法触发 tdObj.html(postData.newFileName); 的动作了
                    if (retData.Status === 1) {
                        tdObj.html(postData.newFileName);
                        var orgImgUrl = window.location.protocol + "//" + window.location.host + "/MediaLib/Files/Orig/" + postData.newFileName + "." + postData.fileExt;
                        var smallImgUrl = window.location.protocol + "//" + window.location.host + "/MediaLib/Files/Orig/" + postData.newFileName + "." + postData.fileExt;

                        //修改图片名称后，fancyBox的属性也要跟着修改
                        $("#dynamicImg").attr("src", orgImgUrl).attr("width", 100).attr("height", 100);
                        $("#single_1").attr("href", orgImgUrl).attr("title", postData.newFileName);
                        $("#h2_url").text(orgImgUrl);//修改鼠标指上去的时候图片的绝对地址

                        //当鼠标离开后又再一次指向图片，而页面未刷新的时候，fancyBox等的属性还是从img本身的oName和src里面查找，所以这里也必须做想要改动
                        var curDiv = tdObj.parents().find("#hidMediaID" + postData.mediaID).find("img");
                        curDiv.attr("oName", postData.newFileName)
                        curDiv.attr("src", smallImgUrl);
                    } else {
                        tdObj.html(postData.oldFileName);
                        $.messager.alert('SearchMediaLibrary', retData.Data, 'error');
                    }
                },
                error: function () {
                    tdObj.html(postData.oldFileName);
                    $.messager.alert('SearchMediaLibrary', 'NetWork Error,Please contact administrator。', 'error');
                }
            })// end of ajax function
        },

        /// <summary>
        ///  修改图片描述/备注
        ///  Change name of image,video etc.
        /// </summary>
        ChangedDescriptionName: function (postData, tdObj) {
            $.ajax({
                url: "EditMediaDescription",
                type: "POST",
                dataType: "json",
                data: postData,
                success: function (retData) {
                    if (retData.Status === 1) {
                        tdObj.html(postData.newDesc);
                        var curDiv = tdObj.parents().find("#hidMediaID" + postData.mediaID).find("img");
                        curDiv.attr("desc", postData.newDesc)
                    } else {
                        tdObj.html(postData.oldDesc);
                        $.messager.alert('SearchMediaLibrary', retData.Data, 'error');
                    }
                },
                error: function () {
                    tdObj.html(postData.oldDesc);
                    $.messager.alert('SearchMediaLibrary', 'NetWork Error,Please contact administrator。', 'error');
                }
            })// end of ajax function
        }// end of func ChangedDescriptionName
    }
});