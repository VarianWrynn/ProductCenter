/// <summary>
///  Reference URL: http://blog.csdn.net/he20101020/article/details/8503308
///  javascript保留两位小数
/// </summary>
/// <param name=""></param>
/// <returns></returns>
define(function (require, exports, module) {
    exports.DecimalFormate = {
        //保留两位小数     
        //功能：将浮点数四舍五入，取小数点后2位   
        toDecimal: function (x) {
            var f = parseFloat(x);
            if (isNaN(f)) {
                return;
            }
            f = Math.round(x * 100) / 100;
            return f;
        },
        //强制保留2位小数，如：2，会在2后面补上00.即2.00
        toDecimal2: function (x) {
            var f = parseFloat(x);
            if (isNaN(f)) {
                return false;
            }
            var f = Math.round(x * 100) / 100;
            var s = f.toString();
            var rs = s.indexOf('.');
            if (rs < 0) {
                rs = s.length;
                s += '.';
            }
            while (s.length <= rs + 2) {
                s += '0';
            }
            return s;
        }
    }
});