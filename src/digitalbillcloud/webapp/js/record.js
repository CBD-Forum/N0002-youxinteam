var arrecord, isBack;
var isfirst = false;
$(document).ready(function () { 
	var jsonString = window.localStorage.getItem("drillPath")
    arrecord = JSON.parse(jsonString);
    $.scroll(function () {
        var $toomenu = $(".bottomtoolmenu");
        if ($toomenu.css("display") == "none" && !isfirst)
        		$toomenu.show(); 
    },
          function () {
              var $toomenu = $(".bottomtoolmenu");   
              $toomenu.hide();   
          });
    isBack = getParaString("isback");
    if (isBack == "true") {
        $(".bottomtoolmenu .bottombackitem").eq(0).bindSkip();
        return;
    }
    else {

        var pkey = getSupUrl().toLowerCase().indexOf("login") >= 0 || getSupUrl().toLowerCase().indexOf("openh5report") >= 0 ? "" : getSupUrl();
        if (arrecord == undefined)
            arrecord = new Array();
        var key = getCurrentUrl();
        var title = document.title;
        var cpage = getItem(arrecord, key);
        if (cpage == undefined) {
            cpage = { url: "", title: "", purl: "" };
            cpage.url = key;
            cpage.title = title;
            cpage.purl = pkey;
            arrecord.push(cpage);
        }
        else {
        	
            cpage.url = key;
            cpage.title = title;
            cpage.purl = pkey;
        }
        window.localStorage.setItem("drillPath",JSON.stringify(arrecord));
        
        $(".bottomtoolmenu .bottombackitem").eq(0).bindSkip();
    } 
});
$.fn.extend({
    bindSkip: function () { 
        var key = getCurrentUrl();
        var pkey = getItem(arrecord, key).purl;
        if (pkey == "") isfirst = true;
        if (arrecord != undefined) {
            if (isfirst == false) {
                if ($(".background").length < 1) {
                    $('body').append("<div class=\"background\"></div>");
                }
                $(".backreportlist").remove();
                var menuhtml = "<div class=\"backreportlist\"><div class=\"weui_cells weui_cells_access\" style=\"margin-top:0px;font-size:12px\"><a class=\"weui_cell describe\" href=\"#\"><div class=\"weui_cell_bd weui_cell_primary\" style=\"color:#F4EDEC\"><p>钻取路径 </p></div></a>";
                while (pkey != undefined && pkey != "") {
                    var ppage = getItem(arrecord, pkey);
                    if( ppage==null){
                    	break;
                    }
                    var para = "&isback=true";
                    if (ppage.url.indexOf("?") < 0)
                        para = "?isback=true";
                    menuhtml += "<a class=\"weui_cell\" href=\"" + ppage.url + para + "\"><div class=\"weui_cell_bd weui_cell_primary\"><p>" + ppage.title + "</p></div><div class=\"weui_cell_ft\"> </div></a>";
                    pkey = getItem(arrecord, pkey).purl;
                }
                menuhtml += "</div></div> ";
                $('body').append(menuhtml);
            }
     
			if (isfirst) { 
				if ($(".tipcontent").length < 1)
					$(".bottomtoolmenu").css("display","none");
			}
        }
        //点击打开报表菜单
        this.click(function (event) { 
            var windowwidth = $(window).width();
            var windowheight = $(window).height();
            $(".background").css("height", windowheight).show(); 
            $(".backreportlist").removeClass("animathideMenu").addClass("animatshowMenu").show();
        });
        //点击任何地方关闭报表菜单
        $(".background").click(function () {
            $(".background").css("display", "none");
            $(".backreportlist").removeClass("animatshowMenu").addClass("animathideMenu");
            setTimeout(function () { $(".backreportlist").hide(); }, 300);
        });
    }
});
$.extend({
    scroll: function (topfun, botfun) {
        var sy = 0;
        document.addEventListener("touchstart", function (e) {
            sy = $(document).scrollTop();
        });
        document.addEventListener("touchend", function (e) {
            var ey = $(document).scrollTop();
            //alert(sy + "-" + ey);
            if (ey < 15) {
                if (topfun)
                    topfun();
            }
            else {
                var shiftY = ey - sy; 
                if (shiftY > 40) {
                    if (botfun)
                        botfun();
                }
                else if (shiftY < -40) {
                    if (topfun)
                        topfun();
                }
            } 
        }, false);
    }
});
var getItem = function (arrecord, key) {
    for (var i = 0; i < arrecord.length; i++) {
        if (arrecord[i].url == key) {
            return arrecord[i];
        }
    }
}
var getParaString = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
var getCurrentUrl = function () {
	
   return location.href.replace("?isback=true", "").replace("&isback=true", "");
}
var getSupUrl = function () {
    return document.referrer.replace("?isback=true", "").replace("&isback=true", "");
}