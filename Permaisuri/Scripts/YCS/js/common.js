/* ************************************************************
    lexus magazine 0.1v 
    2014.8.19
    add special contents
************************************************************ */


/* ***********************************************************************************************************************
    for CSS transitions and animations event compatibility
    var transitionend      = 'transitionend webkitTransitionEnd',
        animationstart     = 'animationstart webkitAnimationStart',
        animationiteration = 'animationiteration webkitAnimationIteration',
        animationend       = 'animationend webkitAnimationEnd';    
*********************************************************************************************************************** */
// (function(){"use strict";function e(){}function t(e,t){for(var n=e.length;n--;)if(e[n].listener===t)return n;return-1}var n=e.prototype;n.getListeners=function(e){var t,n,i=this._getEvents();if("object"==typeof e){t={};for(n in i)i.hasOwnProperty(n)&&e.test(n)&&(t[n]=i[n])}else t=i[e]||(i[e]=[]);return t},n.flattenListeners=function(e){var t,n=[];for(t=0;e.length>t;t+=1)n.push(e[t].listener);return n},n.getListenersAsObject=function(e){var t,n=this.getListeners(e);return n instanceof Array&&(t={},t[e]=n),t||n},n.addListener=function(e,n){var i,r=this.getListenersAsObject(e),s="object"==typeof n;for(i in r)r.hasOwnProperty(i)&&-1===t(r[i],n)&&r[i].push(s?n:{listener:n,once:!1});return this},n.on=n.addListener,n.addOnceListener=function(e,t){return this.addListener(e,{listener:t,once:!0})},n.once=n.addOnceListener,n.defineEvent=function(e){return this.getListeners(e),this},n.defineEvents=function(e){for(var t=0;e.length>t;t+=1)this.defineEvent(e[t]);return this},n.removeListener=function(e,n){var i,r,s=this.getListenersAsObject(e);for(r in s)s.hasOwnProperty(r)&&(i=t(s[r],n),-1!==i&&s[r].splice(i,1));return this},n.off=n.removeListener,n.addListeners=function(e,t){return this.manipulateListeners(!1,e,t)},n.removeListeners=function(e,t){return this.manipulateListeners(!0,e,t)},n.manipulateListeners=function(e,t,n){var i,r,s=e?this.removeListener:this.addListener,o=e?this.removeListeners:this.addListeners;if("object"!=typeof t||t instanceof RegExp)for(i=n.length;i--;)s.call(this,t,n[i]);else for(i in t)t.hasOwnProperty(i)&&(r=t[i])&&("function"==typeof r?s.call(this,i,r):o.call(this,i,r));return this},n.removeEvent=function(e){var t,n=typeof e,i=this._getEvents();if("string"===n)delete i[e];else if("object"===n)for(t in i)i.hasOwnProperty(t)&&e.test(t)&&delete i[t];else delete this._events;return this},n.emitEvent=function(e,t){var n,i,r,s,o=this.getListenersAsObject(e);for(r in o)if(o.hasOwnProperty(r))for(i=o[r].length;i--;)n=o[r][i],s=n.listener.apply(this,t||[]),(s===this._getOnceReturnValue()||n.once===!0)&&this.removeListener(e,o[r][i].listener);return this},n.trigger=n.emitEvent,n.emit=function(e){var t=Array.prototype.slice.call(arguments,1);return this.emitEvent(e,t)},n.setOnceReturnValue=function(e){return this._onceReturnValue=e,this},n._getOnceReturnValue=function(){return this.hasOwnProperty("_onceReturnValue")?this._onceReturnValue:!0},n._getEvents=function(){return this._events||(this._events={})},"function"==typeof define&&define.amd?define(function(){return e}):"undefined"!=typeof module&&module.exports?module.exports=e:this.EventEmitter=e}).call(this),function(e){"use strict";var t=document.documentElement,n=function(){};t.addEventListener?n=function(e,t,n){e.addEventListener(t,n,!1)}:t.attachEvent&&(n=function(t,n,i){t[n+i]=i.handleEvent?function(){var t=e.event;t.target=t.target||t.srcElement,i.handleEvent.call(i,t)}:function(){var n=e.event;n.target=n.target||n.srcElement,i.call(t,n)},t.attachEvent("on"+n,t[n+i])});var i=function(){};t.removeEventListener?i=function(e,t,n){e.removeEventListener(t,n,!1)}:t.detachEvent&&(i=function(e,t,n){e.detachEvent("on"+t,e[t+n]);try{delete e[t+n]}catch(i){e[t+n]=void 0}});var r={bind:n,unbind:i};"function"==typeof define&&define.amd?define(r):e.eventie=r}(this),function(e){"use strict";function t(e,t){for(var n in t)e[n]=t[n];return e}function n(e){return"[object Array]"===h.call(e)}function i(e){var t=[];if(n(e))t=e;else if("number"==typeof e.length)for(var i=0,r=e.length;r>i;i++)t.push(e[i]);else t.push(e);return t}function r(e,n){function r(e,n,o){if(!(this instanceof r))return new r(e,n);"string"==typeof e&&(e=document.querySelectorAll(e)),this.elements=i(e),this.options=t({},this.options),"function"==typeof n?o=n:t(this.options,n),o&&this.on("always",o),this.getImages(),s&&(this.jqDeferred=new s.Deferred);var a=this;setTimeout(function(){a.check()})}function h(e){this.img=e}r.prototype=new e,r.prototype.options={},r.prototype.getImages=function(){this.images=[];for(var e=0,t=this.elements.length;t>e;e++){var n=this.elements[e];"IMG"===n.nodeName&&this.addImage(n);for(var i=n.querySelectorAll("img"),r=0,s=i.length;s>r;r++){var o=i[r];this.addImage(o)}}},r.prototype.addImage=function(e){var t=new h(e);this.images.push(t)},r.prototype.check=function(){function e(e,r){return t.options.debug&&a&&o.log("confirm",e,r),t.progress(e),n++,n===i&&t.complete(),!0}var t=this,n=0,i=this.images.length;if(this.hasAnyBroken=!1,!i)return this.complete(),void 0;for(var r=0;i>r;r++){var s=this.images[r];s.on("confirm",e),s.check()}},r.prototype.progress=function(e){var t=this;this.hasAnyBroken=this.hasAnyBroken||!e.isLoaded,setTimeout(function(){t.emit("progress",t,e),t.jqDeferred&&t.jqDeferred.notify(t,e)})},r.prototype.complete=function(){var e=this.hasAnyBroken?"fail":"done";if(this.isComplete=!0,this.emit(e,this),this.emit("always",this),this.jqDeferred){var t=this.hasAnyBroken?"reject":"resolve";this.jqDeferred[t](this)}},s&&(s.fn.imagesLoaded=function(e,t){var n=new r(this,e,t);return n.jqDeferred.promise(s(this))});var c={};return h.prototype=new e,h.prototype.check=function(){var e=c[this.img.src];if(e)return this.useCached(e),void 0;if(c[this.img.src]=this,this.img.complete&&void 0!==this.img.naturalWidth)return this.confirm(0!==this.img.naturalWidth,"naturalWidth"),void 0;var t=this.proxyImage=new Image;n.bind(t,"load",this),n.bind(t,"error",this),t.src=this.img.src},h.prototype.useCached=function(e){if(e.isConfirmed)this.confirm(e.isLoaded,"cached was confirmed");else{var t=this;e.on("confirm",function(e){return t.confirm(e.isLoaded,"cache emitted confirmed"),!0})}},h.prototype.confirm=function(e,t){this.isConfirmed=!0,this.isLoaded=e,this.emit("confirm",this,t)},h.prototype.handleEvent=function(e){var t="on"+e.type;this[t]&&this[t](e)},h.prototype.onload=function(){this.confirm(!0,"onload"),this.unbindProxyEvents()},h.prototype.onerror=function(){this.confirm(!1,"onerror"),this.unbindProxyEvents()},h.prototype.unbindProxyEvents=function(){n.unbind(this.proxyImage,"load",this),n.unbind(this.proxyImage,"error",this)},r}var s=e.jQuery,o=e.console,a=o!==void 0,h=Object.prototype.toString;"function"==typeof define&&define.amd?define(["eventEmitter","eventie"],r):e.imagesLoaded=r(e.EventEmitter,e.eventie)}(window);

