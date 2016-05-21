/* ###########################################################################
 * IIS extension methods
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace kuujinbo.asp.net.WebForms {
  public static class WebAppExtensions {
// ===========================================================================
/*
 * ###########################################################################
 * miscellaneous
 * ###########################################################################
*/
// prepend CSS file(s) to <head>; master page/site-wide CSS **ALWAYS** win
// programmer is responsible for:
// -- verifying URL / path to file(s)
// -- properly ordering CSS file(s)
// @param Css => virtual url(s)
    public static void PrependCssToHead(this Page p, params string[] Css) {
      foreach (var uri in Css) {
        if ( Regex.IsMatch(uri, @"\.css$", RegexOptions.IgnoreCase) ) {
          p.Header.Controls.AddAt(0, new Literal() {
            Text = string.Format(StringFormat.TAG_LINK, uri)
          });
        }
      }
    }
// ---------------------------------------------------------------------------
// append CSS file(s) to <head>; CSS __overrides__ site-wide CSS files
// programmer is responsible for:
// -- verifying URL / path to file(s)
// -- properly ordering CSS file(s)
// @param Css => virtual url(s)
    public static void AppendCssToHead(this Page p, params string[] Css) {
      foreach (var uri in Css) {
        if ( Regex.IsMatch(uri, @"\.css$", RegexOptions.IgnoreCase) ) {
          p.Header.Controls.Add(new Literal() {
            Text = string.Format(StringFormat.TAG_LINK, uri)
          });
        }
      }
    } 
// ---------------------------------------------------------------------------
// append JavaScript file(s) to <head>; file extension **NOT** verified, 
// third-party JavaSctipt may **NOT** be sent with *.js extension!
// programmer is responsible for verifying URL / path to file(s)
// @param Js => virtual url(s)
    public static void AppendJavaScript(this Page p, params string[] Js) {
      foreach (var uri in Js) {
        p.Header.Controls.Add(new Literal() {
          Text = string.Format(StringFormat.TAG_SCRIPT, uri)
        });
      }
    }

/*
 * ###########################################################################
 * Application
 * ###########################################################################
*/
// ----------------------------------------------------------------------------
    public static object GetApplication(this string name) {
      return HttpContext.Current.Application[name];
    }
// ----------------------------------------------------------------------------
    public static void SetApplication(this string name, object o) {
      HttpContext.Current.Application[name] = o;
    }

/*
 * ###########################################################################
 * Session
 * ###########################################################################
*/
    public static void SetSessionTimeout(this int timeout) {
      HttpContext.Current.Session.Timeout = timeout;
    }
// ----------------------------------------------------------------------------
    public static object GetSession(this string name) {
      return HttpContext.Current.Session[name];
    }
// ----------------------------------------------------------------------------
    public static void SetSession(this string name, object o) {
      HttpContext.Current.Session[name] = o;
    }

/*
 * ###########################################################################
 * Request
 * ###########################################################################
*/
    public static string GetQueryString(this string name) {
      return HttpContext.Current.Request.QueryString[name] != null
        ? HttpContext.Current.Request.QueryString[name] : "";
    }
// ---------------------------------------------------------------------------
    public static string GetForm(this string name) {
      return HttpContext.Current.Request.Form[name];
    }
// ---------------------------------------------------------------------------
// **single** key/value pair
    public static string GetCookie(this string name) {
      return HttpContext.Current.Request.Cookies[name] != null
        ? HttpContext.Current.Request.Cookies[name].Value : "";
    }
// **single** name => **multiple** key/value pairs   
    public static string GetCookieKey(this string name, string key) {
      return  HttpContext.Current.Request.Cookies[name] != null
          && HttpContext.Current.Request.Cookies[name][key] != null
            ? HttpContext.Current.Request.Cookies[name][key] : "";
    }

