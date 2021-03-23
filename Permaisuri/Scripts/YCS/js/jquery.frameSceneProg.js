

(function () {
    var _bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

    /* *********************************************************
    *  Constructor 
    ********************************************************** */

    function FrameSceneProg(option){
        this.nowFrm    = 0;
        this.prog      = 0;
        this.progOld   = 0;
        this.config    = {
             startFrm  : 0,
             endFrm    : 0
             // setup   : function(){},
             // update  : function(){}
        };
        this.flg    = false;

        $.extend(this.config,option);
        this.totalFrm = this.config.endFrm - this.config.startFrm;
    }

    FrameSceneProg.prototype.constructor = FrameSceneProg;
    // FrameSceneProg.prototype.init = function(){};


    /* *********************************************************
    *  Event Handler
    ********************************************************** */

    FrameSceneProg.prototype = {
        progControl : function(){
            this.prog = this.prog.toFixed(4);
            if(this.prog < 0) this.prog = 0;
            if(this.prog > 1) this.prog = 1;
        },
        progCheck : function(){
            if(this.prog > 0 && this.prog < 1){
                this.flg = true;
            }else if(this.prog == 1 && this.progOld == 1 || this.prog ==0 && this.progOld == 0){
                this.flg = false;
            }else{
                this.flg = true;
            }
            // (this.prog == 1 && this.progOld == 1 || this.prog ==0 && this.progOld == 0)? this.flg = true : this.flg = false;
            
            this.progOld = this.prog;
        },
        update : function(frm){
            this.prog = (frm - this.config.startFrm) / this.totalFrm;
            this.progControl();
            this.progCheck();
            
            return {prog:this.prog, flg:this.flg};
        }
    };

    this.FrameSceneProg = FrameSceneProg;

}).call(this);