lexusMagazineOnLoad     = function(){}; //loading complete
lexusMagazineReady      = function(){}; //loading complete after stage view

LoadingAnimationRemove  = function(){} // 

MagazineNaviStatus  = function(){}; //local navi status to opened?closed?
MagazineNaviOpen    = function(){}; //local navi open
MagazineNaviClose   = function(){}; //local navi close
MagazineNaviHidden  = function(){}; //local navi hidden ( display : none )
MagazineNaviView    = function(){}; //local navi view ( display : block )


(function (window, $) {
    var $window = $(window),
        $document = $(document);

    // https://github.com/ftlabs/fastclick
    $(window).on('load', function() {
       new FastClick(document.body);
    }, false);

    var isJp = false;

    $document.ready(function(){
        var mod = window.Modernizr,
            modCSSAnimations  = mod && mod.cssanimations,
            modCSSTransforms  = mod && mod.csstransforms,
            modCSSTransitions = mod && mod.csstransitions,
            modTouch          = mod && mod.touch,
            modAnim           = modCSSAnimations && modCSSTransitions;

        // jQuery imagesLoaded
        // https://github.com/desandro/imagesloaded

        var isCache             = cacheCheck(),
            $loadingWrapper     = $('#loading-wrap'),
            $loading            = $('<div id="loading"></div>'),
            atHome              = $loadingWrapper.hasClass('at-home');

        function imgLoad(){
            var imgLoader           = imagesLoaded("body"),
                imgTotal            = imgLoader.images.length,

                progress1           = 0,
                progress2           = 0,
                percent1            = 0,
                percent2            = 0,

                imgLoaded           = 0,
                animLoaded          = 0,
                spriteFrameSize     = atHome? 170: 100,
                spriteFrameCount    = atHome? 51: 42;

            imgLoader.on("progress",function(instance,image){
                var result = image.isLoaded ? 'loaded' : 'broken';
                imgLoaded++;
                progress1   = (imgLoaded/imgTotal)*100;
                progress2   = (imgLoaded/imgTotal)*spriteFrameCount;
                //image load complete
                if(progress1 == 100){
                    // lexusMagazineOnLoad();
                    // dfdAnimationComplete();
                }
            });

            var progressTimer = setInterval(function(){
                percent1 += (progress1-percent1)*0.1;
                percent2 += (progress2-percent2)*0.1;

                $loading.html(parseInt(percent1));
                $loading.css({backgroundPosition: '0 ' + (-parseInt(percent2)*spriteFrameSize) + 'px'});

                if(percent1 >= 100){
                    //image Load complete
                    clearInterval(progressTimer);
                    setTimeout(function(){
                        lexusMagazineOnLoad();
                        LoadingAnimationRemove();
                    },50);
                    return
                }
                if(percent1 >= 99.9){
                    percent1 = 100;
                    percent2 = spriteFrameCount;
                }

            },1000/60);
        }


        LoadingAnimationRemove = function(){
            $loading.stop().fadeOut(100,"easeOutQuint");
            start();


            function start(){
                if(modAnim){
                    $loadingWrapper
                    .addClass('done')
                    .on('transitionend webkitTransitionEnd', function (event) {
                        $('.anim-stage').addClass('anim-ready');
                        lexusMagazineReady();
                        $loadingWrapper.remove();
                        $loadingWrapper = null;
                    });
                }else{
                    $loadingWrapper.animate({ opacity: 0 }, 500, function(){
                        $('.anim-stage').addClass('anim-ready');
                        lexusMagazineReady();
                        $loadingWrapper.remove();
                        $loadingWrapper = null;
                    });
                }

             
            }
            
        }


 


        jQuery.extend( jQuery.easing,{
            easeInOutBackAlarm: function (x, t, b, c, d, s) {
                if (s == undefined) s = 1.90158;
                return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
            }
        });

        /* ************************************************************
            local storage 
        ************************************************************ */
        // var localData = {
        //     get     : function(key){return localStorage.getItem(key)},
        //     set     : function(key,value){localStorage.setItem(key, value)},
        //     clear   : function(){localStorage.clear()}
        // }

        function cacheCheck() {
            return true;
            var location    = "magazine/",//+Math.random(),//window.location.pathname,
                t           = new Date(),
                date        = t.getFullYear()+""+t.getMonth()+""+t.getDate(),
                cache       = true;
            if(localStorage.getItem(location) == null){
                localStorage.setItem(location,date);
                cache = false;
            }else{
                if(localStorage.getItem(location) != date){
                    cache = false;
                    localStorage.setItem(location,date);
                }
            }

            // localStorage.clear();
            return cache;
        }

        /* ************************************************************
            credit radius mask
        ************************************************************ */
        if(UA.isSafari){
            $('.credit-model-link').css({'-webkit-mask-image':'-webkit-radial-gradient(circle, white, black)'});
        }

       
        //imgLoad();    
        LoadingAnimationRemove();
        lexusMagazineOnLoad();
    });


})(this, jQuery);


