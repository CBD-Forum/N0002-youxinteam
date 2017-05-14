		var windowwidth, windowheight;
        var weuiActionsheet, mask;
        var filterurl,setreportfilterurl;
        
        function initFilterPanel(filterValueURL,setFilterURL){
        	windowwidth = $(window).width();
            windowheight = $(window).height();
            filterurl = filterValueURL;
            setreportfilterurl = setFilterURL;
            $(".filterpanel").css({ "width": windowwidth, "height": windowheight });
           
            $(".filter").click(function () {
                animatFilterPanel("show");  
            }); 
            filtertoolClick();
            filterItemClick();
        }
        
        function setMobileReportFilter( selectedvalues){
        	selectedvalues = window.encodeURIComponent(selectedvalues);
        	$.ajax({ 
           				type: "post", 
           				async: false,
                                        url:setreportfilterurl+"&selectedValues="+selectedvalues,
                                        dataType: "json", 
                                        success: function (data) { 
														            	loadReportBlock(true);
										            
                                        }, 
                                        error: function (XMLHttpRequest, textStatus, errorThrown) { 
                                                alert("设置过滤条件失败,原因:"+errorThrown); 
                                        } 
                                });
        }
        
        function setReportFilter(){
        	var selectedvalues="";
        	 $("div.filteritem").find(".filtervalue").each(function(index,element){
        	 	
        	 	var v1 ;
        	 	if( $(this).is("input") )
        	 		v1 = $(this).val();
        	 	else
        	 		v1 = $(this).text();
        	 	
        	 	
        	 	if (selectedvalues =="")
        	 		selectedvalues = v1;
			    else
			    	selectedvalues = selectedvalues+"|"+v1;
			    	
			    if( index==0){
			    	var a = v1;
			    	if( a.Length>11 )
			    		a = a.substr(0,11)+'...'
			    	
			    	$("#mobileHeaderFilterValue").text(a);
			    	
			    }
			    	
			  });
			  
        	 setMobileReportFilter(selectedvalues);
        	 
        }
        
        function createFilterValueUI(filterItem,filter,ismulti){
        	
        
        	$("#multifiltervaluecontent").empty();
        	$("#singlefiltervaluecontent").empty();
        	
        	if(ismulti){
	        	for( var i=0;i<filter.length;i++)
		 		{
		 	 		var v = filter[i].Value;

		 	 		 var html = "<label class='weui_cell weui_check_label'>\
	                            <div class='weui_cell_hd'>\
	                                <input type='checkbox' name='checkbox1' class='weui_check'>\
	                                <i class='weui_icon_checked'></i>\
	                            </div>\
	                            <div class='weui_cell_bd weui_cell_primary'>\
	                                <span>"+v+"</span>\
	                            </div>\
	                        </label>";
	               
		 	 	    $("#multifiltervaluecontent").append(html);
		 	 	   
		 		}
		 		registerCheckBoxClick(filterItem);
		 	}
        	else
        	{
        		for( var i=0;i<filter.length;i++)
		 		{
		 	 		var v = filter[i].Value;
		 	 	       
	             	var html = "<label class='weui_cell weui_check_label' for='x"+i+"'>\
	                            <div class='weui_cell_ft' style='width:25px;text-align:left'>\
	                                <input type='radio' class='weui_check' name='radio1' id='x"+i+"'>\
	                                <span class='weui_icon_checked'></span>\
	                            </div>\
	                            <div class='weui_cell_bd weui_cell_primary'>\
	                                <p>"+v+"</p>\
	                            </div>\
	                        </label>";
	                
		 	 	   $("#singlefiltervaluecontent").append(html);
		 		}
		 		registerRadioClick(filterItem);
        	}
        }
        
        function createFilterValueList(filterurl,filterItem,filterIndex,isMulti){
        				
                		$.ajax({ 
           				type: "post", 
                                        url:filterurl+"&Index="+filterIndex,
                                        dataType: "json", 
                                        success: function (data) { 
						                                        	if (data.length > 0) 
														            {
														            	createFilterValueUI(filterItem,data,isMulti);
														            }
														            
                                        }, 
                                        error: function (XMLHttpRequest, textStatus, errorThrown) { 
                                                alert("获取过滤值失败,原因:"+errorThrown); 
                                        } 
                                });
         }
        
        var animatFilterPanel = function (operate) {
            if (operate == "show") { 
                $(".filterpanel").removeClass("animathidefilter").addClass("animatshowfilter").show();
            }
            else {
                $(".filterpanel").removeClass("animatshowfilter").addClass("animathidefilter");
                setTimeout(function () { $(".filterpanel").hide(); }, 300);
            }
        }
        var filterItemClick = function () {
            $(".filteritemcontent").find(".weui_cell").on("click", function () {
                var $this = $(this);
                var celltype = $this.data("type");
                if (celltype != "date") {
	                var filterItemName = $this.find("div.filteritem").find(".filtername").text();
	                var filterSValue = $this.find("div.filteritem").find(".filtervalue").text();
	                var isMulti = $this.find("div.filteritem").find(".filtername").data("ismulti");
	                var index = $(this).index();
	               
	                createFilterValueList(filterurl,$this,index,isMulti);
	                animatFilterValue($this, filterItemName, filterSValue, $(this).index(), isMulti);
                }
            });
        }
        var filtertoolClick = function () {
            $(".filtertool").find(".btool").on("click", function () {
                var state = $(this).data("state");
                if (state == "confirm") {
                    //需要调用数据过滤接口后刷新数据
                   
                    setReportFilter();
                    animatFilterPanel("hide");
                }
                else if (state == "cancel") {
                    animatFilterPanel("hide");
                }
            });
        }
        function registerCheckBoxClick(filterItem){
        	$(".weui_cells_checkbox .weui_cell_hd").click(function () { 
                    var $this = $(this); 
                    var isChecked = $this.find(".weui_check").is(':checked');
                    var value = $this.siblings(".weui_cell_bd").find("span").text();
                  
                    var sValue = filterItem.find("div.filteritem").find(".filtervalue").text();
                    if (sValue.length > 0) {
                        var sValues = sValue.split(';')
                        var isExist = false;
                        for (var i = 0; i < sValues.length; i++) {
                            if (sValues[i] == value) {
                                isExist = true;
                                if (!isChecked) {
                                    sValues.splice(i, 1);
                                    i--;
                                }
                            }
                        }
                        if (!isExist && isChecked)
                            sValues.push(value);
                        var fvalues = sValues.join(";");
                        filterItem.find("div.filteritem").find(".filtervalue").text(fvalues);
                    }
                    else {
                        filterItem.find("div.filteritem").find(".filtervalue").text(value);
                    }
                  
                });
        }
        
        function registerRadioClick(filterItem){
        	 $(".weui_cells_radio .weui_cell_bd").click(function () {
                    var $this = $(this);
                    var svalue = $this.find("p").text();
                  
                    filterItem.find("div.filteritem").find(".filtervalue").text(svalue);
                    hideActionSheet(weuiActionsheet, mask);
                });
        }
       
        var animatFilterValue = function (filterItem, filterItemName, filterSValue, filterIndex, isMulti) {
            if (isMulti == true) {
                $(".weui_actionsheet_menu .weui_cells_checkbox").css("display", "block");
                $(".weui_actionsheet_menu .weui_cells_radio").css("display", "none");
                $(".filtervaluetool .confirm").css("display", "block");
                if (filterSValue.length > 0)
                {
                    $(".weui_cells_checkbox .weui_cell_hd").each(function () {
                        var $this = $(this);
                        var itemValue = $this.siblings(".weui_cell_bd").find("span").text();
                        var filterSValues = filterSValue.split(";");
                        for (var i = 0; i < filterSValues.length; i++) {
                            if (itemValue == filterSValues[i])
                            {
                                $this.find(".weui_check").attr("checked",true);
                            }
                        }
                    });
                }
               
                registerCheckBoxClick(filterItem);
            }
            else {
                $(".weui_actionsheet_menu .weui_cells_checkbox").css("display", "none");
                $(".filtervaluetool .confirm").css("display", "none");
                $(".weui_actionsheet_menu .weui_cells_radio").css("display", "block");
                if (filterSValue.length > 0) {
                    $(".weui_cells_radio .weui_cell_bd").each(function () {
                        var $this = $(this);
                        var svalue = $this.find("p").text();
                        if (filterSValue == svalue) {
                            $this.siblings(".weui_cell_ft").find(".weui_check").attr("checked", "checked");
                        }
                    });
                } 
               registerRadioClick(filterItem);
            }
            showActionSheet();
        }
        //从屏幕底端打开过滤值框
        var showActionSheet = function () {
            var windowheight = $(window).height();
            mask = $('#mask');
            weuiActionsheet = $('#weui_actionsheet');
            weuiActionsheet.css("height", windowheight * 0.5).addClass('weui_actionsheet_toggle');
            mask.show().addClass('weui_fade_toggle').click(function () {
                hideActionSheet(weuiActionsheet, mask);
            });
            weuiActionsheet.find(".tool").click(function () {
                var state = $(this).data("state");
                hideActionSheet(weuiActionsheet, mask);
            });
            weuiActionsheet.unbind('transitionend').unbind('webkitTransitionEnd');
        }
        //隐藏过滤值框
        var hideActionSheet = function (weuiActionsheet, mask) {
            weuiActionsheet.removeClass('weui_actionsheet_toggle');
            mask.removeClass('weui_fade_toggle');
            weuiActionsheet.on('transitionend', function () {
                mask.hide();
            }).on('webkitTransitionEnd', function () {
                mask.hide();
            })
        }