/*
 * ###########################################################################
 * Response
 * ###########################################################################
*/
// set response types;
// http://en.wikipedia.org/wiki/Internet_media_type
    public static void ContentTypeCsv(this HttpResponse Response) {
      Response.ContentType = "text/csv";
    }
    public static void ContentTypeHtml(this HttpResponse Response) {
      Response.ContentType = "text/html";
    }
    public static void ContentTypeText(this HttpResponse Response) {
      Response.ContentType = "text/plain";
    }
    public static void ContentTypeXml(this HttpResponse Response) {
      Response.ContentType = "text/xml";
    }    
    public static void ContentTypeJson(this HttpResponse Response) {
      Response.ContentType = "application/json";
    }
    public static void ContentTypeBinary(this HttpResponse Response) {
      Response.ContentType = "application/octet-stream";
    }        
    public static void ContentTypePdf(this HttpResponse Response) {
      Response.ContentType = "application/pdf";
    }
    public static void ContentTypeZip(this HttpResponse Response) {
      Response.ContentType = "application/zip";
    }
    public static void AttachmentZip(this HttpResponse Response, string name) {
      Response.AppendHeader("Expires", "0");
      Response.AppendHeader(
        "Cache-Control",
        "must-revalidate, post-check=0, pre-check=0"
      );
      Response.BufferOutput = false;
      Response.ContentType = "application/zip";
      Response.AddHeader("content-disposition", String.Format(
        "filename={0}.zip", name
      ));
    }
// ---------------------------------------------------------------------------
// **session** cookie, **single** key/value pair
    public static void SetCookie(this string name, string value) {
      SetCookie(name, value, DateTime.MinValue);
    }
// **persistent** cookie, **single** key/value pair
    public static void SetCookie(this string name, string value, DateTime dt) {
      HttpCookie c = new HttpCookie(name);
      c.Value = value;
      if (dt != DateTime.MinValue) c.Expires = dt;
      HttpContext.Current.Response.Cookies.Add(c);
    }
// ---------------------------------------------------------------------------
// named **session** cookie, **multiple** key/value pairs
    public static void SetCookiePairs(this string name, 
        Dictionary<string, string> map)
    {
      SetCookiePairs(name, map, DateTime.MinValue);
    }
// named **persistent** cookie, string key/value pair(s)
    public static void SetCookiePairs(this string name, 
        Dictionary<string, string> map, DateTime dt)
    {
      HttpCookie c = new HttpCookie(name);
      foreach (string k in map.Keys) {
        c.Values[k] = map[k];
      }
      if (dt != DateTime.MinValue) c.Expires = dt;
      HttpContext.Current.Response.Cookies.Add(c);
    }

/*
 * ###########################################################################
 * Server
 * ###########################################################################
*/
    public static string MapPath(this string s) {
      return HttpContext.Current.Server.MapPath(s);
    }

/*
 * ###########################################################################
 * caching
 * ###########################################################################
*/
// insert Cache item => programmer must cast __after__  retrieved
    public static object GetCachedObject(this string name) {
      return HttpContext.Current.Cache[name];
    }
// ---------------------------------------------------------------------------   
// insert item into Cache => **absolute** expiration, minutes
    public static void CacheInsertAbsoluteMinutes(
      this string name, object o, double minutes) 
    {
      HttpContext.Current.Cache.Insert(
        name,
        o,
        null, 
        DateTime.Now.AddMinutes(minutes),
        Cache.NoSlidingExpiration
      );
    } 
// ----------------------------------------------------------------------------
// insert item with **sliding** expiration in minutes
    public static void CacheInsertslidingMinutes(
      this string name, object o, double minutes
    ) 
    {
      HttpContext.Current.Cache.Insert(
        name,
        o,
        null, 
        Cache.NoAbsoluteExpiration,
        TimeSpan.FromMinutes(minutes)
      );
    }
    
    
// ---------------------------------------------------------------------------    
// insert item into Cache => file dependency
    public static void CacheInsertDependency(
      this string key, object obj, string filename) 
    {
      HttpContext context = HttpContext.Current;
      string filePath = context.Server.MapPath(filename);
      if (!File.Exists(filePath)) {
        throw new FileNotFoundException(string.Format(
          "died in {0}(): file [{1}] not found",
          MethodBase.GetCurrentMethod().Name, filePath
        ));
      }
      CacheDependency dep = new CacheDependency(filename);
      context.Cache.Insert(key, obj, dep);
    }    
// ===========================================================================
  }
}