// 
//  DataGridRenderer.js
//  Part of Mr-Data-Converter
//  
//  Created by Shan Carter on 2010-10-18.
// 


var DataGridRenderer = {
 
  
  //---------------------------------------
  // JSON properties
  //---------------------------------------
  
  json: function (dataGrid, headerNames, headerTypes, indent, newLine) {
    //inits...
    var commentLine = "//";
    var commentLineEnd = "";
    var outputText = "[";
    var numRows = dataGrid.length;
    var numColumns = headerNames.length;
    
    //begin render loop
    for (var i=0; i < numRows; i++) {
      var row = dataGrid[i];
      outputText += "{";
      for (var j=0; j < numColumns; j++) {
        if ((headerTypes[j] == "int")||(headerTypes[j] == "float")) {
          var rowOutput = row[j] || "null";
        } else {
          var rowOutput = '"' + ( row[j] || "" ) + '"';
        };
  
      outputText += ('"'+headerNames[j] +'"' + ":" + rowOutput );
  
        if (j < (numColumns-1)) {outputText+=","};
      };
      outputText += "}";
      if (i < (numRows-1)) {outputText += ","+newLine};
    };
    outputText += "]";
    
    return outputText;
  }
  
  
}