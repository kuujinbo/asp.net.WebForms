    ,regexMatch: function (jQueryElement) {
      return true;

      var v = $(jQueryElement).val();
      var re_text = $(jQueryElement).attr(this.regex);
      var re = new RegExp(re_text);
      var is_required = $(jQueryElement).hasClass(pWeb.required);
      var err_msg = $(jQueryElement).attr(this.regexErrorMessage);
      if (!err_msg) err_msg = 'must match "' + re_text + '"';
      var err_ID = $(jQueryElement).attr('id') + '-regex-validator';
      if (is_required && re.test(v)) {
        $(jQueryElement).removeClass(pWeb.errorClass);
        $('#' + err_ID).remove();
      } // [1] blank, [2] non-blank & match
      else if (!v && !is_required || v && re.test(v)) {
        $(jQueryElement).removeClass(pWeb.errorClass);
        $('#' + err_ID).remove();
      }
      else {
        $(jQueryElement).addClass(pWeb.errorClass);
        if ($('#' + err_ID).length > 0) return;
        $(jQueryElement).after(
          '<div id="' + err_ID + '" class="boldRed">' + err_msg + '</div>'
        );
      }
    }

    , dateMatch: function (jQueryElement) {
      return true;

      var v = $(this).val();
      var invalid_day = isNaN(new Date(v).getDay());
      var is_required = $(this).hasClass(this.required);
      var err_ID = $(this).attr('id') + '_datebox';
      if (is_required && !v || v && invalid_day) {
        $(this).addClass(this.errorClass);
        if ($('#' + err_ID).length > 0) return;
        $(this).after(
          '<div id="' + err_ID + '" class="boldRed">Invalid date</div>'
        );
      }
      else {
        $(this).removeClass(this.errorClass);
        $('#' + err_ID).remove();
      }
    }




// add methods without specifying prototype
Function.prototype.method = function (name, func) {
  if (!this.prototype[name]) {
    this.prototype[name] = func;
    return this;
  }
};

if (pWeb.hiddenOrDisabled(this)) {
  return true; // continue
}
if (pWeb.hasValidationGroup(this)) {
  return; // break
}
else {
  var isValid = $(this).val();
  pWeb.setErrorClass(this, isValid)
}



// 
function MyObject() {
  name: 'myObject'
}
MyObject.prototype = { // restore contructor, or will point to object
  constructor: MyObject
}
var o = new MyObject();
console.log('o instanceof MyObject: ', o instanceof MyObject);
console.log('o.constructor === MyObject: ', o.constructor === MyObject);

var sayHi = 'sayHi';
MyObject.method(sayHi, function (word) {
  console.log('Hi there: ' + (word !== undefined ? word : '?!?!?'));
});
o[sayHi]();
o[sayHi]('y');


function dumpForm() {
  var e = document.forms[0].elements;
  var eCount = e.length;
  for (var i = 0; i < eCount; ++i) {
    console.log(e[i].type + ' : ' + (e[i].id || e[i].name || 'no ID/name') + ' : ' + e[i].value);
  }
}




// wrap library so that we don't pollute the global namespace
(function () {
  var MyLibrary = window.MyLibrary = function () {
    // initialize
  };
})();
console.log('MyLibrary !== undefined: ', MyLibrary !== undefined);

/*
create empty object used exclusively as lookup table;
no prototype chain & no naming collisions
*/
var lookup = Object.create(null);
lookup.key = 1;
console.log('lookup.key: ', lookup.key);
try {
  console.log('typeof lookup.key: ', (typeof lookup.key));
  lookup.toString();
} catch (e) {
  console.log('lookup.toString() should be "[object Object]", but is: ', e);
}


// 
function MyObject() {
  name: 'myObject'
}
MyObject.prototype = { // restore contructor, or will point to object
  constructor: MyObject
}
var o = new MyObject();
console.log('o instanceof MyObject: ', o instanceof MyObject);
console.log('o.constructor === MyObject: ', o.constructor === MyObject);

var sayHi = 'sayHi';
MyObject.method(sayHi, function (word) {
  console.log('Hi there: ' + (word !== undefined ? word : '?!?!?'));
});
o[sayHi]();
o[sayHi]('y');

/*
  var fade = function (node) {
    var level = 15;
    console.log('in fade main');
    var step = function () {
      var hex = level.toString(16);
      var bgcolor = '#ffff' + hex + hex;
      node.style.backgroundColor = bgcolor;
      console.log('in step main: ' + bgcolor);
      if (level > 0) {
      //  if (level < 15) {
          console.log('in level check');
          level--;
          // level++;
          setTimeout(step, 476);
      }
    };
    setTimeout(step, 476);
  }
*/

var s = 'dhadahj dfshjahf ja';
var a = s.split(/','/);
console.log("Object.prototype.toString.call(a) === '[object Array]': " + (Object.prototype.toString.call(a) === '[object Array]'));

function dumpForm() {
  var e = document.forms[0].elements;
  var eCount = e.length;
  for (var i = 0; i < eCount; ++i) {
    console.log(e[i].type + ' : ' + (e[i].id || e[i].name || 'no ID/name') + ' : ' + e[i].value);
  }
}

function whatBody() { console.log('clicked on <body>'); }

document.body.addEventListener('click', whatBody, false);
