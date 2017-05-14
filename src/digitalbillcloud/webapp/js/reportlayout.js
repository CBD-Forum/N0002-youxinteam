
function loadReportMenu(reportId){
	
	$.ajax({ 
           			type: "get",
			    	dataType: "json",
			    	cache: false,
			    	async: true,
			    	url: getWebApiURI()+"/GetH5MenuItemList/?reportId="+reportId+"&token="+getUserToken(),               
                	success: function (data) { 
                							createMenu(data);
                                        }, 
                                        error: function (XMLHttpRequest, textStatus, errorThrown) { 
                                                alert(errorThrown); 
                                        } 
            });					
}

function fireToolbarCommand()
{
	var reportId = $(document.body).data("reportid");
	
	var id = $(this).attr("id");
	if(id=="ReportFilter"){
	 	loadReportFilter(reportId);
	}
	if(id=="Close"){
		closeReport(reportId);
	}
}


function closeReport(reportId){
	$.ajax({ 
           			type: "get",
			    	dataType: "json",
			    	cache: false,
			    	async: true,
			    	url: getWebApiURI()+"/CloseReportSession/?userId="+getUserId()+"&reportId="+reportId,               
                	success: function (data) { 
                							reportId = reportId.replace(new RegExp("-","gm"),"_");  
											var m = new Array();
											m[0] = "CloseDocument";
											m[1] = reportId;
											
											window.parent.postMessage(m,'*');
                                        }, 
                                        error: function (XMLHttpRequest, textStatus, errorThrown) { 
                                                alert(errorThrown); 
                                        } 
     });		
            
    
}


function createMenu(data){
	/*
	$("#menu").kendoMenu( 
       {
           //select: onSelect,
          // open: onOpen
       }
  );
   var menu = $("#menu").data("kendoMenu");
   
   if( data.MoreMenuItem.text!=null)
		menu.append(data.MoreMenuItem);
	*/
	
	var h,id;
	for (var i=0;i<data.Items.length;i++) {
		id = data.Items[i].Id;
		h = "<button id='"+id+"' class='reportToolbarButton'>"+data.Items[i].text+"</button>";
             	
		$(h).appendTo($("#reportMainMenu"));
		   	
		$("#"+id).on('click', fireToolbarCommand);
		   		
		$("#"+id).kendoButton();
	} 
	
	//alert("aaa");
   //$("#Export").kendoButton();
   //$("#Print").kendoButton();
}

function onOpen(e) {
	
    var t = $(e.item).children().children(".k-menuitem").attr("id");
    if(t=="filter")
    	$("#reportFilter").show();
    	
    
    /*
    $(e.item).children("ul").children("li").remove() 
    $(e.item).children("div").children("ul").children("li").remove()
 
   
    var menu = $("#menu").data("kendoMenu");
   
   
   
    $("#main").children("div").each(
       function(){
       	 	var reportId = $(this).attr("id");
       	 	var reportName = $(this).data("reportname");;
       	 	reportName = "<span id='"+reportId+"' class='k-menuitem-report'>"+reportName+"</span>";
       	 	menu.append({ text: reportName, encoded: false}, e.item);
       });
       */
}

function onSelect(e) {
  
   var t = $(e.item).children().children(".k-menuitem").attr("id");
    if(t=="filter")
    	$("#reportFilter").show();
    	
  	/*
     var t = $(e.item).children().children(".k-menuitem-report").attr("id");
  	 var opened = false;
  	  $("#main").children("div").each(
       function(){
       		var reportId = $(this).attr("id");
       		if(reportId==t){
       			opened = true
       			$(this).show();
       		}
       			
       		else
       			$(this).hide();
       });
       
      if( opened)
        	return;
        				
      
		$.get('S_'+t+'.htm',function(data,status){
			
			$(data).appendTo($("#main"));
			window['loadReport'+t](); 
			
	});
	*/
}


