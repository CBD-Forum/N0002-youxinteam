function fullScreen(){
			if ((document.fullScreenElement && document.fullScreenElement !== null) || (!document.mozFullScreen && !document.webkitIsFullScreen)) {
		           if (document.documentElement.requestFullScreen) {
		                document.documentElement.requestFullScreen();
		            } else if (document.documentElement.mozRequestFullScreen) {
		                document.documentElement.mozRequestFullScreen();
		            } else if (document.documentElement.webkitRequestFullScreen) {
		                document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
		            }
		        } else {
		            if (document.cancelFullScreen) {
		                document.cancelFullScreen();
		            } else if (document.mozCancelFullScreen) {
		                document.mozCancelFullScreen();
		            } else if (document.webkitCancelFullScreen) {
		                document.webkitCancelFullScreen();
		            }
		        }
}



function getUrlParam(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
            var r = window.location.search.substr(1).match(reg);  //匹配目标参数
            if (r != null) return unescape(r[2]); return null; //返回参数值
}



function showDocument(x){
				$(".b-doc").hide();
				$("#"+x).show();
}
function openDocument( id,url,name){
					
		$(".b-doc").hide();
		 
		if( $("#"+id).length==1){
			$("#"+id).show();
			return;
		}
		
        
        var h;
        h = "<div class='b-doc' id='"+id+"' data-title='"+name+"'>";
		h = h+"	<iframe src='"+url+"' style='width: 100%;height: 100%;' scrolling='auto'  frameborder='no' border='0' marginwidth='0' ></iframe>";
		h = h+"</div>";
		$(h).appendTo($("#mainDiv"));
		updateThisDocument(id);
							
}

function openReportDocument( id,name,filename){
					
		$(".b-doc").hide();
					 
		if( $("#"+id).length==1){
			$("#"+id).show();
			return;
		}
		
		
		var url = "../share/"+filename+".htm";
		
		
        
        var h;
        h = "<div class='b-doc' id='"+id+"' data-title='"+name+"'>";
		h = h+"	<iframe src='"+url+"' style='width: 100%;height: 100%;' scrolling='auto'  frameborder='no' border='0' marginwidth='0' ></iframe>";
		h = h+"</div>";
		$(h).appendTo($("#mainDiv"));
					
		updateThisDocument(id);
		
		var rid =  id.replace(new RegExp("_","gm"),"-");  
		
		refreshHistoryList(rid);
					
}

function refreshHistoryList(reportId){
	
	$.ajax({
		type: "get",
		dataType: "json",
		cache: false,
		async: true,
		url: getWebApiAuthURI() + "/WriteReportHistory/?token=" + getUserToken() + "&reportId="+reportId,
		success: function(data) {
			var m = new Array();
			m[0] = "RefreshHistoryList";
			window.frames[1].postMessage(m,'*');
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			alert(errorThrown);
		}
	});
	
	
}

function showDialog( id,name){
		
		var index = layer.open({
								  type: 1,
								  shift:5,
								  title:name,
								  shade: [0.6, '#393D49'],
								  shadeClose: true, //开启遮罩关闭
								  area: ['412px', '400px'], //宽高
								  content: "",
								 
								});

				
}

function updateAllDocument(){
	var cheight = $(window).height()-40-4;
	var cwidth = $(window).width()-41;
	
	$(".b-doc").height(cheight);
	$(".b-doc").width(cwidth);
	
	
}


function updateThisDocument(id){
	var cheight = $(window).height()-40;
	var cwidth = $(window).width()-41;
	
	$("#"+id).height(cheight);
	$("#"+id).width(cwidth);
}

function postOpenDocumentMessage(id,name,filename){
	
	hid = id.replace(new RegExp("-","gm"),"_");  
	var m = new Array();
	m[0] = "OpenDocument";
	m[1] = hid;
	m[2] = name;
	m[3] = filename;
	window.parent.postMessage(m,'*');
}

function postCloseDocumentMessage(r){
	r = r.replace(new RegExp("-","gm"),"_");  
	var m = new Array();
	m[0] = "CloseDocument";
	m[1] = r;
	
	window.parent.postMessage(m,'*');
}

function postShowDialogMessage(id,name){
	id = id.replace(new RegExp("-","gm"),"_");  
	var m = new Array();
	m[0] = "ShowDialog";
	m[1] = id;
	m[2] = name;
	window.parent.postMessage(m,'*');
}

function getWebApiURI(){
	return "http://"+location.hostname+":8008/api/Report";
}

function getWebApiAuthURI(){
	return "http://"+location.hostname+":8008/api/Portal";
}

