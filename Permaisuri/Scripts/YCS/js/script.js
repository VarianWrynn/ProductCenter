(function($){

	var narrow      = false;
	var header      = $('#globalheader');
	var headerTop   = header.find('.top');
	var menuSec     = header.find('.sec');
	var menuSub     = header.find('.sub');
	var buttonsTop  = headerTop.find('a');
	var buttonsSub   = menuSec.find('a');
	var buttonMenu  = buttonsTop.eq(0);
	var buttonAmaz  = buttonsTop.eq(1);
	var buttonSearch = buttonsTop.eq(2);
	var buttonLang  = buttonsTop.eq(3);
	var buttonLocal = buttonsTop.eq(4);
	var spClose     = header.find('.intsp-btn-close');
	var regions     = header.find('#local .regions a');

	var chageTimerID = null;
	var closeTimerID = null;
	var closeTimeout = 2500;


	/*****************************************************************************************************************
	 * RESPONSIVE
	 *****************************************************************************************************************/

	if(window.useResponsive){
		jQuery.windowSizeChanged(function(){
			narrow = false;
			menuSub.css({display:'none'}).removeClass('open');
			menuSec.css({display:'none'}).removeClass('open');
			buttonsTop.removeClass('selected');
			buttonsSub.removeClass('selected');

			if(Shared.ua.isAndroid){
				menuSub.css({opacity:1});
				menuSec.css({opacity:1});
			}
		}, function(){
			narrow = true;
			menuSub.css({display:'none'}).removeClass('open');
			menuSec.css({display:'none'}).removeClass('open');
			buttonsTop.removeClass('selected');
			buttonsSub.removeClass('selected');
		});
	}else{
		var mapHolder = header.find('.map-holder');
		var globalSites = header.find('.global-sites');
		var globalRegion = globalSites.find('.region');

		jQuery.addResizeObserver(function(w, h){
			if(w > 1100){
				mapHolder.clearStyle();
				globalSites.clearStyle();
				globalRegion.clearStyle();
			}else{
				mapHolder.css({display:'none'});
				globalSites.css({paddingRight:20});
				globalRegion.css({paddingRight:20});
			}
		});
	}

	// 横幅1024px未満の場合の横スクロール対応
	if(!Shared.ua.isSmartPhone){
		var winWidth = null;
		var minWidth = 1024;

		jQuery.addResizeObserver(function(w){
			winWidth = w;
		});
		jQuery.addScrollObserver(function(t, b, l, r){
			if(!narrow && winWidth < minWidth){
				if(l < 0) l = 0;
				if(l > minWidth-winWidth) l = minWidth-winWidth;
				header.css({marginLeft:-l});
			}else{
				header.css({marginLeft:0});
			}
		});
	}else{
		header.find('a').css('-webkit-tap-highlight-color', 'rgba(0,0,0,0)');
		header.find('.region-title').css('-webkit-tap-highlight-color', 'rgba(0,0,0,0)');
	}



	/*****************************************************************************************************************
	 * INIT
	 *****************************************************************************************************************/

	$('#menu a').each(function(i){
		var rel = this.getAttribute('rel');

		if(rel && rel.indexOf('#') === 0){
			$(this).append('<span class="ind"></span>');
		}
	});

	if(Shared.css.hasTransition){
		header.transform('translateZ(0)');

		menuSec.css({display:'none', opacity:0, '-webkit-backface-visibility':'hidden'}).transition('all', 400, 'easeInOutQuart').transformOrigin('50%', '0%');
		menuSub.css({display:'none', opacity:0, '-webkit-backface-visibility':'hidden'}).transition('all', 400, 'easeInOutQuart').transformOrigin('50%', '0%');

		if(Shared.ua.isAndroid){
			menuSec.transform('rotateX(-90deg)');
			menuSub.transform('rotateX(-90deg)');
		}else{
			menuSec.transform('perspective(1000px) rotateX(-90deg)');
			menuSub.transform('perspective(1000px) rotateX(-90deg)');
		}

		menuSec.transitionEnd(function(){
			if($(this).hasClass('open')){
				// videoタグのクリック対応
				if(Shared.ua.isiPhone){
					$('.flplayer video, .ytplayer iframe').each(function(){
						if($(this).offset().top < 1000){
							$(this).addClass('video_unclickable');
						}
					});
				}
			}else{
				this.style.display = 'none';
			}
		});
		menuSub.transitionEnd(function(){
			if(!$(this).hasClass('open')) this.style.display = 'none';
		});
		if(Shared.ua.isAndroid){
			menuSec.css({webkitPerspective:'none', opacity:1});
			menuSub.css({webkitPerspective:'none', opacity:1});
		}
	}else{
		menuSec.css({display:'none', opacity:1});
		menuSub.css({display:'none', opacity:1});
	}



	/*****************************************************************************************************************
	 * EVENT BINDING
	 *****************************************************************************************************************/

	if(Shared.html.hasTouch){
		closeTimeout = 8000;

		buttonMenu.bind('click',  clickSecButton);
		buttonAmaz.bind('click',  clickSecButton);
		buttonLocal.bind('click', clickSecButton);
		buttonSearch.bind('click', clickSecButton);
		buttonsSub.bind('click',   clickSubButton);
		regions.bind('click',     showRegion);
		spClose.bind('click',     closeSp); // narrow only

		// close event
		window.addEventListener('touchend', function(){
			clearTimeout(closeTimerID);
			setCloseTimer();
		}, false);
	}else{
		buttonMenu.bind('click',      clickSecButton);
		buttonAmaz.bind('click',      clickSecButton);
		buttonSearch.bind('click',      clickSecButton);
		buttonMenu.bind('mouseenter', enterSecButton);
		buttonAmaz.bind('mouseenter', enterSecButton);
		buttonSearch.bind('mouseenter', enterSecButton);
		buttonsSub.bind('mouseenter', enterSubButton);
		regions.bind('mouseenter',    showRegion);
		regions.bind('click',         showRegion);
		spClose.bind('click',         closeSp); // narrow only

		// local only
		buttonLocal.bind('click', clickSecButton).bind('click', spinGlobe);
		buttonLocal.find('.ico').bind('mouseenter', enterSecButton).bind('mouseenter', spinGlobe);

		// close event
		menuSec.hover(function(){
			clearTimeout(closeTimerID);
		}, function(){
			setCloseTimer();
		});
		menuSub.hover(function(){
			clearTimeout(closeTimerID);
		}, function(){
			setCloseTimer();
		});

		// lang
		buttonLang.hover(function(){
			closeAll();
			buttonLang.addClass('selected');

		}, function(){
			buttonLang.removeClass('selected');
		});
	}



	/*****************************************************************************************************************
	 * ENTER EVENT
	 *****************************************************************************************************************/

	// sub
	function enterSecButton(e){

		if(this.tagName == 'A'){
			var that = this;
		}else{
			var that = this.parentNode;
		}
		var self = $(that);

		if(!self.hasClass('selected')){

			clearTimeout(closeTimerID);

			function _open(){
				self.addClass('selected');

				var selected = buttonsTop.filter('.selected');

				if(menuSub.filter('.open').length > 0){
					closeSub();

					setTimeout(function(){
						selected.each(function(i){
							if(this == that){
								openSec(selected.eq(i));
							}else{
								closeSec(selected.eq(i));
							}
						});
					}, 100);
				}else{
					selected.each(function(i){
						if(this == that){
							openSec(selected.eq(i));
						}else{
							closeSec(selected.eq(i));
						}
					});
				}
			}

			if(e.type == 'mouseenter'){
				clearTimeout(chageTimerID);

				self.addClass('hover').bind('mouseleave', function(){
					self.unbind('mouseleave', arguments.callee);
					self.removeClass('hover');
				});
				chageTimerID = setTimeout(function(){
					if(self.hasClass('hover')){
						_open();
					}
				}, 300);
			}else{
				_open();
			}
		}
	}

	// sub
	function enterSubButton(e){
		if(narrow) return false;

		var that = this;
		var self = $(that);

		clearTimeout(closeTimerID);
		clearTimeout(chageTimerID);

		if(self.hasClass('selected')){
			return false;
		}

		self.addClass('hover').bind('mouseleave', function(){
			self.unbind('mouseleave', arguments.callee);
			self.removeClass('hover');
		});

		if(self.attr('rel')){
			// wait 300msec
			chageTimerID = setTimeout(function(){
				if(self.hasClass('hover')){
					self.addClass('selected');

					var selected = buttonsSub.filter('.selected');

					selected.each(function(i){
						if(this == that){
							openSub(selected.eq(i), e);
						}else{
							closeSub(selected.eq(i));
						}
					});
				}
				self.removeClass('hover');
			}, 300);
		}else{
			// wait 1000msec
			chageTimerID = setTimeout(function(){
				if(self.hasClass('hover')){
					closeSub();
				}
				self.removeClass('hover');
			}, 1000);
		}
	}



	/*****************************************************************************************************************
	 * CLICK BINDING
	 *****************************************************************************************************************/

	// sec
	function clickSecButton(e){
		e.preventDefault();

		var self = $(this);

		if(self.hasClass('selected')){
			if(menuSub.filter('.open').length > 0){
				closeAll();
			}else{
				closeSec(self);
			}
		}else{
			enterSecButton.call(this, e);
		}
	}

	// sub
	function clickSubButton(e){
		if(narrow) return true;

		var that = this;
		var self = $(that);

		if(self.attr('rel')){
			if(self.hasClass('selected')){
				return true; // link
			}else{
				e.preventDefault();

				var selectedButton = buttonsSub.filter('.selected');

				if(selectedButton.length > 0){
					closeSub();

					setTimeout(function(){
						enterSubButton.call(that, e);
					}, 100);
				}else{
					enterSubButton.call(that, e);
				}
			}
		}
	}



	/*****************************************************************************************************************
	 * ANIMATION
	 *****************************************************************************************************************/

	// open sec
	function openSec(button){
		var sec = $('#' + button.attr('href').split('#')[1]);

		sec.addClass('open').css({display:'block'});

		if(Shared.css.hasTransition){
			setTimeout(function(){
				if(button.hasClass('selected')){
					if(Shared.ua.isAndroid){
						sec.transform('rotateX(0deg)').css({opacity:1});
					}else{
						sec.transform('perspective(1000px) rotateX(0deg)').css({opacity:1});
					}
				}
			}, 300);
		}

		if(sec.attr('id') == 'local'){
			map();
		}
	}

	// open sub
	function openSub(button, e){
		var sub = $(button.attr('rel'));

		sub.addClass('open').css({display:'block'});

		if(e.type == 'click'){
			var delay = 30;
		}else{
			var delay = 300;
		}
		if(Shared.css.hasTransition){
			setTimeout(function(){
				if(button.hasClass('selected')){
					if(Shared.ua.isAndroid){
						sub.transform('rotateX(0deg)').css({opacity:1});
					}else{
						sub.transform('perspective(1000px) rotateX(0deg)').css({opacity:1});
					}
				}
			}, delay);
		}
	}

	// close sec
	function closeSec(button){
		if(button){
			var sec = $('#' + button.attr('href').split('#')[1]);
			button.removeClass('selected');
		}else{
			var sec = menuSec.filter('.open');
			buttonsTop.filter('.selected').not('.intlang').removeClass('selected');
		}

		if(Shared.ua.isiPhone){
			$('.flplayer video, .ytplayer iframe').removeClass('video_unclickable');
		}

		sec.removeClass('open');

		if(Shared.css.hasTransition){
			if(Shared.ua.isAndroid){
				sec.transform('rotateX(-90deg)').css({opacity:1});
			}else{
				sec.transform('perspective(1000px) rotateX(-90deg)').css({opacity:0});
			}
		}else{
			sec.css({display:'none'});
		}
	}

	// close sub
	function closeSub(button){
		if(button){
			var sub = $(button.attr('rel'));
			button.removeClass('selected');
		}else{
			var sub = menuSub.filter('.open');
			buttonsSub.filter('.selected').removeClass('selected');
		}

		sub.removeClass('open');

		if(Shared.css.hasTransition){
			if(Shared.ua.isAndroid){
				sub.transform('rotateX(-90deg)').css({opacity:1});
			}else{
				sub.transform('perspective(1000px) rotateX(-90deg)').css({opacity:0});
			}
		}else{
			sub.css({display:'none'});

		}
	}



	/*****************************************************************************************************************
	 * UTILITY
	 *****************************************************************************************************************/

	// close for narow mode
	function closeSp(e){
		e.preventDefault();
		window.scrollTo(0, 1);
		closeSec();
	}

	// close all menu
	function closeAll(){
		closeSub();
		setTimeout(closeSec, 200);
	}

	// close timer
	function setCloseTimer(){
		clearTimeout(closeTimerID);
		closeTimerID = setTimeout(closeAll, closeTimeout);
	}



	/*****************************************************************************************************************
	 * MAP
	 *****************************************************************************************************************/

	geoPosition.init();

	var continentMap = {"AD":"Europe","AE":"Asia","AF":"Asia","AG":"North America","AI":"North America","AL":"Europe","AM":"Asia","AN":"North America","AO":"Africa","AQ":"Antarctica","AR":"South America","AS":"Oceania","AT":"Europe","AU":"Oceania","AW":"North America","AZ":"Asia","BA":"Europe","BB":"North America","BD":"Asia","BE":"Europe","BF":"Africa","BG":"Europe","BH":"Asia","BI":"Africa","BJ":"Africa","BM":"North America","BN":"Asia","BO":"South America","BR":"South America","BS":"North America","BT":"Asia","BW":"Africa","BY":"Europe","BZ":"North America","CA":"North America","CC":"Asia","CD":"Africa","CF":"Africa","CG":"Africa","CH":"Europe","CI":"Africa","CK":"Oceania","CL":"South America","CM":"Africa","CN":"Asia","CO":"South America","CR":"North America","CU":"North America","CV":"Africa","CX":"Asia","CY":"Europe","CZ":"Europe","DE":"Europe","DJ":"Africa","DK":"Europe","DM":"North America","DO":"North America","DZ":"Africa","EC":"South America","EE":"Europe","EG":"Africa","EH":"Africa","ER":"Africa","ES":"Europe","ET":"Africa","FI":"Europe","FJ":"Oceania","FK":"South America","FM":"Oceania","FO":"Europe","FR":"Europe","GA":"Africa","GB":"Europe","GD":"North America","GE":"Asia","GF":"South America","GG":"Europe","GH":"Africa","GI":"Europe","GL":"North America","GM":"Africa","GN":"Africa","GP":"North America","GQ":"Africa","GR":"Europe","GS":"Antarctica","GT":"North America","GU":"Oceania","GW":"Africa","GY":"South America","HK":"Asia","HN":"North America","HR":"Europe","HT":"North America","HU":"Europe","ID":"Asia","IE":"Europe","IL":"Asia","IM":"Europe","IN":"Asia","IO":"Asia","IQ":"Asia","IR":"Asia","IS":"Europe","IT":"Europe","JE":"Europe","JM":"North America","JO":"Asia","JP":"Asia","KE":"Africa","KG":"Asia","KH":"Asia","KI":"Oceania","KM":"Africa","KN":"North America","KP":"Asia","KR":"Asia","KW":"Asia","KY":"North America","KZ":"Europe","LA":"Asia","LB":"Asia","LC":"North America","LI":"Europe","LK":"Asia","LR":"Africa","LS":"Africa","LT":"Europe","LU":"Europe","LV":"Europe","LY":"Africa","MA":"Africa","MC":"Europe","MD":"Europe","ME":"Europe","MG":"Africa","MH":"Oceania","MK":"Europe","ML":"Africa","MM":"Asia","MN":"Asia","MO":"Asia","MP":"Oceania","MQ":"North America","MR":"Africa","MS":"North America","MT":"Europe","MU":"Africa","MV":"Asia","MW":"Africa","MX":"North America","MY":"Asia","MZ":"Africa","NA":"Africa","NC":"Oceania","NE":"Africa","NF":"Oceania","NG":"Africa","NI":"North America","NL":"Europe","NO":"Europe","NP":"Asia","NR":"Oceania","NU":"Oceania","NZ":"Oceania","OM":"Asia","PA":"South America","PE":"South America","PF":"Oceania","PG":"Oceania","PH":"Asia","PK":"Asia","PL":"Europe","PM":"North America","PN":"Oceania","PR":"North America","PS":"Asia","PT":"Europe","PW":"Oceania","PY":"South America","QA":"Asia","RE":"Europe","RO":"Europe","RS":"Europe","RU":"Europe","RW":"Africa","SA":"Asia","SB":"Oceania","SC":"Africa","SD":"Africa","SE":"Europe","SG":"Asia","SH":"Africa","SI":"Europe","SJ":"Europe","SK":"Europe","SL":"Africa","SM":"Europe","SN":"Africa","SO":"Africa","SR":"South America","ST":"Africa","SV":"North America","SY":"Asia","SZ":"Africa","TC":"North America","TD":"Africa","TF":"Antarctica","TG":"Africa","TH":"Asia","TJ":"Asia","TK":"Oceania","TM":"Asia","TN":"Africa","TO":"Oceania","TR":"Asia","TT":"North America","TV":"Oceania","TW":"Asia","TZ":"Africa","UA":"Europe","UG":"Africa","US":"North America","UY":"South America","UZ":"Asia","VC":"North America","VE":"South America","VG":"North America","VI":"North America","VN":"Asia","VU":"Oceania","WF":"Oceania","WS":"Oceania","YE":"Asia","YT":"Africa","ZA":"Africa","ZM":"Africa","ZW":"Africa"};

	// spin glove
	function spinGlobe(){
		var ico  = buttonLocal.find('.ico');
		var idx  = 0;
		var spin = 0;

		setTimeout(function(){
			if(narrow){
				ico.css({backgroundPosition:'-'+(160+30*idx)+'px -5px'});
			}else{
				ico.css({backgroundPosition:'-'+(540+30*idx)+'px -30px'});
			}
			if(spin < 3){
				if(++idx < 4){
					setTimeout(arguments.callee, 40);
				}else{
					idx = 0;
					spin++;
					setTimeout(arguments.callee, 40);
				}
			}else{
				ico.get(0).style.cssText = '';
			}
		}, 40);
	}

	function showRegion(e){
		e.preventDefault();

		$('#local .region').removeClass( 'show' );

		$('#local .regions a').removeClass('select');

		$(this).addClass('select');

		$( '#' + this.href.split('#')[1] ).addClass( 'show' );
	}

	// show map

	function map(){
		var currentLocation = $.cookie('current_location');

		if(currentLocation){//Cookieに保存されている場合
			currentLocation = currentLocation.split('/');
			var countryName   = currentLocation[0];
			var continentName = currentLocation[1];
			var countryCode   = currentLocation[2];

			setLocation( countryName, continentName, countryCode );
		}else{//位置情報がCookieに保存されていないor初アクセス
			geoPosition.getCurrentPosition(function(_loc){//第一引数→位置情報が取得できた時のscript
				function _success(result){
					var countryCode   = result.countryCode;
					var countryName   = result.countryName.toLowerCase();
					var continentName = continentMap[result.countryCode];

					if(countryCode == 'US'){
						if(13 < _loc.coords.latitude && _loc.coords.latitude < 27 && -170 < _loc.coords.longitude && _loc.coords.longitude < -140){
							countryCode = 'US2';
							countryName = 'HAWAII, USA';
						}
					}
					if(countryCode == 'ES'){
						if(26 < _loc.coords.latitude && _loc.coords.latitude < 30 && -20 < _loc.coords.longitude && _loc.coords.longitude < -12){
							countryCode = 'ES2';
							countryName = 'CANARY ISLANDS, SPAIN';
						}
					}

					// save 60 minutes
					$.cookie('current_location', countryName+'/'+continentName+'/'+countryCode, {path:'/', expires:60});

					setLocation( countryName, continentName, countryCode);

					spinGlobe();
				}

				if( window.XMLHttpRequest ){
					var xhr=window.XDomainRequest?new XDomainRequest:new XMLHttpRequest;

					xhr.onload = function(){//ajax
					 	var result = (new Function("return " + xhr.responseText))();
						_success(result);
          };
          xhr.open("GET","http://api.geonames.org/countryCodeJSON?formatted=true&lat="+_loc.coords.latitude+'&lng='+_loc.coords.longitude+"&username=ssftokyo&style=full");
          xhr.send();
				}else{
					$.getJSON('http://api.geonames.org/countryCodeJSON', {
					  lat: _loc.coords.latitude,
					  lng: _loc.coords.longitude,
					  formatted:'true',
					  username:'ssftokyo',
					  style:'full'
					},function(data,textStatus){
					  if(textStatus == 'success'){
					    _success(data);
					  }
					});
				}
			});
		}
	}


	// show
	var localSec    = menuSec.filter('#local');
	var svgMap      = localSec.find('svg');
	var svgPaths    = svgMap.find('path');
	var regionLinks = localSec.find('.regions a');
	var siteLinks   = localSec.find('.region a');
	var localOutput = localSec.find('p');

	localOutput.html(' ');

	function setLocation(_country, _continent, _code){
		if(svgPaths.length > 0){
			var selectedPaths = svgPaths.filter(function(){
				if( $(this).attr('class').indexOf(_code) != -1 ) return this;
			});
			selectedPaths.attr('class', 'selected');

		}else{
			var mapHolder = header.find('.map-holder');
			var mapImage  = mapHolder.find('img').clone();

			var src = mapImage.attr('src').replace(/^(.*)(\.png)$/, '$1-'+_code.toLowerCase()+'$2');

			mapImage.attr('src', src).css({position:'absolute', top:0, left:0});
			mapHolder.append(mapImage);
		}

		var outerLinks = [];
		var continentIndex = 0;

		switch(_continent.toUpperCase()){
			case 'EUROPE':
				continentIndex = 0;
				outerLinks = window.regionList['europe']['children']; break;
			case 'ASIA':
				continentIndex = 1;
				outerLinks = window.regionList['asia']['children']; break;
			case 'AFRICA':
				continentIndex = 2;
				outerLinks = window.regionList['africa']['children']; break;
			case 'NORTH AMERICA':
				continentIndex = 3;
				outerLinks = window.regionList['north-america']['children']; break;
			case 'SOUTH AMERICA':
				continentIndex = 4;
				outerLinks = window.regionList['south-america']['children']; break;
			case 'OCEANIA':
				continentIndex = 5;
				outerLinks = window.regionList['oceania']['children']; break;
		}
		for(var i=0; i<outerLinks.length; i++){
			if(outerLinks[i].code == _code){
				localOutput.html( 'We think you are in:<strong><a class="localoutput" href="' + outerLinks[i]['href'] + '" target="_blank">' + _country + '</a> <sub>in</sub> ' + _continent + '<strong>' );
				break;
			}
		}

		localSec.find('.region').eq(continentIndex).addClass('show');

		if(narrow){
			localSec.find('.region').eq(continentIndex).find('.region-title').not('.show').trigger('click');
		}else{
			regionLinks.filter( function(){
				if( $(this).text().toLowerCase() === _continent.toLowerCase() ) return true;
			}).trigger('click');
		}

		siteLinks.filter( function(){
			if( $(this).attr('class') == _code ) return true;
		}).addClass('your-local');
	}

	// region accordion
	if(window.useResponsive){
		var regionsAccordion = localSec.find('.region');
		var regionsWrapper   = localSec.find('.global-sites');
		var localClose       = localSec.find('.intsp-btn-close');

		jQuery.windowSizeChanged(function(){
			regionsAccordion.each(function(i){
				regionsAccordion.eq(i).find('.region-title').removeClass('show');
				regionsAccordion.eq(i).stop().clearStyle();
			});
		}, function(){
			regionsAccordion.each(function(i){
				regionsAccordion.eq(i).find('.region-title').removeClass('show');

				if(Shared.ua.isiPhone){
					regionsWrapper.stop().clearStyle().css({height:46+34*regionsAccordion.size()});
					regionsAccordion.eq(i).stop().clearStyle().css({height:34, position:'absolute', top:0, left:0, overflow:'hidden'}).translate3d(0, 34*i);
				}else{
					regionsAccordion.eq(i).stop().clearStyle().css({height:34, overflow:'hidden'});
				}
			});
			if(Shared.ua.isiPhone){
				localClose.clearStyle().css({position:'absolute', top:108+34*regionsAccordion.size(), left:0}).translate3d(0, 0);
			}
		});

		regionsAccordion.find('ul').transformOrigin('50%', '0%');

		regionsAccordion.each(function(i){
			var that  = this;
			var wrap  = $(this);
			var title = wrap.find('.region-title');
			var list  = wrap.find('ul');
			var height = Math.ceil(list.find('li').size()/2) * 40 + 33;

			list.transformOrigin('50%', '0%');

			title.bind('click', function(e){

				document.body.scrollTop = 38;
				document.documentElement.scrollTop = 38;

				if(title.hasClass('show')){
					// close self
					if(Shared.ua.isiPhone){
						regionsWrapper.stop().animate({height:46+34*regionsAccordion.size()}, 400, 'easeOutQuad');
						list.transition('transform', 400, 'easeOutQuad').transform('scaleY(0)').fadeOut(500);

						regionsAccordion.each(function(j){
							regionsAccordion.eq(j).transition('transform', 400, 'easeOutQuad').translate3d(0, 34*j);
						});
						localClose.transition('transform', 400, 'easeOutQuad').translate3d(0, 0);
					}else{
						wrap.stop().animate({height:34}, 400, 'easeOutQuad');
					}
				}else{
					regionsAccordion.each(function(j){

						if(this == that){
							// open self
							if(Shared.ua.isiPhone){
								wrap.css({height:height});
								regionsWrapper.stop().animate({height:46+height+34*(regionsAccordion.size()-1)}, 400, 'easeOutQuad');
								list.transition('transform', 400, 'easeOutQuad').transform('scaleY(1)').fadeIn(500);
								list.css({height:height});
							}else{
								wrap.stop().animate({height:height}, 400, 'easeOutQuad');
							}
						}else{
							// close other
							var _title = $(this).find('.region-title');

							if(_title.hasClass('show')){
								_title.removeClass('show');

								if(Shared.ua.isiPhone){
									$(this).css({height:34});
									$(this).find('ul').transition('transform', 400, 'easeOutQuad').transform('scaleY(0)');
								}else{
									$(this).stop().animate({height:34}, 400, 'easeOutQuad');
								}
							}
						}
						if(Shared.ua.isiPhone){
							if(j > i){
								regionsAccordion.eq(j).transition('transform', 400, 'easeOutQuad').translate3d(0, height+34*(j-1));
							}else{
								regionsAccordion.eq(j).transition('transform', 400, 'easeOutQuad').translate3d(0, 34*j);
							}
							localClose.transition('transform', 400, 'easeOutQuad').translate3d(0, height-34);
						}
					});
				}
				title.toggleClass('show');
			});
		});
	}



	/*****************************************************************************************************************
	 * FOOTER
	 *****************************************************************************************************************/



	if(window.useResponsive){
		var footer = $('#globalfooter');

		// デザイン調整
		jQuery.windowSizeChanged(function(){
			footer.find('.sec-nav ul').removeClass('show').clearStyle().find('li').clearStyle();

		}, function(){
			footer.find('.sec-nav ul').removeClass('show').each(function(i){
				$(this).clearStyle().css({height:44, overflow:'hidden'}).find('li').clearStyle().each(function(j){
					if(j == 0 || j == 1){
						;
					}else if(j == 2){
						$(this).css({opacity:0}).matrix(1, 0, 0, 1, 10);
					}else{
						$(this).css({opacity:0}).matrix(1, 0, 0, 1, 10);
					}
				});
			});
		});

		// 開閉
		footer.find('.sec-nav ul').each(function(){
			var ul = $(this);
			var li = ul.find('li');

			li.eq(1).bind('click', function(e){
				if(narrow){
					e.preventDefault();

					if(!ul.hasClass('show')){
						ul.css({height:'auto'});
						var h = ul.height();

						li.each(function(i){
							if(i > 1) $(this).transition('all', 400, 'easeInOutCubic', i*25).css({opacity:1}).matrix(1, 0, 0, 1);
						});
						if(Shared.ua.isiPhone || !Shared.ua.isMobile){
							ul.stop().css({height:44}).animate({height:h}, 400, 'easeOutQuad');
						}else{
							ul.stop().css({height:'auto'});
						}
					}else{
						li.each(function(i){
							if(i > 1) $(this).transition('all', 300, 'easeInOutCubic', (li.length-i)*20).css({opacity:0}).matrix(1, 0, 0, 1, 10);
						});
						if(Shared.ua.isiPhone || !Shared.ua.isMobile){
							ul.stop().animate({height:44}, 300, 'easeOutQuad');
						}else{
							ul.stop().css({height:44});
						}
					}
					ul.toggleClass('show');
				}
			});
		});
	}





	/*****************************************************************************************************************
	 * SEARCH
	 *****************************************************************************************************************/

	var searchForm = $('#globalheader #nav_search_form');
	var searchTerm = $('#globalheader #nav_search_term');

	if(Shared.ua.isIElt9){
		searchForm.attr('method', 'post').find('input').removeAttr('disabled');
	}

	searchForm.submit(function(e){
		var val = jQuery.trim(searchTerm.val());

		if(Shared.ua.isIElt9){
			if(val != ''){
				searchForm.find('input').attr('disabled', 'disabled');
				searchForm.attr('action', '/search/');
				$.cookie('search_hash', val, {path:'/', expires:1});
			}else{
				return false;
			}
		}else{
			e.preventDefault();

			if(val != ''){

				if(val.indexOf('　') != -1){
				  val = val.split('　').join(' ');
				}
				if(val.indexOf('+') != -1){
				  val = val.split('+').join(' ');
				}

				location.href = '/search/?q='+val;
			}
		}
	});

	searchTerm.bind('focus', function(){
		searchTerm.addClass('hastext');
	});

	searchTerm.bind('blur', function(){
		var val = jQuery.trim(searchTerm.val());

		if(val !== ''){
			searchTerm.addClass('hastext');
		}else{
			searchTerm.val('');
			searchTerm.removeClass('hastext');
		}
	});

	searchTerm.bind('change', function(){
		if(searchTerm.val() !== ''){
			searchTerm.addClass('hastext');
		}else{
			searchTerm.removeClass('hastext');
		}
	});

})(jQuery);
