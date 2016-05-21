/// <reference path="jquery-2.1.3.min.js" />
var pWeb = function () {
  return {
    ajaxContentType: 'application/json;charset=utf-8'

    , invalidCount: 0
    , skipValidation: false
    , validationGroupVal: undefined
    , buttonClicked: false
    , required: 'required'
    , validationGroup: 'validation-group'
    , errorClass: 'input-validation-error'
    , regex: 'regex'
    , regexSelector: 'input[type=text][regex]'
    , dateSelector: 'input[type=text].date'
    , textboxSelector: 'input[type=text]:not([regex],.date),textarea[required]'
    , dropdownSelector: 'select[required]'
    , checkboxSelector: 'span.checkbox[required],table.checkbox[required]'
    , radiolistSelector: 'table.radio[required],span.radio[required]'
    , regexErrorMessage: 'regexErrorMessage'

    , init: function () {
      this.addButtonHandlers();
      this.togglecheckboxList();
      this.textareaCounter();
    }
/**************************************************************************
individual validation callbacks 
**************************************************************************/
    , dateMatch: function (jQueryElement) {
      var v = $(jQueryElement).val();
      var invalidDay = isNaN(new Date(v).getDay());
      var isRequired = $(jQueryElement).attr(this.required);

      return v && !invalidDay || !isRequired;
    }
    , hasStringVal: function (jQueryElement) {
      var valid = false;
      var v = $(jQueryElement).val();
      if (v) {
        valid = !!(v.trim());
      }
      v = null;
      return valid;
    }
    , hasChecked: function (jQueryElement) {
      // return $(jQueryElement).find('input[type=checkbox]:checked').length > 0;
      return $(jQueryElement).find('input:checked').length > 0;
    }
    , regexMatch: function (jQueryElement) {
      var valid = false;
      var v = $(jQueryElement).val();
      var reText = $(jQueryElement).attr(this.regex);
      var reObj = new RegExp(reText);
      var isRequired = $(jQueryElement).attr(this.required);
      if (isRequired) {
        valid = v && reObj.test(v);
        // console.log('isRequired && v && reObj.test(v): ' + valid);
      }
      else {
        v = v.trim();
        valid = !v || (v && re.test(v));
        // console.log('!v || (v && re.test(v)): ' + valid);
      }
      return valid;
    }
/**************************************************************************
validation setup 
**************************************************************************/
// skip check on hidden / disabled elements
    , hiddenOrDisabled: function (n) {
      return $(n).is(':hidden') || $(n).is(':disabled');
    }

// check if multiple buttons / validation groups contained in form
    , hasValidationGroup: function (jQueryElement) {
      if (this.validationGroupVal != $(jQueryElement).attr(this.validationGroup)) {
        // if multiple validation groups, clean up potential prior click(s)
        $(jQueryElement).removeClass(this.errorClass);
        return true;
      }
      return false;
    }

// add/remove error class on element
    , setErrorClass: function (jQueryElement, callback) {
      if (this.hiddenOrDisabled(jQueryElement)) {
        return true; // continue $.each()
      }
      if (this.hasValidationGroup(jQueryElement)) {
        return; // break $.each()
      }

      // var valid = callback.bind(jQueryElement);
      var valid = callback.call(this, jQueryElement);
      if (valid) {
        $(jQueryElement).removeClass(this.errorClass);
      }
      else {
        this.invalidCount += 1;
        // console.log('invalid element ID: ' + $(jQueryElement).attr('id'));
        $(jQueryElement).addClass(this.errorClass);
      }
      return true; // continue $.each()
    }

    , validateControls: function () {
      var that = this;
      $(that.textboxSelector).each(function (i) {
        that.setErrorClass(this, that.hasStringVal);
      });
      $(that.dropdownSelector).each(function () {
        that.setErrorClass(this, that.hasStringVal);
      });
      $(that.radiolistSelector).each(function () {
        that.setErrorClass(this, that.hasChecked);
      });
      $(that.checkboxSelector).each(function () {
        that.setErrorClass(this, that.hasChecked);
      });
      $(that.regexSelector).each(function () {
        that.setErrorClass(this, that.regexMatch);
      });
      $(that.dateSelector).each(function (i) {
        that.setErrorClass(this, that.dateMatch);
      });
    }

    , addButtonHandlers: function () {
      var that = this;
      $('input[type=submit]').each(function () {
        $(this).off().on('click', function () {
            that.validationGroupVal = $(this).attr(that.validationGroup);
          // ASP.NET adds 'onclick' **ONLY** if causesvalidation true,
          if ($(this).attr('onclick')) {
            that.buttonClicked = $(this).attr('id')
            // server control button ID **NOT** set
              || $(this).attr('name').replace(/\$/g, '_');
            that.validateControls();
            that.skipValidation = false;
          }
          else {
            that.skipValidation = true;
            that.invalidCount = 0;
          }
          console.log('invalidCount: ' + that.invalidCount);
// modal validatation failure message
          var submitForm = that.invalidCount === 0;
          if (!submitForm) {
            if (typeof (window.$().dialog) === 'function') {
              $('body').append(
                '<div id="dialog" title="More Information Needed" style="display:none">'
                + '<p>Please fill in highlighted fields.</p>'
                + '</div>'
              );
              $("#dialog").dialog();
            }
          }

          that.invalidCount = 0;
          return submitForm;
        });
      });
    }
/**************************************************************************
utility functions
**************************************************************************/
    , isNumber: function (n) { return !isNaN(parseFloat(n)) && isFinite(n); }

    // [un]check custom server control checkboxlist
    // TODO: revisit FIRST element
    ,togglecheckboxList: function () {
      $('input[type=checkbox].checkAll').bind('click', function (e) {
        var first = $(this).prop('checked');
        console.log('in ca: ' + first);
        $("input[type='checkbox']").prop('checked', function (i, val) {
          console.log(i);
          return first;
        });
      });
    }
    ,textareaCounter: function () {
      if (typeof (window.$().charCounter) === 'function') {
        var $ta = $('textarea[maxlength]');
        if ($ta.length > 0) {
          var cc = function (o) {
            $(o).charCounter(
              $(o).attr('maxlength'), {
                container: '<div style="color:#990000;"></div>',
                delay: 200
              }
            );
          };
          $ta.each(function () { cc(this); });
        }
      }
    }
  };
}();


$(function () {
  pWeb.init();
});