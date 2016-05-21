/* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 * EVERYTHING HERE IS DEPENDENT on custom server controls:
 * ~/bin/kuujinbo.web.dll
 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
*/

// jQuery UI autocomplete
var DELAY = 500;
var MINLENGTH = 2;
var DATATYPE = 'json';

// you **MUST** specify this when working with ASP.NET
var MS_AJAX_CONTENT_TYPE = 'application/json;charset=utf-8';

// ############################################################################
// utility functions
// ############################################################################
function isNumber(n) {
  return !isNaN(parseFloat(n)) && isFinite(n); 
}
// ############################################################################
// plugins
// ############################################################################
// http://www.learningjquery.com/2007/08/clearing-form-data
$.fn.clearForm = function() {
  return this.each(function() {
    var type = this.type, tag = this.tagName.toLowerCase();
    if (tag == 'form')
      return $(':input', this).clearForm();
    if (type == 'text' || type == 'password' || tag == 'textarea')
      this.value = '';
    else if (type == 'checkbox' || type == 'radio')
      this.checked = false;
    else if (tag == 'select')
      this.selectedIndex = -1;
  });
};

var __IS_EXPLODER__;
var __VGROUP__;
var __VGROUP_ATTR__     = 'validation-group';
var __REQUIRED__        = 'required';
var __SKIP_VALIDATION__ = false;
var __OUR_ERROR_CLASS__ = 'validation-errors';
var __REGEXBOX__        = ':text.regex';
var __DATEBOX__         = ':text.jQuery-calendar';
var __TEXTBOX__         = ':text.required:not(.regex,.jQuery-calendar),textarea.required';
var __DROPDOWN__         = 'select.required';
var __IE_SELECT_SUCKS__ = '__IE_SELECT_SUCKS__';
// M$ can't do checkboxlist validation
var __CHECKBOXLIST__    = 'span.required-list,table.required-list';
// :not filters out checkboxlist
var __RADIOBUTTONLIST__ = 'table.required.radio,span.required.radio';
// var __RADIOBUTTONLIST__ = 'table.required:not(.required-list),span.required:not(.required-list)';
// ############################################################################
// DOCUMENT READY
// ############################################################################
$(function() {
  __IS_EXPLODER__ = $.browser.msie;
  var __BUTTON_CLICKED__ = '';
  $(':text[readonly]').css('background','#ccc');
  doTextareaCounter();
  doAllCheckbox();
// ============================================================================
// bind buttons that POST data
// ============================================================================
  $('input[type=submit]').each(function(){
    $(this).click(function(){
      var vgroup = __VGROUP__ = $(this).attr(__VGROUP_ATTR__);
      // ASP.NET adds 'onclick' **ONLY** if causesvalidation true,       
      if ($(this).attr('onclick')) {
        __BUTTON_CLICKED__ = $(this).attr('id')
        // server control button ID **NOT** set
          || $(this).attr('name').replace(/\$/g, '_');
        validate_scalar_controls( vgroup );
        validate_list_controls( vgroup );
        __SKIP_VALIDATION__ = false;
      }
      else {
        __SKIP_VALIDATION__ = true;
      }
    });
  });
// ============================================================================
// verify all form fields valid; 2011-01-20 replaced facebox with fancybox
// ============================================================================
  var __tmp_is_valid__ = false;
  $('form[id$=aspnetForm], form:first').submit(function(){
    if (__SKIP_VALIDATION__) return true;
    if (__VGROUP__) {
      var tmp = '.' + __OUR_ERROR_CLASS__ + '[' + __VGROUP_ATTR__ + '=' + __VGROUP__ + ']';
      __tmp_is_valid__ = $(tmp).length < 1;
    } else {
      __tmp_is_valid__ = $('.' + __OUR_ERROR_CLASS__).length < 1;
    }
    if (!__tmp_is_valid__ && __BUTTON_CLICKED__) {
      if ( typeof(window.$().fancybox) == 'function' ) {
        var errorMessage = $('#__' + __BUTTON_CLICKED__ + '__')
          .html()
        ;
        // IE hack, what else is new...
        $.fancybox({
          'content':errorMessage,
          'title':'More Information Required'
        });
      }
    }
    return __tmp_is_valid__;
  });
});
// ############################################################################
// MISCELLANEOUS GLOBAL FUNCTIONS
// ############################################################################
/*
 * textarea character counter
 */