function onResizeRegister(){

   // $("#reportMenu").show();
    $("#main").show();
    $("#loading").hide();


    $(window).resize( function(){	
      //  updateLayout();
    });
}
function updateSingleLayout(){
	
    var cheight = $(window).height(); 
    var cwidth = $(window).width();      
  
    var h1 = $("#reportTop").height();
    var d = $('#reportTop').css('display');
    if (d=="none")
    	h1 = 0;
	
    $("div[data-block*='block']").each(
       function(){
       	
       	   var block = $(this).data("block");
       	   cheight  =cheight-h1-5;
       	   
       	  
       	   if( block =="blockTable") //分页控件，如果不分页会加上36
       			cheight = cheight-36;
       			
       	 
           $(this).width(cwidth-2);
           $(this).height(cheight);
       
           $(this).css("marginLeft","0px");
           $(this).css("marginTop","0px");
          
        
           if( block =="blockChart"){
               var chart = $(this).data("kendoChart");
               if(chart!=null)
                   chart.redraw();
           }
    
       });

   	
       $("#main").height(cheight);
}

function updateChartTableLayout(){
    $("#main").show();
    var owidth = $(document.body).data("width");
    var cheight = $(window).height()-42;
    var cwidth = $(window).width();
    var rate = cwidth/owidth;
    var maxHeight = 0;
    var width,height;
    var maxTopHeight = 0;

    $("div[data-block*='block']").each(
       function(){
       		if( block =="blockTable")
       			return;
       	
           
            var left = $(this).data("left")*rate;
            var top = $(this).data("top")*rate;
            var block = $(this).data("block");
            var resize = $(this).data("resize");
        
       
           	width = $(this).data("width")*rate ;
 			height = $(this).data("height")*rate;
           	 	
           	//if( height>maxHeight)
           		//maxHeight = height;
          
           if( top+height >cheight)
               cheight = top+height;
               
           if( block =="blockChart" ){
				var showtitle = $(this).data("showtitle");
				if( showtitle ==true)
					height = height-35;
		   }

           $(this).width(width);
           $(this).height(height);
           $(this).css("marginLeft",left+"px");
           $(this).css("marginTop",top+"px");
          
           if(maxTopHeight<top+height)
           		maxTopHeight = top+height+10;

           if( block =="blockChart"){
               var chart = $(this).data("kendoChart");
               if(chart!=null)
                   chart.redraw();
           }
       });
      
       $("#topBlocks").height(maxTopHeight);
       $("#main").height(cheight);
       
       $("div[data-block='blockTable']").each(
       function(){
       		 
       		var left = $(this).data("left")*rate;
           
           	var h1 = $("#reportTop").height();
           	var h2 = $("#topBlocks").height();
           	
       		height = cheight- h1-h2;
       		
       		$(this).width(cwidth-2);
           	$(this).height(height);

          
          
           	$(this).css("marginLeft","0px");
           	$(this).css("marginTop","0px");
           
       });
       
}


function updateLayout(){
   
    var owidth = $(document.body).data("width");
    var cheight = $(window).height();
    var cwidth = $(window).width()-50;       
    var rate = cwidth/owidth;
   // alert($(window).width());
   // alert($(window).height());

	//alert($("div[data-block*='block']").length);

    $("div[data-block*='block']").each(
       function(){
            
           
            var width,height;
            var left = $(this).data("left")*rate;
            var top = $(this).data("top")*rate;
            var block = $(this).data("block");
            var resize = $(this).data("resize");
           
           
           	width = $(this).data("width")*rate ;
           	height = $(this).data("height")*rate;
           	 
          
           if( top+height >cheight)
               cheight = top+height;
              
           
           if( block =="blockChart" ){
				var showtitle = $(this).data("showtitle");
				
				if( showtitle ==true)
					height = height-35;
		   }
		  
          
          /*
		   if( block =="blockChart"){
					top=15;
					left = 15;
					width = width-30;
					height = height-30-25;
		   }*/
		  			
           $(this).width(width);
           $(this).height(height);
           $(this).css("marginLeft",left+"px");
           $(this).css("marginTop",top+"px");
          
         
           if( block =="blockChart"){
               var chart = $(this).data("kendoChart");
               if(chart!=null)
                   chart.redraw();
           }
    
       });

   	
       $("#main").height(cheight);
}