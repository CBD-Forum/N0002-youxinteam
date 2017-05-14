function updateMapChartLayout(){

	var owidth = $(document.body).data("width");
    var cheight = $(window).height();
	var cwidth = $(window).width();
    var rate = cwidth/owidth;
    
    var top = $(".b-map").data("top")*rate;
    var left = $(".b-map").data("left")*rate
    var width = $(".b-map").data("width")*rate;
    var height = $(".b-map").data("height")*rate-5;
    var offset = left+width;
    
    $(".b-map").css("marginLeft",left+"px");
    $(".b-map").css("marginTop",top+"px");
	$(".b-map").width(width);
	$(".b-map").height(height);


	$("#rightRegion").width(cwidth-width-20);

	$(".b-map-block").css("marginLeft",left+"px");
    $(".b-map-block").css("marginTop",top+"px");
	$(".b-map-block").width(width);
	$(".b-map-block").height(height);

	

	$("#rightRegion div[data-block*='block']").each(
   		function(){
   			 	var left = $(this).data("left")*rate;
            	var top = $(this).data("top")*rate;
            	var block = $(this).data("block");
            	var width = $(this).data("width")*rate;
            	var height = $(this).data("height")*rate;
            	var resize = $(this).data("resize");
            	
        		if( block =="block"){
        			left = left-offset;
        		}
        		//alert(top+"---"+height);
        		$(this).css("marginLeft",left+"px");
    			$(this).css("marginTop",top+"px");
				$(this).width(width);
				$(this).height(height);
     });
}