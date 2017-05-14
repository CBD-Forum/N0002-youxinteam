var firstFilter = true;

function createReportFilter(fis,schemas) {
		//$('#logo').on('click', function() {
		/*
		layer.tips("<div id='filterContainer' style='margin:5px;background:white;width:390px;height:220px'></div>", '#logo', {
			tips: 3,
			time: 0,
			closeBtn: 0,
			shade: [0.6, '#393D49'],
			shadeClose: true, //开启遮罩关闭
			area: ['420px', '240px'] //宽高
		});
		*/
		
		var w = $(this).height();
		var len = fis.length;
		if( len==0){
			$("#ReportFilter").hide();
			loadReport();
			return;
		}
		
		var index = layer.open({
								  type: 1,
								  shift:5,
								  title:" ",
								  shade: [0.6, '#393D49'],
								  shadeClose: true, //开启遮罩关闭
								  area: ['412px', w+'px'], //宽高
								  content: "<div id='filterContainer' style='margin-top:5px;margin-left:5px;width:400px;background:white;height:220px'></div>",
								  offset: 'rb'
								});


		
		for(var i = 0; i < len; i++) {
			var fi = fis[i];
			if( fi.IsDateDimension && (fi.DateType=="month" || fi.DateType=="date"))
				createDate(fi.Id,fi.Name,fi.DefaultValue,fi.DateFormat,fi.DateStart);
			else
				createSingleSelect(fi.Id, fi.Name, fi.DefaultValue, fi.Values);
		}
		
		

		var h = "<div class='footer'>";
		h = h + "<button id='saveFilter' class='saveFilterButton'>存为方案</button>";
		h = h + "<button id='reportFilterButton' class='filterButton'>过滤</button>";
		h = h + "</div>";
		$(h).appendTo($("#filterContainer"));
		
		$('#reportFilterButton').kendoButton();
		$('#saveFilter').kendoButton();
		
		loadLastRuntimFilterValue();
		
		$('#reportFilterButton').on('click',function(){
			 
			  saveFilterLocalStorage();
			  
			  
			  loadReport();
			  layer.close(index);
		});


		$('#saveFilter').on('click',function(){
			  saveFilterSchema();
		});
		
		
		
		h = "<div>";
	
		h = h + "<div style='height: 100px; border-top: 1px  lightgray dashed; margin:5px 10px;'>";
		h = h + "<span style='margin-left: 1px;margin-top: 10px; display: block;font-size:1em;'>我的过滤方案</span>";
		h = h + "<ul id='filterSchemaList'>";
		
		for(var x=0;x<schemas.length;x++){
			
			var y = JSON.stringify(schemas[x]); 
			
			h = h + "<li class='b-li-d' data-json='"+y+"' ><a>"+schemas[x].Name+"</a></li>";
			
			
		}
		
		
		
		//h = h + "<li class='b-li-c'>上月订单</li>";
		//h = h + "<li class='b-li-c'>近三月订单</li>";
		h = h + "</ul>";
		h = h + "</div>";
		h = h + "</div>";
		
		$(h).appendTo($("#filterContainer"));

	//});
	
		$('#filterSchemaList li a').on('click', function(event){
		 	var target = event.target;
	    	//alert(event.target.innerHTML);
	    	var json = $(target).parent().data("json");
	    	loadFilterSchema(json);
		    $("#reportFilterButton").trigger("click");
		});


		$ ("#filterSchemaList li").hover(function(){
	        	$(this).addClass('hover');
	       		var h= "<span id='schemaAction' style='float:right;margin-right:5px;margin-top:0px'><img src='../images/default.png' style='margin-right:20px'><img id='imgRemoveFilter' src='../images/remove.png' style='margin-right:10px'></span>";
	       		$(h).appendTo($(this));
	       		
	       		$('#imgRemoveFilter').on('click',function(event){
					 removeFilter(event);
				});
		
	       		
	  		},function(){
	       		$(this).removeClass('hover');
	        	$('#schemaAction').remove();
	  	});
	  	
	  	if( firstFilter){
	  		$("#reportFilterButton").trigger("click");
	  		firstFilter = false;
	  	}
	  		
}



function loadLastRuntimFilterValue(){
	var x = getLocalStorage();
	loadFilterSchema(x);
}

function loadFilterSchema(x){
	var len = x.Items.length;
	for (var i=0;i<len;i++) {
		var item = x.Items[i];
		var k = item.Key;
		var v = item.Value1;
		
		$("select[data-caption='"+k+"']").each(
       	function(){
       			var id = $(this).attr("id")
       			var combobox = $("#"+id).data("kendoComboBox");
       			combobox.value(v);
       });
      
	}
}

