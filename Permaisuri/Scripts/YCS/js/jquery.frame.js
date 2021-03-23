

(function () {
    var _bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

    /* *********************************************************
    *  Constructor 
    ********************************************************** */

    function Frame(option){
        this.frm    = {nowFrm:0};
        this.config = {
            status : false,
            maxFrm : 1000,
            scrollbarUpdate   : function(){},
            sceneFrameUpdate  : function(){}
        };
        $.extend(this.config,option);
    }

    Frame.prototype.constructor = Frame;
    // Frame.prototype.init = function(){};


    /* *********************************************************
    *  Event Handler
    ********************************************************** */

    Frame.prototype = {
        scroll : function(delta, ratioY){
            var _del    = delta,
                _ratioY = ratioY;

            this.frm.nowFrm += _del;
            if(this.frm.nowFrm < 0) this.frm.nowFrm = 0;
            if(this.frm.nowFrm > this.config.maxFrm)this.frm.nowFrm = this.config.maxFrm;
            if(typeof _ratioY != 'undefined') this.frm.nowFrm = this.config.maxFrm * _ratioY;
            this.update(_del);
        },

        update : function(delta){
            var _del    = delta;
            if(_del != 0) this.config.scrollbarUpdate(this.frm.nowFrm / this.config.maxFrm);
            this.config.sceneFrameUpdate(this.frm.nowFrm);
            if(this.config.status)this.status();
        },

        autoframe : function(defFrm, desFrm, duration, easing){
            var _defFrm   = defFrm,
                _desFrm   = desFrm,
                _duration = duration * 1000,
                _easing   = easing,
                _this     = this;

                // _this.scrFlg = false;
                // if(scroll)scroll.stopRender();
                // this.frm.nowFrm = _defFrm;

            $(this.frm).stop().animate( {nowFrm:_desFrm}, {
                duration : _duration,
                easing   : _easing,
                progress : function(){
                    _this.update();
                },
                complete : function(){
                    // _this.scrFlg = true;
                }
            });
        },

        status : function(){
            if(typeof this.scrollStatus == 'undefined'){
                this.scrollStatus = $("<div id='scrollStatus'></div>").prependTo($('body'));
                this.scrollStatus.css({
                    'position' : 'absolute',
                    'z-index': 1000000,
                    'padding': 10,
                    'font-size' : 10,
                    'font-weight' : 300,
                    'text-transform' : 'uppercase',
                    'background-color' : 'rgba(0,0,0,.9)',
                    'color' : '#fff',
                    'width' : 150,
                    'top'  : 50,
                    'letter-spacing' : '0.02em',
                    'line-height' : '1.7em',
                    'font-family' : 'Helvetica'
                });
            }

            this.scrollStatus.html(
                "max frame = " + this.config.maxFrm + "<br>" +
                "frame = " + this.frm.nowFrm.toFixed(2)
            );
        }
    };

    this.Frame = Frame;

}).call(this);