function getWebApiRestfulURI(){
	return "http://"+location.hostname+":8008/api/RestfulApi";
}

function getUserId(){
	return window.localStorage.getItem("BAUserId");
}

function getUserToken(){
	return window.localStorage.getItem("BAUserToken");
}

function getCompanyCode(){
	return window.localStorage.getItem("BACompanyCode");
}

function saveDrillContextValue(dimName,dimValue){
	
	window.localStorage.setItem("DC_"+dimName,dimValue);
}
function getDrillContextValue(dimName){
	return window.localStorage.getItem("DC_"+dimName);
}

function clearDrillContextValue(dimName){
	var storage = window.localStorage;

 	for(var i=0;i<storage.length;i++){
    	var k = storage.key(i);
  		if( k.indexOf("DC_")==0)
  			window.localStorage.setItem(k,"");
 	}

}

function checkLoginStatus(){
	var a = window.localStorage.getItem("userCode");
	if(a!=null && a!="")
	{
		return true;
	}
	else
	{
		window.localStorage.setItem("reportUri",window.location.href);
		window.location.href = getRelativePath() +"login.htm";
	}
	return false;
}

function getRelativePath(){
	var company="";
	company = window.localStorage.getItem("companyCode");
	if( company!="")
		return "../../"
	return "../"
}

function reportAuthRequest(reportId){
		    var a = window.localStorage.getItem("userCode");
			this.UserName = a;
			this.ReportId=reportId;
			this.Company = window.localStorage.getItem("companyCode");
}


function uuid(len, radix) {
    var chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'.split('');
    var uuid = [], i;
    radix = radix || chars.length;
    if (len) {
      for (i = 0; i < len; i++) uuid[i] = chars[0 | Math.random()*radix];
    } 
    else {
      var r;
      uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
      uuid[14] = '4';
      for (i = 0; i < 36; i++) {
        if (!uuid[i]) {
          r = 0 | Math.random()*16;
          uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
        }
      }
    }
    return uuid.join('');
}


function checkReportAuth(reportId){
	
		if( checkLoginStatus()==false)
			return;
		
		company = window.localStorage.getItem("companyCode");
		
		var postUrl = getRelativePath() + "CheckReportAuth.aspx";
	
			
		var m  = new reportAuthRequest(reportId);
        var jsonStr = JSON.stringify(m);
        jQuery.ajax({
            type: "POST",
            url: postUrl,
            cache: false,
            data: { "JsonStr": jsonStr },
            dataType: "json",
            success: onReportAuthSuccess,
            error: onReportAuthError
        });
}


 function onReportAuthError(data, status) {
        alert("检查报表权限发生异常!");
}
 

function onReportAuthSuccess(data, status) {
    	
    	if (data.Result != "OK") 
    	{
           	window.location.href = getRelativePath() + "noAccess.htm";
        }
    	
}


function getProperty(r,c,obj){
			r = r-1;
			if(c==1)
				return obj[r].FormatValue1;
			if(c==2)
				return obj[r].FormatValue2;
			if(c==3)
				return obj[r].FormatValue3;
			if(c==4)
				return obj[r].FormatValue4;
			if(c==5)
				return obj[r].FormatValue5;
			if(c==6)
				return obj[r].FormatValue6;
			if(c==7)
				return obj[r].FormatValue7;
			return 0;
}
function saveH5ReportFilterValue(reportId){
	var f1,f2,f3,f4,f5;
	$("div.filteritem").find(".filtervalue").each(function(index,element){
	 			if(index==0)
	 			{
	 				if( $(this).is("input") )
	 					f1 = $(this).val();
	 				else
	 					f1 = $(this).text();
	 				window.localStorage.setItem("filterItem1_"+reportId,f1);
	 			}
	 			else if (index==1)
	 			{
	 				if( $(this).is("input") )
	 					f2 = $(this).val();
	 				else
	 					f2 = $(this).text();
	 				window.localStorage.setItem("filterItem2_"+reportId,f2);
	 			}
	 			else if (index==2){
	 				if( $(this).is("input") )
	 					f3 = $(this).val();
	 				else
	 					f3 = $(this).text();
	 				window.localStorage.setItem("filterItem3_"+reportId,f3);
	 			}
	 			else if (index==3){
	 				if( $(this).is("input") )
	 					f4 = $(this).val();
	 				else
	 					f4 = $(this).text();
	 				window.localStorage.setItem("filterItem4_"+reportId,f4);
	 			}
	 			else if (index==4){
	 				if( $(this).is("input") )
	 					f5 = $(this).val();
	 				else
	 					f5 = $(this).text();
	 				window.localStorage.setItem("filterItem5_"+reportId,f5);
	 			}
			  });
			  

}

