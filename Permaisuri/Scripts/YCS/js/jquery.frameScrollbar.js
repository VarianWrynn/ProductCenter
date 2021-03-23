

(function () {
    var _bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

    var getPagePos = function(e){
        var pos, touch;
        pos = {x: 0, y: 0};
        if("ontouchstart" in window) {
            if (e.touches != null) {
                touch = e.touches[0];
            } else {
                touch = e.originalEvent.touches[0];
            }
            pos.x = touch.clientX;
            pos.y = touch.clientY;
        } else {
            pos.x = e.clientX;
            pos.y = e.clientY;
        }
        return pos;
    };

    var e, t, n, r;


    /* *********************************************************
    *  Constructor 
    ********************************************************** */

    function FrameScrollBar(option){
        r   = navigator.userAgent.toLowerCase();
        nv  = window.navigator;
        t   = {
            isIPad          : false,
            isAndroid       : false,
            isAndroidMobile : false,
            isTablet        : false,
            isMac : /mac/i.test(nv['platform']),
            isWin : /win/i.test(nv['platform'])
        };

        if(t.isIPad = /ipad/.test(r)){
            e = window.devicePixelRatio === 2;
        }
        
        t.isAndroid       = /android/.test(r);
        t.isAndroidMobile = /android(.+)?mobile/.test(r);
        t.isTablet        = t.isIPad || t.isAndroid && t.isAndroidMobile;


        this.config = {
            target             : document.body,
            show               : true,
            screenFix          : true,
            frameScrollUpdate  : function(){},
            scrollDelete       : function(){}
        };

        $.extend(this.config,option);

        this.mouseUpDwFlg       = false;
        this.selectFlg          = false;
        this.alarmScrFlg        = true;
        this.mouseDraggerDeltaY = 0;
        this.scrollbarRatioDesY = 0;
        this.scrollbarRatioY    = 0;
        this.railTotalH         = 0;
        this.draggerVertical;
        this.scrollbarVertical;
        this.renderingID;
        this.onRender           = _bind(this.onRender,this);


        var _scrollBar;
        if(this.config.show){
            _scrollBar = $("<div id='scrollbar'><div id='scrollbar-vertical' class='scroll-tools'><div class='dragger-container'><div id='dragger-vertical' class='dragger'><div class='dragger-bar'></div></div></div><div class='scrollbar-mask'></div></div></div>").prependTo($('#scrollbar-wrap'));
        }else{
            _scrollBar = $("<div id='scrollbar'><div id='scrollbar-vertical' class='scroll-tools scroll-tools-off'><div class='dragger-container'><div id='dragger-vertical' class='dragger'><div class='dragger-bar'></div></div></div><div class='scrollbar-mask'></div></div></div>").prependTo($('#scrollbar-wrap'));
        }

        this.draggerVertical   = $('#dragger-vertical');
        this.scrollbarVertical = $('#scrollbar-vertical');
        $('.scrollbar-mask').css({opacity:0});
        this.addEvent();
    }

    FrameScrollBar.prototype.constructor = FrameScrollBar;

    // FrameScrollBar.prototype.init = function(){};


    /* *********************************************************
    *  Event Handler
    ********************************************************** */

    FrameScrollBar.prototype.onResize = function(e){
        var _elementH   = this.config.target.height() - 30;
        var _draggerH   = _elementH * 0.2;
        this.railTotalH = _elementH - _draggerH;
        this.draggerVertical.css({height: _draggerH, top: this.railTotalH * this.scrollbarRatioY});
    };

    FrameScrollBar.prototype.getMousePosition = function(e){
        var obj = new Object();
         
        if(e) {
            obj.x = e.pageX;
            obj.y = e.pageY;
        } else {
            obj.x = event.x + document.body.scrollLeft;
            obj.y = event.y + document.body.scrollTop;
        }
         
        return obj;
    };

    FrameScrollBar.prototype.onMouseup = function(e){
        if(t.isTablet) return;

        this.mouseUpDwFlg  = false;
        this.selectFlg     = false;
        $('body').removeClass('select-none');
        this.mouseDraggerDeltaY = 0;
    };

    FrameScrollBar.prototype.onMousedown = function(e){
        if(t.isTablet) return;

        this.mouseDraggerDeltaY = this.getMousePosition(e).y - this.draggerVertical.offset().top;
        this.mouseUpDwFlg = true;
        this.selectFlg    = true;
        $('body').addClass('select-none');
    };

    FrameScrollBar.prototype.onMousemove = function(e){
        if(t.isTablet) return;

        if(!this.mouseUpDwFlg) return;

        //for $('#alarm-scroll')
        $(document).trigger('mousewheel');

        var _elementY = this.config.target.offset().top,
            _mouseY, _y, _ratioY;

        this.config.scrollDelete();

        _mouseY = this.getMousePosition(e).y;
        _y      = _mouseY - _elementY - this.mouseDraggerDeltaY;

        this.scrollbarRatioDesY = _y / this.railTotalH;
        _ratioY = this.scrollbarRatioDesY;

        if(this.scrollbarRatioDesY < 0){
            this.scrollbarRatioDesY = 0;
            _y = 0;
        }else if(this.scrollbarRatioDesY > 1){
            this.scrollbarRatioDesY = 1;
            _y = this.railTotalH;
        }

        if(_ratioY < -0.1 || _ratioY > 1.1) return;

        this.draggerVertical.css({top: _y});
        this.startRender();
    };

    FrameScrollBar.prototype.onTouchStart = function(e){
        this.mouseDraggerDeltaY = this.getTouchInfo(e).y - this.draggerVertical.offset().top;
        this.mouseUpDwFlg = true;
        this.selectFlg    = true;
        $('body').addClass('select-none');
    };

    FrameScrollBar.prototype.onTouchMove = function(e){
        e.preventDefault();
        if(!this.mouseUpDwFlg) return;

        //for $('#alarm-scroll')
        $(document).trigger('mousewheel');

        var _elementY = this.config.target.offset().top,
            _mouseY, _y, _ratioY;

        this.config.scrollDelete();

        _mouseY = this.getTouchInfo(e).y;
        _y      = _mouseY - _elementY - this.mouseDraggerDeltaY;

        this.scrollbarRatioDesY = _y / this.railTotalH;
        _ratioY = this.scrollbarRatioDesY;

        if(this.scrollbarRatioDesY < 0){
            this.scrollbarRatioDesY = 0;
            _y = 0;
        }else if(this.scrollbarRatioDesY > 1){
            this.scrollbarRatioDesY = 1;
            _y = this.railTotalH;
        }

        if(_ratioY < -0.1 || _ratioY > 1.1) return;

        this.draggerVertical.css({top: _y});
        this.startRender();
    };

    FrameScrollBar.prototype.onTouchEnd = function(e){
        this.mouseUpDwFlg  = false;
        this.selectFlg     = false;
        $('body').removeClass('select-none');
        this.mouseDraggerDeltaY = 0;
    };

    FrameScrollBar.prototype.getTouchInfo = function(e){
        var info = { x: 0 , y: 0 };
        $.extend(info,getPagePos(e));
        return info;
    };

    FrameScrollBar.prototype.show = function(){
        this.scrollbarVertical.removeClass("scroll-tools-off");
    };

    FrameScrollBar.prototype.hide = function(){
        this.scrollbarVertical.addClass("scroll-tools-off");
    };


    /* *********************************************************
    *  Rendering
    ********************************************************** */

    FrameScrollBar.prototype.startRender = function(){
        if(typeof this.renderingID == 'undefined'){
            this.renderingID = requestAnimationFrame(this.onRender);
        }
    };

    FrameScrollBar.prototype.stopRender = function(){
        if(typeof this.renderingID != 'undefined'){
            cancelAnimationFrame(this.renderingID);
            this.renderingID = undefined;
        }
    };

    FrameScrollBar.prototype.onRender = function(){
        var _delta = (this.scrollbarRatioDesY - this.scrollbarRatioY) * 0.1;

        if(Math.abs(_delta) < 0.00001){
            this.stopRender();
            this.scrollbarRatioY = this.scrollbarRatioDesY;
            this.config.frameScrollUpdate(this.scrollbarRatioY);
            return;
        }

        this.scrollbarRatioY += _delta;
        this.config.frameScrollUpdate(this.scrollbarRatioY);
        this.renderingID = requestAnimationFrame(this.onRender);
    };


    /* ************************************************************
        Add Event
    ************************************************************ */

    FrameScrollBar.prototype.addEvent = function(){
        var _this = this;

        this.onResize = _bind(this.onResize,this);
        $(window).on("resize", this.onResize);
        this.onResize();

        this.onMouseup = _bind(this.onMouseup,this);
        // $(window).on('mouseup', this.onMouseup);
        $(document).on('mouseup', this.onMouseup);

        this.onMousedown = _bind(this.onMousedown,this);
        this.draggerVertical.on('mousedown', this.onMousedown);

        this.onMousemove = _bind(this.onMousemove,this);

        // $(window).on('mousemove', this.onMousemove);
        $(document).on('mousemove', this.onMousemove);

        $('body').on('selectstart', function(){
            if(_this.selectFlg) return false;
        })
        .on('mousedown', function(){
            if(_this.selectFlg) return false;
        });

        this.onTouchStart   = _bind(this.onTouchStart,this);
        this.onTouchMove    = _bind(this.onTouchMove,this);
        this.onTouchEnd     = _bind(this.onTouchEnd,this);

        this.draggerVertical.bind("touchstart", this.onTouchStart);
        this.scrollbarVertical.bind("touchmove", this.onTouchMove);
        this.scrollbarVertical.bind("touchend", this.onTouchEnd);
    };

    this.FrameScrollBar = FrameScrollBar;
}).call(this);

// requestAnimationFrame polyfill by Erik Möller
// https://gist.github.com/paulirish/1579671
// (function(){for(var d=0,a=["ms","moz","webkit","o"],b=0;b<a.length&&!window.requestAnimationFrame;++b)window.requestAnimationFrame=window[a[b]+"RequestAnimationFrame"],window.cancelAnimationFrame=window[a[b]+"CancelAnimationFrame"]||window[a[b]+"CancelRequestAnimationFrame"];window.requestAnimationFrame||(window.requestAnimationFrame=function(b){var a=(new Date).getTime(),c=Math.max(0,16-(a-d)),e=window.setTimeout(function(){b(a+c)},c);d=a+c;return e});window.cancelAnimationFrame||(window.cancelAnimationFrame=function(a){clearTimeout(a)})})();