/* ************************************************************
    CSS3 
************************************************************ */
function css3_scale(scale){
    var css3 = "scale("+scale+")";
    return{
        "-webkit-transform" : css3,
        "-moz-transform"    : css3,
        "-o-transform"      : css3,
        "-ms-transform"     : css3,
        "transform"         : css3
    };
}

function css3_XYZ(tx,ty,tz){
    var css3 = "translateX("+tx+"px) translateY("+ty+"px) translateZ("+tz+"px)"
    return{
        "-webkit-transform" : css3,
        "-moz-transform"    : css3,
        "-o-transform"      : css3,
        "-ms-transform"     : css3,
        "transform"         : css3
    };
}

function css3_XYZ2(tx,ty,tz){
    var css3 = "translateZ("+tz+"px) translateX("+tx+"px) translateY("+ty+"%)"
    return{
        "-webkit-transform" : css3,
        "-moz-transform"    : css3,
        "-o-transform"      : css3,
        "-ms-transform"     : css3,
        "transform"         : css3
    };
}

function css3_ROTATE_Z(tz,rx,ry,rz){
    var css3 = "translateZ("+tz+"px) rotateX("+rx+"deg) rotateY("+ry+"deg) rotateZ("+rz+"deg)"
    return{
        "-webkit-transform" : css3,
        "-moz-transform"    : css3,
        "-o-transform"      : css3,
        "-ms-transform"     : css3,
        "transform"         : css3
    };
}

