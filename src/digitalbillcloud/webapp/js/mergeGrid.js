  
    function  getMergeCells(rowindex,mergeCells){
		var cells = new Array();
		for (var i=0;i<mergeCells.length;i++) {
			mi = mergeCells[i];
			if(mi.RowIndex==rowindex)
				cells.push(mi);
			else if( mi.RowIndex>rowindex)
				return cells;
		}
		return cells;
	}
	
	
	function  getSurplusRanges(rowindex,ranges){
		var cells = new Array();
		for (var i=0;i<ranges.length;i++) {
			mi = ranges[i];
			if(mi.BeginRowIndex<=rowindex && mi.EndRowIndex>=rowindex)
				cells.push(mi);
		}
		return cells;
	}

	function mergeGridRows(id,mergeCells,ranges){
		
		$("#"+id+">table>tbody>tr").each(function (index, item) {
							var cells  = getMergeCells(index,mergeCells);
							
							for (var i=0;i<cells.length;i++) {
								mi = cells[i];
								var td = $("td:eq("+mi.ColumnIndex+")",item);
								td.attr("rowspan",mi.Span);
							}
							
							cells  = getSurplusRanges(index,ranges);
							for (var i=0;i<cells.length;i++) {
							
					 			mi = cells[i];
					 			var td = $("td:eq("+mi.ColumnIndex+")",item);
					 			var td2 = $("td:eq("+mi.ColumnIndex+1+")",item);
					 			td2.css("border-left-width", "1px");
					 			td.remove();
					 }
					 });
}
	
function getRow(id,rowIndex){   
	return $("#"+id+" tbody tr").eq(rowIndex);
}

function getCell(id,rowIndex,cellIndex)
{    
	return $("#"+id+" tbody tr").eq(rowIndex).find("td").eq(cellIndex);
}

function setReportWarningCells(id,cells){
	for (var i=0;i<cells.length;i++) {
		var c = cells[i];
		var tc = getCell(id,c.RowIndex,c.ColumnIndex);
		tc.css("background", c.Background);
		tc.css("color", c.Color);
		tc.css("font-weight", c.FontWeight);
	}
}
        
function setGridContentHeight(blockId,pages){
	
	 //alert( pages);
	 
		var h = $("#"+blockId).height();
		
		var header = $("#"+blockId+">.k-grid-header").height();
		var footer = $("#"+blockId+">.k-grid-footer").height();
		
		h  = h-header-footer;

		if(pages==1)
		{
			 h = h+36;
			$("#"+blockId+"Pager").hide();
			//$("#"+blockId).children(".k-grid-footer").css("border-top-width","1px");
			//$("#"+blockId).children(".k-grid-footer").css("border-bottom-width","1px");
			//$("#"+blockId).children(".k-grid-footer").css("border-left-width","1px");
		}
		
		$("#"+blockId).children(".k-grid-content").height(h);
	}

function getGridProperty(c,obj){
			if(c==1)
				return obj.S1;
			if(c==2)
				return obj.S2;
			if(c==3)
				return obj.S3;
			if(c==4)
				return obj.S4;
			if(c==5)
				return obj.S5;
			if(c==6)
				return obj.S6;
			if(c==7)
				return obj.S7;
			if(c==8)
				return obj.S8;
			if(c==9)
				return obj.S9;
			if(c==10)
				return obj.S10;
			if(c==11)
				return obj.S11;
			return "";
}