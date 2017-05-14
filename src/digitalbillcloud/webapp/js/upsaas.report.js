var t,tt;
var mysqlConn = "xx";
function postPortalFilterMessage(reportId){
		$.ajax({ 
       				type: "Get",
            		dataType: "json",
            		contentType: "application/json; charset=utf-8",
		    		cache: false,
		    		async: true,
		    		url: getWebApiURI()+"/api/Test/GetReportFilterContext/?reportId="+reportId,               
            		success: function (data) { 
            			alert(JSON.stringify(data));  
            			t = data.modelTableName;
            			tt = data.runtimeTableName;
                 		window.parent.postMessage(data, '*');
					
            		}, 
            		error: function (XMLHttpRequest, textStatus, errorThrown) { 
                    	alert(errorThrown); 
            		} 
          		});
              		
        document.body.addEventListener({"key":"click"},function(){
         		alert("click");
				window.parent.postMessage('click', "*");
		});
		
}

window.addEventListener('message', function (event) {
				alert("查询执行完毕,临时表已经准备好了,可以进行报表计算");
				alert(JSON.stringify(event.data));  
				
				var d = event.data;
				var dbName = d.dbname;
				var server = d.dbserver;
				var user = d.username;
				var pwd = d.pwd;
				
				mysqlConn=server+";"+user+";"+pwd+";"+dbName+";"+t+";"+tt;
				alert(mysqlConn);
				loadReport();
			}, false);