function doTextareaCounter() {
  var $ta = $('textarea[maxlength]');
  if ( $ta.length > 0 ) {
    var cc = function(jqObj){
      $(jqObj).charCounter(
        $(jqObj).attr('maxlength'), {
          container:'<div style="color:#990000;"></div>',
          delay:200
        }
      );
    };    
    if (typeof (window.$().charCounter) === 'function') {
      $ta.each(function() { cc(this); });      
    }
  }
}
/*
 * add 'Check All'/'Clear All' to custom server checkboxlist control
 */
function doAllCheckbox() {
  $('.checkboxlist-checkall').each(function(i, cbl) {
/*    ^^^^^^^^^^^^^^
 * do **NOT** modify class selector above!
 */
/*
 * if checkboxlist does **NOT** have any items, stop here
 */
    if ( $(this).find('input[type=checkbox]').length < 1 ) {
// same as continue statement in for loop; skip to next iteration
      return true;
    }

    var myid = '#' + $(this).attr('id');
    var cb_div = $("<div style='margin:8px 0;font-weight:bold;'>"
      + "[ <a href='#' class='_check_all_'>Check All</a> |"
      + " <a href='#'>Clear All</a> ]</div>"
    )
    .bind('click', function (e){
      var tar = $(e.target);
      if ( $(tar).is('a') && $(tar).hasClass('_check_all_') ){
        $(cbl).find('input[type=checkbox]').each(function(){
          this.checked = true;
        });            
      }
      else {
        $(cbl).find('input[type=checkbox]').each(function(){
          this.checked = false;
        });           
      }
      return false;
    });
    $(this).after($(cb_div));
  });
}
// ============================================================================
// VALIDATION FUNCTIONS
// ============================================================================
// check if server control has validation group
// ----------------------------------------------------------------------------
function _control_has_vgroup(vgroup, jq_obj) {
  // jQuery attr() returns undefined if attribute not found
  if (vgroup != $(jq_obj).attr(__VGROUP_ATTR__)) {
    // if there are multiple validation groups, validation error 
    // class attribute may have been set by different button.
    $(jq_obj).removeClass(__OUR_ERROR_CLASS__);
    return true;
  }
  return false;
/*
  return vgroup != $(jq_obj).attr(__VGROUP_ATTR__)
    ? true : false;
*/
}
// ----------------------------------------------------------------------------
// checkbox lists have one OR more value
// ----------------------------------------------------------------------------
function validate_list_controls(vgroup) {
  $(__CHECKBOXLIST__).each(function(){
/*
 * same as continue statement in for loop, skip to next iteration; ignore
 * validating hidden/disabled data entry fields
 */
    if ( $(this).is(':hidden') || $(this).is(':disabled') ) {
      return true; // continue
    }

    if ( _control_has_vgroup(vgroup, this) ) {
      return; // break
    }
    else if ( $(this).find('input[type=checkbox]:checked').length > 0 ) {
      $(this).removeClass(__OUR_ERROR_CLASS__);
    }
    else {
      $(this).addClass(__OUR_ERROR_CLASS__);
    }
  });
}
// ----------------------------------------------------------------------------
// validate form fields with scalar/single value
// ----------------------------------------------------------------------------
function validate_scalar_controls(vgroup) {
  // [1] required textbox w/regex, [2] regex only validation
  $(__REGEXBOX__).each(function(){
/*
 * same as continue statement in for loop, skip to next iteration; ignore
 * validating hidden/disabled data entry fields
 */
    if ( $(this).is(':hidden') || $(this).is(':disabled') ) {
      return true;
    }

    if ( _control_has_vgroup(vgroup, this) ) {
      return; 
    }
    else {
      var v = $(this).val();
      var re_text = $(this).attr('regex')
      var re = new RegExp(re_text);
      var is_required = $(this).hasClass(__REQUIRED__);
      var err_msg = $(this).attr('regexErrorMessage');
      if (!err_msg) err_msg = 'must match "' + re_text + '"';
      var err_ID = $(this).attr('id') + '-regex-validator';
      if ( is_required && re.test(v) ) {
        $(this).removeClass(__OUR_ERROR_CLASS__);
        $('#' + err_ID).remove();
      } // [1] blank, [2] non-blank & match
      else if ( !v && !is_required || v && re.test(v) ) {
        $(this).removeClass(__OUR_ERROR_CLASS__);
        $('#' + err_ID ).remove();
      }
      else {
        $(this).addClass(__OUR_ERROR_CLASS__);
        if ( $('#' + err_ID ).length > 0 ) return;
        $(this).after(
          '<div id="' + err_ID + '" class="boldRed">' + err_msg + '</div>'
        );
      }
    }
  });
  // [1] required datebox, [2] datebox only validation
  $(__DATEBOX__).each(function(){
/*
 * same as continue statement in for loop, skip to next iteration; ignore
 * validating hidden/disabled data entry fields
 */
    if ( $(this).is(':hidden') || $(this).is(':disabled') ) {
      return true;
    }

    if ( _control_has_vgroup(vgroup, this) ) {
      return; 
    }
    else {
      var v = $(this).val();
      var invalid_day = isNaN(new Date(v).getDay());
      var is_required = $(this).hasClass(__REQUIRED__);
      var err_ID = $(this).attr('id') + '_datebox';
      if ( is_required && !v || v && invalid_day ) {
        $(this).addClass(__OUR_ERROR_CLASS__);
        if ( $('#' + err_ID ).length > 0 ) return;
        $(this).after(
          '<div id="' + err_ID + '" class="boldRed">Invalid date</div>'
        );
      }
      else {
        $(this).removeClass(__OUR_ERROR_CLASS__);
        $('#' + err_ID ).remove();
      }
    }
  });
  // required textbox/textarea
  $(__TEXTBOX__).each(function(){
/*
 * same as continue statement in for loop, skip to next iteration; ignore
 * validating hidden/disabled data entry fields
 */
    if ( $(this).is(':hidden') || $(this).is(':disabled') ) {
      return true;
    }

    if ( _control_has_vgroup(vgroup, this) ) {
      return; 
    }
    else if ( $(this).val() ) {
      $(this).removeClass(__OUR_ERROR_CLASS__);
    }
    else {
      $(this).addClass(__OUR_ERROR_CLASS__);
    }
  });
  // required dropdownlist/optgroup (SELECT)
  // IExploder SELECT makes life miserable for everyone;
  // among other things it can't handle setting borders
  // http://www.gklein.org/tests/bordertest.html
  $(__DROPDOWN__).each(function(){
/*
 * same as continue statement in for loop, skip to next iteration; ignore
 * validating hidden/disabled data entry fields
 */
    if ( $(this).is(':hidden') || $(this).is(':disabled') ) {
      return true;
    }

    if ( _control_has_vgroup(vgroup, this) ) {
      $(this).parent('span:first').removeClass(__IE_SELECT_SUCKS__);
      return; 
    }
    else if ( $(this).val() ) {
      $(this).removeClass(__OUR_ERROR_CLASS__);
      $(this).parent('span:first').removeClass(__IE_SELECT_SUCKS__);
    }
    else {
      $(this).addClass(__OUR_ERROR_CLASS__);
      var sp =  $(this).parent('span:first');
      $(sp).addClass(__IE_SELECT_SUCKS__);
      if (!__IS_EXPLODER__) $(sp).css('display', 'inline-block');
    }
  });
  // required [1] radiobuttonlist (table), [2] radiobuttonlist (flow)
  $(__RADIOBUTTONLIST__).each(function(){
/*
 * same as continue statement in for loop, skip to next iteration; ignore
 * validating hidden/disabled data entry fields
 */
    if ( $(this).is(':hidden') || $(this).is(':disabled') ) {
      return true;
    }

    if ( _control_has_vgroup(vgroup, this) ) {
      return; 
    }
    else if ( $(this).find('input:radio:checked').length > 0 ) {
      $(this).removeClass(__OUR_ERROR_CLASS__);
    }
    else {
      $(this).addClass(__OUR_ERROR_CLASS__);
    }
  });
}