function saveFilterLocalStorage(){
	
	var json = JSON.stringify(getRuntimeReportFilter());
	  
	var reportId = $(document.body).data("reportid");
	var key = "FV_"+reportId;
	window.localStorage.setItem(key,json);
}

function getLocalStorage(){
   var reportId = $(document.body).data("reportid");
   var key = "FV_"+reportId;
   var x = window.localStorage.getItem(key);
   if(x==undefined|| x==null)
   		return new runtimeReportFilter("empty");
   
   var fx = JSON.parse(x);
   var dd = getDrilldownLocalStorage();
   for (var i=0;i<fx.Items.length;i++) {
   		if( dd[i]!=undefined && dd[i]!=null)
   			fx.Items[i].Value1 = dd[i];
   	
   }
   
   return fx;
}

function getDrilldownLocalStorage(){
   
   	var reportId = $(document.body).data("reportid");
   	var key = "Drilldown_"+reportId;
   	var x = window.localStorage.getItem(key);
   	if(x==undefined|| x==null)
   		return new Array(5);
   
   	var fx = JSON.parse(x);

	window.localStorage.removeItem(key);
	
   	return fx;
}

function saveDrilldownLocalStorage(rid,f1,f2,f3,f4,f5){
   	/*
   	var fx;
   	var key = "Drilldown_"+rid;
   	var x = window.localStorage.getItem(key);
   	if(x==undefined|| x==null)
   		fx =  new Array(5);
   	else
   		fx = JSON.parse(x);
   	*/
   	var key = "Drilldown_"+rid;
   	var fx = new Array(5);
   	fx[0] = f1;
   	fx[1] = f2;
   	fx[2] = f3;
   	fx[3] = f4;
   	fx[4] = f5;
   	
   
   	var json = JSON.stringify(fx);
   	window.localStorage.setItem(key,json);
   	return fx;
}

function getFilterValue(c){
	
	
	var fx = getLocalStorage();
	
	for (var i=0;i<fx.Items.length;i++) {
   		if( fx.Items[i].Key==c)
   			return fx.Items[i].Value1;
   	
    }
		       
	return c;
}

function drilldownReport(r,x,i,f1,f2,f3,f4,f5){
	
	var fv1,fv2,fv3,fv4,fv5;
	if( f1!="")
		fv1 = getFilterValue(f1);
	if( f2!="")
		fv2 = getFilterValue(f2);
	if( f3!="")
		fv3 = getFilterValue(f3);
	if( f4!="")
		fv4 = getFilterValue(f4);
	if( f5!="")
		fv5 = getFilterValue(f5);

	if(i==1)
		fv1 = x;
	if(i==2)
		fv3 = x;
	if(i==3)
		fv3 = x;
	if(i==4)
		fv4 = x
	if(i==5)
		fv5 = x;
		
	saveDrilldownLocalStorage(r,fv1,fv2,fv3,fv4,fv5);

	postCloseDocumentMessage(r);
	
	
	
	$.ajax({
			type: "get",
			cache: false,
			async: true,
			url: getWebApiURI() + "/GetReportName/?token=" + getUserToken() + "&reportId="+r,
			success: function(data) {	
				postOpenDocumentMessage(r,data);
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			alert(errorThrown);
		}
	});
	
	
	
}



function removeFilter(event){
	var target = event.target;
	var par = $(event.target).parent().parent();
	par.remove();
	
	var reportId = $(document.body).data("reportid");
	var postData = JSON.stringify(getFilterSchema(false));
		 	
	$.ajax({
			type: "post",
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: true,
			url: getWebApiURI() + "/SaveFilterSchema/?reportId=" + reportId + "&token="+getUserToken(),
			data:postData,
			success: function(data) {			
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			alert(errorThrown);
		}
	});
}

function saveFilterSchema(){
			var fsLayer;
			fsLayer = layer.tips("<div id='filterSchema' style='margin-top:10px;width:290px;height:50px;'><input placeholder='方案名称' class='k-textbox' id='txtSchemaName' style='margin-left: 5px;line-height: 32px;height:32px; width: 190px;'/><button id='btnSaveSchema' class='k-button' style='width: 60px;height: 32px; margin-left: 10px;'>保存</button></div>", '#saveFilter', {
				tips: 3,
				time: 0,
				closeBtn: 0,
				shade: [0.1, '#393D49'],
				shadeClose: true, //开启遮罩关闭
				area: ['290px', '60px'] //宽高
			});
		
			
			var reportId = $(document.body).data("reportid");
			
			$('#btnSaveSchema').on('click', function(event){
			var postData = JSON.stringify(getFilterSchema(true));
		 	
			$.ajax({
						type: "post",
						dataType: "json",
						contentType: "application/json; charset=utf-8",
						cache: false,
						async: true,
						url: getWebApiURI() + "/SaveFilterSchema/?reportId=" + reportId + "&token="+getUserToken(),
						data:postData,
						success: function(data) {
							layer.close(fsLayer);
			},
			error: function(XMLHttpRequest, textStatus, errorThrown) {
			alert(errorThrown);
			}
		});
	
	});
}