function setDrillFilterValue(reportId,index,filterValue){
	
	var f1,f2,f3,f4,f5;
	var newFilterValue = filterValue;
	$("div.filteritem").find(".filtervalue").each(function(i,element){
	 			if(i==0){
	 				if( $(this).is("input") )
	 					f1 = $(this).val();
	 				else
	 					f1 = $(this).text();
	 			}
	 			else if (i==1){
	 				if( $(this).is("input") )
	 					f2 = $(this).val();
	 				else
	 					f2 = $(this).text();
	 			}
	 			else if (i==2){
	 				if( $(this).is("input") )
	 					f3 = $(this).val();
	 				else
	 					f3 = $(this).text();
	 			}
	 			else if (i==3){
	 				if( $(this).is("input") )
	 					f4 = $(this).val();
	 				else
	 					f4 = $(this).text();
	 			}
	 			else if (i==4){
	 				if( $(this).is("input") )
	 					f5 = $(this).val();
	 				else
	 					f5 = $(this).text();
	 			}
			  });
			  
	if(filterValue.indexOf("过滤项1")==0)
			  newFilterValue = f1;
	if(filterValue.indexOf("过滤项2")==0)
			  newFilterValue = f2;
	if(filterValue.indexOf("过滤项3")==0)
			  newFilterValue = f3;
	if(filterValue.indexOf("过滤项4")==0)
			  newFilterValue = f4;
	if(filterValue.indexOf("过滤项5")==0)
			  newFilterValue = f5;
			  
	if(filterValue.indexOf("{yyyy年}")>0){
		if( newFilterValue.length>4)
			newFilterValue = newFilterValue.substr(0,4)+"年";
	}
	else if(filterValue.indexOf("{yyyy年MM月}")>0){
		if( newFilterValue.length>7)
			newFilterValue = newFilterValue.substr(0,4)+"年"+newFilterValue.substr(5,2)+"月";
	}

		
	
			  
	if(index=="1")
		window.localStorage.setItem("filterItem1_"+reportId,newFilterValue);
	if(index=="2")
		window.localStorage.setItem("filterItem2_"+reportId,newFilterValue);
	if(index=="3")
		window.localStorage.setItem("filterItem3_"+reportId,newFilterValue);
	if(index=="4")
		window.localStorage.setItem("filterItem4_"+reportId,newFilterValue);
	if(index=="5")
		window.localStorage.setItem("filterItem5_"+reportId,newFilterValue);
}

function getDrillFilterValue(reportId,index){
	var filterValue;
	if(index=="1")
		filterValue = window.localStorage.getItem("filterItem1_"+reportId);
	if(index=="2")
		filterValue = window.localStorage.getItem("filterItem2_"+reportId);
	if(index=="3")
		filterValue = window.localStorage.getItem("filterItem3_"+reportId);
	if(index=="4")
		filterValue = window.localStorage.getItem("filterItem4_"+reportId);
	if(index=="5")
		filterValue = window.localStorage.getItem("filterItem5_"+reportId);
	

	return parseSystemFilterValue(filterValue);
}
function parseSystemFilterValue(filtervalue){
	var today = new Date();
	var y = today.getFullYear(); 
	var m = today.getMonth()+1; 
	var m1 = today.getMonth()+1; 
	var d = today.getDate();
	var q = (m-1)/3+1;
	
	if (m >= 1 && m <= 9) {
        m = "0" + m;
    }
	if (d >= 1 && d <= 9) {
        d = "0" + d;
    }
	if(filtervalue=="当年")
	{
		return y+"年";
	}
	if(filtervalue=="当季")
	{
		return y+"年"+q+"季度";
	}
	if(filtervalue=="当月")
	{
		return y+"年"+m+"月";
	}
	if(filtervalue=="当日")
	{
		return y+"-"+m+"-"+d;
	}
	if(filtervalue=="当日{yyyy年MM月dd日}" ||filtervalue=="当日{yyyy年mm月dd日}")
	{
		return y+"年"+m+"月"+d+"日";
	}
	if(filtervalue=="当日{MM月dd日}" ||filtervalue=="当日{mm月dd日}")
	{
		return m+"月"+d+"日";
	}
	if(filtervalue=="当日{MM-dd}" ||filtervalue=="当日{mm-dd}")
	{
		return m+"-"+d;
	}
	
	return filtervalue;
}
