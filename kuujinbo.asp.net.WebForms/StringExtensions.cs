/* ###########################################################################
 * string extension methods
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace kuujinbo.asp.net.WebForms {
  public static class StringExtensions {
// ===========================================================================
// ---------------------------------------------------------------------------
// convenience method
    public static string[] RegexSplit(this string s, string pattern) {
      return Regex.Split(s, pattern);
    }
/* ---------------------------------------------------------------------------
 * from SDK:
 * "a word that is entirely uppercase, such as an acronym, is not converted".
 * M$ assumes ALL strings with all uppercase characters are acronyms?!?!?
 */
    public static string TitleCase(this string s) {
      TextInfo t = new CultureInfo("en-US", false).TextInfo;
      return t.ToTitleCase(s.ToLower());
    }
/* ---------------------------------------------------------------------------
 * user-friendly CAC/EDI username;
 * usually LASTNAME.FIRSTNAME.[MI].[\d++] 
 * first last name 
 */
  public static string[] CacFirstLastName(this string userName) {
    string[] splitValue = userName.Split(new char[] {'.'}, 3);
    string[] returnArray = new string[2];
    switch (splitValue.Length) {
      case 3:
        returnArray[0] = splitValue[1].TitleCase();
        returnArray[1] = splitValue[0].TitleCase();
        break;
      case 2:
        returnArray[0] = splitValue[1].TitleCase();
        returnArray[1] = splitValue[0].TitleCase();
        break;
      default: break;
    }
    return returnArray;
  }
/* ---------------------------------------------------------------------------
 * user-friendly CAC/EDI username;
 * usually LASTNAME.FIRSTNAME.[MI].[\d++] 
 * last, first name 
 */
    public static string CacLastFirstName(this string userName) {
      MatchCollection lastFirst = Regex.Matches(
        userName, @"^([^\.]+)\.([^\.]+)"
      );
      if (lastFirst.Count > 0) {
        GroupCollection gc = lastFirst[0].Groups;
        if (gc[0].Success && gc.Count == 3) {
          return string.Format("{0}, {1}", 
            TitleCase(gc[1].Value), TitleCase(gc[2].Value)
          );
        }
      }
          
// shouldn't fail, but just in case strip the EDI      
      return Regex.Replace(userName, @"\.\d+$", "");
    }
/* ---------------------------------------------------------------------------
 * user-friendly CAC/EDI username;
 * usually LASTNAME.FIRSTNAME.[MI].[\d++] 
 */
    public static string CacFriendlyName(this string EDI) {
      string[] split = EDI.Split(new char[] {'.'}, 3);
      switch (split.Length) {
        case 3:
          split[2] = Regex.Replace(split[2], @"\.?\d+", "");
          EDI = String.Format(
            "{0}, {1} {2}",
            split[0].ToUpper(),
            TitleCase(split[1]),
            TitleCase(split[2])
          )
          .Trim();
          break;
        case 2:
          EDI = String.Format("{0}, {1}",
            split[0].ToUpper(),
            TitleCase(split[1])
          );
          break;
        default: 
          break;
      }
      return EDI;
    }
/*
 * ###########################################################################
 * HTML string helpers
 * ###########################################################################
*/
    public static string HtmlEncode(this string s) {
      return HttpUtility.HtmlEncode(s);
    }
    public static string HtmlDecode(this string s) {
      return HttpUtility.HtmlDecode(s);
    }
// ----------------------------------------------------------------------------
    public static string GetEmailLink(this string email) {
      return String.Format("<a href='mailto:{0}'>{0}</a>", email);
    }
    public static string GetEmailLink(this string email, string subject) {
      return String.Format(
        "<a href='mailto:{0}?subject={1}'>{0}</a>", 
        email, subject
      );
    }
    public static string GetEmailLink(
      this string email, string subject, string body
    ) 
    {
      return String.Format(
        "<a href='mailto:{0}?subject={1}&body={2}'>{0}</a>",
        email, subject, body
      );
    }    
/* ----------------------------------------------------------------------------
 * format text contained in TEXTAREA stored in database, which may include 
 * newlines; when displaying in UI separate those newlines into paragraphs.
 * we're __NOT__ using Environment.NewLine in regex to account for differences 
 * in Ajax/how browsers handle newlines in TEXTAREA
 */
    public static string TextareaToHtml(this string s) {
      string multiLine = "(?:\r?\n){2,}";
      s = Regex.Replace(s, multiLine, "<br /><br />", RegexOptions.Singleline);      
      return "<div>" + 
      Regex.Replace(s, 
        "(?:\r?\n){1}",
        "<br />",
        RegexOptions.Singleline
      )
      + "</div>"
      ;     
    }
/*
 * ###########################################################################
 * array helpers
 * ###########################################################################
*/
// http://blog.binaryocean.com/CommentView,guid,4eed34a3-dd5c-4476-b801-65d7aa37c3cf.aspx  
// !!!! lazy load; no type checking !!!!   
    public static int[] ToIntArray(this string[] input) {
      if ( input == null ) return null;
      
      return Array.ConvertAll<string, int>(
        input, 
        delegate(string s) { return int.Parse(s); }
      );
    }
    public static int[] CsvToIntArray(this string input) {
      if ( string.IsNullOrEmpty(input) ) return null;
      
      return Array.ConvertAll<string, int>(
        input.Split(new Char[] {','} ),
        delegate(string s) { return int.Parse(s); }
      );
    }
// ===========================================================================
  }
}