function loadReportFilter(reportId) {

	$.ajax({
		type: "get",
		dataType: "json",
		cache: false,
		async: true,
		url: getWebApiURI() + "/GetReportFilter/?reportId=" + reportId + "&token="+getUserToken(),
		success: function(data) {
			if(data.Message != "") {
				return;
			}
			createReportFilter(data.Items,data.FilterSchemas);
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			alert(errorThrown);
		}
	});

}

function createSingleSelect(id, caption, defaultValue, values) {
	h = "<div class='b-fi' >";
	h = h + "<span class='b-fi-n'>" + caption + "</span>";
	h = h + "<div class='b-fi-v'>";
	h = h + "<select id='" + id + "' class='b-fi-c' data-itemtype='1' data-caption='"+caption+"'>";
	h = h + "</div>";
	h = h + "</div>";
	$(h).appendTo($("#filterContainer"));
	
	for(var i = 0; i < values.length; i++) {
		$("#" + id).append("<option value='" + values[i] + "'>" + values[i] + "</option>");
	}

	$("#" + id).kendoComboBox();
}

function createMultiSelect(id, caption, defaultValue, values) {
	h = "<div class='b-fi' >";
	h = h + "<span class='b-fi-n'>" + caption + "</span>";
	h = h + "<div class='b-fi-v'>";
	h = h + "<select id='" + id + "' multiple='multiple' class='b-fi-c' data-itemtype='2' data-caption='"+caption+"'>";
	h = h + "</div>";
	h = h + "</div>";
	$(h).appendTo($("#filterContainer"));

	for(var i = 0; i < values.length; i++) {
		$("#" + id).append("<option value='" + values[i] + "'>" + values[i] + "</option>");
	}

	$("#" + id).multiselect({
		search: true,
		columns: 1,
		texts: {
			placeholder: ''
		},
		maxHeight: 300,
	});
}

function createDate(id, caption, defaultvalue, format, start) {
	var h;
	h = "<div class='b-fi'>";
	h = h + "<span class='b-fi-n'>" + caption + "</span>";
	h = h + "<input id='" + id + "' value='" + defaultvalue + "' class='b-fi-c b-fi-date' data-itemtype='3' data-caption='"+caption+"'>";
	h = h + "</div>";
	$(h).appendTo($("#filterContainer"));

	$("#" + id).kendoDatePicker({
		start: start,
		depth: start,
		format: format
	});

}

function runtimeFilterItem(k,n,v1,v2) {

	this.Key = k;
	this.Name = n;
	this.Value1 = v1;
	this.Value2 = v2;
}

function runtimeReportFilter(name) {

	this.Name = name;
	this.Items = new Array();
	//this.Items[0] = new runtimeFilterItem();
}

function getRuntimeReportFilter(name){
	var f = new runtimeReportFilter(name);
	var i = 0;
	$("select.b-fi-c").each( function(){
					           		var t = $(this).data("itemtype");
					           		var c = $(this).data("caption");
					           		var v = $(this).val();
					           		f.Items[i] = new runtimeFilterItem(c,c,v);
					           		i++;
					           }
					       );
	$(".b-fi-date").each( function(){
					           		var t = $(this).data("itemtype");
					           		var c = $(this).data("caption");
					           		var v = $(this).val();
					           		f.Items[i] = new runtimeFilterItem(c,c,v);
					           		i++;
					           }
					       );

	return f;
}


function getFilterSchema(create){
	var f= new Array();
	var i = 0;
	
	if( create )
	{
		var n = $('#txtSchemaName').val()				       
		var fi = getRuntimeReportFilter(n);
		var y = JSON.stringify(fi); 
		var h =  "<li class='b-li-d' data-json='"+y+"' >"+n+"</li>";
		$(h).appendTo($("#filterSchemaList"));
	}
	
	
	
	$("#filterSchemaList li").each( function(){
					           		var t = $(this).data("json");
					           		//var y = JSON.stringify(t); 
					           		f[i] = t;
					           		i++;
					           }
					       );
	

	return f;
}