function css3DSet(perspective,originX,originY){
    var perspective = perspective+"px",
        style       = "preserve-3d",
        orign       = originX+" "+originY;
        css         = { "-webkit-perspective"   : perspective,
                        "-moz-perspective"      : perspective,
                        "-o-perspective"        : perspective,
                        "-ms-perspective"       : perspective,
                        "perspective"           : perspective,

                        // "-webkit-transform-style"   : style,
                        // "-moz-transform-style"      : style,
                        // "-o-transform-style"        : style,
                        // "-ms-transform-style"       : style,
                        // "transform-style"           : style,

                        "-webkit-perspective-origin"    : orign,
                        "-moz-perspective-origin"       : orign,
                        "-o-perspective-origin"         : orign,
                        "-ms-perspective-origin"        : orign,
                        "-transform-perspective-origin" : orign
                        }
    return css
}


/* util.js v 1.2.7 Copyright (c) 2013 SHIFTBRAIN Inc. Licensed under the MIT license. https://github.com/devjam */
(function() {
    if (this.console == null) {this.console = { log : function() {}}
    }
    if(this.UA)return;
    this.UA = function() {
        var e, t, n, r;
        r = navigator.userAgent.toLowerCase();
        t = {
            isIE : false,
            isIE6 : false,
            isIE7 : false,
            isIE8 : false,
            isIE9 : false,
            isLtIE9 : false,
            isIE11 : false,
            isIOS : false,
            isIPhone : false,
            isIPad : false,
            isIPhone4 : false,
            isIPad3 : false,
            isAndroid : false,
            isAndroidMobile : false,
            isChrome : false,
            isSafari : false,
            isMozilla : false,
            isWebkit : false,
            isOpera : false,
            isPC : false,
            isTablet : false,
            isSmartPhone : false,
            browser : r
        };
        if (t.isIE = /msie\s(\d+)/.test(r)) {
            n = RegExp.$1;
            n *= 1;
            t.isIE6 = n === 6;
            t.isIE7 = n === 7;
            t.isIE8 = n === 8;
            t.isIE9 = n === 9;
            t.isLtIE9 = n < 9
        }
        if (t.isIE7 && r.indexOf("trident/4.0") !== -1) {
            t.isIE7 = false;
            t.isIE8 = true
        }
        if (r.indexOf("trident/7.0") !== -1) {
            t.isIE = true;
            t.isIE11 = true;
        }
        if (t.isIPhone = /i(phone|pod)/.test(r)) {
            t.isIPhone4 = window.devicePixelRatio === 2
        }
        if (t.isIPad = /ipad/.test(r)) {
            e = window.devicePixelRatio === 2
        }
        t.isIOS = t.isIPhone || t.isIPad;
        t.isAndroid = /android/.test(r);
        t.isAndroidMobile = /android(.+)?mobile/.test(r);
        t.isPC = !t.isIOS && !t.isAndroid;
        t.isTablet = t.isIPad || t.isAndroid && t.isAndroidMobile;
        t.isSmartPhone = t.isIPhone || t.isAndroidMobile;
        t.isChrome = /chrome/.test(r);
        t.isWebkit = /webkit/.test(r);
        t.isOpera = /opera/.test(r);
        t.isMozilla = r.indexOf("compatible") < 0 && /mozilla/.test(r);
        t.isSafari = !t.isChrome && t.isWebkit;
        return t
    }();
}).call(this);