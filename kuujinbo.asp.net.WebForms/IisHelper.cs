/* ###########################################################################
 * miscellaneous helper methods => IIS
 * ###########################################################################
 */
using System;
using System.Web;
using System.Web.Configuration;

namespace kuujinbo.asp.net.WebForms {
  public static class IisHelper {
// ============================================================================
    public const string KEY_SUBJECTCN = "SUBJECTCN";
// ---------------------------------------------------------------------------
// CAC username
    public static string CacName {
      get { 
        HttpContext hc = HttpContext.Current;
// __simple__ check => server / local development
        return !hc.Request.IsLocal
          ? hc.Request.ClientCertificate[KEY_SUBJECTCN]
          : hc.User.Identity.Name
        ;
      }
    }
// ---------------------------------------------------------------------------
// web server base URL
    public static string BaseUrl {
      get {
        return HttpContext.Current.Request.Url.GetLeftPart(
          UriPartial.Authority
        );
      }
    }
// ---------------------------------------------------------------------------
// ++M$_WTF; you need to jump through rings of fire to get something ALWAYS
// present in web.config; size returned in **bytes**
    public static int WebConfigMaxUploadSize {
      get { return ((HttpRuntimeSection)
        WebConfigurationManager
        .GetSection("system.web/httpRuntime")
        ).MaxRequestLength
        ; 
      }
    }  
// ============================================================================  
  }
}