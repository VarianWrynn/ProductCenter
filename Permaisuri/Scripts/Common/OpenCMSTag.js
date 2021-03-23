define(function (require, exports, module) {
    var $ = require('jquery');
    
    /// <summary>
    /// 调用此方法打开一个Tag标签(以iFrame方式），由于这个方法太常用，故而提取出来公用
    /// 2013年10月25日10:05:47
    /// Change1: 由于管理tab下的iframe十分混乱，以后一律先关闭再打开！ 2014年2月14日17:19:54
    /// </summary>
    /// <param name="TagsParam">打开标签的参数 Name,Title,iframeID,URL,closable </param>
    /// <returns></returns>
    exports.CMSTags = {
        OpenTags: function (Options) {
            if (window.parent.jQuery('#centerTabs').tabs('exists', Options.name)) {
                window.parent.jQuery('#centerTabs').tabs('close', Options.name);
            }
            window.parent.jQuery('#centerTabs').tabs('add', {
                title: Options.title,
                content: '<iframe id="' + Options.iframeID + '" width="100%" height="99%" frameborder="0" scrolling="auto" src=' + Options.URL + '></iframe>',
                closable: Options.closable
            });
        },

        getTagOptions: function () {
            var obj =
            {
                name: "",
                title: "",
                iframeID: "",
                URL: "",
                closable: true,
                reload: false
            }

            return obj;
        },

        ///2014年4月17日
        BackSpace: function () {
            $(document).keydown(function (e) {

                //获取当前按键的键值   
                //jQuery的event对象上有一个which的属性可以获得键盘按键的键值   
                /*var keycode = event.which;
                if (keycode == 8) {
                    //var centerTabs = $('#centerTabs');
                    //if (centerTabs.length == 0)
                    //{
                    //    centerTabs = window.parent.jQuery('#centerTabs');
                    //}
                    //var tab = centerTabs.tabs('getSelected');
                    //var index = centerTabs.tabs('getTabIndex', tab);
                    //centerTabs.tabs('select', index - 1);

                    //console.info(JSON.stringify(tabs));
                    //here,return false does never work..
                    //2014年4月22日17:00:11 这种方法会导致input款输入东西之后不能用backSpace键撤销
                    event.preventDefault();
                }*/
                var target = e.target;
                var tag = e.target.tagName.toUpperCase();
                if (e.keyCode == 8) {
                    if ((tag == 'INPUT' && !$(target).attr("readonly")) || (tag == 'TEXTAREA' && !$(target).attr("readonly"))) {
                        if ((target.type.toUpperCase() == "RADIO") || (target.type.toUpperCase() == "CHECKBOX")) {
                            return false;
                        } else {
                            return true;
                        }
                    } else {
                        return false;
                    }
                }
            });
        }
    }
});