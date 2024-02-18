mergeInto(LibraryManager.library, {

  CopyJS: function (content) {
    var parsedContent = UTF8ToString(content);
    window.clipboardText = parsedContent
      var listener = function (e) {
      e.clipboardData.setData("text/plain", parsedContent);
      e.preventDefault();
      document.removeEventListener("copy", listener);
    }
    document.addEventListener("copy", listener);
    document.execCommand("copy");
  },

  PasteJS: function () {
    var content = window.clipboardText || '';
    var bufferSize = lengthBytesUTF8(content) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(content, buffer, bufferSize);
    return buffer;
  },
  
  OpenUrlJS: function(url) {
    var parsedUrl = UTF8ToString(url);
    document.onpointerup = function() { //Use onpointerup for touch input compatibility
      window.open(parsedUrl);
      document.onpointerup = null;
    }
  }
});
