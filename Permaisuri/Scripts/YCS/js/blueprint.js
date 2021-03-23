(function(){


/* ******************************************************************************************
    LOADING
****************************************************************************************** */

    var mod, modCSSAnimations, modCSSTransforms, modCSSTransitions, modTouch, modAnim;
    var stage = WindowSize();
    var jpFlg = false;

    $(document).ready(function(){
        mod = window.Modernizr;
        modCSSAnimations  = mod && mod.cssanimations;
        modCSSTransforms  = mod && mod.csstransforms;
        modCSSTransitions = mod && mod.csstransitions;
        modTouch          = mod && mod.touch;
        modAnim           = modCSSTransforms && modCSSTransitions;
        if(location.href.indexOf('/jp/') != -1){
            jpFlg = true;
        }
    });

    lexusMagazineOnLoad = function(){
        init();
        setup();
    };

    lexusMagazineReady = function(){
        addEvent();
        introMotion();
    };


/* ******************************************************************************************
    INITIALIZE    
****************************************************************************************** */

    var $wrap, $contents, $scr_cont0, $scr_cont1,
        $cover_bg, $cover_img0, $cover_img1, $cover_img2, $cover_img_mask,
        $cover_text_wrap, $cover_title, $cover_copy, $cover_line, $cover_summary_wrap, $cover_summary,
        $lips_img, $profile_img, $resume_wrap,
        $credit_wrap, $credit_bg_upper, $credit_text_wrap, $credit_bg_bottom, $repeat_wrap, $repeat_btn;

    var $block0_0, $block0_1, $block0_2, $block0_3, $block0_4, $block0_5, $block0_6, $block0_7, $block0_8, $block0_9;

    var $block1_0, $block1_1, $block1_2, $block1_3, $block1_4, $block1_5, $block1_6, $block1_7, $block1_8, $block1_9, $block1_10,
        $block1_11, $block1_12, $block1_13, $block1_14, $block1_15, $block1_16;

    var $blk0_0_block_bg_wh, $blk0_0_sentence, $blk0_1_block_img, $blk0_1_block_cap, $blk0_2_block_bg_wh, $blk0_2_sentence,
        $blk0_3_block_bg_wh, $blk0_3_block_img, $blk0_4_ul_text, $blk0_5_block_bg_wh, $blk0_5_block_img, $blk0_5_block_cap,
        $blk0_6_sentence, $blk0_7_block_img, $blk0_7_block_cap, $blk0_8_block_bg_wh, $blk0_8_block_img, $blk0_9_block_bg_wh, $blk0_9_sentence;

    var $blk1_0_block_bg_wh, $blk1_0_sentence, $blk1_1_block_bg_wh, $blk1_1_block_img, $blk1_2_block_img, $blk1_3_block_bg_wh, $blk1_3_sentence,
        $blk1_4_block_img, $blk1_5_block_bg_wh, $blk1_5_sentence, $blk1_6_block_bg_wh, $blk1_6_block_img, $blk1_6_block_cap, $blk1_7_ul_text,
        $blk1_8_block_bg_ye, $blk1_8_block_img, $blk1_9_block_bg_ye, $blk1_9_sentence, $blk1_10_block_bg_ye, $blk1_10_block_img,
        $blk1_11_block_bg_ye, $blk1_11_sentence, $blk1_12_block_bg_ye, $blk1_12_block_img, $blk1_13_block_bg_ye, $blk1_13_sentence,
        $blk1_14_block_bg_ye, $blk1_14_block_img, $blk1_15_block_bg_ye, $blk1_15_block_bg_wh, $blk1_15_sentence;

    var $pop_btn, $btn_text, $underline, $btn_icon, $popup, $popup_wrap, $popup_close, $popup_close_inner, $popup_outside_bg,
        $block1_16_btn ,$block1_16_btn_txt0, $block1_16_btn_txt1, $block1_16_btn_icon;

    var scroll, framescrollbar, frame;

    var blkTopAry0 = [],
        blkTopAry1 = [];

    var introFlg  = true,
        repeatFlg = false,
        popFlg    = false;


    function init(){
        $wrap                = $('#wrap');
        $contents            = $('#contents');
         
        $scr_cont0           = $('#scr-cont0');
        $scr_cont1           = $('#scr-cont1');

        $cover_bg            = $('.cover-bg');
        $cover_img0          = $('#cover-img0');
        $cover_img1          = $('#cover-img1');
        $cover_img2          = $('#cover-img2');
        $cover_img_mask      = $('.cover-img-mask');
        $cover_text_wrap     = $('.cover-text-wrap');
        $cover_title         = $('.cover-title');
        $cover_copy          = $('.cover-copy');
        $cover_line          = $('.cover-line');
        $cover_summary_wrap  = $('.cover-summary-wrap');
        $cover_summary       = $('.cover-summary');

        $block0_0            = $('#block0-0');
        $block0_1            = $('#block0-1');
        $block0_2            = $('#block0-2');
        $block0_3            = $('#block0-3');
        $block0_4            = $('#block0-4');
        $block0_5            = $('#block0-5');
        $block0_6            = $('#block0-6');
        $block0_7            = $('#block0-7');
        $block0_8            = $('#block0-8');
        $block0_9            = $('#block0-9');

        
        $blk0_0_block_bg_wh  = $('#block0-0 .block-bg-wh');
        $blk0_0_sentence     = $('#block0-0 .sentence');
        $blk0_1_block_img    = $('#block0-1 .block-img');
        $blk0_1_block_cap    = $('#block0-1 .block-cap');
        $blk0_2_block_bg_wh  = $('#block0-2 .block-bg-wh');
        $blk0_2_sentence     = $('#block0-2 .sentence');
        $blk0_3_block_bg_wh  = $('#block0-3 .block-bg-wh');
        $blk0_3_block_img    = $('#block0-3 .block-img');
        $blk0_4_ul_text      = $('#block0-4 .underline-text');
        $blk0_5_block_bg_wh = $('#block0-5 .block-bg-wh');
        $blk0_5_sentence = $('#block0-5 .sentence');

        $blk0_6_block_img = $('#block0-6 .block-img');
        $blk0_71_block_img = $('#block0-71 .block-img');


        $blk0_7_block_bg_wh = $('#block0-7 .block-bg-wh');
        $blk0_7_sentence = $('#block0-7 .sentence');
      
        $blk0_8_block_img = $('#block0-8 .block-img');

        $blk0_9_block_bg_wh  = $('#block0-9 .block-bg-wh');
        $blk0_9_sentence     = $('#block0-9 .sentence');

      

        $lips_img            = $('.lips-img');
        $profile_img         = $('.profile-img');
        $resume_wrap         = $('.resume-wrap');
        $credit_wrap         = $('.credit-wrap');
        $credit_bg_upper     = $('.credit-bg-upper');
        $credit_text_wrap    = $('.credit-text-wrap');
        $credit_bg_bottom    = $('.credit-bg-bottom');
        $repeat_wrap         = $('.repeat-wrap');
        $repeat_btn          = $('.repeat-btn');

        $pop_btn            = $('.pop-btn');
        $btn_text           = $('.btn-text p');
        $underline          = $('.underline');
        $btn_icon           = $('.btn-icon');
       

    

        $lips_img.easymask({align:'C', width:'100%', height:0}, 0);

        for (var i = 0; i < $('#scr-cont0 .block').length; i++) {
            blkTopAry0.push($('#scr-cont0 .block').eq(i).position().top);
        }

        for (var i = 0; i < $('#scr-cont1 .block').length; i++) {
            blkTopAry1.push($('#scr-cont1 .block').eq(i).position().top);
        }

        $("html[lang='en'] .sentence p").addClass('ta-left');
        $("html[lang='ja'] .sentence p").addClass('ta-justify');


        if(navigator.userAgent.indexOf("Win") > -1 && UA.isWebkit){
            $("html[lang='en'] .popup-sentence").addClass('ta-justify');
            $("html[lang='ja'] .popup-sentence").addClass('ta-left');
        }else{
            $('.popup-sentence').addClass('ta-justify');
        }

      
        $btn_text.css(cssTransition('', 0.25, '0.39, 0.575, 0.565, 1'));
        $underline.css(cssTransition('background', 0.25, '0.39, 0.575, 0.565, 1'));

        if(UA.isIE8){
            $('.btn-icon-over').css({opacity:0});
        }

  

      
    }


/* ******************************************************************************************
    SETUP    
****************************************************************************************** */

    function setup(){
        if(navigator.userAgent.indexOf("Win") > -1 && UA.isChrome){
            $('.block-cap').css({letterSpacing: 0});
        }

        $('.fit-img-c').imgfit({align :'c', fit :'cover', canvasMode : true, position :'absolute', callBack : function(){}});

        $cover_img0.css({display:'none'});
        $cover_img1.css({display:'none'});

        if(UA.isIE8){
            $cover_img2.find('img').css({opacity: 0});
            $('.resume-txt p').css({letterSpacing:'0.01em'});
        }else{
            $cover_img2.css({opacity: 0});
        }

        $cover_text_wrap.easymask({align:'B', width: '100%', height: 0}, 0);

        $blk0_0_block_bg_wh.easymask({align:'C', width: 0, height: 0}, 0);
        $blk0_0_sentence.easymask({align:'B', width: 0, height: 0}, 0);

        $blk0_1_block_img.easymask({ align: 'R', width: 0, height: 0 }, 0);
        $blk0_1_block_cap.easymask({align:'R', width: 0, height: '100%'}, 0);

        $blk0_2_block_bg_wh.easymask({align:'R', width: 0, height: 0}, 0);
        $blk0_2_sentence.easymask({align:'R', width: 0, height: 0}, 0);

        $blk0_3_block_bg_wh.easymask({align:'LB', width: 0, height: 0}, 0);
        $blk0_3_block_img.easymask({align:'LB', width: 0, height: 0}, 0);

        $blk0_5_block_bg_wh.easymask({ align: 'LB', width: 0, height: 0 }, 0);
        $blk0_5_sentence.easymask({ align: 'LB', width: 0, height: 0 }, 0);

        $blk0_7_block_bg_wh.easymask({ align: 'RB', width: 0, height: 0 }, 0);
        $blk0_7_sentence.easymask({ align: 'RB', width: 0, height: 0 }, 0);

        $blk0_8_block_img.easymask({ align: 'LB', width: 0, height: 0 }, 0);

        $blk0_9_block_bg_wh.easymask({ align: 'C', width: 0, height: 0 }, 0);
        $blk0_9_sentence.easymask({ align: 'C', width: 0, height: 0 }, 0);



        $blk0_6_block_img.easymask({ align: 'C', width: 0, height: 0 }, 0);
        $blk0_71_block_img.easymask({ align: 'RB', width: 0, height: 0 }, 0);

        $resume_wrap.easymask({align:'C', width: 0, height: 0}, 0);
        $repeat_wrap.easymask({align:'B', width: '100%', height: 0}, 0);

   

        scroll = new Scroll({
            target      : '#wrap',
            // speed       : UA.isIE8?1.8:0.7,
            speed       : UA.isIE8?1.6:1.2,
            // friction    : 0.92,
            friction    : 0.94,
            touchSpeed  : 5,
            scrollLimit : 50,
            type        : 'wheel',
            screenFix   : true,
            stats       : false,
            step        : onScroll
        });

        framescrollbar = new FrameScrollBar({
            target             : $contents,
            show               : false,
            frameScrollUpdate  : onFrameScrollUpdate,
            scrollDelete       : onScrollDelete,
        });

        frame = new Frame({
            status : false,
            maxFrm : 3000,
            scrollbarUpdate   : onScrollbarUpdate,
            sceneFrameUpdate  : onSceneFrameUpdate
        });

        frameSceneSetup();
    }


/* ******************************************************************************************
    FUNCTION
****************************************************************************************** */

    function introMotion(){
        // var _duration = 700,
        var _duration = 650,
            _easing = 'easeInOutExpo';

        $cover_img0.css({display:'block'}).easymask({align:'B', width: '100%', height: 0}, 0);
        $cover_img1.css({display:'block'}).easymask({align:'T', width: '100%', height: 0}, 0);

        var obj0 = {x: 0},
            obj1 = {x: 0};

        $(obj0).animate({x: 100}, {duration: _duration, easing: _easing,
            progress: function(){
                var _now = this.x;
                $cover_img0.easymask({align:'B', width: '100%', height: _now + '%'}, 0);
            },
            complete: function(){
                $cover_img0.css({clip:'auto'});
            }
        });

        $(obj1).stop(true).delay(_duration-50).animate({x: 100}, {duration: _duration, easing: _easing,
            progress: function(){
                var _now = this.x;
                $cover_img1.easymask({align:'T', width:'100%', height: _now + '%'}, 0);
            },
            complete: function(){
                $cover_img1.css({clip:'auto'});
            }
        });

        $cover_text_wrap.easymask({align:'B', width:'100%', height:'100%', ease : _easing, delay : _duration-50+_duration+100-50}, _duration-50+200, function(){
            $cover_text_wrap.css({clip:'auto'});
        });

                 introFlg = false;
            framescrollbar.show();
            if(UA.isIE8){
                $cover_img2.find('img').animate({opacity: 1}, 1000,'easeOutQuart',function(){
                    $cover_img0.css({clip:'auto', display:'none'});
                    $cover_img1.css({clip:'auto', display:'none'});
                });
            }else{
                $cover_img2.animate({opacity: 1}, 1000,'easeOutQuart',function(){
                    $cover_img0.css({clip:'auto', display:'none'});
                    $cover_img1.css({clip:'auto', display:'none'});
                });
            }
      
    }


    var coverSceneProg, coverBgSceneProg, coverImgMaskSceneProg,

        scrContSceneProg0,
        blockScrollProg0_0, blockBgSceneProg0_0, blockSceneProg0_0,
        blockSceneProg0_1, blockCapSceneProg0_1,
        blockScrollProg0_2, blockBgSceneProg0_2, blockSceneProg0_2,
        blockScrollProg0_3, blockBgSceneProg0_3, blockSceneProg0_3,
        blockScrollProg0_4, blockTxtSceneProg0_4_0, blockTxtSceneProg0_4_1, blockTxtSceneProg0_4_2, blockTxtSceneProg0_4_3, blockTxtSceneProg0_4_4, blockTxtSceneProg0_4_5, blockTxtSceneProg0_4_6,
        blockScrollProg0_5, blockBgSceneProg0_5, blockSceneProg0_5, blockCapSceneProg0_5,
        blockScrollProg0_7, blockBgSceneProg0_7, blockSceneProg0_7, blockCapSceneProg0_7,
        blockScrollProg0_6, blockBgSceneProg0_6, blockSceneProg0_6, blockCapSceneProg0_6, 
        blockSceneProg0_71,
        blockSceneProg0_8,
        blockScrollProg0_9, blockBgSceneProg0_9, blockSceneProg0_9, blockCapSceneProg0_9,
        

        lipsMaskSceneProg, lipsScrollSceneProg,

       

     
        lastImgSceneProg;

    var blkDur      = 400,
        blkBgDltDur = 60,
        UlTxtDltDur = 40,
        // capDur      = 300,
        capDur      = 180,
        scrDur      = 0;

    function frameSceneSetup(){
        coverSceneProg          = new FrameSceneProg({startFrm :    0, endFrm :  190});
        coverBgSceneProg        = new FrameSceneProg({startFrm :   0, endFrm :  190});
        coverImgMaskSceneProg = new FrameSceneProg({ startFrm: 0, endFrm: 390 });
        scrContSceneProg0 = new FrameSceneProg({ startFrm:0, endFrm: 190 + 1800 + 1000 });

        var contFrm0 = 0;
        blockScrollProg0_0 = new FrameSceneProg({ startFrm: 0, endFrm: 380 });
        blockBgSceneProg0_0 = new FrameSceneProg({ startFrm: 0, endFrm: 300 });
        blockSceneProg0_0       = new FrameSceneProg({startFrm :  20, endFrm : 300});

        blockSceneProg0_1       = new FrameSceneProg({startFrm : 100,  endFrm : 280});

        blockScrollProg0_2 = new FrameSceneProg({ startFrm: 180, endFrm: 500});
        blockBgSceneProg0_2 = new FrameSceneProg({ startFrm: 180, endFrm: 500 });
        blockSceneProg0_2 = new FrameSceneProg({ startFrm: 180, endFrm: 600 });

        blockScrollProg0_3      = new FrameSceneProg({startFrm : 300,                endFrm : 600});
        blockBgSceneProg0_3 = new FrameSceneProg({ startFrm: 300, endFrm: 500 });
        blockSceneProg0_3 = new FrameSceneProg({ startFrm: 300, endFrm: 600 });


        blockScrollProg0_5 = new FrameSceneProg({ startFrm: 600, endFrm: 900 });
        blockBgSceneProg0_5 = new FrameSceneProg({ startFrm: 600, endFrm: 900 });
        blockSceneProg0_5 = new FrameSceneProg({ startFrm: 600, endFrm: 900 });

        blockSceneProg0_6 = new FrameSceneProg({ startFrm: 1000, endFrm: 1400 });
        blockSceneProg0_71 = new FrameSceneProg({ startFrm: 1400, endFrm: 1800 });
        blockSceneProg0_8 = new FrameSceneProg({ startFrm: 1500, endFrm: 2100 });

        blockScrollProg0_7 = new FrameSceneProg({ startFrm: 1100, endFrm: 1400 });
        blockBgSceneProg0_7 = new FrameSceneProg({ startFrm: 1100, endFrm: 1400 });
        blockSceneProg0_7 = new FrameSceneProg({ startFrm: 1100, endFrm: 1400 });


        blockBgSceneProg0_9 = new FrameSceneProg({ startFrm: 1700, endFrm: 2100 });
        blockSceneProg0_9 = new FrameSceneProg({ startFrm: 1700, endFrm: 2100 });


        lastImgSceneProg        = new FrameSceneProg({startFrm : 2200,  endFrm : 3000});


    }


    function onSceneFrameUpdate(frm){
        scrContScene0(scrContSceneProg0.update(frm));
       

        coverScene(coverSceneProg.update(frm));
        coverBgScene(coverBgSceneProg.update(frm));
        coverImgMaskScene(coverImgMaskSceneProg.update(frm));

        lastImgScene(lastImgSceneProg.update(frm));
       
      

        blockScroll($block0_0, blockScrollProg0_0.update(frm), blkTopAry0[0], 500);

        blockMotion($blk0_0_block_bg_wh, 'B', blockBgSceneProg0_0.update(frm));
        blockMotion($blk0_0_sentence, 'B', blockSceneProg0_0.update(frm));

        blockMotion($blk0_1_block_img, 'B', blockSceneProg0_1.update(frm));

        blockScroll($block0_2, blockScrollProg0_2.update(frm), blkTopAry0[2], 100);
        blockMotion($blk0_2_block_bg_wh, 'R', blockBgSceneProg0_2.update(frm));
        blockMotion($blk0_2_sentence, 'R', blockSceneProg0_2.update(frm));

        blockScroll($block0_3, blockScrollProg0_3.update(frm), blkTopAry0[3], 180);
        blockMotion($blk0_3_block_bg_wh, 'LB', blockBgSceneProg0_3.update(frm));
        blockMotion($blk0_3_block_img, 'LB', blockSceneProg0_3.update(frm));

       
        blockScroll($block0_5, blockScrollProg0_5.update(frm), 910, 200);
        blockMotion($blk0_5_block_bg_wh, 'LB', blockBgSceneProg0_5.update(frm));
        blockMotion($blk0_5_sentence, 'LB', blockSceneProg0_5.update(frm));

        blockMotion($blk0_6_block_img, 'C', blockSceneProg0_6.update(frm));
        blockMotion($blk0_71_block_img, 'RB', blockSceneProg0_71.update(frm));
        blockMotion($blk0_8_block_img, 'LB', blockSceneProg0_8.update(frm));

        blockScroll($block0_7, blockScrollProg0_7.update(frm), 1300, 200);
        blockMotion($blk0_7_block_bg_wh, 'RB', blockBgSceneProg0_7.update(frm));
        blockMotion($blk0_7_sentence, 'RB', blockSceneProg0_7.update(frm));


        blockMotion($blk0_9_block_bg_wh, 'C', blockBgSceneProg0_9.update(frm));
        blockMotion($blk0_9_sentence, 'C', blockSceneProg0_9.update(frm));
       

   
     

      
     
    }

    function scrContScene0(obj){
        if(!obj.flg) return;
        var _prog = obj.prog;
        $scr_cont0.css({top: (-(1800+1000) * _prog) + 'px'});
    }

  

    function coverScene(obj){
        if(!obj.flg) return;
        var _prog = $.easing['easeInOutCubic'](obj.prog, obj.prog,0,1,1);
        var _p    = 1 - _prog;

        if(_p == 0){
            $cover_text_wrap.css({height: 0, display:'none'});
        }else{
            $cover_text_wrap.css({height: (67 * _p) + '%', display:'block'});
            $cover_title.css({top: (40 * _prog) + 'px'});
            $cover_copy.css({top: (40 + 80 * _prog) + 'px'});
            $cover_line.css({top: (98 + 120 * _prog) + 'px'});
            $cover_summary_wrap.css({top: (124 + 120 * _prog) + 'px'});
            var _lh = (1.7 + 6 * _prog).toFixed(2);
            $cover_summary.css({lineHeight: _lh + 'em'})
        }
    }

    function coverBgScene(obj){
        if(!obj.flg) return;
        var _prog = 1 - obj.prog;
        _prog = $.easing['easeInOutCubic'](_prog,_prog,0,1,1);
        $cover_bg.css({height: (0 * _prog) + '%'});
    }

    function coverImgMaskScene(obj){
        if(!obj.flg) return;
        var _prog = obj.prog;
        _prog = $.easing['easeInOutCubic'](_prog,_prog,0,1,1);

        $cover_img_mask.css({height: (100 * _prog) + '%'});
        if(_prog == 1){
            $cover_img2.css({display : 'none'});
        }else{
            $cover_img2.css({display : 'block'});
        }
    }

    var lipsProg = 0;
    function lipsMaskScene(obj){
        if(!obj.flg) return;
        lipsProg = $.easing['easeInOutCubic'](obj.prog,obj.prog,0,1,1);
        $lips_img.easymask({align:'C', width:'100%', height: lipsProg * 100 + '%'}, 0);
    }

    function lipsScrollScene(obj){
        if(!obj.flg) return;
        var _prog = $.easing['easeInQuad'](obj.prog, obj.prog, 0, 1, 1);
        $lips_img.css({top: -100 * _prog + '%'});
    }

    function lastImgScene(obj){
        if(!obj.flg) return;
        var _prog = $.easing['easeInQuad'](1 - obj.prog, 1 - obj.prog, 0, 1, 1);
        $profile_img.css({top: _prog * 100 + '%'});
    }

    var repeatProg = 0;
    function lastScene(obj){
        if(!obj.flg) return;
        var _prog = $.easing['easeInOutQuart'](obj.prog,obj.prog,0,1,1);
        repeatProg = _prog;

        $credit_bg_upper.css({top: (-100 + (50 * _prog)) + '%'});
        $credit_text_wrap.css({top: (50 * (1 - _prog)) + '%'});
        $credit_bg_bottom.css({top: (100 - (50.1 * _prog)).toFixed(2) + '%'});

        if(repeatProg == 0){
            framescrollbar.alarmScrFlg = true;
            $credit_wrap.css({display:'none'});
        }else{
            framescrollbar.alarmScrFlg = false;
            $credit_wrap.css({display:'block'});
        }
        $repeat_wrap.easymask({align:'B', width: '100%', height: repeatProg * 100 + '%'}, 0);
    }

    function alarmScrIconScene(obj){
        if(!obj.flg) return;
        var _prog = obj.prog;

        if(_prog == 0){
            framescrollbar.alarmScrFlg = true;
            if($('#alarm-scroll')[0]){
                $('#alarm-scroll').css({display:'block'}).stop(true).animate({opacity:1}, 600, 'easeOutQuart');
            }
        }else{
            framescrollbar.alarmScrFlg = false;
            if($('#alarm-scroll')[0]){
                $('#alarm-scroll').stop(true).animate({opacity:0}, 600, 'easeOutQuart', function(){
                    $(this).css({display:'none'});
                });
            }
        }
    }

    function blockScroll(element, obj, defY, deltaY){
        if(!obj.flg) return;
        var _element = element;
        var _defY    = defY;
        var _deltaY  = deltaY;
        var _prog    = $.easing['easeInOutQuad'](obj.prog, obj.prog, 0, 1, 1);
        _element.css({top:_defY - (_deltaY * _prog) + (_deltaY * 0.5) + 'px'});
    }

    function blockMotion(element, align, obj){
        if(!obj.flg) return;
        var _element = element;
        var _prog = $.easing['easeInOutCubic'](obj.prog, obj.prog, 0, 1, 1);
        var _align = align;
        _element.easymask({align:_align, width: _prog * 100 + '%', height: _prog * 100 + '%'}, 0);
    }



    function blockCapMotion(element, align, obj){
        if(!obj.flg) return;
        var _element = element;
        var _prog = $.easing['easeInOutCubic'](obj.prog, obj.prog, 0, 1, 1);
        var _align = align;
        _element.easymask({align:_align, width: _prog * 100 + '%', height: '100%'}, 0);
    }

    function blockUnderlineTxtMotion(element, align, obj){
        if(!obj.flg) return;
        var _element = element,
            _prog    = obj.prog,
            _align   = align;

        _prog = $.easing['easeInOutCubic'](obj.prog, obj.prog, 0, 1, 1);
        _element.easymask({align:_align, width: _prog * 100 + '%', height: '100%'}, 0);
    }

    function resumeScroll(obj){
        if(!obj.flg) return;
        var _prog = obj.prog;
        _prog = $.easing['easeInOutQuart'](obj.prog, obj.prog, 0, 1, 1);
        $resume_wrap.css({marginTop: -323 + scrDur * (1 - _prog) + 'px'});
    }

    function resumeMotion(element, align, obj){
        if(!obj.flg) return;
        var _element = element,
            _prog    = obj.prog,
            _align   = align;

        _prog = $.easing['easeInOutQuart'](obj.prog, obj.prog, 0, 1, 1);
        _element.easymask({align:_align, width: _prog * 100 + '%', height: _prog * 100 + '%'}, 0);
    }

    var repeatTime = 4000;
    function repeatMotion(){
        if(repeatFlg) return;

        framescrollbar.stopRender();
        frame.autoframe(9560 + 100, 0, 3.0, 'easeInOutCubic');

        repeatFlg = true;
        $('.scrollbar-mask').css({width:'100%'});

        setTimeout(function(){
            repeatFlg = false;
            $('.scrollbar-mask').css({width: 0});
        }, repeatTime);

        //for $('#alarm-scroll')
        $(document).trigger('mousewheel');
    }

   

  

    function urlhoverChange(elem, out, over ,style){
        var _element = elem,
            _out     = out,
            _over    = over,
            _style   = style,
            _url     = _element.attr("src");

        if(_url.indexOf(_out)>-1 && style == 'OVER'){
            _url = _url.replace(_out, _over);
        }else if(_url.indexOf(_over)>-1 && style == 'OUT'){
            _url = _url.replace(_over, _out);
        }
        _element.attr("src",_url);
    }


/* ******************************************************************************************
    EVENT & EVENT HANDLER    
****************************************************************************************** */

    function onScrollbarUpdate(p) {
       
        framescrollbar.scrollbarRatioY = p;
        framescrollbar.draggerVertical.css({top: framescrollbar.railTotalH * framescrollbar.scrollbarRatioY});
    }

    function onFrameScrollUpdate(p) {
  
        frame.scroll(0, p);
    }

    function onScroll(p) {
 
        if(!framescrollbar.mouseUpDwFlg) framescrollbar.stopRender();
        var mnsFlg = MagazineNaviStatus();
        if(mnsFlg || framescrollbar.mouseUpDwFlg || introFlg || repeatFlg || popFlg) return;
        frame.scroll(p);
    }

    function onScrollDelete(){
        if(scroll) scroll.stopRender();
    }

    function addEvent(){
        $(window).on('keydown', onKeydown);
        $(window).on('resize', onResize);
        $(window).trigger('resize');
        $repeat_btn.on('click',function(){repeatMotion();});
    }

    function onResize(){
        stage = WindowSize();
       
        $lips_img.easymask({align:'C', width:'100%', height: lipsProg * 100 + '%'}, 0);
        $repeat_wrap.easymask({align:'B', width: '100%', height: repeatProg * 100 + '%'}, 0);

        var _blkDur,
            // _h = Math.floor(stage.height * 0.3);
            _h = Math.floor(stage.height * 0.5);

        if(_h < 350){
            _blkDur = 350;
        }else if(_h > 500){
            _blkDur = 500;
        }else{
            _blkDur = _h;
        }

        blkDur = _blkDur;
        frameSceneSetup();
    }

    function onKeydown(e){
        switch(e.keyCode){
            case 38 : onKeyScrollControl(1); break;
            case 40 : onKeyScrollControl(-1); break;
        }
    }

    // function onKeyScrollControl(delta){
    //     for(var i=0; i<5; i++){
    //         var del = delta * i * 20;
    //         if(scroll)scroll.onWheel({}, del, del, del);
    //     }
    // }

    var keybordValue = UA.isMac?10:1;
    function onKeyScrollControl(delta){
        for(var i=0; i<10; i++){
            var del = delta*keybordValue;
            if(scroll)scroll.onWheel({},del, del, del);
            // if(scroll)scroll.onWheel({},0,0,delta);
        }
    }

    function WindowSize(){
        var size = { width:0, height:0, halfX:0, halfY:0};
        if (document.documentElement.clientHeight) {
            size.width  = document.documentElement.clientWidth;
            size.height = document.documentElement.clientHeight;
            size.halfX  = document.documentElement.clientWidth * 0.5;
            size.halfY  = document.documentElement.clientHeight * 0.5;
        } else if (document.body.clientHeight) {
            size.width  = document.body.clientWidth;
            size.height = document.body.clientHeight;
            size.halfX  = document.body.clientWidth * 0.5;
            size.halfY  = document.body.clientHeight * 0.5;
        } else if (window.innerHeight) {
            size.width  = window.innerWidth;
            size.height = window.innerHeight;
            size.halfX  = window.innerWidth * 0.5;
            size.halfY  = window.innerHeight * 0.5;
        }
        return size;
    }


/* ******************************************************************************************
    UTIL
****************************************************************************************** */

    function cssTransition(selector, duration, easing){
        var css = selector + ' ' + duration + 's' + ' cubic-bezier(' + easing + ')';
        return {
            '-webkit-transition' : css,
            '-moz-transition'    : css,
            '-o-transition'      : css,
            '-ms-transition'     : css,
            'transition'         : css
        };
    }


}).call(this);

// requestAnimationFrame polyfill by Erik Möller
// https://gist.github.com/paulirish/1579671
// (function(){for(var d=0,a=["ms","moz","webkit","o"],b=0;b<a.length&&!window.requestAnimationFrame;++b)window.requestAnimationFrame=window[a[b]+"RequestAnimationFrame"],window.cancelAnimationFrame=window[a[b]+"CancelAnimationFrame"]||window[a[b]+"CancelRequestAnimationFrame"];window.requestAnimationFrame||(window.requestAnimationFrame=function(b){var a=(new Date).getTime(),c=Math.max(0,16-(a-d)),e=window.setTimeout(function(){b(a+c)},c);d=a+c;return e});window.cancelAnimationFrame||(window.cancelAnimationFrame=function(a){clearTimeout(a)})})();
